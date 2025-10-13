using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class AtodCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private bool atodInitialized = false;

        public AtodCodeGenerator(
            StringBuilder declarations,
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.declarations = declarations;
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
        }

        public string VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            InitializeAtod();

            string? strPtr = visitExpression(context.expr());
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = call double @atof(i8* {strPtr})");
            registerTypes[resultReg] = "double";
            return resultReg;
        }

        private void InitializeAtod()
        {
            if (atodInitialized)
                return;
                
            declarations.AppendLine("declare double @atof(i8*)");
            atodInitialized = true;
        }
    }
}