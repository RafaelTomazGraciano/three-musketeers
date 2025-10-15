using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class DtoaCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public DtoaCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
        }

        public string VisitDtoaConversion([NotNull] ExprParser.DtoaConversionContext context)
        {
            string? doubleValue = visitExpression(context.expr());
            //Allocate buffer for the string (32 bytes is enough for double)
            string bufferReg = nextRegister();
            getCurrentBody().AppendLine($"  {bufferReg} = alloca [32 x i8], align 1");
            //pointer to buffer
            string bufferPtr = nextRegister();
            getCurrentBody().AppendLine($"  {bufferPtr} = getelementptr inbounds [32 x i8], [32 x i8]* {bufferReg}, i32 0, i32 0");
            //sprintf(buffer, "%lf", value)
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call i32 (i8*, i8*, ...) @sprintf(i8* {bufferPtr}, i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.fmt.lf, i32 0, i32 0), double {doubleValue})");
            
            registerTypes[bufferPtr] = "i8*";
            return bufferPtr;
        }
    }
}