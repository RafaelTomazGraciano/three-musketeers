using Antlr4.Runtime.Misc;
using System.Collections.Specialized;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors{
    public class CodeGenerator : ExprBaseVisitor<string>
    {

        private StringBuilder globalStrings = new StringBuilder();
        private StringBuilder declarations = new StringBuilder();
        private StringBuilder mainBody = new StringBuilder();
        private Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        private Dictionary<string, string> registerTypes = new Dictionary<string, string>();
        private int stringCounter = 0;
        private int registerCount = 0;

        private string NextRegister() => $"%{++registerCount}";
        private string NextStringLabel() => $"@.str.{stringCounter++}";

        private string GetLLVMType(string type)
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

        public override string VisitStart([NotNull] ExprParser.StartContext context)
        {
            //declare printf
            declarations.AppendLine("declare i32 @printf(i8*, ...)");
            declarations.AppendLine();

            base.VisitStart(context);

            var finalCode = new StringBuilder();
            finalCode.AppendLine("; ModuleID = 'Three_Musketeers'");
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

        public override string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = GetLLVMType(type);

            string allocReg = $"%{varName}";
            mainBody.AppendLine($"  {allocReg} = alloca {llvmType}");

            var variable = new Variable(varName, type, llvmType, allocReg);
            variables[varName] = variable;

            string value = Visit(context.expr());

            mainBody.AppendLine($"  store {llvmType} {value}, {llvmType}* {allocReg}");

            return null;
        }

        public override string VisitIntLiteral([NotNull] ExprParser.IntLiteralContext context)
        {
            string value = context.INT().GetText();
            registerTypes[value] = "i32";
            return value;
        }

        public override string VisitDoubleLiteral([NotNull] ExprParser.DoubleLiteralContext context)
        {
            string value = context.DOUBLE().GetText();
            registerTypes[value] = "double";
            return value;
        }

        public override string VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();

            if (!variables.ContainsKey(varName))
            {
                throw new Exception($"Variable '{varName}' not found");
            }

            Variable variable = variables[varName];

            string loadReg = NextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}");

            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }

        public override string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); //remove quotes

            var (processedString, byteCount) = ProcessEscapeSequencesWithCount(formatString);

            string stringLabel = NextStringLabel();
            int stringLength = byteCount + 1; // for \00 (null terminator)

            globalStrings.AppendLine($"{stringLabel} = private unnamed_addr constant [{stringLength} x i8] c\"{processedString}\\00\"");

            string strPtrReg = NextRegister();
            mainBody.AppendLine($"  {strPtrReg} = getelementptr [{stringLength} x i8], [{stringLength} x i8]* {stringLabel}, i32 0, i32 0");

            var args = new List<string>();
            args.Add($"i8* {strPtrReg}");

            if (context.expr() != null)
            {
                var specifiers = ParseFormatSpecifiers(formatString);

                for (int i = 0; i < context.expr().Length; i++)
                {
                    string argReg = Visit(context.expr()[i]);
                    string actualType = registerTypes.ContainsKey(argReg) ? registerTypes[argReg] : "i32";

                    string expectedType = specifiers[i].expectedLLVMType;

                    if (actualType != expectedType)
                    {
                        if (actualType == "i32" && expectedType == "double")
                        {
                            string convertedReg = NextRegister();
                            mainBody.AppendLine($"  {convertedReg} = sitofp i32 {argReg} to double");
                            registerTypes[convertedReg] = "double";
                            argReg = convertedReg;
                            actualType = "double";
                        }
                        else if (actualType == "double" && expectedType == "i32")
                        {
                            string convertedReg = NextRegister();
                            mainBody.AppendLine($"  {convertedReg} = fptosi double {argReg} to i32");
                            registerTypes[convertedReg] = "i32";
                            argReg = convertedReg;
                            actualType = "i32";
                        }   
                    }
                    args.Add($"{actualType} {argReg}");
                }

                // foreach (var exprContext in context.expr())
            // {
            //     string argReg = Visit(exprContext);
            //     string argType = registerTypes.ContainsKey(argReg) ? registerTypes[argReg] : "i32";
            //     args.Add($"{argType} {argReg}");
            // }
        }

            string resultReg = NextRegister();
            string argsString = string.Join(", ", args);
            mainBody.AppendLine($"  {resultReg} = call i32 (i8*, ...) @printf({argsString})");

            return null;
        }

        private (string processed, int byteCount) ProcessEscapeSequencesWithCount(string str)
        {
            var result = new StringBuilder();
            int byteCount = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\\' && i + 1 < str.Length)
                {
                    char next = str[i + 1];
                    switch (next)
                    {
                        case 'n':
                            result.Append("\\0A"); //newline
                            byteCount++;
                            i++;
                            break;
                        case 't':
                            result.Append("\\09"); // tab
                            byteCount++;
                            i++;
                            break;
                        case 'r':
                            result.Append("\\0D"); //carriage return
                            byteCount++;
                            i++;
                            break;
                        case '\\':
                            result.Append("\\\\"); //backslash
                            byteCount++;
                            i++;
                            break;
                        case '"':
                            result.Append("\\22"); //quotes
                            byteCount++;
                            i++;
                            break;
                        case '0':
                            result.Append("\\00"); //null
                            byteCount++;
                            i++;
                            break;
                        default:
                            result.Append("\\");
                            result.Append(next);
                            byteCount += 2;
                            i++;
                            break;
                    }
                }
                else
                {
                    result.Append(str[i]);
                    byteCount++;
                }
            }
            return (result.ToString(), byteCount);
        }

        private List<FormatSpecifier> ParseFormatSpecifiers(string formatString){
            var specifiers = new List<FormatSpecifier>();

            for (int i = 0; i < formatString.Length; i++){
                if (formatString[i] == '%'){
                    if (i + 1 >= formatString.Length){
                        break;
                    }
                    if (formatString[i + 1] == '%'){
                        i++;
                        continue;
                    }
                    i++;

                    int? precision = null;

                    while (i < formatString.Length && "+-0 #".Contains(formatString)){
                        i++;
                    }
                    while (i < formatString.Length && char.IsDigit(formatString[i])){
                        i++;
                    }
                    if (i < formatString.Length && formatString[i] == '.'){
                        i++;
                        int precisionValue = 0;
                        while (i < formatString.Length && char.IsDigit(formatString[i])){
                            precisionValue = precisionValue * 10 + (formatString[i] - '0');
                            i++;
                        }
                        precision = precisionValue;
                    }
                    while (i < formatString.Length && "lhLzjt".Contains(formatString)){
                        i++;
                    }
                    if (i < formatString.Length){
                        char type = formatString[i];
                        specifiers.Add(new FormatSpecifier(type, precision));
                    }
                }
            }
            return specifiers;
        }

    }
}