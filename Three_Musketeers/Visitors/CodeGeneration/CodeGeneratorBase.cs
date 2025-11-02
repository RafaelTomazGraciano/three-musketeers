using System.Text;
using System.Text.RegularExpressions;
using LLVMSharp;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.CodeGeneration.Functions;
using Three_Musketeers.Visitors.CodeGeneration.Struct;

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

        //for functions
        protected StringBuilder functionDefinitions = new StringBuilder();
        protected Dictionary<string, FunctionInfo> declaredFunctions = new Dictionary<string, FunctionInfo>();
        protected MainFunctionCodeGenerator? mainFunctionCodeGenerator;
        protected FunctionCodeGenerator? functionCodeGenerator;

        protected StructCodeGenerator? structCodeGenerator;

        //for structs
        protected Dictionary<string, HeterogenousType> structTypes = [];
        protected StringBuilder structBuilder = new();

        protected int stringCounter = 0;
        protected int registerCount = 0;

        protected string NextRegister() => $"%{++registerCount}";
        protected string NextStringLabel() => $"@.str.{stringCounter++}";

        protected string GetLLVMType(string type)
        {
            return type switch
            {
                "int" => "i32",
                "double" => "double",
                "bool" => "i1",
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
                var t when structTypes.ContainsKey(t) => 4,
                "i32" => 4,
                "double" => 8,
                "i1" or "i8" => 1,
                _ => 4
            };
        }

        protected int GetSize(string type)
        {
            if (type.Contains('*') || type == "double") return 8;
            if (type == "i1" || type == "i8") return 1;
            if (type == "i32") return 4;
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
            
            
            // collect Signatures
            var functions = prog.Where(p => p.function() != null).Select(p => p.function()).ToList();
            foreach (var func in functions)
            {
                functionCodeGenerator!.CollectFunctionSignature(func);
            }

            // generate all definitios of functions
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

            Visit(context.mainFunction());

            return GenerateFinalCode();
        }

        protected virtual string GenerateFinalCode()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("; ModuleID = 'Three_Musketeers'");
            output.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
            output.AppendLine();
            
            output.Append(globalStrings);
            output.AppendLine();
            
            output.Append(structBuilder);
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
    }
}

