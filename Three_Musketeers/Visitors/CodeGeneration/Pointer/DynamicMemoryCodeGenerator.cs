using System.Diagnostics.CodeAnalysis;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Pointer
{
    public class DynamicMemoryCodeGenerator
    {
        private Func<StringBuilder> getCurrentBody;
        private Dictionary<string, string> registerTypes;
        private Func<string> nextRegister;
        private Func<ExprParser.ExprContext, string> visitExpression;
        private Func<string, int> getAlignment;
        private Func<string, string> getLLVMType;
        private Func<string?> getCurrentFunctionName;
        private VariableResolver variableResolver;
        private Dictionary<string, Variable> variables; 

        public DynamicMemoryCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string> visitExpression,
            Func<string, int> getAlignment,
            Func<string, string> getLLVMType,
            Func<string?> getCurrentFunctionName,
            VariableResolver variableResolver,
            Dictionary<string, Variable> variables
            ) 
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.getAlignment = getAlignment;
            this.getLLVMType = getLLVMType;
            this.getCurrentFunctionName = getCurrentFunctionName;
            this.variableResolver = variableResolver;
            this.variables = variables;
        }

        public string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            var mainBody = getCurrentBody();

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

            Variable? variable = variableResolver.GetVariable(varName);

            if (variable == null)
            {
                throw new Exception($"Variable '{varName}' not found in current scope");
            }

            var indexes = context.index();

            // Handle array element assignment: ptrs[i] = malloc(...)
            if (indexes != null && indexes.Length > 0)
            {
                // This is an array of pointers
                if (variable is ArrayVariable arrayVar)
                {
                    string elementType = arrayVar.innerType; // This should be the pointer type (e.g., i32*)
                    string arrBaseType = CodeGeneratorBase.RemoveOneAsterisk(elementType);
                    size = getAlignment(arrBaseType);
                    
                    mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
                    mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
                    mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {elementType}");
                    
                    // Get pointer to array element
                    string indexExpr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {variable.register}, i32 0, i32 {indexExpr}");
                    
                    // Store the malloc'd pointer in the array element
                    mainBody.AppendLine($"  store {elementType} {bitcastReg}, {elementType}* {gepReg}, align 8");
                    
                    return null;
                }
                else
                {
                    throw new Exception($"Variable '{varName}' is not an array but is being indexed in malloc assignment");
                }
            }

            // Handle simple pointer assignment: pointer = malloc(...)
            register = variable.register;
            llvmType = variable.LLVMType;

            // Remove the '*' to calculate the base type size
            string baseType = CodeGeneratorBase.RemoveOneAsterisk(llvmType);
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
            string varName = context.ID().GetText();
            var indexes = context.index();

            Variable variable = variableResolver.GetVariable(varName)!;

            string loadReg;
            string bitcastReg;
            
            // Handle free(ptrs[i])
            if (indexes != null && indexes.Length > 0)
            {
                if (variable is ArrayVariable arrayVar)
                {
                    string indexExpr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {variable.register}, i32 0, i32 {indexExpr}");
                    
                    loadReg = nextRegister();
                    mainBody.AppendLine($"  {loadReg} = load {arrayVar.innerType}, {arrayVar.innerType}* {gepReg}");
                    
                    bitcastReg = nextRegister();
                    mainBody.AppendLine($"  {bitcastReg} = bitcast {arrayVar.innerType} {loadReg} to i8*");
                    mainBody.AppendLine($"  call void @free(i8* {bitcastReg})");
                    
                    return null;
                }
            }
            
            // Handle free(ptr) - código existente
            loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}");
            
            bitcastReg = nextRegister();
            mainBody.AppendLine($"  {bitcastReg} = bitcast {variable.LLVMType} {loadReg} to i8*");
            mainBody.AppendLine($"  call void @free(i8* {bitcastReg})");
            
            return null;
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