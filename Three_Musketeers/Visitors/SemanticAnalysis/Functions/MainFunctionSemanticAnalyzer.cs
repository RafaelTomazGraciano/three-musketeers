using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Functions
{
    public class MainFunctionSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;

        private bool hasReturnStatement = false;

        public MainFunctionSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Func<ExprParser.StmContext, string?> visitStatement)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.visitStatement = visitStatement;
        }

        public void AnalyzeMainFunction(ExprParser.MainFunctionContext context)
        {
            int line = context.Start.Line;

            //enter main scope
            symbolTable.EnterScope();
            hasReturnStatement = false;

            //check if has arguments
            var argsCtx = context.mainArgs();
            if (argsCtx != null)
            {
                ProcessMainArguments(argsCtx);
            }

            //analyze body
            var funcBodyCtx = context.func_body();
            if (funcBodyCtx != null)
            {
                AnalyzeMainBody(funcBodyCtx);
            }

            if (!hasReturnStatement)
            {
                reportError(line, "Main function does not have a 'return' statement");
            }

            symbolTable.ExitScope();
        }

        private void ProcessMainArguments(ExprParser.MainArgsContext context)
        {
            int line = context.Start.Line;

            // get argument names from grammar
            var idTokens = context.ID();
            
            if (idTokens.Length == 2)
            {
                // first argument: int argc
                string argcName = idTokens[0].GetText();
                var argcSymbol = new Symbol(argcName, "int", line);
                argcSymbol.isInitializated = true;
                
                if (!symbolTable.AddSymbol(argcSymbol))
                {
                    reportError(line, $"Failed to add argument '{argcName}' to symbol table");
                }

                // second argument: char argv[]
                string argvName = idTokens[1].GetText();
                var argvSymbol = new ArraySymbol(argvName, "string", line, new List<int>());
                argvSymbol.isInitializated = true;
                
                if (!symbolTable.AddSymbol(argvSymbol))
                {
                    reportError(line, $"Failed to add argument '{argvName}' to symbol table");
                }
            }
        }

        private void AnalyzeMainBody(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                if (stm.RETURN() != null)
                {
                    hasReturnStatement = true;
                    AnalyzeReturnStatement(stm);
                }
                else
                {
                    visitStatement(stm);
                }
            }
        }

        private void AnalyzeReturnStatement(ExprParser.StmContext context)
        {
            int line = context.Start.Line;
            var exprCtx = context.expr();

            if (exprCtx == null)
            {
                reportError(line, "Main function must return an integer value");
            }
        }
    }
}