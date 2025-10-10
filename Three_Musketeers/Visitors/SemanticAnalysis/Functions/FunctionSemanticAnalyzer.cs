using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Functions
{
    public class FunctionSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly Func<ExprParser.ExprContext, string?> getExpressionType;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;

        private readonly Dictionary<string, FunctionInfo> declaredFunctions;
        private string? currentFunctionName = null;
        private FunctionInfo? currentFunction = null;

        public FunctionSemanticAnalyzer(
            SymbolTable symbolTable,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Func<ExprParser.ExprContext, string?> getExpressionType,
            Func<ExprParser.StmContext, string?> visitStatement)
        {
            this.symbolTable = symbolTable;
            this.declaredFunctions = declaredFunctions;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.getExpressionType = getExpressionType;
            this.visitStatement = visitStatement;
        }

        public void AnalyzeFunction(ExprParser.FunctionContext context)
        {
            int line = context.Start.Line;

            //type of return
            var returnTypeCtx = context.function_return();
            string returnType = "";
            List<int>? returnDimensions = null;

            if (returnTypeCtx.GetText() == "void")
            {
                returnType = "void";
            }
            else if (returnTypeCtx.type() != null)
            {
                returnType = GetTypeString(returnTypeCtx.type());

                // check if return type is an array
                // var indices = returnTypeCtx.index();
                // if (indices != null && indices.Length > 0)
                // {
                //     returnDimensions = new List<int>();
                //     foreach (var idx in indices)
                //     {
                //         string sizeText = idx.INT().GetText();
                //         if (int.TryParse(sizeText, out int size))
                //         {
                //             if (size <= 0)
                //             {
                //                 reportError(line, $"Array dimension must be positive, got {size}");
                //                 size = 1; // Default to prevent further errors
                //             }
                //             returnDimensions.Add(size);
                //         }
                //     }
                // }
            }
            else
            {
                reportError(line, $"Invalid return type in function");
            }

            //get function name
            string functionName = context.ID().GetText();

            if (declaredFunctions.ContainsKey(functionName))
            {
                reportError(line, $"Function '{functionName}' has already been declared");
                return;
            }

            // check if function name conflicts with existing variable
            if (symbolTable.Contains(functionName))
            {
                reportError(line, $"Function name '{functionName}' conflicts with existing variable");
                return;
            }

            //function info
            var functionInfo = new FunctionInfo
            {
                returnType = returnType,
                returnDimensions = returnDimensions,
                parameters = new List<(string, string)>(),
                hasReturnStatement = false
            };

            //args
            var argsCtx = context.args();
            if (argsCtx != null)
            {
                ProcessFunctionArguments(argsCtx, functionInfo, functionName);
            }

            declaredFunctions[functionName] = functionInfo;

            // Add function to symbol table as a special symbol
            var functionSymbol = new Symbol(functionName, returnType, line);
            functionSymbol.isInitializated = true;
            symbolTable.AddSymbol(functionSymbol);

            // enter new scope for function body
            symbolTable.EnterScope();

            // set current function context
            currentFunctionName = functionName;
            currentFunction = functionInfo;

            // add parameters to function scope
            if (functionInfo.parameters != null)
            {
                foreach (var (type, name) in functionInfo.parameters)
                {
                    var paramSymbol = new Symbol(name, type, line);
                    paramSymbol.isInitializated = true; // Parameters are always initialized

                    if (!symbolTable.AddSymbol(paramSymbol))
                    {
                        reportError(line, $"Parameter '{name}' conflicts with existing symbol in function scope");
                    }
                }
            }

            // analyze function body
            var funcBodyCtx = context.func_body();
            if (funcBodyCtx != null)
            {
                AnalyzeFunctionBody(funcBodyCtx);
            }

            // check if non-void function has return statement
            if (!functionInfo.isVoid && !functionInfo.hasReturnStatement)
            {
                if (functionInfo.isArray)
                {
                    reportError(line, "Functions returning arrays are not supported yet");
                    return;
                }
                reportWarning(line, $"Function '{functionName}' with return type '{returnType}' does not have a 'return' statement");

                // string returnTypeDisplay = functionInfo.isArray
                //     ? $"{returnType}[{string.Join("][", returnDimensions!)}]"
                //     : returnType;
                // reportWarning(line, $"Function '{functionName}' with return type '{returnTypeDisplay}' does not have a 'return' statement");
            }

            // exit function scope
            symbolTable.ExitScope();

            // exit function context
            currentFunctionName = null;
            currentFunction = null;

        }

        private void ProcessFunctionArguments(
            ExprParser.ArgsContext argsCtx,
            FunctionInfo functionInfo,
            string functionName)
        {
            var types = argsCtx.type();
            var ids = argsCtx.ID();

            HashSet<string> paramNames = new HashSet<string>();

            for (int i = 0; i < types.Length; i++)
            {
                string paramType = GetTypeString(types[i]);
                string paramName = ids[i].GetText();

                if (paramNames.Contains(paramName))
                {
                    reportError(argsCtx.Start.Line, $"Parameter '{functionName}' duplicated in function '{functionName}'");
                    continue;
                }

                paramNames.Add(paramName);
                functionInfo.parameters?.Add((paramType, paramName));
            }
        }

        private void AnalyzeFunctionBody(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                if (stm.RETURN() != null)
                {
                    AnalyzeReturnStatement(stm);
                }
                else
                {
                    visitStatement(stm);
                }
            }
        }

        public void AnalyzeReturnStatement(ExprParser.StmContext context)
        {
            int line = context.Start.Line;

            if (currentFunction == null)
            {
                reportError(line, $"'return' out of a function");
                return;
            }

            currentFunction.hasReturnStatement = true;

            var exprCtx = context.expr();

            //void Function
            if (currentFunction.isVoid)
            {
                if (exprCtx != null)
                {
                    reportError(line, $"Void function '{currentFunctionName}' cannot return a value");
                }
                return;
            }

            //non void function
            if (exprCtx == null)
            {
                string? returnTypeDisplay = currentFunction.isArray
                    ? $"{currentFunction.returnType}[{string.Join("][", currentFunction.returnDimensions!)}]"
                    : currentFunction.returnType;
                reportError(line, $"Function '{currentFunctionName}' with return type '{returnTypeDisplay}' must return a value");
                return;
            }

            string? returnExprType = getExpressionType(exprCtx);
            if (returnExprType == null)
            {
                reportError(line, $"Could not determine the type of return expression");
                return;
            }

            // string expectedType = currentFunction.isArray
            //     ? $"{currentFunction.returnType}_array" 
            //     : currentFunction.returnType!;
            if (currentFunction.isArray)
            {
                reportError(line, "Returning arrays is not supported yet");
                return;
            }

            string expectedType = currentFunction.returnType!;

            if (currentFunction.returnType != returnExprType)
            {
                string? returnTypeDisplay = currentFunction.isArray
                    ? $"{currentFunction.returnType}[{string.Join("][", currentFunction.returnDimensions!)}]"
                    : currentFunction.returnType;
                reportError(line, $"Incompatible return type: expected '{returnTypeDisplay}', but got '{returnExprType}'");
            }
        }

        private string GetTypeString(ExprParser.TypeContext context)
        {
            if (context.GetText() == "int") return "int";
            if (context.GetText() == "double") return "double";
            if (context.GetText() == "bool") return "bool";
            if (context.GetText() == "char") return "char";
            if (context.GetText() == "string") return "string";

            var id = context.ID();
            if (id != null)
            {
                string typeName = id.GetText();
                var typeSymbol = symbolTable.GetSymbol(typeName);

                if (typeSymbol != null)
                {
                    return typeSymbol.type;
                }
                reportError(context.Start.Line, $"type '{typeName}' has not been defined");
                return "error";
            }

            return "unknown";
        }

        private bool IsFunctionDeclared(string functionName)
        {
            return declaredFunctions.ContainsKey(functionName);
        }

        public FunctionInfo? GetFunctionInfo(string functionName)
        {
            return declaredFunctions.ContainsKey(functionName)
                ? declaredFunctions[functionName]
                : null;
        }

        public Dictionary<string, FunctionInfo> GetAllFunctions()
        {
            return new Dictionary<string, FunctionInfo>(declaredFunctions);
        }
        
        public bool IsInsideFunction()
        {
            return currentFunction != null;
        }

        public string? GetCurrentFunctionName()
        {
            return currentFunctionName;
        }

    }
}