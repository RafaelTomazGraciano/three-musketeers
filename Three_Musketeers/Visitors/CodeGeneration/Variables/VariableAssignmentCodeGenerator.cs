using Antlr4.Runtime.Misc;
using LLVMSharp;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Variables
{
    public class VariableAssignmentCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;
        private readonly Func<string?> getCurrentFunctionName;
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Func<string, int> GetAlignment;
        private readonly Func<ExprParser.IndexContext[], string> calculateArrayPosition;

        public VariableAssignmentCodeGenerator(
            StringBuilder declarations,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string> visitExpression,
            Func<string?> getCurrentFunctionName,
            Func<StringBuilder> getCurrentBody,
            Func<string, int> GetAlignment,
            Func<ExprParser.IndexContext[], string> calculateArrayPosition)
        {
            this.declarations = declarations;
            this.variables = variables;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitExpression = visitExpression;
            this.getCurrentFunctionName = getCurrentFunctionName;
            this.getCurrentBody = getCurrentBody;
            this.GetAlignment = GetAlignment;
            this.calculateArrayPosition = calculateArrayPosition;
        }

        public string? VisitGenericAtt([NotNull] ExprParser.GenericAttContext context)
        {
            string varType;
            string varName = context.ID().GetText();
            string llvmType;
            string register;
            string? currentFunc = getCurrentFunctionName();

            if (context.type() != null)
            {
                varType = context.type().GetText();
                llvmType = getLLVMType(varType);

                // Global variable
                if (currentFunc == null)
                {
                    string globalReg = $"@{varName}";

                    if (context.expr() != null)
                    {
                        // Evaluate expression first to get the value
                        string initValue = visitExpression(context.expr());
                        
                        if (varType == "string")
                        {
                            // String: store pointer directly in declaration
                            declarations.AppendLine($"{globalReg} = global i8* {initValue}, align 8");
                            variables[varName] = new Variable(varName, varType, "i8*", globalReg);
                        }
                        else
                        {
                            // Other types: store value directly
                            string valType = registerTypes[initValue];
                            declarations.AppendLine($"{globalReg} = global {valType} {initValue}, align {GetAlignment(valType)}");
                            variables[varName] = new Variable(varName, varType, valType, globalReg);
                        }
                    }
                    else
                    {
                        // No initialization - use zeroinitializer
                        if (varType == "string")
                        {
                            declarations.AppendLine($"{globalReg} = global i8* null, align 8");
                            variables[varName] = new Variable(varName, varType, "i8*", globalReg);
                        }
                        else
                        {
                            declarations.AppendLine($"{globalReg} = global {llvmType} zeroinitializer, align {GetAlignment(llvmType)}");
                            variables[varName] = new Variable(varName, varType, llvmType, globalReg);
                        }
                    }

                    return null;
                }
                
                // Local variable
                register = nextRegister();
                string actualVarName = $"@{currentFunc}.{varName}";

                if (varType == "string")
                {
                    WriteAlloca(register, "[256 x i8]", GetAlignment("i8"));
                    variables[actualVarName] = new Variable(varName, varType, "[256 x i8]", register);
                }
                else
                {
                    WriteAlloca(register, llvmType, GetAlignment(llvmType));
                    variables[actualVarName] = new Variable(varName, varType, llvmType, register);
                }
            }
            else
            {
                Variable variable = GetVariableWithScope(varName)!;
                varType = variable.type;
                llvmType = variable.LLVMType;
                register = variable.register;
            }

            if (varType == "string")
            {
                string exprResult = visitExpression(context.expr());

                if (registerTypes.TryGetValue(exprResult, out string? exprType) && exprType == "i8*")
                {
                    WriteStrCopy(register, exprResult);
                }
                return null;
            }

            string value = visitExpression(context.expr());
            
            // Check if we need to convert i1 to i32 (for bool variables)
            string valueType = registerTypes.ContainsKey(value) ? registerTypes[value] : llvmType;

            if (llvmType == "i32" && valueType == "i1")
            {
                // Convert i1 to i32 for bool variables
                string convertedReg = nextRegister();
                getCurrentBody().AppendLine($"  {convertedReg} = zext i1 {value} to i32");
                registerTypes[convertedReg] = "i32";
                value = convertedReg;
                valueType = "i32";
            }

            // Convert double to i32 (for int variables)
            if (llvmType == "i32" && valueType == "double")
            {
                string convertedReg = nextRegister();
                getCurrentBody().AppendLine($"  {convertedReg} = fptosi double {value} to i32");
                registerTypes[convertedReg] = "i32";
                value = convertedReg;
                valueType = "i32";
            }

            // Convert i32 to double (for double variables)
            if (llvmType == "double" && valueType == "i32")
            {
                string convertedReg = nextRegister();
                getCurrentBody().AppendLine($"  {convertedReg} = sitofp i32 {value} to double");
                registerTypes[convertedReg] = "double";
                value = convertedReg;
                valueType = "double";
            }

            getCurrentBody().AppendLine($"  store {llvmType} {value}, {llvmType}* {register}, align {GetAlignment(llvmType)}");
            return null;
        }

        private void WriteStrCopy(string register, string exprResult, int pos = 0)
        {
            if (!declarations.ToString().Contains("declare i8* @strcpy"))
            {
                declarations.AppendLine("declare i8* @strcpy(i8*, i8*)");
            }
            string destPtr = nextRegister();
            getCurrentBody().AppendLine($"  {destPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 {pos}");
            string srcPtr = nextRegister();
            getCurrentBody().AppendLine($"  {srcPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {exprResult}, i32 0, i32 0");

            string copyResult = nextRegister();
            getCurrentBody().AppendLine($"  {copyResult} = call i8* @strcpy(i8* {destPtr}, i8* {srcPtr})");
        }

        public string VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();
            Variable variable = GetVariableWithScope(varName)!;

            if (variable == null)
            {
                // Return a default value or throw a more meaningful error
                return "0";
            }

            if (variable is ArrayVariable arrayVar)
            {
                string ptrReg = nextRegister();
                string elementType = arrayVar.innerType;
                getCurrentBody().AppendLine($"  {ptrReg} = getelementptr inbounds {arrayVar.LLVMType}, {arrayVar.LLVMType}* {variable.register}, i32 0, i32 0");
                registerTypes[ptrReg] = elementType + "*";
                return ptrReg;
            }

            if (variable.isDirectPointerParam)
            {
                registerTypes[variable.register] = variable.LLVMType;
                return variable.register;
            }

            if (variable.type == "string")
            {
                if (variable.LLVMType == "[256 x i8]")
                {
                    string ptrReg = nextRegister();
                    getCurrentBody().AppendLine($"  {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");
                    registerTypes[ptrReg] = "i8*";
                    return ptrReg;
                }
                else if (variable.LLVMType == "i8*")
                {
                    string loadRegStr = nextRegister();
                    getCurrentBody().AppendLine($"  {loadRegStr} = load i8*, i8** {variable.register}, align 8");
                    registerTypes[loadRegStr] = "i8*";
                    return loadRegStr;
                }
            }

            string loadReg = nextRegister();

            //pinter handling
            if (variable.LLVMType.EndsWith("*"))
            {
                int alignment = GetAlignment(variable.LLVMType);
                getCurrentBody().AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {alignment}");
            }
            //regular types
            else
            {
                getCurrentBody().AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            }

            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }

        public string? VisitDec([NotNull] ExprParser.DeclarationContext context)
        {
            string varType = context.type().GetText();
            string varName = context.ID().GetText();

            bool isPointer = context.POINTER() != null && context.POINTER().Length > 0;
            string pointers = isPointer
                ? context.POINTER().Aggregate("", (a, b) => a + b.GetText())
                : "";

            string llvmType = getLLVMType(varType) + pointers;

            string? currentFunc = getCurrentFunctionName();
            bool isGlobal = currentFunc == null;
            string varKey = isGlobal ? varName : $"@{currentFunc}.{varName}";

            if (!isPointer && context.intIndex() != null && context.intIndex().Length > 0)
            {
                var indexes = context.intIndex();
                List<int> dimensions = new List<int>();

                foreach (var index in indexes)
                {
                    int size = int.Parse(index.INT().GetText());
                    dimensions.Add(size);
                }

                string arrayType = llvmType;
                for (int i = dimensions.Count - 1; i >= 0; i--)
                {
                    arrayType = $"[{dimensions[i]} x {arrayType}]";
                }

                string register = isGlobal ? $"@{varName}" : nextRegister();

                if (isGlobal)
                {
                    declarations.AppendLine($"{register} = global {arrayType} zeroinitializer, align {GetAlignment(llvmType)}");
                }
                else
                {
                    WriteAlloca(register, arrayType, GetAlignment(llvmType));
                }

                // Calculate total size for ArrayVariable
                int totalSize = 1;
                foreach (var dim in dimensions)
                {
                    totalSize *= dim;
                }

                variables[varKey] = new ArrayVariable(varName, varType, arrayType, register, totalSize, llvmType);
                return null;
            }

            // Handle string declarations (non-pointer, non-array)
            if (!isPointer && varType == "string")
            {
                string register = isGlobal ? $"@{varName}" : nextRegister();
                string actualLlvmType = isGlobal ? "i8*" : "[256 x i8]";
                int alignment = isGlobal ? 8 : GetAlignment("i8");

                if (isGlobal)
                {
                    declarations.AppendLine($"{register} = global {actualLlvmType} null, align {alignment}");
                }
                else
                {
                    WriteAlloca(register, actualLlvmType, alignment);
                }

                variables[varKey] = new Variable(varName, varType, actualLlvmType, register);
                return null;
            }

            // Handle regular variables, pointers, and structs
            string reg = isGlobal ? $"@{varName}" : nextRegister();
            int align = isPointer ? 8 : GetAlignment(llvmType);
            string initialValue = isPointer ? "null" : "zeroinitializer";

            if (isGlobal)
            {
                declarations.AppendLine($"{reg} = global {llvmType} {initialValue}, align {align}");
            }
            else
            {
                WriteAlloca(reg, llvmType, align);
                if (isPointer)
                {
                    registerTypes[reg] = llvmType + "*";
                }
            }

            variables[varKey] = new Variable(varName, varType, llvmType, reg);
            return null;
        }

        public Variable? GetVariableWithScope(string varName)
        {
            string? currentFunc = getCurrentFunctionName();

            // search local
            if (currentFunc != null)
            {
                string scopedName = $"@{currentFunc}.{varName}";
                if (variables.ContainsKey(scopedName))
                {
                    return variables[scopedName];
                }
            }

            // search global
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }

            return null;
        }


        private void WriteAlloca(string register, string type, int alignment)
        {
            getCurrentBody().AppendLine($"  {register} = alloca {type}, align {alignment}");
        }

        public string VisitVarArray(ExprParser.VarArrayContext context)
        {
            string name = context.ID().GetText();
            var indexes = context.index();
            var currentBody = getCurrentBody();
            Variable variable = GetVariableWithScope(name)!;

            var llvmType = variable.LLVMType;

            // Check if this is a pointer variable (not an array)
            bool isPointer = !(variable is ArrayVariable) && llvmType.Contains('*');

            if (isPointer)
            {

                string loadedPointer;
                if (variable.isDirectPointerParam)
                {
                    loadedPointer = variable.register;  
                }
                else
                {
                    loadedPointer = nextRegister();
                    currentBody.AppendLine($"  {loadedPointer} = load {llvmType}, {llvmType}* {variable.register}, align {GetAlignment(llvmType)}");
                }

                // Get element address with single index (no i32 0)
                string expr = visitExpression(indexes[0].expr());
                string gepReg = nextRegister();
                string elementType = llvmType.TrimEnd('*');
                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {llvmType} {loadedPointer}, i32 {expr}");

                // Load the value at this address
                string loadReg = nextRegister();
                currentBody.AppendLine($"  {loadReg} = load {elementType}, {elementType}* {gepReg}, align {GetAlignment(elementType)}");

                registerTypes[loadReg] = elementType;
                return loadReg;
            }
            else
            {
                // For arrays: use getelementptr with i32 0 as first index
                string result = calculateArrayPosition(indexes);
                string gepReg = nextRegister();
                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {llvmType}, {llvmType}* {variable.register}, i32 0, {result}");

                string elementType;
                if (variable is ArrayVariable arrayVariable)
                {
                    elementType = arrayVariable.innerType;
                }
                else
                {
                    elementType = llvmType;
                }

                // Load the value
                string loadReg = nextRegister();
                currentBody.AppendLine($"  {loadReg} = load {elementType}, {elementType}* {gepReg}, align {GetAlignment(elementType)}");

                registerTypes[loadReg] = elementType;
                return loadReg;
            }
        }

        public string? VisitSingleArrayAtt(ExprParser.SingleArrayAttContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            string exprValue = visitExpression(context.expr());
            Variable variable = GetVariableWithScope(varName)!;
            var currentBody = getCurrentBody();

            var llvmType = variable.LLVMType;

            if (variable == null)
            {
                return null;
            }

            string targetRegister;
            string targetType;

            // Check if this is a pointer variable
            bool isPointer = !(variable is ArrayVariable) && variable.LLVMType.Contains('*');

            if (indexes != null && indexes.Length > 0)
            {
                if (isPointer)
                {
                    string loadedPointer;
                    if (variable.isDirectPointerParam)
                    {
                        loadedPointer = variable.register; 
                    }
                    else
                    {
                        loadedPointer = nextRegister();
                        currentBody.AppendLine($"  {loadedPointer} = load {llvmType}, {llvmType}* {variable.register}, align {GetAlignment(llvmType)}");
                    }

                    // Get element address with single index (no i32 0)
                    string expr = visitExpression(indexes[0].expr());
                    string gepReg = nextRegister();
                    string elementType = llvmType.TrimEnd('*');
                    currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {llvmType} {loadedPointer}, i32 {expr}");
                    
                    targetRegister = gepReg;
                    targetType = elementType;
                }
                else
                {
                    // For arrays: GEP with i32 0 prefix
                    string position = calculateArrayPosition(indexes);
                    string gepReg = nextRegister();

                    if (variable is ArrayVariable arrayVar)
                    {
                        currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, i32 0, {position}");
                        targetType = arrayVar.innerType;
                    }
                    else
                    {
                        currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {variable.LLVMType}, {variable.LLVMType}* {variable.register}, {position}");
                        targetType = variable.LLVMType;
                    }

                    targetRegister = gepReg;
                }
            }
            else
            {
                // No index - direct assignment to variable
                targetRegister = variable.register;
                targetType = variable.LLVMType;
            }

            // Store the value
            string valueType = registerTypes.ContainsKey(exprValue) ? registerTypes[exprValue] : targetType;

            // Convert i1 to i32 if needed (for bool variables/arrays)
            if (targetType == "i32" && valueType == "i1")
            {
                string convertedReg = nextRegister();
                currentBody.AppendLine($"  {convertedReg} = zext i1 {exprValue} to i32");
                registerTypes[convertedReg] = "i32";
                exprValue = convertedReg;
                valueType = "i32";
            }

            // Convert i32 to i8 (for char arrays/variables)**
            if (targetType == "i8" && valueType == "i32")
            {
                string convertedReg = nextRegister();
                currentBody.AppendLine($"  {convertedReg} = trunc i32 {exprValue} to i8");
                registerTypes[convertedReg] = "i8";
                exprValue = convertedReg;
                valueType = "i8";
            }

            // Convert double to i32 (for int variables)
            if (llvmType == "i32" && valueType == "double")
            {
                string convertedReg = nextRegister();
                getCurrentBody().AppendLine($"  {convertedReg} = fptosi double {exprValue} to i32");
                registerTypes[convertedReg] = "i32";
                exprValue = convertedReg;
                valueType = "i32";
            }

            // Convert i32 to double (for double variables)
            if (llvmType == "double" && valueType == "i32")
            {
                string convertedReg = nextRegister();
                getCurrentBody().AppendLine($"  {convertedReg} = sitofp i32 {exprValue} to double");
                registerTypes[convertedReg] = "double";
                exprValue = convertedReg;
                valueType = "double";
            }

            currentBody.AppendLine($"  store {valueType} {exprValue}, {targetType}* {targetRegister}, align {GetAlignment(targetType)}");
            return null;
        }

        public string VisitVarArrayPointer(string varName, ExprParser.IndexContext[] indexes)
        {
            var currentBody = getCurrentBody();
            Variable variable = GetVariableWithScope(varName)!;

            if (variable == null)
            {
                return nextRegister(); // Error case
            }

            var llvmType = variable.LLVMType;
            bool isPointer = !(variable is ArrayVariable) && llvmType.Contains('*');

            if (isPointer)
            {

                string loadedPointer;
                if (variable.isDirectPointerParam)
                {
                    loadedPointer = variable.register; 
                }
                else
                {
                    loadedPointer = nextRegister();
                    currentBody.AppendLine($"  {loadedPointer} = load {llvmType}, {llvmType}* {variable.register}, align {GetAlignment(llvmType)}");
                }

                // Get element address with single index (no i32 0)
                string expr = visitExpression(indexes[0].expr());
                string gepReg = nextRegister();
                string elementType = llvmType.TrimEnd('*');
                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {llvmType} {loadedPointer}, i32 {expr}");

                registerTypes[gepReg] = elementType + "*";
                return gepReg;
            }
            else
            {
                // For arrays
                string result = calculateArrayPosition(indexes);
                string gepReg = nextRegister();
                currentBody.AppendLine($"  {gepReg} = getelementptr inbounds {llvmType}, {llvmType}* {variable.register}, i32 0, {result}");

                string elementType;
                if (variable is ArrayVariable arrayVariable)
                {
                    elementType = arrayVariable.innerType;
                }
                else
                {
                    elementType = llvmType;
                }

                registerTypes[gepReg] = elementType + "*";
                return gepReg;
            }
        }

    }
}