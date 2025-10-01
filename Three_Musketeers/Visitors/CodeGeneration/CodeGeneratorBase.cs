using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public abstract class CodeGeneratorBase : ExprBaseVisitor<string>
    {
        protected StringBuilder globalStrings = new StringBuilder();
        protected StringBuilder declarations = new StringBuilder();
        protected StringBuilder mainBody = new StringBuilder();
        protected Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        protected Dictionary<string, string> registerTypes = new Dictionary<string, string>();
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
                "float" => "float",
                "bool" => "i1",
                "string" => "i8*",
                _ => "i32"
            };
        }

        public override string VisitStart(ExprParser.StartContext context)
        {
            // declare printf
            declarations.AppendLine("declare i32 @printf(i8*, ...)");
            declarations.AppendLine();

            base.VisitStart(context);

            var finalCode = new StringBuilder();
            finalCode.AppendLine("; ModuleID = \'Three_Musketeers\'");
            finalCode.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
            finalCode.AppendLine();

            if (globalStrings.Length > 0)
            {
                finalCode.Append(globalStrings.ToString());
                finalCode.AppendLine();
            }

            finalCode.Append(declarations.ToString());

            finalCode.AppendLine("define i32 @main() {");
            finalCode.AppendLine("entry:");
            finalCode.Append(mainBody.ToString());
            finalCode.AppendLine("  ret i32 0");
            finalCode.AppendLine("}");

            return finalCode.ToString();
        }
    }
}

