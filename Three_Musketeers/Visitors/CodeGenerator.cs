using Antlr4.Runtime.Misc;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors{
    public class CodeGenerator : ExprBaseVisitor<string>{

        private StringBuilder globalStrings = new StringBuilder();
        private StringBuilder declarations = new StringBuilder();
        private StringBuilder mainBody = new StringBuilder();

        private Dictionary<string, string> variables = new Dictionary<string, string>();
        private int stringCounter = 0;
        private int registerCount = 0;

        private string NextRegister() => $"%{++registerCount}";
        private string NextStringLabel() => $"@.str.{stringCounter++}";

        private string GetLLVMType(string type){
            return type switch{
                "int" => "i32",
                "double" => "double",
                "float" => "float",
                "bool" => "i1",
                "string" => "i8*",
                _ => "i32"
            };
        }

        public override string VisitStart([NotNull] ExprParser.StartContext context){
            //declare printf
            declarations.AppendLine("declare i32 @printf(i8*, ...)");
            declarations.AppendLine();

            base.VisitStart(context);

            var finalCode = new StringBuilder();
            finalCode.AppendLine("; ModuleID = 'Three_Musketeers'");
            finalCode.AppendLine("target triple = \"x86_64-pc-linux-gnu\"");
            finalCode.AppendLine();

            if(globalStrings.Length > 0){
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

        public override string? VisitAtt([NotNull] ExprParser.AttContext context){
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = GetLLVMType(type);

            string alloc = $"%{varName}";
            mainBody.AppendLine($"  {alloc} = alloca {llvmType}");

            variables[varName] = alloc;

            string value = Visit(context.expr());

            mainBody.AppendLine($"  store {llvmType} {value}, {llvmType}* {alloc}");

            return null;
        }

        public override string VisitIntLiteral([NotNull] ExprParser.IntLiteralContext context){
            return context.INT().GetText();
        }

        public override string VisitDoubleLiteral([NotNull] ExprParser.DoubleLiteralContext context){
            return context.DOUBLE().GetText();
        }

        public override string VisitVar([NotNull] ExprParser.VarContext context){
            string varName = context.ID().GetText();

            if(!variables.ContainsKey(varName)){
                throw new Exception($"Variable '{varName}' not found");
            }

            string varReg = variables[varName];
            string type = "i32";

            string loadReg = NextRegister();
            mainBody.AppendLine($"  {loadReg} = load {type}, {type}* {varReg}");

            return loadReg;
        }

        public override string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context){
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

            if(context.expr() != null){
                foreach(var exprContext in context.expr()){
                    string argReg =  Visit(exprContext);
                    args.Add($"i32 {argReg}");
                }
            }

            string resultReg = NextRegister();
            string argsString = string.Join(", ", args);
            mainBody.AppendLine($"  {resultReg} = call i32 (i8*, ...) @printf({argsString})");

            return null;
        }

        private (string processed, int byteCount)  ProcessEscapeSequencesWithCount(string str){
            var result = new StringBuilder();
            int byteCount = 0;

            for(int i = 0; i < str.Length; i++){
                if(str[i] == '\\' && i + 1 < str.Length){
                    char next = str[i+1];
                    switch(next){
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
                else{
                    result.Append(str[i]);
                    byteCount++;
                }
            }
            return (result.ToString(), byteCount);
        }

    }
}