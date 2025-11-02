using Antlr4.Runtime.Misc;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class PrintfCodeGenerator
    {
        private readonly StringBuilder globalStrings;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string> nextStringLabel;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public PrintfCodeGenerator(
            StringBuilder globalStrings,
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string> nextStringLabel,
            Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.globalStrings = globalStrings;
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.nextStringLabel = nextStringLabel;
            this.visitExpression = visitExpression;
        }

        public string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); //remove quotes

            var (processedString, byteCount) = EscapeSequenceProcessor.Process(formatString);

            string stringLabel = nextStringLabel();
            int stringLength = byteCount + 1; // for \00 (null terminator)

            globalStrings.AppendLine($"{stringLabel} = private unnamed_addr constant [{stringLength} x i8] c\"{processedString}\\00\"");

            string strPtrReg = nextRegister();
            getCurrentBody().AppendLine($"  {strPtrReg} = getelementptr [{stringLength} x i8], [{stringLength} x i8]* {stringLabel}, i32 0, i32 0");

            var args = new List<string>();
            args.Add($"i8* {strPtrReg}");

            if (context.expr() != null)
            {
                var specifiers = FormatSpecifierParser.Parse(formatString);

                for (int i = 0; i < context.expr().Length; i++)
                {
                    string argReg = visitExpression(context.expr()[i]);
                    string actualType = registerTypes.ContainsKey(argReg) ? registerTypes[argReg] : "i32";

                    string expectedType = specifiers[i].expectedLLVMType;

                    if (actualType != expectedType)
                    {
                        if (actualType == "i32" && expectedType == "double")
                        {
                            string convertedReg = nextRegister();
                            getCurrentBody().AppendLine($"  {convertedReg} = sitofp i32 {argReg} to double");
                            registerTypes[convertedReg] = "double";
                            argReg = convertedReg;
                            actualType = "double";
                        }
                        else if (actualType == "double" && expectedType == "i32")
                        {
                            string convertedReg = nextRegister();
                            getCurrentBody().AppendLine($"  {convertedReg} = fptosi double {argReg} to i32");
                            registerTypes[convertedReg] = "i32";
                            argReg = convertedReg;
                            actualType = "i32";
                        }
                    }
                    args.Add($"{actualType} {argReg}");
                }
            }

            string resultReg = nextRegister();
            string argsString = string.Join(", ", args);
            getCurrentBody().AppendLine($"  {resultReg} = call i32 (i8*, ...) @printf({argsString})");

            return null;
        }
    }
}

