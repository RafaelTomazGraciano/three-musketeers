using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Comparison
{
    public class ComparisonSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public ComparisonSemanticAnalyzer(
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public string VisitComparison([NotNull] ExprParser.ComparisonContext context)
        {
            var leftType = getExpressionType(context.expr(0));
            var rightType = getExpressionType(context.expr(1));
            
            // Visit both expressions to ensure they're valid
            visitExpression(context.expr(0));
            visitExpression(context.expr(1));
            
            // Comparison operations require numeric types
            if (!AreComparableTypes(leftType, rightType))
            {
                reportError(context.Start.Line, 
                    $"Cannot compare non-numeric types '{leftType}' and '{rightType}'");
                return "bool";
            }
            
            // Comparison operations always return bool
            return "bool";
        }

        private bool AreComparableTypes(string type1, string type2)
        {
            // Both must be numeric types for comparison
            return IsNumericType(type1) && IsNumericType(type2);
        }

        private bool IsNumericType(string type)
        {
            return type == "int" || type == "double" || type == "char" || type == "bool";
        }
    }
}
