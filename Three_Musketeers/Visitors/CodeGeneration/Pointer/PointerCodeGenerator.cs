using System.Text;
using LLVMSharp;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Pointer
{

    public class PointerCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string?> getCurrentFunctionName;

        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public PointerCodeGenerator(
        Func<StringBuilder> getCurrentBody, 
        Dictionary<string, Variable> variables, 
        Dictionary<string, string> registerTypes, 
        Func<string> nextRegister, 
        Func<ExprParser.ExprContext, string> visitExpression,
        Func<string?> getCurrentFunctionName)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.variables = variables;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.getCurrentFunctionName = getCurrentFunctionName;
        }

        public string VisitExprAddress(ExprParser.ExprAddressContext context)
        {
            var exprCtx = context.expr();
        
            // Case: &ID
            if (exprCtx is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                Variable var = GetVariableWithScope(varName)!;

                string pointerType = var.LLVMType + "*";
                registerTypes[var.register] = pointerType;

                return var.register;
            }

            // Case: &(*expr)
            if (exprCtx is ExprParser.DerrefExprContext derrefCtx)
            {
                return visitExpression(derrefCtx.derref().expr());
            }

            // Case: &(array[i]) - getelementptr
            if (exprCtx is ExprParser.VarArrayContext arrayCtx)
            {
                string arrayReg = visitExpression(exprCtx);
                return arrayReg;
            }

            return visitExpression(exprCtx);
        }

        public string VisitDerref(ExprParser.DerrefContext context)
        {
            string reg = visitExpression(context.expr());

            string pointerType = registerTypes[reg];
            string baseType = pointerType.Substring(0, pointerType.Length - 1);
            string result = nextRegister();

            getCurrentBody().AppendLine($"  {result} = load {baseType}, {pointerType} {reg}");
            registerTypes[result] = baseType;
            return result;
        }

        public string? VisitDerrefAtt(ExprParser.DerrefAttContext context)
        {
            string pointerReg = visitExpression(context.derref().expr());
            string expr = visitExpression(context.expr());
            string pointerType = registerTypes[pointerReg];

            string baseType = pointerType.EndsWith('*')
            ? pointerType.Substring(0, pointerType.Length - 1)
            : pointerType;

            getCurrentBody().AppendLine($"  store {baseType} {expr}, {pointerType} {pointerReg}");

            return null;
        }
        
        private Variable? GetVariableWithScope(string varName)
        {
            string? currentFunc = getCurrentFunctionName();
            
            if (currentFunc != null)
            {
                string scopedName = $"@{currentFunc}.{varName}";
                if (variables.ContainsKey(scopedName))
                {
                    return variables[scopedName];
                }
            }
            
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }
            
            return null;
        }
    }
}