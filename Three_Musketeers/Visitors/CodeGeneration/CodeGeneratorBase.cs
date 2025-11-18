using System.Text;
using System.Text.RegularExpressions;
using LLVMSharp;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.CodeGeneration.Functions;
using Three_Musketeers.Visitors.CodeGeneration.Struct_Unions;
using Three_Musketeers.Visitors.CodeGeneration.CompilerDirectives;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public abstract class CodeGeneratorBase : ExprBaseVisitor<string?>
    {
        protected StringBuilder globalStrings = new StringBuilder();
        protected StringBuilder declarations = new StringBuilder();
        protected StringBuilder mainDefinition = new StringBuilder();
        protected readonly StringBuilder forwardDeclarations = new StringBuilder();
        protected Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        protected Dictionary<string, string> registerTypes = new Dictionary<string, string>();
        protected Dictionary<string, string> defineValues = new Dictionary<string, string>();

        //for functions
        protected StringBuilder functionDefinitions = new StringBuilder();
        protected Dictionary<string, FunctionInfo> declaredFunctions = new Dictionary<string, FunctionInfo>();
        protected MainFunctionCodeGenerator? mainFunctionCodeGenerator;
        protected FunctionCodeGenerator? functionCodeGenerator;
        protected DefineCodeGenerator? defineCodeGenerator;

        protected StructCodeGenerator? structCodeGenerator;
        protected UnionCodeGenerator? unionCodeGenerator;

        //for structs
        protected Dictionary<string, HeterogenousType> structTypes = [];
        protected StringBuilder structBuilder = new();

        protected int stringCounter = 0;
        protected int registerCount = 0;

        protected string NextRegister() => $"%{++registerCount}";
        protected string NextStringLabel() => $"@.str.{stringCounter++}";

        protected string GetLLVMType(string type)
        {
            if (type.StartsWith("struct "))
            {
                string structName = type.Substring(7); // Remove "struct "
                if (structTypes.TryGetValue(structName, out var structModel))
                {
                    return structModel.GetLLVMName();
                }
                // Even if struct is not registered yet, return the LLVM name
                return '%' + structName;
            }
            
            return type switch
            {
                "int" => "i32",
                "double" => "double",
                "bool" => "i32",
                "char" => "i8",
                "string" => "i8*",
                var t when structTypes.TryGetValue(t, out var structModel) => structModel.GetLLVMName(),
                _ => "i32"
            };
        }

        protected int GetAlignment(string type)
        {
            return type switch
            {
                var t when t.Contains('*') => 8,
                var t when t.StartsWith("%") => GetStructOrUnionAlignment(t), 
                "i32" => 4,
                "double" => 8,
                "i8" => 1,
                _ => 4
            };
        }

        private int GetStructOrUnionAlignment(string type)
        {
            string typeName = type.TrimStart('%');
            
            if (structTypes.ContainsKey(typeName))
            {
                var hetType = structTypes[typeName];
                
                // For unions, return the MAXIMUM alignment of ALL members
                if (hetType is UnionType unionType)
                {
                    int maxAlignment = 1;
                    foreach (var member in unionType.GetMembers())
                    {
                        int memberAlignment = GetAlignment(member.LLVMType);
                        if (memberAlignment > maxAlignment)
                        {
                            maxAlignment = memberAlignment;
                        }
                    }
                    return maxAlignment;
                }
                
                // For structs, return 4 (or calculate based on members)
                return 4;
            }

            return 4;
        }

        protected int GetSize(string type)
        {
            // Check for POINTERS FIRST
            if (type.Contains('*'))
            {
                return 8; // All pointers are 8 bytes on x64
            }
            
            // Check for basic types
            if (type == "double") return 8;
            if (type == "i1" || type == "i8") return 1;
            if (type == "i32") return 4;
            
            // Structs/Unions (LLVM types start with %)
            if (type.StartsWith("%"))
            {
                string structName = type.TrimStart('%');
                
                if (structTypes.ContainsKey(structName))
                {
                    var structType = structTypes[structName];
                    
                    // Calculate size with proper alignment
                    int totalSize = 0;
                    int maxAlignment = 1;
                    
                    foreach (var member in structType.GetMembers())
                    {
                        string memberType = member.LLVMType;
                        int memberSize = GetSize(memberType);
                        int memberAlignment = GetAlignment(memberType);
                       
                        // Track maximum alignment for final padding
                        if (memberAlignment > maxAlignment)
                        {
                            maxAlignment = memberAlignment;
                        }
                        
                        // Align current offset to member's alignment requirement
                        if (totalSize % memberAlignment != 0)
                        {
                            int padding = memberAlignment - (totalSize % memberAlignment);
                            totalSize += padding;
                        }
                        
                        totalSize += memberSize;
                    }
                    
                    // Add final padding to align to the largest member alignment
                    if (totalSize % maxAlignment != 0)
                    {
                        int finalPadding = maxAlignment - (totalSize % maxAlignment);
                        totalSize += finalPadding;
                    }
                    
                    return totalSize;
                }
                return 32; // fallback
            }
            
            // Arrays
            int total = 1;
            var splits = Regex.Split(type, @"[\[\]x]+");
            foreach (var split in splits)
            {
                if (int.TryParse(split.Trim(), out int dim))
                {
                    total *= dim;
                    continue;
                }
                if (split.Contains('i'))
                {
                    total *= GetSize(split.Trim());
                }
                else if (split.StartsWith("%"))
                {
                    total *= GetSize(split.Trim());
                }
            }
            return total;
        }

        protected string CalculateArrayPosition(ExprParser.IndexContext index)
        {
            string expr = Visit(index.expr())!;
            return "i32 "+expr;
        }

        protected string? CalculateArrayPosition(ExprParser.IndexContext[] indexes)
        {
            string expr = Visit(indexes[0].expr())!;
            string result = "i32 " + expr;

            for (int i = 1; i < indexes.Length; i++)
            {
                expr = Visit(indexes[i].expr())!;
                result += $", i32 {expr}";
            }
            return result;
        }

        public override string? VisitStart(ExprParser.StartContext context)
        {
            var prog = context.prog();
            var heteregeneousDeclarationContexts = prog.Where(p => p.heteregeneousDeclaration() != null).Select(p => p.heteregeneousDeclaration());
            
            foreach (var hetDecl in heteregeneousDeclarationContexts)
            {
                if (hetDecl.structStatement() != null)
                {
                    structCodeGenerator!.VisitStructStatement(hetDecl.structStatement());
                }

                if (hetDecl.unionStatement() != null)
                {
                    unionCodeGenerator!.VisitUnionStatement(hetDecl.unionStatement());
                }
            }

            // collect Signatures
            var functions = prog.Where(p => p.function() != null).Select(p => p.function()).ToList();
            {
                // Process all #include directives first
                var includes = context.include();
                foreach (var include in includes)
                {
                    Visit(include);
                }

                // Process all #define directives first
                var defines = context.define();
                foreach (var define in defines)
                {
                    defineCodeGenerator!.ProcessDefine(define);
                }

                foreach (var func in functions)
                {
                    functionCodeGenerator!.CollectFunctionSignature(func);
                }

                // generate all function definitions
                foreach (var func in functions)
                {
                    functionCodeGenerator!.VisitFunction(func);
                }

                foreach (var stm in context.prog())
                {
                    if (stm.function() == null && stm.heteregeneousDeclaration() == null)
                    {
                        Visit(stm);
                    }
                }

                // Visit main function
                Visit(context.mainFunction());

                return GenerateFinalCode();
            }
        }
        
        public override string? VisitProg(ExprParser.ProgContext context)
        {
            
            if (context.declaration() != null)
            {
                return Visit(context.declaration());
            }
            
            if (context.att() != null)
            {
                return Visit(context.att());
            }
            
            if (context.function() != null)
            {
                // Don't visit here - will be visited later
                return null;
            }
            
            return base.VisitProg(context);
        }

        protected virtual string GenerateFinalCode()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("; ModuleID = 'Three_Musketeers'");
            output.AppendLine("target datalayout = \"e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128\"");
            output.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
            output.AppendLine();
            
            output.Append(structBuilder);
            output.Append(globalStrings);
            output.AppendLine();
            output.Append(declarations);
            output.AppendLine();
            output.Append(functionDefinitions);
            output.AppendLine();
            output.Append(mainDefinition);

            return output.ToString();
        }

        protected StringBuilder GetCurrentBody()
        {
            if (functionCodeGenerator != null && functionCodeGenerator.IsInsideFunction())
            {
                return functionCodeGenerator.GetCurrentFunctionBody()!;
            }
            
            if (mainFunctionCodeGenerator != null && mainFunctionCodeGenerator.IsInsideMain())
            {
                return mainFunctionCodeGenerator.GetMainBody();
            }
            
            // Fallback
            return new StringBuilder();
        }

        public static string RemoveOneAsterisk(string type)
        {
            return type.EndsWith("*") ? type.Substring(0, type.Length - 1) : type;
        }
    }
}
