using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class AtoiCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public AtoiCodeGenerator(
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

        public string VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            string? strPtr = visitExpression(context.expr());
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call i32 @atoi(i8* {strPtr})");
            registerTypes[resultReg] = "i32";
            return resultReg;
        }
    }
}