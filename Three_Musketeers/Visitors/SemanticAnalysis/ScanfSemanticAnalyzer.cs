using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public class ScanfSemanticAnalyzer
    {

        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, object?> visitExpression;

        public ScanfSemanticAnalyzer(
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, object?> visitExpression)
        {
            this.reportError = reportError;
            this.visitExpression = visitExpression;
        }

        public object? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); // remove quotes

            int expectedArgs = CountFormatSpecifier.Count(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if (expectedArgs != providedArgs)
            {
                reportError(context.Start.Line, $"scanf expects {expectedArgs} argument(s), but received {providedArgs}");
            }

            if (context.expr() != null)
            {
                foreach (var expr in context.expr())
                {
                    visitExpression(expr);
                }
            }
            return null;
        }
    }
}