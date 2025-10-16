using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class AtodCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public AtodCodeGenerator(
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

        public string VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            string? strPtr = visitExpression(context.expr());
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call double @atof(i8* {strPtr})");
            registerTypes[resultReg] = "double";
            return resultReg;
        }
    }
}