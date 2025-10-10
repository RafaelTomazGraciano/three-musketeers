using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.CodeGeneration.Functions;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public abstract class CodeGeneratorBase : ExprBaseVisitor<string?>
    {
        protected StringBuilder globalStrings = new StringBuilder();
        protected StringBuilder declarations = new StringBuilder();
        protected StringBuilder mainDefinition = new StringBuilder();
        protected Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        protected Dictionary<string, string> registerTypes = new Dictionary<string, string>();

        //for functions
        protected StringBuilder functionDefinitions = new StringBuilder();
        protected Dictionary<string, FunctionInfo> declaredFunctions = new Dictionary<string, FunctionInfo>();

        protected MainFunctionCodeGenerator? mainFunctionCodeGenerator;
        protected FunctionCodeGenerator? functionCodeGenerator;

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
                _ => "i32"
            };
        }

        protected int GetAlignment(string type)
        {
            return type switch
            {
                var t when t.Contains('*') => 8,
                "i32" => 4,
                "double" => 8,
                "i1" or "i8" => 1,
                _ => 4
            };
        }

        public override string VisitStart(ExprParser.StartContext context)
        {
            // Declare external functions
            declarations.AppendLine("declare i32 @printf(i8*, ...)");
            declarations.AppendLine("declare i32 @scanf(i8*, ...)");
            declarations.AppendLine("declare i8* @strcpy(i8*, i8*)");
            declarations.AppendLine();
            
            base.VisitStart(context);
            
            var finalCode = new StringBuilder();
            finalCode.AppendLine("; ModuleID = 'Three_Musketeers'");
            finalCode.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
            finalCode.AppendLine();
            
            // Global strings
            if (globalStrings.Length > 0)
            {
                finalCode.Append(globalStrings);
                finalCode.AppendLine();
            }
            
            // External declarations
            finalCode.Append(declarations);
            
            // User-defined functions
            if (functionDefinitions.Length > 0)
            {
                finalCode.Append(functionDefinitions);
                finalCode.AppendLine();
            }
            
            // Main function
            finalCode.Append(mainDefinition);
            
            return finalCode.ToString();
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

