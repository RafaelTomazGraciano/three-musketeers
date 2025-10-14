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

        public override string? VisitStart(ExprParser.StartContext context)
        {
            Console.WriteLine("=== Starting VisitStart ===");
            
            // FIRST: Process global declarations and assignments
            foreach (var prog in context.prog())
            {
                // Skip functions - they'll be processed later
                if (prog.function() != null)
                {
                    Console.WriteLine($"Skipping function: {prog.function().ID().GetText()}");
                    continue;
                }
                
                Console.WriteLine($"Visiting prog: {prog.GetText().Substring(0, Math.Min(30, prog.GetText().Length))}...");
                Visit(prog);
            }
            
            Console.WriteLine($"\nVariables after global declarations: {string.Join(", ", variables.Keys)}\n");
            
            // SECOND: collect function signatures
            var functions = context.prog().Where(p => p.function() != null)
                                .Select(p => p.function()).ToList();
            foreach (var func in functions)
            {
                functionCodeGenerator!.CollectFunctionSignature(func);
            }

            // THIRD: generate all function definitions
            foreach (var func in functions)
            {
                functionCodeGenerator!.VisitFunction(func);
            }

            // FOURTH: Visit main function
            Visit(context.mainFunction());

            return GenerateFinalCode();
        }
        
        public override string? VisitProg(ExprParser.ProgContext context)
        {
            Console.WriteLine($"VisitProg called: {context.GetText().Substring(0, Math.Min(50, context.GetText().Length))}");
            
            if (context.declaration() != null)
            {
                Console.WriteLine($"  -> Has declaration");
                return Visit(context.declaration());
            }
            
            if (context.att() != null)
            {
                Console.WriteLine($"  -> Has att");
                return Visit(context.att());
            }
            
            if (context.function() != null)
            {
                Console.WriteLine($"  -> Has function");
                // Don't visit here - will be visited later
                return null;
            }
            
            return base.VisitProg(context);
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

