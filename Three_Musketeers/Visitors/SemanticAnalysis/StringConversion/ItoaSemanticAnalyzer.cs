using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.StringConversion
{
    public class ItoaSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public ItoaSemanticAnalyzer(
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

        public string VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
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