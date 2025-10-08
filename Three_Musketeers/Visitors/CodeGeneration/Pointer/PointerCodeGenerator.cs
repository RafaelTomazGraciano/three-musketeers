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

        public string VisitVarAddress(ExprParser.VarAddressContext context)
        {
            string varName = context.ID().GetText();
            Variable var = variables[varName];

            string baseType = var.LLVMType;
            string pointerType = baseType + "*";
            string varLLVMName = var.register;

            registerTypes[varLLVMName] = pointerType;
            return varLLVMName;
        }

        public string VisitVarDerref(ExprParser.VarDerrefContext context)
        {
            string varName = context.ID().GetText();
            Variable var = variables[varName];
    
            string varLLVMName = var.register;
            string pointerType = var.LLVMType;  // Ex: "i32*"
    
            // Remove o * final para obter o tipo base
            string baseType = pointerType.Substring(0, pointerType.Length - 1);  // Ex: "i32"
    
            // Primeiro load: carrega o ponteiro armazenado em alloca
            string loadedReg = nextRegister();
            getCurrentBody().AppendLine($"  {loadedReg} = load {pointerType}, {pointerType}* {varLLVMName}");
    
            // Segundo load: carrega o valor apontado pelo ponteiro
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = load {baseType}, {pointerType} {loadedReg}");
    
            registerTypes[resultReg] = baseType;
            return resultReg;
        }
    }
}