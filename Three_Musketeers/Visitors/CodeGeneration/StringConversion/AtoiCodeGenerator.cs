using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.StringConversion
{
    public class AtoiCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly StringBuilder mainBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private bool atoiInitialized = false;

        public AtoiCodeGenerator(
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

        public string VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            InitializeAtoi();
            string? strPtr = visitExpression(context.expr());
            string resultReg = nextRegister();
            mainBody.AppendLine($"  {resultReg} = call i32 @atoi(i8* {strPtr})");
            registerTypes[resultReg] = "i32";
            return resultReg;
        }

        private void InitializeAtoi()
        {
            if (atoiInitialized)
                return;
                
            declarations.AppendLine("declare i32 @atoi(i8*)");
            atoiInitialized = true;
        }
    }
}