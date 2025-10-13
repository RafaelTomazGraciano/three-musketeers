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

        public VariableAssignmentCodeGenerator(
            StringBuilder declarations,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string> visitExpression,
            Func<string?> getCurrentFunctionName,
            Func<StringBuilder> getCurrentBody,
            Func<string, int> GetAlignment)
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
        }

        public string? VisitGenericAtt([NotNull] ExprParser.GenericAttContext context)
        {
            string varType;
            string varName = context.ID().GetText();
            string llvmType;
            string register;

            if (context.type() != null)
            {
                varType = context.type().GetText();
                llvmType = getLLVMType(varType);
                register = nextRegister();

                if (varType == "string")
                {
                    WriteAlloca(register, "[256 x i8]", GetAlignment("i8"));
                    variables[varName] = new Variable(varName, varType, "[256 x i8]", register);
                }
                else
                {
                    WriteAlloca(register, llvmType, GetAlignment(llvmType));
                    variables[varName] = new Variable(varName, varType, llvmType, register);
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

                if (registerTypes.ContainsKey(exprResult) && registerTypes[exprResult] == "i8*")
                {
                    WriteStrCopy(register, exprResult);
                }
                return null;
            }

            string value = visitExpression(context.expr());
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

            if (variable.type == "string")
            {
                // Se é [256 x i8] (string local), faz getelementptr
        if (variable.LLVMType == "[256 x i8]")
        {
            string ptrReg = nextRegister();
            getCurrentBody().AppendLine($"  {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");
            registerTypes[ptrReg] = "i8*";
            return ptrReg;
        }
        // Se é i8* (parâmetro de string), apenas faz load
        else if (variable.LLVMType == "i8*")
        {
            string loadRegStr = nextRegister();
            getCurrentBody().AppendLine($"  {loadRegStr} = load i8*, i8** {variable.register}, align 8");
            registerTypes[loadRegStr] = "i8*";
            return loadRegStr;
        }
            }

            string loadReg = nextRegister();
            getCurrentBody().AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }

        public string? VisitDec([NotNull] ExprParser.BaseDecContext context)
        {
            string varType = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = getLLVMType(varType);
            string register = nextRegister();
            var indexes = context.index();

            if (context.index().Length > 0)
            {
                List<int> dimensions = new ArrayList<int>();
                int totalSize = 1;
                string arrayType;
                foreach (var index in indexes)
                {
                    int size = int.Parse(index.INT().GetText());
                    totalSize *= size;
                    dimensions.Add(size);
                }

                if (varType == "string")
                {
                    totalSize *= 256;
                }

                arrayType = $"[{totalSize} x {llvmType}]";

                WriteAlloca(register, arrayType, GetAlignment(llvmType));
                variables[varName] = new ArrayVariable(varName, varType, arrayType, register, totalSize, dimensions, llvmType);
                return null;
            }

            if (varType == "string")
            {
                WriteAlloca(register, "[256 x i8]", GetAlignment("i8"));
                variables[varName] = new Variable(varName, varType, "[256 x i8]", register);
                return null;
            }

            WriteAlloca(register, llvmType, GetAlignment(llvmType));
            variables[varName] = new Variable(varName, varType, llvmType, register);
            return null;
        }

        public string? VisitDec(ExprParser.PointerDecContext context)
        {
            string varType = context.type().GetText();
            string varName = context.ID().GetText();
            string pointers = context.POINTER().Aggregate("", (a, b) => a + b.GetText());
            string llvmType = getLLVMType(varType) + pointers;
            string register = nextRegister();
            variables[varName] = new Variable(varName, varType, llvmType, register);

            WriteAlloca(register, llvmType, GetAlignment(llvmType));
            registerTypes[register] = llvmType + "*";
            return null;
        }

        public string VisitVarArray(ExprParser.VarArrayContext context)
        {
            string varName = context.ID().GetText();
            var mainBody = getCurrentBody();
            var indexes = context.index();
            int i = 0;
            int pos = 0;
            Variable variable = GetVariableWithScope(varName)!;
            string llvmType;
            string loadReg;
            if (variable is ArrayVariable arrayVar)
            {
                foreach (var dim in arrayVar.dimensions)
                {
                    int index = int.Parse(indexes[i].INT().GetText());
                    pos = pos * dim + index;
                    i++;
                }

                string elementPtrReg = nextRegister();

                llvmType = arrayVar.type == "string" ? "i8" : arrayVar.innerType;

                mainBody.AppendLine($"  {elementPtrReg} = getelementptr inbounds [{arrayVar.size} x {llvmType}], [{arrayVar.size} x {llvmType}]* {arrayVar.register}, i32 0, i32 {pos}");

                if (arrayVar.type == "string")
                {
                    registerTypes[elementPtrReg] = "i8*";
                    return elementPtrReg;
                }

                loadReg = nextRegister();
                getCurrentBody().AppendLine($"  {loadReg} = load {arrayVar.innerType}, {arrayVar.innerType}* {elementPtrReg}, align {GetAlignment(arrayVar.innerType)}");
                registerTypes[loadReg] = arrayVar.innerType;
                return loadReg;
            }

            string currentPtr = nextRegister();
            llvmType = variable.LLVMType;
    
            mainBody.AppendLine($"  {currentPtr} = load {llvmType}, {llvmType}* {variable.register}, align {GetAlignment(llvmType)}");
    
            string elementType = llvmType.TrimEnd('*');
            string resultPtr = currentPtr;
            foreach (var index in indexes)
            {
                int value = int.Parse(index.INT().GetText());
                string gepReg = nextRegister();

                mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {elementType}* {resultPtr}, i32 {value}");
                resultPtr = gepReg;
            }
    
            loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {elementType}, {elementType}* {resultPtr}, align {GetAlignment(elementType)}");
            registerTypes[loadReg] = elementType;
    
            return loadReg;
        }

        public string? VisitSingleAtt(ExprParser.SingleAttContext context)
        {
            string varName = context.ID().GetText();
            string expression = visitExpression(context.expr());
            var indexes = context.index();
            var mainBody = getCurrentBody();
            int pos = 0;
            int i = 0;
            Variable variable = GetVariableWithScope(varName)!;

            if (variable is ArrayVariable arrayVar)
            {
                foreach (var dim in arrayVar.dimensions)
                {
                    int index = int.Parse(indexes[i].INT().GetText());
                    pos = pos * dim + index;
                    i++;
                }

                if (arrayVar.type == "string")
                {
                    if (registerTypes.TryGetValue(expression, out string? value) && value == "i8*")
                    {
                        WriteStrCopy(arrayVar.register, expression, pos * 256);
                    }
                    return null;
                }

            string regElementPtr = nextRegister();
            mainBody.AppendLine($"  {regElementPtr} = getelementptr inbounds [{arrayVar.size} x {arrayVar.innerType}], [{arrayVar.size} x {arrayVar.innerType}]* {arrayVar.register}, {arrayVar.innerType} 0, {arrayVar.innerType} {pos}");
            mainBody.AppendLine($"  store {arrayVar.innerType} {expression}, {arrayVar.innerType}* {regElementPtr}, align {GetAlignment(arrayVar.innerType)}");
            return null;
        }

        string currentPtr = nextRegister();
        string llvmType = variable.LLVMType;
    
        mainBody.AppendLine($"  {currentPtr} = load {llvmType}, {llvmType}* {variable.register}, align {GetAlignment(llvmType)}");

        string elementType = llvmType.TrimEnd('*');
        string resultPtr = currentPtr;

        foreach (var index in indexes)
        {
            int value = int.Parse(index.INT().GetText());
            string gepReg = nextRegister();
            mainBody.AppendLine($"  {gepReg} = getelementptr inbounds {elementType}, {elementType}* {resultPtr}, i32 {value}");
            resultPtr = gepReg;
        }
    
        mainBody.AppendLine($"  store {elementType} {expression}, {elementType}* {resultPtr}, align {GetAlignment(elementType)}");
    
        return null;
    }

        private Variable? GetVariableWithScope(string varName)
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
    }
}