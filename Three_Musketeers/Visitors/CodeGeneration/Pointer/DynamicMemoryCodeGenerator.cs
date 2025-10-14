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
        private Func<string?> getCurrentFunctionName;
        private Func<string, Variable?> getVariableWithScope;

        private bool isDeclared;

        public DynamicMemoryCodeGenerator(
            Func<StringBuilder> getCurrentBody, 
            Dictionary<string, Variable> variables, 
            StringBuilder declarations, 
            Dictionary<string, string> registerTypes, 
            Func<string> nextRegister, 
            Func<ExprParser.ExprContext, string> visitExpression, 
            Func<string, int> getAlignment, 
            Func<string, string> getLLVMType, 
            Func<string?> getCurrentFunctionName,
            Func<string, Variable?> getVariableWithScope)
        {
            this.getCurrentBody = getCurrentBody;
            this.variables = variables;
            this.declarations = declarations;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.getAlignment = getAlignment;
            this.getLLVMType = getLLVMType;
            this.getCurrentFunctionName = getCurrentFunctionName;
            this.getVariableWithScope = getVariableWithScope;
            isDeclared = false;
        }

        public string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            var mainBody = getCurrentBody();
            Declare();

            string varName = context.ID().GetText();
            string expr = visitExpression(context.expr());
            var type = context.type();
            string varType;
            string register;
            string llvmType;
            string i64ToUse = CastToI64(expr, registerTypes[expr]);
            string mulReg = nextRegister();
            string mallocReg = nextRegister();
            string bitcastReg = nextRegister();
            int size = 0;

            // Declaration with type (int *pointer = malloc(5))
            if (type != null)
            {
                varType = type.GetText();
                llvmType = getLLVMType(varType);
                size = getAlignment(llvmType);
                register = nextRegister();

                // Build LLVM type with pointers
                llvmType = $"{llvmType}{context.POINTER().Aggregate("", (a, b) => a + b.GetText())}";

                string? currentFunc = getCurrentFunctionName();
                string actualVarName = currentFunc != null ? $"@{currentFunc}.{varName}" : varName;

                mainBody.AppendLine($"  {register} = alloca {llvmType}, align 8");
                mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
                mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
                
                mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {llvmType}");
                mainBody.AppendLine($"  store {llvmType} {bitcastReg}, {llvmType}* {register}, align 8");

                registerTypes[register] = llvmType + "*";
                variables[actualVarName] = new Variable(varName, varType, llvmType, register);
                return null;
            }

            // Case 2: Assignment to existing variable (pointer = malloc(5))

             Variable? variable = getVariableWithScope(varName);
    
            if (variable == null)
            {
                throw new Exception($"Variable '{varName}' not found in current scope");
            }
            
            register = variable.register;
            llvmType = variable.LLVMType;
            
            // Remove the '*' to calculate the base type size
            string baseType = llvmType.TrimEnd('*');
            size = getAlignment(baseType);

            mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
            mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
            mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {llvmType}");
            mainBody.AppendLine($"  store {llvmType} {bitcastReg}, {llvmType}* {register}, align 8");
            
            return null;
        }

        public string? VisitFreeStatment([NotNull] ExprParser.FreeStatementContext context)
        {
            var mainBody = getCurrentBody();
            Declare();

            string varName = context.ID().GetText();

            Variable? variable = getVariableWithScope(varName)!;
            
            // load the pointer value first
            string loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}");
            
            // bitcast the loaded value
            string bitcastReg = nextRegister();
            mainBody.AppendLine($"  {bitcastReg} = bitcast {variable.LLVMType} {loadReg} to i8*");
            mainBody.AppendLine($"  call void @free(i8* {bitcastReg})");
            
            return null;
        }

        private void Declare()
        {
            if (!isDeclared)
            {
                isDeclared = true;
                declarations.AppendLine("declare i8* @malloc(i64)");
                declarations.AppendLine("declare void @free(i8*)");
            }
        }
        
        private string CastToI64(string expr, string exprType)
        {
            string resultReg = nextRegister();
            var mainBody = getCurrentBody();
            
            if (exprType.Contains('*'))
            {
                mainBody.AppendLine($"  {resultReg} = ptrtoint {exprType} {expr} to i64");
                return resultReg;
            }

            if (exprType == "double")
            {
                mainBody.AppendLine($"  {resultReg} = fptosi double {expr} to i64");
                return resultReg;
            }

            mainBody.AppendLine($"  {resultReg} = sext {exprType} {expr} to i64");

            return resultReg;
        }
    }
}