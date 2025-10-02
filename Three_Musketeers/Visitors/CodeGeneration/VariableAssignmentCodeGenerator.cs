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

        public string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            string varType = context.type().GetText();
            string varName = context.ID().GetText();
            string llvmType = getLLVMType(varType);
            string register = nextRegister();

            if (varType == "string")
            {
                mainBody.AppendLine($"  {register} = alloca [256 x i8], align 1");

                variables[varName] = new Variable(varName, varType, "[256 x i8]", register);

                if (context.expr() != null)
                {
                    string exprResult = visitExpression(context.expr());
                    
                    if (registerTypes.ContainsKey(exprResult) && registerTypes[exprResult] == "i8*")
                    {
                        if (!declarations.ToString().Contains("declare i8* @strcpy"))
                        {
                            declarations.AppendLine("declare i8* @strcpy(i8*, i8*)");
                        }

                        string destPtr = nextRegister();
                        mainBody.AppendLine($"  {destPtr} = getelementptr inbounds [256 x i8], [256 x i8]* {register}, i32 0, i32 0");
                        
                        string srcPtr = nextRegister();

                        mainBody.AppendLine($"  {srcPtr} = bitcast i8* getelementptr inbounds ([256 x i8], [256 x i8]* {exprResult}, i32 0, i32 0) to i8*");

                        string copyResult = nextRegister();
                        mainBody.AppendLine($"  {copyResult} = call i8* @strcpy(i8* {destPtr}, i8* {srcPtr})");
                    }
                }
            }
            else
            {
                mainBody.AppendLine($"  {register} = alloca {llvmType}, align 4");

                variables[varName] = new Variable(varName, varType, llvmType, register);

                if (context.expr() != null)
                {
                    string value = visitExpression(context.expr());
                    mainBody.AppendLine($"  store {llvmType} {value}, {llvmType}* {register}, align 4");
                }
            }

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

            if (variable.type == "string")
            {
                string ptrReg = nextRegister();
                mainBody.AppendLine($"  {ptrReg} = getelementptr inbounds [256 x i8], [256 x i8]* {variable.register}, i32 0, i32 0");
                registerTypes[ptrReg] = "i8*";
                return ptrReg;
            }

            string loadReg = nextRegister();
            mainBody.AppendLine($"  {loadReg} = load {variable.LLVMType}, {variable.LLVMType}* {variable.register}, align 4");
            registerTypes[loadReg] = variable.LLVMType;
            return loadReg;
        }
    }
}