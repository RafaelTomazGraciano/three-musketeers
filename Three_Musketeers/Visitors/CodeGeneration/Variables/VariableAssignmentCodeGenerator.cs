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
        private readonly StringBuilder mainBody;
        private readonly StringBuilder declarations;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public VariableAssignmentCodeGenerator(
            StringBuilder mainBody,
            StringBuilder declarations,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.mainBody = mainBody;
            this.declarations = declarations;
            this.variables = variables;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitExpression = visitExpression;
        }

        public string? VisitAttRegular([NotNull] ExprParser.AttRegularContext context)
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
                WriteAlloca(register, llvmType, GetAlignment(llvmType));
                variables[varName] = new Variable(varName, varType, llvmType, register);
            }
            else
            {
                Variable variable = variables[varName];
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
            mainBody.AppendLine($"  store {llvmType} {value}, {llvmType}* {register}, align {GetAlignment(llvmType)}");
            return null;
        }

        private void WriteStrCopy(string register, string exprResult, int pos = 0)
        {
            if (!declarations.ToString().Contains("declare i8* @strcpy"))
            {
                declarations.AppendLine("declare i8* @strcpy(i8*, i8*)");
            }
            string destPtr = nextRegister();
            mainBody.AppendLine($"  {destPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 {pos}");
            string srcPtr = nextRegister();
            mainBody.AppendLine($"  {srcPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {exprResult}, i32 0, i32 0");

            string copyResult = nextRegister();
            mainBody.AppendLine($"  {copyResult} = call i8* @strcpy(i8* {destPtr}, i8* {srcPtr})");
        }

        public string VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();

            Variable variable = variables[varName];

            if (variable.type == "string")
            {
                string ptrReg = nextRegister();
                mainBody.AppendLine($"  {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");
                registerTypes[ptrReg] = "i8*";
                return ptrReg;
            }

            string loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align {GetAlignment(variable.LLVMType)}");
            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }

        public string? VisitDec([NotNull] ExprParser.DeclarationContext context)
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

        public string VisitVarArray(ExprParser.VarArrayContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            int i = 0;
            int pos = 0;
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            foreach (var dim in arrayVar.dimensions)
            {
                int index = int.Parse(indexes[i].INT().GetText());
                pos = pos * dim + index;
                i++;
            }

            string elementPtrReg = nextRegister();

            string llvmType = arrayVar.type == "string" ? "i8" : arrayVar.innerType;

            mainBody.AppendLine($"  {elementPtrReg} = getelementptr inbounds [{arrayVar.size} x {llvmType}], [{arrayVar.size} x {llvmType}]* {arrayVar.register}, i32 0, i32 {pos}");

            if (arrayVar.type == "string")
            {
                registerTypes[elementPtrReg] = "i8*";
                return elementPtrReg;
            }

            string loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {arrayVar.innerType}, {arrayVar.innerType}* {elementPtrReg}, align {GetAlignment(arrayVar.innerType)}");
            registerTypes[loadReg] = arrayVar.innerType;
            return loadReg;
        }

        public string? VisitSingleAtt(ExprParser.SingleAttContext context)
        {
            string varName = context.ID().GetText();
            string expression = visitExpression(context.expr());
            var indexes = context.index();
            int pos = 0;
            int i = 0;
            ArrayVariable arrayVar = (ArrayVariable)variables[varName];
            foreach (var dim in arrayVar.dimensions)
            {
                int index = int.Parse(indexes[i].INT().GetText());
                pos = pos * dim + index;
                i++;
            }

            if (arrayVar.type == "string")
            {
                if (registerTypes.ContainsKey(expression) && registerTypes[expression] == "i8*")
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

        private void WriteAlloca(string register, string type, int alignment)
        {
            mainBody.AppendLine($"  {register} = alloca {type}, align {alignment}");
        }

        private static int GetAlignment(string type)
        {
            return type switch
            {
                "i32" or "i32*" => 4,
                "double" => 8,
                "i1" or "i1*" or "i8" or "i8*" => 1,
                _ => 4
            };
        }
    }
}