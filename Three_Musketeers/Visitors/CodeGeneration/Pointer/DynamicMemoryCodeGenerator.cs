using System.Diagnostics.CodeAnalysis;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Pointer
{
    class DynamicMemoryCodeGenerator
    {
        private Func<StringBuilder> getCurrentBody;
        private Dictionary<string, Variable> variables;
        private StringBuilder declarations;
        private Dictionary<string, string> registerTypes;
        private Func<string> nextRegister;
        private Func<ExprParser.ExprContext, string> visitExpression;
        private Func<string, int> getAlignment;
        private Func<string, string> getLLVMType;

        private bool isDeclared;

        public DynamicMemoryCodeGenerator(Func<StringBuilder> getCurrentBody, Dictionary<string, Variable> variables, StringBuilder declarations, Dictionary<string, string> registerTypes, Func<string> nextRegister, Func<ExprParser.ExprContext, string> visitExpression, Func<string, int> getAlignment, Func<string, string> getLLVMType)
        {
            this.getCurrentBody = getCurrentBody;
            this.variables = variables;
            this.declarations = declarations;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.getAlignment = getAlignment;
            this.getLLVMType = getLLVMType;
            isDeclared = false;
        }

        public string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            var mainBody = getCurrentBody();
            declare();

            string varName = context.ID().GetText();
            string expr = visitExpression(context.expr());
            var type = context.type();
            string varType;
            string register;
            string llvmType;
            string bitcastReg = nextRegister();
            string mallocReg = nextRegister();
            string exprType = registerTypes[expr];
            string mulReg = nextRegister();
            int size = 0;
            mainBody.AppendLine($"   {bitcastReg} = bitcast {exprType} {expr} to i64");

            if (type != null)
            {
                varType = type.GetText();
                llvmType = getLLVMType(varType);
                size = getAlignment(llvmType);
                register = nextRegister();
                size = getAlignment(llvmType);
                llvmType = $"{llvmType}{context.POINTER().Aggregate("", (a, b) => a + b.GetText())}";
                mainBody.AppendLine($"  {mulReg} = i64 mul {bitcastReg}, {size}");
                mainBody.AppendLine($" {mallocReg} = call i8* @malloc({mulReg})");
                mainBody.AppendLine($" {register} = bitcast i8* {mallocReg} to {llvmType}");
                registerTypes[register] = llvmType;
                variables[varName] = new Variable(varName, varType, llvmType, register);
                return null;
            }
            Variable variable = variables[varName];
            register = variable.register;
            llvmType = variable.LLVMType;
            size = getAlignment(llvmType);
            mainBody.AppendLine($"  {mulReg} = i64 mul {bitcastReg}, {size}");
            mainBody.AppendLine($" {mallocReg} = call i8* @malloc({mulReg})");
            mainBody.AppendLine($" {register} = bitcast i8* {mallocReg} to {llvmType}");
            return null;
        }

        public string? VisitFreeStatment([NotNull] ExprParser.FreeStatementContext context)
        {
            var mainBody = getCurrentBody();
            declare();
            Variable variable = variables[context.ID().GetText()];
            string bitcastReg = nextRegister();
            mainBody.AppendLine($"  {bitcastReg} = bitcast {variable.LLVMType} {variable.register} to i8*");
            mainBody.AppendLine($"  call void @free({bitcastReg})");
            return null;
        }
        
        private void declare()
        {
            if (!isDeclared)
            {
                declarations.AppendLine("declare i8* @malloc(i64)");
                declarations.AppendLine("declare void @free(i8*)");
            }
        }
    }
}