using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Functions
{
    public class MainFunctionCodeGenerator
    {
        private readonly StringBuilder mainDefinition;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        private bool hasReturnedInMain = false;
        private bool hasMainArgs = false;
        private bool isInsideMain = false;

        public MainFunctionCodeGenerator(
            StringBuilder mainDefinition,
            Dictionary<string, string> registerTypes,
            Dictionary<string, Variable> variables,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.StmContext, string?> visitStatement,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.mainDefinition = mainDefinition;
            this.registerTypes = registerTypes;
            this.variables = variables;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitStatement = visitStatement;
            this.visitExpression = visitExpression;
        }

        public void GenerateMainFunction(ExprParser.MainFunctionContext context)
        {
            hasReturnedInMain = false;
            isInsideMain = true;

            //check if main has arguments
            var argsCtx = context.mainArgs();
            hasMainArgs = argsCtx != null;

            GenerateFunctionSignature(argsCtx);

            var funcBodyCtx = context.func_body();
            if (funcBodyCtx != null)
            {
                GenerateFunctionBody(funcBodyCtx);
            }

            mainDefinition.AppendLine("}");

            CleanupMainArguments();
            
            isInsideMain = false;
        }
        
        private void GenerateFunctionSignature(ExprParser.MainArgsContext? argsCtx)
        {
            if (hasMainArgs && argsCtx != null)
            {
                // get argument names from grammar: 'int' ID ',' 'char' ID '[' ']'
                var idTokens = argsCtx.ID();
                string argcName = idTokens.Length > 0 ? idTokens[0].GetText() : "argc";
                string argvName = idTokens.Length > 1 ? idTokens[1].GetText() : "argv";

                // Main with arguments: int main(int argc, char argv[])
                mainDefinition.AppendLine("define i32 @main(i32 %argc.param, i8** %argv.param) {");
                mainDefinition.AppendLine("entry:");

                AllocateIntArgument(argcName);

                AllocateCharArrayArgument(argvName);
            }
            else
            {
                // main without arguments
                mainDefinition.AppendLine("define i32 @main() {");
                mainDefinition.AppendLine("entry:");
            }
        }

        private void AllocateIntArgument(string argcName)
        {
            string argcAlloca = nextRegister();
            mainDefinition.AppendLine($"  {argcAlloca} = alloca i32");
            mainDefinition.AppendLine($"  store i32 %argc.param, i32* {argcAlloca}");
            
            variables[argcName] = new Variable(argcName, "int", "i32", argcAlloca);
            registerTypes[argcAlloca] = "i32*";
        }

        private void AllocateCharArrayArgument(string argvName)
        {
            string argvAlloca = nextRegister();
            mainDefinition.AppendLine($"  {argvAlloca} = alloca i8**");
            mainDefinition.AppendLine($"  store i8** %argv.param, i8*** {argvAlloca}");
            
            // argv is a pointer to an array of strings (char**)
            variables[argvName] = new Variable(argvName, "char", argvAlloca, argvAlloca);
            registerTypes[argvAlloca] = "i8***";
        }
        
        private void GenerateFunctionBody(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            
            foreach (var stm in statements)
            {
                // stop processing if already returned
                if (hasReturnedInMain)
                {
                    break;
                }

                // handle return statements specially
                if (stm.RETURN() != null)
                {
                    GenerateReturnStatement(stm);
                }
                else
                {
                    visitStatement(stm);
                }
            }
        }

        private void GenerateReturnStatement(ExprParser.StmContext context)
        {
            var exprCtx = context.expr();
            
            if (exprCtx != null)
            {
                // evaluate the return expression
                string? returnValue = visitExpression(exprCtx);
                
                if (returnValue != null)
                {
                    // get the type of the return value
                    string returnType = registerTypes.ContainsKey(returnValue)
                        ? registerTypes[returnValue]
                        : "i32";

                    // generate return instruction
                    mainDefinition.AppendLine($"  ret {returnType} {returnValue}");
                    hasReturnedInMain = true;
                }
            }
            else
            {
                // return without value (defaults to 0)
                mainDefinition.AppendLine("  ret i32 0");
                hasReturnedInMain = true;
            }
        }
        
        private void CleanupMainArguments()
        {
            if (hasMainArgs)
            {
                var toRemove = new List<string>();
                
                foreach (var kvp in variables)
                {
                    var variable = kvp.Value;
                    if ((variable.type == "int" || variable.type == "char") && 
                        (registerTypes.ContainsKey(variable.LLVMType) &&
                         (registerTypes[variable.LLVMType] == "i32*" || 
                          registerTypes[variable.LLVMType] == "i8***")))
                    {
                        toRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in toRemove)
                {
                    variables.Remove(key);
                }
            }
        }

        public bool IsInsideMain()
        {
            return isInsideMain;
        }

        public StringBuilder GetMainBody()
        {
            return mainDefinition;
        }
    }
}