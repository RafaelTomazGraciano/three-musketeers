using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.StringConversion
{
    public class AtodSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly LibraryDependencyTracker libraryTracker;

        public AtodSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression,
            LibraryDependencyTracker libraryTracker)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
            this.libraryTracker = libraryTracker;
        }

        public string? VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("atod", context.Start.Line))
            {
                return null;
            }

            var expr = context.expr();
            visitExpression(expr);

            string exprType = getExpressionType(expr);
            if (exprType != "string")
            {
                reportError(context.Start.Line,
                    $"atod() expects a string argument, but got '{exprType}'");
            }

            return "double";
        }
    }
}