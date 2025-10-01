using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public class PrintfSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, object?> visitExpression;

        public PrintfSemanticAnalyzer(
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, object?> visitExpression)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public object? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            string formatString = context.STRING_LITERAL().GetText();
            formatString = formatString.Substring(1, formatString.Length - 2); // remove quotes

            var specifiers = FormatSpecifierParser.Parse(formatString);
            int providedArgs = context.expr()?.Length ?? 0;

            if (specifiers.Count != providedArgs)
            {
                reportError(context.Start.Line,
                $"printf expects {specifiers.Count} argument(s) for \'{formatString}\' but received {providedArgs}");
                return null;
            }

            if (context.expr() != null)
            {
                for (int i = 0; i < context.expr().Length; i++)
                {
                    var expr = context.expr()[i];
                    visitExpression(expr);

                    string exprType = getExpressionType(expr);
                    char specifier = specifiers[i].type;

                    if (!TypesAreCompatible(exprType, specifier))
                    {
                        bool canConvert = (exprType == "int" && "f".Contains(specifier)) ||
                        (exprType == "double" && "di".Contains(specifier));

                        if (canConvert && exprType == "double" && "di".Contains(specifier))
                        {
                            reportWarning(context.Start.Line,
                            $"Argument {i + 1}: implicit conversion from \'{exprType}\' to \'int\' for \'%{specifier}\' may lose precision");
                        }
                        else if (!canConvert)
                        {
                            reportError(context.Start.Line,
                            $"Argument {i + 1}: format specifier \'%{specifier}\' expects compatible type, but got \'{exprType}\'");
                        }
                    }
                }
            }
            return null;
        }

        private bool TypesAreCompatible(string exprType, char specifier)
        {
            return specifier switch
            {
                'd' or 'i' => exprType == "int",
                'f' => exprType == "double" || exprType == "float",
                'c' => exprType == "int" || exprType == "char",
                's' => exprType == "string",
                _ => false
            };
        }
    }
}

