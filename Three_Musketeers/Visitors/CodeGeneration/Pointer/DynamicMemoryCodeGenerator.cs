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
        private Func<string, int> getSize;
        private Func<string, string> getLLVMType;
        private Func<string?> getCurrentFunctionName;
        private VariableResolver variableResolver;
        private Dictionary<string, Variable> variables; 
        private Func<ExprParser.StructGetContext, string> visitStructGet;

        public DynamicMemoryCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string> visitExpression,
            Func<string, int> getAlignment,
            Func<string, int> getSize,
            Func<string, string> getLLVMType,
            Func<string?> getCurrentFunctionName,
            VariableResolver variableResolver,
            Dictionary<string, Variable> variables,
            Func<ExprParser.StructGetContext, string> visitStructGet 
        ) 
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.getAlignment = getAlignment;
            this.getSize = getSize;
            this.getLLVMType = getLLVMType;
            this.getCurrentFunctionName = getCurrentFunctionName;
            this.variableResolver = variableResolver;
            this.variables = variables;
            this.visitStructGet = visitStructGet;
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

            // Declaration with type 
            if (type != null)
            {
                varType = type.GetText();
                llvmType = getLLVMType(varType);
                size = getSize(llvmType);
                
                // Ensure proper allocation size for structs (safety margin for calculation errors)
                if (llvmType.StartsWith("%") && size < 32)
                {
                    size = 32;
                }
                
                register = nextRegister();

                // Build LLVM type with pointers
                llvmType = $"{llvmType}{context.POINTER().Aggregate("", (a, b) => a + b.GetText())}";

                string? currentFunc = getCurrentFunctionName();
                string actualVarName = currentFunc != null ? $"@{currentFunc}.{varName}" : varName;

                // Use correct alignment for the pointer type (always 8 for pointers)
                mainBody.AppendLine($"  {register} = alloca {llvmType}, align 8");
                mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
                mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
                
                mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {llvmType}");
                // Pointer types always use align 8
                mainBody.AppendLine($"  store {llvmType} {bitcastReg}, {llvmType}* {register}, align 8");

                registerTypes[register] = llvmType + "*";
                variables[actualVarName] = new Variable(varName, varType, llvmType, register);
                return null;
            }

            // Case 2: Assignment to existing variable 
            Variable? variable = variableResolver.GetVariable(varName);

            if (variable == null)
            {
                throw new Exception($"Variable '{varName}' not found in current scope");
            }

            var indexes = context.index();

            // Handle array element assignment
            if (indexes != null && indexes.Length > 0)
            {
                // This is an array of pointers
                if (variable is ArrayVariable arrayVar)
                {
                    string elementType = arrayVar.innerType; 
                    string arrBaseType = CodeGeneratorBase.RemoveOneAsterisk(elementType);
                    size = getSize(arrBaseType);
                    
                    mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
                    mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
                    mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {elementType}");
                    
                    // Get pointer to array element
                    string indexExpr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {variable.register}, i32 0, i32 {indexExpr}");
                    
                    //Pointer types always use align 8
                    mainBody.AppendLine($"  store {elementType} {bitcastReg}, {elementType}* {gepReg}, align 8");
                    
                    return null;
                }
                else
                {
                    throw new Exception($"Variable '{varName}' is not an array but is being indexed in malloc assignment");
                }
            }

            // Handle simple pointer assignment
            register = variable.register;
            llvmType = variable.LLVMType;

            // Remove the '*' to calculate the base type size
            string baseType = CodeGeneratorBase.RemoveOneAsterisk(llvmType);
            size = getSize(baseType);
            
            // Ensure proper allocation size for structs (safety margin for calculation errors)
            if (baseType.StartsWith("%") && size < 32)
            {
                size = 32;  // Reasonable default for struct allocations
            }

            mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
            mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
            mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {llvmType}");
            // Pointer types always use align 8
            mainBody.AppendLine($"  store {llvmType} {bitcastReg}, {llvmType}* {register}, align 8");

            return null;
        }

        public string? VisitMallocStructAtt([NotNull] ExprParser.MallocStructAttContext context)
        {
            var mainBody = getCurrentBody();
            
            // Get the struct member pointer register
            string memberPtrReg = visitStructGet(context.structGet());
            
            if (!registerTypes.ContainsKey(memberPtrReg))
            {
                throw new Exception($"Register type not found for struct member in malloc");
            }
            
            string memberPtrType = registerTypes[memberPtrReg];
            string memberType = CodeGeneratorBase.RemoveOneAsterisk(memberPtrType);
            
            // Evaluate the size expression
            string expr = visitExpression(context.expr());
            string i64ToUse = CastToI64(expr, registerTypes[expr]);
            
            // Calculate size to allocate
            string baseType = CodeGeneratorBase.RemoveOneAsterisk(memberType);
            int size = getSize(baseType);
            
            string mulReg = nextRegister();
            string mallocReg = nextRegister();
            string bitcastReg = nextRegister();
            
            mainBody.AppendLine($"  {mulReg} = mul i64 {i64ToUse}, {size}");
            mainBody.AppendLine($"  {mallocReg} = call i8* @malloc(i64 {mulReg})");
            mainBody.AppendLine($"  {bitcastReg} = bitcast i8* {mallocReg} to {memberType}");
            mainBody.AppendLine($"  store {memberType} {bitcastReg}, {memberPtrType} {memberPtrReg}, align 8");
            
            return null;
        }

        public string? VisitFreeStatment([NotNull] ExprParser.FreeStatementContext context)
        {
            var mainBody = getCurrentBody();
            string loadReg;
            string bitcastReg;
            
            // Check if it's a struct/union member access (e.g., ptr.iptr)
            var structGet = context.structGet();
            if (structGet != null)
            {
                // Visit the structGet to get the pointer register
                string memberPtrReg = visitStructGet(structGet);
                
                if (!registerTypes.ContainsKey(memberPtrReg))
                {
                    throw new Exception($"Register type not found for struct member pointer");
                }
                
                string memberPtrType = registerTypes[memberPtrReg];
                string memberType = CodeGeneratorBase.RemoveOneAsterisk(memberPtrType);
                
                // Load the pointer value from the struct member
                loadReg = nextRegister();
                mainBody.AppendLine($"  {loadReg} = load {memberType}, {memberPtrType} {memberPtrReg}, align {getAlignment(memberPtrType)}");
                
                // Cast to i8* for free
                bitcastReg = nextRegister();
                mainBody.AppendLine($"  {bitcastReg} = bitcast {memberType} {loadReg} to i8*");
                
                // Call free
                mainBody.AppendLine($"  call void @free(i8* {bitcastReg})");
                
                return null;
            }
            
            // Handle regular variable or array element
            string varName = context.ID().GetText();
            var indexes = context.index();

            Variable variable = variableResolver.GetVariable(varName)!;
            
            // Handle free(ptrs[i])
            if (indexes != null && indexes.Length > 0)
            {
                if (variable is ArrayVariable arrayVar)
                {
                    string indexExpr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {variable.register}, i32 0, i32 {indexExpr}");
                    
                    loadReg = nextRegister();
                    mainBody.AppendLine($"  {loadReg} = load {arrayVar.innerType}, {arrayVar.innerType}* {gepReg}, align {getAlignment(arrayVar.innerType + "*")}");
                    
                    bitcastReg = nextRegister();
                    mainBody.AppendLine($"  {bitcastReg} = bitcast {arrayVar.innerType} {loadReg} to i8*");
                    mainBody.AppendLine($"  call void @free(i8* {bitcastReg})");
                    
                    return null;
                }
            }
            
            // Handle free(ptr)
            loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {getAlignment(variable.LLVMType + "*")}");
            
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