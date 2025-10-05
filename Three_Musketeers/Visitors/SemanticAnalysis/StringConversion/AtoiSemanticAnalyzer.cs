using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.StringConversion
{
    public class AtoiSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public AtoiSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public string VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            var expr = context.expr();
            visitExpression(expr);

            string exprType = getExpressionType(expr);

            if (exprType != "string")
            {
                reportError(context.Start.Line,
                    $"atoi() expects a string argument, but got '{exprType}'");
            }

            return "int";
        }
        
    }
}