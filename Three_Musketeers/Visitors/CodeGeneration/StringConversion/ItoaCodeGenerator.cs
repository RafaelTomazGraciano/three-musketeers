using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class ItoaCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly StringBuilder mainBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private bool itoaInitialized = false;

        public ItoaCodeGenerator(
            StringBuilder declarations,
            StringBuilder mainBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.declarations = declarations;
            this.mainBody = mainBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
        }

        public string VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
            InitializeItoa();
            string? intValue = visitExpression(context.expr());
            //Allocate buffer for string (20 bytes is enough for int)
            string bufferReg = nextRegister();
            mainBody.AppendLine($"  {bufferReg} = alloca [20 x i8], align 1");
            //pointer to buffer
            string bufferPtr = nextRegister();
            mainBody.AppendLine($"  {bufferPtr} = getelementptr inbounds [20 x i8], [20 x i8]* {bufferReg}, i32 0, i32 0");
            //sprintf(buffer, "%d", value)
            string resultReg = nextRegister();
            mainBody.AppendLine($"  {resultReg} = call i32 (i8*, i8*, ...) @sprintf(i8* {bufferPtr}, i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.fmt.d, i32 0, i32 0), i32 {intValue})");
            
            registerTypes[bufferPtr] = "i8*";
            return bufferPtr;
        }

        private void InitializeItoa()
        {
            if (itoaInitialized)
                return;
                
            declarations.AppendLine("declare i32 @sprintf(i8*, i8*, ...)");
            declarations.AppendLine("@.fmt.d = private unnamed_addr constant [3 x i8] c\"%d\\00\", align 1");
            itoaInitialized = true;
        }
    }
}