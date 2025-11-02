using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.StringConversion
{
    public class ItoaSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly LibraryDependencyTracker libraryTracker;

        public ItoaSemanticAnalyzer(
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

        public string? VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("itoa", context.Start.Line))
            {
                return null;
            }
            
            var expr = context.expr();
            visitExpression(expr);

            string exprType = getExpressionType(expr);
            if (exprType != "int")
            {
                reportError(context.Start.Line,
                    $"itoa() expects an int argument, but got '{exprType}'");
            }
            return "string";
        }
    }
}    