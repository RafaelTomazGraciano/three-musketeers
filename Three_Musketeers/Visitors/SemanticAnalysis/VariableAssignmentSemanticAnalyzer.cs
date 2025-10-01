using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public class VariableAssignmentSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly Func<ExprParser.ExprContext, object?> visitExpression;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Func<ExprParser.ExprContext, object?> visitExpression)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.visitExpression = visitExpression;
        }

        public object? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);
            symbolTable.MarkInitializated(varName);

            visitExpression(context.expr());

            return null;
        }

        public object? VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if (!symbol.isInitializated)
            {
                reportWarning(line, $"Variable '{varName}' may not be initialized");
            }

            return symbol.type;
        }
    }
}

