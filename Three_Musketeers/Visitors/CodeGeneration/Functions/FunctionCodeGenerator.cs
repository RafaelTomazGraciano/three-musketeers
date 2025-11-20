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

        // Current function context
        private string? currentFunctionName = null;
        private FunctionInfo? currentFunction = null;
        private StringBuilder? currentFunctionBody = null;

        public FunctionCodeGenerator(
            StringBuilder functionDefinitions,
            Dictionary<string, string> registerTypes,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Dictionary<string, Variable> variables,
            Func<string> nextRegister,
            Func<string, string> getLLVMType,
            Func<ExprParser.StmContext, string?> visitStatement,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.functionDefinitions = functionDefinitions;
            this.registerTypes = registerTypes;
            this.declaredFunctions = declaredFunctions;
            this.variables = variables;
            this.nextRegister = nextRegister;
            this.getLLVMType = getLLVMType;
            this.visitStatement = visitStatement;
            this.visitExpression = visitExpression;
        }

        public void CollectFunctionSignature([NotNull] ExprParser.FunctionContext context)
        {
            var returnTypeCtx = context.function_return();
            string llvmReturnType;
            int returnPointerLevel = returnTypeCtx.POINTER()?.Length ?? 0;

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
                //add * for each pointer level
                for (int i = 0; i < returnPointerLevel; i++)
                {
                    llvmReturnType += "*";
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
                    returnPointerLevel = returnPointerLevel,
                    parameters = new List<(string, string, int)>()
                };

                // Process parameters
                var argsCtx = context.args();
                if (argsCtx != null)
                {
                    var types = argsCtx.type();
                    var ids = argsCtx.ID();
                    var allPointers = argsCtx.POINTER();
                    int pointerIndex = 0;

                    for (int i = 0; i < types.Length; i++)
                    {
                        string paramType = GetTypeString(types[i]);
                        string paramName = ids[i].GetText();
                        //pointers
                        int pointerLevel = 0;
                        int typeEndPos = types[i].Stop.StopIndex;
                        int idStartPos = ids[i].Symbol.StartIndex;
                        
                        while (pointerIndex < allPointers.Length && 
                            allPointers[pointerIndex].Symbol.StartIndex > typeEndPos &&
                            allPointers[pointerIndex].Symbol.StartIndex < idStartPos)
                        {
                            pointerLevel++;
                            pointerIndex++;
                        }
                        
                        funcInfo.parameters.Add((paramType, paramName, pointerLevel));
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
            int returnPointerLevel = returnTypeCtx.POINTER()?.Length ?? 0;

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
                
                // Add * for pointer levels
                for (int i = 0; i < returnPointerLevel; i++)
                {
                    llvmReturnType += "*";
                }
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
                    returnPointerLevel = returnPointerLevel,
                    parameters = new List<(string, string, int)>()
                };
                declaredFunctions[functionName] = funcInfo;
            }

            //initialize function context
            currentFunctionName = functionName;
            currentFunction = declaredFunctions[functionName];
            currentFunctionBody = new StringBuilder();

            //build parameter
            List<string> paramDeclarations = new List<string>();
            var argsCtx = context.args();

            if (argsCtx != null)
            {
                var types = argsCtx.type();
                var ids = argsCtx.ID();
                var allPointers = argsCtx.POINTER();
                int pointerIndex = 0;

                for (int i = 0; i < types.Length; i++)
                {
                    string paramType = GetTypeString(types[i]);
                    string llvmParamType = getLLVMType(paramType);
                    string paramName = ids[i].GetText();

                    //count pointers
                    int pointerLevel = 0;
                    int typeEndPos = types[i].Stop.StopIndex;
                    int idStartPos = ids[i].Symbol.StartIndex;

                    while (pointerIndex < allPointers.Length &&
                        allPointers[pointerIndex].Symbol.StartIndex > typeEndPos &&
                        allPointers[pointerIndex].Symbol.StartIndex < idStartPos)
                    {
                        pointerLevel++;
                        pointerIndex++;
                    }
                    
                    if (paramType == "string")
                    {
                        llvmParamType = "i8*";
                    }
                    
                    for (int j = 0; j < pointerLevel; j++)
                    {
                        llvmParamType += "*";
                    }

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
                var allPointers = argsCtx.POINTER();
                int pointerIndex = 0;

                for (int i = 0; i < ids.Length; i++)
                {
                    string paramName = ids[i].GetText();
                    string paramType = GetTypeString(types[i]);
                    string llvmParamType = getLLVMType(paramType);

                    // Count pointers 
                    int pointerLevel = 0;
                    int typeEndPos = types[i].Stop.StopIndex;
                    int idStartPos = ids[i].Symbol.StartIndex;

                    while (pointerIndex < allPointers.Length &&
                        allPointers[pointerIndex].Symbol.StartIndex > typeEndPos &&
                        allPointers[pointerIndex].Symbol.StartIndex < idStartPos)
                    {
                        pointerLevel++;
                        pointerIndex++;
                    }

                    if (paramType == "string")
                    {
                        llvmParamType = "i8*";
                    }

                    // Add * for each pointer level
                    for (int j = 0; j < pointerLevel; j++)
                    {
                        llvmParamType += "*";
                    }

                    string paramReg = $"%param.{paramName}";

                    string allocaReg = nextRegister();
                    int alignment = llvmParamType.Contains("*") ? 8 : llvmParamType == "double" ? 8 : llvmParamType == "i32" ? 4 : 1;
                    
                    functionDefinitions.AppendLine($"  {allocaReg} = alloca {llvmParamType}, align {alignment}");
                    functionDefinitions.AppendLine($"  store {llvmParamType} {paramReg}, {llvmParamType}* {allocaReg}, align {alignment}");

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

            // Check if currentFunctionBody ends with a terminator
            if (!EndsWithTerminator())
            {
                if (llvmReturnType == "void")
                {
                    currentFunctionBody.AppendLine("  ret void");
                }
                else
                {
                    currentFunctionBody.AppendLine($"  ret {llvmReturnType} undef");
                }
            }

            functionDefinitions.Append(currentFunctionBody);

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

            if (currentFunction.isVoid)
            {
                currentFunctionBody.AppendLine("  ret void");
                return;
            }

            if (exprCtx != null)
            {
                string? returnValue = visitExpression(exprCtx);
                if (returnValue != null)
                {
                    string returnType = registerTypes.ContainsKey(returnValue)
                        ? registerTypes[returnValue]
                        : getLLVMType(currentFunction.returnType ?? "i32");

                    // Convert 0 to null if function returns a pointer type
                    string expectedReturnType = getLLVMType(currentFunction.returnType ?? "i32");
                    for (int i = 0; i < currentFunction.returnPointerLevel; i++)
                    {
                        expectedReturnType += "*";
                    }

                    // If returning 0 to a pointer type, convert to null
                    if (expectedReturnType.Contains("*") && returnValue == "0")
                    {
                        returnValue = "null";
                        returnType = expectedReturnType;
                    }

                    if (returnType == "i1")
                    {
                        string convertedReg = nextRegister();
                        currentFunctionBody.AppendLine($"  {convertedReg} = zext i1 {returnValue} to i32");
                        returnValue = convertedReg;
                        returnType = "i32";
                    }

                    currentFunctionBody.AppendLine($"  ret {returnType} {returnValue}");
                }
            }
        }

        private bool EndsWithTerminator()
        {
            if (currentFunctionBody == null || currentFunctionBody.Length == 0)
            {
                return false;
            }

            string bodyText = currentFunctionBody.ToString();
            string[] lines = bodyText.Split('\n');

            string? lastMeaningfulLine = null;
            
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                if (line.EndsWith(":"))
                {
                    if (lastMeaningfulLine == null)
                    {
                        return false;
                    }

                    break;
                }

                lastMeaningfulLine = line;
                break;
            }
            
            if (lastMeaningfulLine == null)
            {
                return false;
            }
            if (lastMeaningfulLine.StartsWith("ret ") || 
                lastMeaningfulLine.StartsWith("br ") ||         
                lastMeaningfulLine.StartsWith("switch ") || 
                lastMeaningfulLine.StartsWith("unreachable"))
            {
                return true;
            }
            
            return false;
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