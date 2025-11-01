using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class PutsCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly StringBuilder mainBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<StringBuilder?> getCurrentBody;
        private readonly VariableResolver variableResolver;
        private bool putsInitialized = false;
        private readonly Func<ExprParser.IndexContext, string> calculateArrayPosition;

        public PutsCodeGenerator(
            StringBuilder declarations,
            StringBuilder mainBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<StringBuilder?> getCurrentBody,
            VariableResolver variableResolver,
            Func<ExprParser.IndexContext, string> calculateArrayPosition
            )
        {
            this.declarations = declarations;
            this.mainBody = mainBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.getCurrentBody = getCurrentBody;
            this.calculateArrayPosition = calculateArrayPosition;
        }

        public string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            InitializePuts();

            StringBuilder body = getCurrentBody() ?? mainBody;

            //puts(ID) or puts(ID[index])
            if (context.ID() != null)
            {
                string varName = context.ID().GetText();

                Variable variable = variableResolver.GetVariable(varName);
                bool hasIndexAccess = context.index() != null;

                if (hasIndexAccess)
                {
                    GenerateArrayElementPuts(variable!, context.index(), body);
                }
                else
                {
                    GenerateWholeVariablePuts(variable!, body);
                }

                return null;
            }

            // puts(STRING_LITERAL)
            if (context.STRING_LITERAL() != null)
            {
                GenerateStringLiteralPuts(context.STRING_LITERAL().GetText(), body);
                return null;
            }

            return null;
        }
        
        private void GenerateArrayElementPuts(Variable variable, ExprParser.IndexContext indexContext, StringBuilder body)
        {
            string indexValue = calculateArrayPosition(indexContext);

            if (registerTypes.ContainsKey(variable.register) && 
                registerTypes[variable.register] == "i8***")
            {
                string argvLoaded = nextRegister();
                body.AppendLine($"  {argvLoaded} = load i8**, i8*** {variable.register}");

                string argvElementPtr = nextRegister();
                body.AppendLine($"  {argvElementPtr} = getelementptr inbounds i8*, i8** {argvLoaded}, i32 {indexValue}");

                string stringPtr = nextRegister();
                body.AppendLine($"  {stringPtr} = load i8*, i8** {argvElementPtr}");

                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {stringPtr})");
            }
            else
            {
                string ptrReg = nextRegister();
                
                if (variable.LLVMType.Contains("["))
                {
                    body.AppendLine(
                        $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, i32 {indexValue}");
                }
                else
                {
                    body.AppendLine(
                        $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 {indexValue}");
                }

                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {ptrReg})");
            }
        }

            private void GenerateWholeVariablePuts(Variable variable, StringBuilder body)
        {
            string ptrReg = nextRegister();

            if (variable.LLVMType.Contains("["))
            {
                body.AppendLine(
                    $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, i32 0");
            }
            else
            {
                body.AppendLine(
                    $"  {ptrReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}");
            }

            string resultReg = nextRegister();
            body.AppendLine($"  {resultReg} = call i32 @puts(i8* {ptrReg})");
        }

        private void GenerateStringLiteralPuts(string literal, StringBuilder body)
        {
            literal = literal.Substring(1, literal.Length - 2)
                .Replace("\\n", "\\0A")
                .Replace("\\t", "\\09")
                .Replace("\\r", "\\0D")
                .Replace("\\\\", "\\5C")
                .Replace("\\\"", "\\22");

            string strLabel = $"@.str.puts.{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            int length = literal.Length + 1;

            declarations.AppendLine($"{strLabel} = private unnamed_addr constant [{length} x i8] c\"{literal}\\00\"");

            string strPtr = nextRegister();
            body.AppendLine($"  {strPtr} = getelementptr inbounds [{length} x i8], [{length} x i8]* {strLabel}, i32 0, i32 0");

            string resultReg = nextRegister();
            body.AppendLine($"  {resultReg} = call i32 @puts(i8* {strPtr})");
        }

        private void InitializePuts()
        {
            if (putsInitialized)
                return;

            declarations.AppendLine("declare i32 @puts(i8*)");
            putsInitialized = true;
        }
    }
} 