using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.ControlFlow
{
    public class SwitchStatementSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string?> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;

        public SwitchStatementSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string?> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression,
            Func<ExprParser.StmContext, string?> visitStatement)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
            this.visitStatement = visitStatement;
        }

        public string? VisitSwitchStatement([NotNull] ExprParser.SwitchStatementContext context)
        {
            int line = context.Start.Line;
            var switchExpr = context.expr();
            var caseLabels = context.caseLabel();
            var defaultLabel = context.defaultLabel();

            // Validate switch expression type
            string? switchExprType = getExpressionType(switchExpr);
            visitExpression(switchExpr);

            if (switchExprType != null && !IsValidSwitchType(switchExprType))
            {
                reportError(switchExpr.Start.Line,
                    $"Switch expression must be an integer-compatible type (int, char, bool), got '{switchExprType}'");
            }

            // Track case values to detect duplicates
            HashSet<string> caseValues = new HashSet<string>();

            // Analyze each case label
            foreach (var caseLabel in caseLabels)
            {
                AnalyzeCaseLabel(caseLabel, caseValues);
            }

            // Analyze default label if present
            if (defaultLabel != null)
            {
                AnalyzeDefaultLabel(defaultLabel);
            }

            return null;
        }

        private void AnalyzeCaseLabel([NotNull] ExprParser.CaseLabelContext context, HashSet<string> caseValues)
        {
            int line = context.Start.Line;
            var intToken = context.INT();
            var charToken = context.CHAR_LITERAL();

            // Case value must be a constant (INT or CHAR_LITERAL)
            string caseValue;
            if (intToken != null)
            {
                caseValue = intToken.GetText();
            }
            else if (charToken != null)
            {
                caseValue = charToken.GetText();
            }
            else
            {
                reportError(line, "Case label must have a constant integer or character literal value");
                return;
            }

            // Check for duplicate case values
            if (caseValues.Contains(caseValue))
            {
                reportError(line, $"Duplicate case value: {caseValue}");
            }
            else
            {
                caseValues.Add(caseValue);
            }

            // Enter scope for case block
            symbolTable.EnterScope();
            AnalyzeBlock(context.func_body());
            symbolTable.ExitScope();
        }

        private void AnalyzeDefaultLabel([NotNull] ExprParser.DefaultLabelContext context)
        {
            // Enter scope for default block
            symbolTable.EnterScope();
            AnalyzeBlock(context.func_body());
            symbolTable.ExitScope();
        }

        private void AnalyzeBlock(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                visitStatement(stm);
            }
        }

        private bool IsValidSwitchType(string type)
        {
            // Switch expressions can be int, char, or bool (integer-compatible types)
            return type == "int" || type == "char" || type == "bool";
        }
    }
}

