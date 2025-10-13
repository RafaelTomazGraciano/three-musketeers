using Antlr4.Runtime.Misc;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.CodeGeneration.Functions
{
    public class FunctionCodeGenerator
    {
        private readonly StringBuilder functionDefinitions;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Dictionary<string, FunctionInfo> declaredFunctions;
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string> nextRegister;
        private readonly Func<string, string> getLLVMType;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly StringBuilder forwardDeclarations;

        // Current function context
        private string? currentFunctionName = null;
        private FunctionInfo? currentFunction = null;
        private StringBuilder? currentFunctionBody = null;
        private int functionRegisterCounter = 0;
        private bool hasReturnedInCurrentBlock = false;

        public FunctionCodeGenerator(
            StringBuilder functionDefinitions,
            Dictionary<string, string> registerTypes,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Dictionary<string, Variable> variables,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.StmContext, string?> visitStatement,
            Func<ExprParser.ExprContext, string?> visitExpression,
            StringBuilder forwardDeclarations)
        {
            this.functionDefinitions = functionDefinitions;
            this.registerTypes = registerTypes;
            this.declaredFunctions = declaredFunctions;
            this.variables = variables;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitStatement = visitStatement;
            this.visitExpression = visitExpression;
            this.forwardDeclarations = forwardDeclarations;
        }

        public void CollectFunctionSignature([NotNull] ExprParser.FunctionContext context)
        {
            var returnTypeCtx = context.function_return();
            string llvmReturnType;

            if (returnTypeCtx.VOID() != null)
            {
                llvmReturnType = "void";
            }
            else if (returnTypeCtx.type != null)
            {
                string basetype = GetTypeString(returnTypeCtx.type());
                llvmReturnType = getLLVMType(basetype);
                
                if (basetype == "string")
                {
                    llvmReturnType = "i8*";
                }
            }
            else
            {
                return;
            }

            string functionName = context.ID().GetText();

            // register function info
            if (!declaredFunctions.ContainsKey(functionName))
            {
                var funcInfo = new FunctionInfo
                {
                    returnType = GetTypeString(returnTypeCtx.type()),
                    parameters = new List<(string, string)>()
                };

                // Process parameters
                var argsCtx = context.args();
                if (argsCtx != null)
                {
                    var types = argsCtx.type();
                    var ids = argsCtx.ID();

                    for (int i = 0; i < types.Length; i++)
                    {
                        string paramType = GetTypeString(types[i]);
                        string paramName = ids[i].GetText();
                        funcInfo.parameters.Add((paramType, paramName));
                    }
                }

                declaredFunctions[functionName] = funcInfo;
            }
        }

        public string? VisitFunction([NotNull] ExprParser.FunctionContext context)
        {
            //get return type
            var returnTypeCtx = context.function_return();
            string llvmReturnType;

            if (returnTypeCtx.VOID() != null)
            {
                llvmReturnType = "void";
            }
            else if (returnTypeCtx.type != null)
            {
                string basetype = GetTypeString(returnTypeCtx.type());
                llvmReturnType = getLLVMType(basetype);
            }
            else
            {
                return null;
            }

            //get function name
            string functionName = context.ID().GetText();

            //register function info
            if (!declaredFunctions.ContainsKey(functionName))
            {
                var funcInfo = new FunctionInfo
                {
                    returnType = GetTypeString(returnTypeCtx.type()),
                    parameters = new List<(string, string)>()
                };
                declaredFunctions[functionName] = funcInfo;
            }

            //initialize function context
            currentFunctionName = functionName;
            currentFunction = declaredFunctions[functionName];
            currentFunctionBody = new StringBuilder();
            functionRegisterCounter = 0;
            hasReturnedInCurrentBlock = false;

            //build parameter
            List<string> paramDeclarations = new List<string>();
            var argsCtx = context.args();

            if (argsCtx != null)
            {
                var types = argsCtx.type();
                var ids = argsCtx.ID();

                for (int i = 0; i < types.Length; i++)
                {
                    string paramType = GetTypeString(types[i]);
                    string llvmParamType = getLLVMType(paramType);
                    string paramName = ids[i].GetText();

                    //register parameter
                    string paramReg = $"%param.{paramName}";
                    paramDeclarations.Add($"{llvmParamType} {paramReg}");
                }
            }

            string paramList = string.Join(", ", paramDeclarations);

            //generate function definition header
            functionDefinitions.AppendLine($"define {llvmReturnType} @{functionName}({paramList}) {{");
            functionDefinitions.AppendLine("entry:");

            //allocate space for parameters (making them mutable local variables)
            if (argsCtx != null)
            {
                var ids = argsCtx.ID();
                var types = argsCtx.type();

                for (int i = 0; i < ids.Length; i++)
                {
                    string paramName = ids[i].GetText();
                    string paramType = GetTypeString(types[i]);
                    string llvmParamType = getLLVMType(paramType);

                    if (paramType == "string")
                    {
                        llvmParamType = "i8*";
                    }

                    string allocaReg = NextFunctionRegister();
                    string paramReg = $"%param.{paramName}";

                    functionDefinitions.AppendLine($"  {allocaReg} = alloca {llvmParamType}");
                    functionDefinitions.AppendLine($"  store {llvmParamType} {paramReg}, {llvmParamType}* {allocaReg}");

                    // register parameter as variable
                    variables[$"@{functionName}.{paramName}"] = new Variable(
                        paramName,
                        paramType,
                        llvmParamType,
                        allocaReg
                    );

                    registerTypes[allocaReg] = llvmParamType + "*";
                }
            }

            //visit function body
            var funcBodyCtx = context.func_body();
            if (funcBodyCtx != null)
            {
                VisitFunctionBody(funcBodyCtx);
            }

            functionDefinitions.Append(currentFunctionBody);

            //append default return if needed
            if (!hasReturnedInCurrentBlock)
            {
                if (llvmReturnType == "void")
                {
                    functionDefinitions.AppendLine("  ret void");
                }
                else
                {
                    // for non-void functions without explicit return, use undef
                    functionDefinitions.AppendLine($"  ret {llvmReturnType} undef");
                }
            }

            functionDefinitions.AppendLine("}");
            functionDefinitions.AppendLine();

            // clean up parameter variables
            if (argsCtx != null)
            {
                var ids = argsCtx.ID();
                for (int i = 0; i < ids.Length; i++)
                {
                    string paramName = ids[i].GetText();
                    variables.Remove($"@{functionName}.{paramName}");
                }
            }

            // reset function context
            currentFunctionName = null;
            currentFunction = null;
            currentFunctionBody = null;

            return null;
        }

        private void VisitFunctionBody(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                if (hasReturnedInCurrentBlock)
                {
                    break; // skip unreachable code after return
                }

                if (stm.RETURN() != null)
                {
                    VisitReturnStatement(stm);
                }
                else
                {
                    visitStatement(stm);
                }
            }
        }

        public void VisitReturnStatement(ExprParser.StmContext context)
        {
            if (currentFunctionBody == null || currentFunction == null)
            {
                return;
            }

            var exprCtx = context.expr();

            // Void return
            if (currentFunction.isVoid)
            {
                currentFunctionBody.AppendLine("  ret void");
                hasReturnedInCurrentBlock = true;
                return;
            }

            // Non-void return
            if (exprCtx != null)
            {
                string? returnValue = visitExpression(exprCtx);
                if (returnValue != null)
                {
                    string returnType = registerTypes.ContainsKey(returnValue)
                        ? registerTypes[returnValue]
                        : getLLVMType(currentFunction.returnType ?? "i32");

                    currentFunctionBody.AppendLine($"  ret {returnType} {returnValue}");
                    hasReturnedInCurrentBlock = true;
                }
            }
        }
        
        private string GetTypeString(ExprParser.TypeContext context)
        {
            if (context == null) return "void";
            
            string typeText = context.GetText();
            
            if (typeText == "int") return "int";
            if (typeText == "double") return "double";
            if (typeText == "bool") return "bool";
            if (typeText == "char") return "char";
            if (typeText == "string") return "string";
            
            var id = context.ID();
            if (id != null)
            {
                return id.GetText();
            }
            
            return "unknown";
        }

        private string NextFunctionRegister()
        {
            return $"%{functionRegisterCounter++}";
        }

        public bool IsInsideFunction()
        {
            return currentFunctionBody != null;
        }

        public StringBuilder? GetCurrentFunctionBody()
        {
            return currentFunctionBody;
        }

        public string? GetCurrentFunctionName()
        {
            return currentFunctionName;
        }
    }
}