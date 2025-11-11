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
            hasReturnedInCurrentBlock = false;

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

                    if (pointerLevel > 0)
                    {
                        variables[$"@{functionName}.{paramName}"] = new Variable(
                            paramName,
                            paramType,
                            llvmParamType,
                            paramReg,
                            isDirectPointerParam: true  
                        );
                        
                        registerTypes[paramReg] = llvmParamType;
                    }
                    else
                    {
                        string allocaReg = nextRegister();
                        
                        functionDefinitions.AppendLine($"  {allocaReg} = alloca {llvmParamType}");
                        functionDefinitions.AppendLine($"  store {llvmParamType} {paramReg}, {llvmParamType}* {allocaReg}");

                        variables[$"@{functionName}.{paramName}"] = new Variable(
                            paramName,
                            paramType,
                            llvmParamType,
                            allocaReg
                        );

                        registerTypes[allocaReg] = llvmParamType + "*";
                    }
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

            // Check if we're inside a control flow structure (if/switch/loop)
            // by checking if the last line in the body is a label (indicating we're in a control flow block)
            bool isInControlFlow = IsInsideControlFlow();

            // Void return
            if (currentFunction.isVoid)
            {
                currentFunctionBody.AppendLine("  ret void");
                // Only set flag for unconditional returns (not inside control flow)
                if (!isInControlFlow)
                {
                    hasReturnedInCurrentBlock = true;
                }
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

                    if (returnType == "i1")
                    {
                        string convertedReg = nextRegister();
                        currentFunctionBody.AppendLine($"  {convertedReg} = zext i1 {returnValue} to i32");
                        returnValue = convertedReg;
                        returnType = "i32";
                    }

                    currentFunctionBody.AppendLine($"  ret {returnType} {returnValue}");
                    // Only set flag for unconditional returns (not inside control flow)
                    if (!isInControlFlow)
                    {
                        hasReturnedInCurrentBlock = true;
                    }
                }
            }
        }

        private bool IsInsideControlFlow()
        {
            if (currentFunctionBody == null || currentFunctionBody.Length == 0)
            {
                return false;
            }

            // Check recent lines to see if we're inside a control flow structure
            // We're inside control flow if we just branched TO a control flow label,
            // not if we're AT a merge label (merge labels mark the end of control flow)
            string bodyText = currentFunctionBody.ToString();
            string[] lines = bodyText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            if (lines.Length == 0)
            {
                return false;
            }

            // Check the last few non-empty lines
            int checkedLines = 0;
            for (int i = lines.Length - 1; i >= 0 && checkedLines < 3; i--)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                checkedLines++;
                
                // If the line is a merge label, we're past control flow
                if (line.StartsWith("merge_") && line.EndsWith(":"))
                {
                    return false; // We're at a merge point, past the control flow
                }
                
                // If the line is a control flow block label (if_, case_, while_, etc.) but not merge_,
                // we're inside control flow
                if ((line.StartsWith("if_") || line.StartsWith("case_") || line.StartsWith("while_") || 
                     line.StartsWith("for_") || line.StartsWith("dowhile_") || line.StartsWith("else_")) 
                    && line.EndsWith(":"))
                {
                    return true; // We're in a control flow block
                }
                
                // If it's a branch TO a control flow label (but not merge), we're entering control flow
                if ((line.Contains("br i1") || line.Contains("br label %")) && 
                    !line.Contains("merge_"))
                {
                    return true;
                }
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