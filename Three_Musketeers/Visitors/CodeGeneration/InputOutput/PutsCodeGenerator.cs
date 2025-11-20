using Antlr4.Runtime.Misc;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.CodeGeneration.CompilerDirectives;

namespace Three_Musketeers.Visitors.CodeGeneration.InputOutput
{
    public class PutsCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly VariableResolver variableResolver;
        private readonly Func<ExprParser.IndexContext, string> calculateArrayPosition;
        private readonly DefineCodeGenerator defineCodeGenerator;
        private readonly Func<ExprParser.StructGetContext, string> visitStructGet;

        public PutsCodeGenerator(
            StringBuilder declarations, 
            Func<StringBuilder> getCurrentBody, 
            Dictionary<string, string> registerTypes, 
            Func<string> nextRegister, 
            VariableResolver variableResolver, 
            DefineCodeGenerator defineCodeGenerator, 
            Func<ExprParser.IndexContext, string> calculateArrayPosition,
            Func<ExprParser.StructGetContext, string> visitStructGet)
        {
            this.declarations = declarations;
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.variableResolver = variableResolver;
            this.defineCodeGenerator = defineCodeGenerator;
            this.calculateArrayPosition = calculateArrayPosition;
            this.visitStructGet = visitStructGet;
        }

        public string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            StringBuilder body = getCurrentBody();

            // puts(STRING_LITERAL)
            if (context.STRING_LITERAL() != null)
            {
                GenerateStringLiteralPuts(context.STRING_LITERAL().GetText(), body);
                return null;
            }

            // puts(structGet) - e.g., puts(unionTest.ms.a)
            if (context.structGet() != null)
            {
                GenerateStructGetPuts(context.structGet(), body);
                return null;
            }

            // puts(ID) or puts(ID[index])
            if (context.ID() != null)
            {
                string varName = context.ID().GetText();

                // Check if it's a #define constant
                if (defineCodeGenerator.IsDefine(varName))
                {
                    string? defineValue = defineCodeGenerator.GetDefineValue(varName);
                    
                    if (defineValue != null && defineValue.StartsWith("\"") && defineValue.EndsWith("\""))
                    {
                        GenerateStringLiteralPuts(defineValue, body);
                        return null;
                    }
                }

                Variable variable = variableResolver.GetVariable(varName)!;
                bool hasIndexAccess = context.index() != null;

                if (hasIndexAccess)
                {
                    GenerateArrayElementPuts(variable, context.index(), body);
                }
                else
                {
                    GenerateWholeVariablePuts(variable, body);
                }

                return null;
            }

            return null;
        }

        private void GenerateStructGetPuts(ExprParser.StructGetContext structGetCtx, StringBuilder body)
        {
            // VisitStructGet returns a pointer to the member
            string memberPtrReg = visitStructGet(structGetCtx);
            
            if (memberPtrReg == null || !registerTypes.ContainsKey(memberPtrReg))
            {
                throw new Exception($"Failed to resolve struct member in puts()");
            }

            string memberPtrType = registerTypes[memberPtrReg]; // This is the POINTER type (e.g., "[256 x i8]*")
            
            // Remove the pointer to get the actual member type
            string memberType = memberPtrType.EndsWith("*") 
                ? memberPtrType.Substring(0, memberPtrType.Length - 1) 
                : memberPtrType;

            // Check if it's a string array (char[256])
            if (memberType == "[256 x i8]")
            {
                // Get pointer to first element
                string ptrReg = nextRegister();
                body.AppendLine($"  {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {memberPtrReg}, i32 0, i32 0");
                
                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {ptrReg})");
            }
            else if (memberType.StartsWith("[") && memberType.Contains("i8"))
            {
                // Other char array sizes
                string ptrReg = nextRegister();
                body.AppendLine($"  {ptrReg} = getelementptr inbounds {memberType}, {memberType}* {memberPtrReg}, i32 0, i32 0");
                
                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {ptrReg})");
            }
            else if (memberType == "i8*")
            {
                // String pointer - load it first
                string loadedPtr = nextRegister();
                body.AppendLine($"  {loadedPtr} = load i8*, i8** {memberPtrReg}, align 8");
                
                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {loadedPtr})");
            }
            else
            {
                // Should not reach here if semantic analysis is correct
                throw new Exception($"Invalid type '{memberType}' for puts()");
            }
        }
        
        private void GenerateArrayElementPuts(Variable variable, ExprParser.IndexContext indexContext, StringBuilder body)
        {
            string indexValue = calculateArrayPosition(indexContext);

            // Special case for argv (i8***)
            if (registerTypes.ContainsKey(variable.register) && 
                registerTypes[variable.register] == "i8***")
            {
                string argvLoaded = nextRegister();
                body.AppendLine($"  {argvLoaded} = load i8**, i8*** {variable.register}");

                string argvElementPtr = nextRegister();
                body.AppendLine($"  {argvElementPtr} = getelementptr inbounds i8*, i8** {argvLoaded}, {indexValue}");

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
                        $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, {indexValue}");
                }
                else
                {
                    body.AppendLine(
                        $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, {indexValue}");
                }

                string resultReg = nextRegister();
                body.AppendLine($"  {resultReg} = call i32 @puts(i8* {ptrReg})");
            }
        }

        private void GenerateWholeVariablePuts(Variable variable, StringBuilder body)
        {
            string ptrReg = nextRegister();

            // Char array - get pointer to first element
            if (variable.LLVMType.Contains("[") && variable.LLVMType.Contains("i8"))
            {
                body.AppendLine(
                    $"  {ptrReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, i32 0");
            }
            // String pointer (i8*) - load the pointer
            else if (variable.LLVMType == "i8*")
            {
                body.AppendLine(
                    $"  {ptrReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align 8");
            }
            else
            {
                // Should not reach here if semantic analysis is correct
                throw new Exception($"Invalid type '{variable.LLVMType}' for puts()");
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
    }
}