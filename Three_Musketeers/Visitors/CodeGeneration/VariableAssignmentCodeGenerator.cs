using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public class VariableAssignmentCodeGenerator
    {
        private readonly StringBuilder mainBody;
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public VariableAssignmentCodeGenerator(
            StringBuilder mainBody,
            Dictionary<string, Variable> variables,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.ExprContext, string> visitExpression)
        {
            this.mainBody = mainBody;
            this.variables = variables;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitExpression = visitExpression;
        }

        public string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = getLLVMType(type);

            string allocReg = $"%{varName}";
            mainBody.AppendLine($"  {allocReg} = alloca {llvmType}");

            var variable = new Variable(varName, type, llvmType, allocReg);
            variables[varName] = variable;

            string value = visitExpression(context.expr());

            mainBody.AppendLine($"  store {llvmType} {value}, {llvmType}* {allocReg}");

            return null;
        }

        public string VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();

            if (!variables.ContainsKey(varName))
            {
                throw new Exception($"Variable '{varName}' not found");
            }

            Variable variable = variables[varName];

            string loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}");

            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }
    }
}

