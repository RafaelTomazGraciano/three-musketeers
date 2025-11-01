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
        private readonly Dictionary<string, HeterogenousType> structs;

        public VariableAssignmentCodeGenerator (
            StringBuilder declarations,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string> visitExpression,
            Func<string?> getCurrentFunctionName,
            Func<StringBuilder> getCurrentBody,
            Func<string, int> GetAlignment,
            Dictionary<string, HeterogenousType> structs)
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
            this.structs = structs;
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
                string pointer = new('*', context.POINTER().Length);
                llvmType = getLLVMType(varType) + pointer;
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

        public string? VisitDec([NotNull] ExprParser.DeclarationContext context)
        {
            string varType = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = getLLVMType(varType);
            string register = nextRegister();
            string pointers = new('*', context.POINTER().Length);
            var indexes = context.intIndex();

            if (indexes.Length > 0)
            {
                (var arrayType, var totalSize) = GetArrayDimensions(indexes, varType, llvmType);
                WriteAlloca(register, arrayType, GetAlignment(llvmType));
                variables[varName] = new ArrayVariable(varName, varType, arrayType, register, totalSize, llvmType);
                return null;
            }

            if (varType == "string")
            {
                llvmType = "[256 x i8]" + pointers;
                WriteAlloca(register, llvmType, GetAlignment("i8*"));
                variables[varName] = new Variable(varName, varType, llvmType, register);
                return null;
            }

            WriteAlloca(register, llvmType+pointers, GetAlignment(llvmType));
            variables[varName] = new Variable(varName, varType, llvmType, register);
            return null;
        }

        private (string dimension, int totalSize) GetArrayDimensions(ExprParser.IntIndexContext[] indexes, string varType, string llvmType)
        {
            List<int> dimensions = new ArrayList<int>();
            int totalSize = 1;
            int i;
            string arrayType = new('[', 1);
            int dimension;

            for (i = 0; i < indexes.Length; i++)
            {
                dimension = int.Parse(indexes[i].GetText());
                totalSize *= dimension;
                arrayType += $"{dimension} x [";
            }

            dimension = int.Parse(indexes[i].GetText());
            arrayType += $"{dimension}";

            if (varType == "string")
            {
                arrayType += "[256 x i8]";
                totalSize *= 256;
            }
            else
            {
                arrayType += $" x {llvmType}";
            }
            arrayType += new string(']', indexes.Length);
            return (arrayType, totalSize);
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

        public string VisitVarArray(ExprParser.VarArrayContext context)
        {
            string name = context.ID().GetText();
            var indexes = context.index();
            var currentBody = getCurrentBody();
            var register = nextRegister();
            Variable variable = GetVariableWithScope(name)!;
            string expr;
            var llvmType = variable.LLVMType;
            int i;
            currentBody.Append($"    {register} = getelementptr inbounds {llvmType}, {llvmType}* {register}, i32 0, ");
            for (i = 0; i < indexes.Length-1; i++)
            {
                expr = visitExpression(indexes[i].expr());
                currentBody.Append($"i32 {expr}, ");
            }
            expr = visitExpression(indexes[i].expr());
            currentBody.AppendLine($"i32 {expr}");

            if (variable is ArrayVariable arrayVariable)
            {
                llvmType = arrayVariable.innerType;
            }
            registerTypes[register] = llvmType;
            return register;
        }
    }
}