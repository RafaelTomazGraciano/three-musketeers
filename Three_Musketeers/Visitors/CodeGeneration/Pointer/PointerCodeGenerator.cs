using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Pointer
{

    public class PointerCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly StringBuilder declarations;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;

        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public PointerCodeGenerator(Func<StringBuilder> getCurrentBody, StringBuilder declarations, Dictionary<string, string> registerTypes, Func<string> nextRegister, Func<string, string> getLLVMType, Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.getCurrentBody = getCurrentBody;
            this.declarations = declarations;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitExpression = visitExpression;
        }

        public string VisitExprAddress(ExprParser.ExprAddressContext context)
        {
            var exprCtx = context.expr();
        
            // Caso: &ID (variável simples)
            if (exprCtx is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                Variable var = variables[varName];

                string baseType = var.LLVMType;
                string pointerType = baseType + "*";

                registerTypes[var.register] = pointerType;
                return var.register;
            }

            // Caso: &(*expr) - simplifica para expr
            if (exprCtx is ExprParser.ExprDerrefContext derrefCtx)
            {
                return visitExpression(derrefCtx.expr());
            }

            // Caso: &(array[i]) - getelementptr
            if (exprCtx is ExprParser.VarArrayContext arrayCtx)
            {
                // Retorna o endereço calculado do elemento
                // (assumindo que você já tem lógica para arrays)
                string arrayReg = visitExpression(exprCtx);
                return arrayReg; // O GEP já retorna um ponteiro
            }

            // Outros casos: já retornam endereços válidos
            return visitExpression(exprCtx);
        }

        public string VisitExprDerref(ExprParser.ExprDerrefContext context)
        {
            string reg = visitExpression(context.expr());

            string pointerType = registerTypes[reg];
            string baseType = pointerType.Substring(0, pointerType.Length - 1);
            string result = nextRegister();

            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = load {baseType}, {pointerType} {reg}");
            registerTypes[result] = baseType;
            return result;
        }
    }
}