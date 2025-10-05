using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Logical
{
    public class LogicalSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public LogicalSemanticAnalyzer(
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public string VisitLogicalAndOr([NotNull] ExprParser.LogicalAndOrContext context)
        {
            var leftType = getExpressionType(context.expr(0));
            var rightType = getExpressionType(context.expr(1));
            
            // Visit both expressions to ensure they're valid
            visitExpression(context.expr(0));
            visitExpression(context.expr(1));
            
            // Logical operations require boolean-compatible types
            if (!IsValidLogicalType(leftType) || !IsValidLogicalType(rightType))
            {
                reportError(context.Start.Line, 
                    $"Logical operations require boolean-compatible types, got '{leftType}' and '{rightType}'");
                return "bool";
            }
            
            // Logical operations always return bool
            return "bool";
        }

        public string VisitLogicalNot([NotNull] ExprParser.LogicalNotContext context)
        {
            var exprType = getExpressionType(context.expr());
            
            // Visit the expression to ensure it's valid
            visitExpression(context.expr());
            
            // Logical NOT requires boolean-compatible type
            if (!IsValidLogicalType(exprType))
            {
                reportError(context.Start.Line, 
                    $"Logical NOT operation requires boolean-compatible type, got '{exprType}'");
                return "bool";
            }
            
            // Logical NOT always returns bool
            return "bool";
        }

        private bool IsValidLogicalType(string type)
        {
            return type == "bool" || type == "int" || type == "double" || type == "char";
        }
    }
}
