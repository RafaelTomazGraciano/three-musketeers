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
        protected readonly StringBuilder forwardDeclarations = new StringBuilder();
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

        public override string? VisitStart(ExprParser.StartContext context)
        {
            // collect SIgnatures
            var functions = context.prog().Where(p => p.function() != null).Select(p => p.function()).ToList();
            foreach (var func in functions)
            {
                functionCodeGenerator!.CollectFunctionSignature(func);
            }

            // generate all definitios of functions
            foreach (var func in functions)
            {
                functionCodeGenerator!.VisitFunction(func);
            }

            foreach (var prog in context.prog())
            {
                if (prog.function() == null)
                {
                    Visit(prog);
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

