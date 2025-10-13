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
            string functionName = context.ID().GetText();

            // function already declared
            if (!declaredFunctions.ContainsKey(functionName))
            {
                reportError(line, $"Internal error: Function '{functionName}' not pre-registered");
                return;
            }

            var functionInfo = declaredFunctions[functionName];

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
                reportWarning(line, $"Function '{functionName}' with return type '{functionInfo.returnType}' does not have a 'return' statement");
            }

            // exit function scope
            symbolTable.ExitScope();

            // exit function context
            currentFunctionName = null;
            currentFunction = null;
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
                string? returnTypeDisplay = currentFunction.returnType;
                return;
            }

            string? returnExprType = getExpressionType(exprCtx);
            if (returnExprType == null)
            {
                reportError(line, $"Could not determine the type of return expression");
                return;
            }

            string expectedType = currentFunction.returnType!;

            if (currentFunction.returnType != returnExprType)
            {
                string? returnTypeDisplay = currentFunction.returnType;
            }
        }

    }
}