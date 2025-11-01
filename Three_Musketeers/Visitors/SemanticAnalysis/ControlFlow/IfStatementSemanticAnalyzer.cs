using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.ControlFlow
{
    public class IfStatementSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string?> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;

        public IfStatementSemanticAnalyzer(
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

        public string? VisitIfStatement([NotNull] ExprParser.IfStatementContext context)
        {
            int line = context.Start.Line;
            var expressions = context.expr();
            var bodies = context.func_body();
            var elseTokens = context.ELSE();

            // Validate that we have matching conditions and bodies
            // The grammar ensures this, but we validate for safety
            if (expressions.Length != bodies.Length && expressions.Length != bodies.Length - 1)
            {
                reportError(line, "Internal error: Mismatch between conditions and blocks in if statement");
                return null;
            }

            // Analyze the if condition
            var ifCondition = expressions[0];
            string? conditionType = getExpressionType(ifCondition);
            visitExpression(ifCondition);

            if (conditionType != null && !IsValidConditionType(conditionType))
            {
                reportError(ifCondition.Start.Line, 
                    $"If condition must be a boolean-compatible type, got '{conditionType}'");
            }

            // Enter scope for if block
            symbolTable.EnterScope();
            AnalyzeBlock(bodies[0]);
            symbolTable.ExitScope();

            // Analyze else if chains
            int elseIfCount = expressions.Length - 1;
            if (elseTokens != null && elseTokens.Length > 0 && bodies.Length > 1)
            {
                // Check if last ELSE is a final else or an else if
                bool hasFinalElse = elseTokens.Length > elseIfCount;
                
                // Process else if blocks (if any)
                for (int i = 1; i <= elseIfCount; i++)
                {
                    var elseIfCondition = expressions[i];
                    string? elseIfConditionType = getExpressionType(elseIfCondition);
                    visitExpression(elseIfCondition);

                    if (elseIfConditionType != null && !IsValidConditionType(elseIfConditionType))
                    {
                        reportError(elseIfCondition.Start.Line, 
                            $"Else if condition must be a boolean-compatible type, got '{elseIfConditionType}'");
                    }

                    // Enter scope for else if block
                    symbolTable.EnterScope();
                    AnalyzeBlock(bodies[i]);
                    symbolTable.ExitScope();
                }

                // Process final else block (if present)
                if (hasFinalElse)
                {
                    int elseBodyIndex = bodies.Length - 1;
                    symbolTable.EnterScope();
                    AnalyzeBlock(bodies[elseBodyIndex]);
                    symbolTable.ExitScope();
                }
            }
            else if (elseIfCount > 0)
            {
                // Process else if blocks without final else
                for (int i = 1; i <= elseIfCount; i++)
                {
                    var elseIfCondition = expressions[i];
                    string? elseIfConditionType = getExpressionType(elseIfCondition);
                    visitExpression(elseIfCondition);

                    if (elseIfConditionType != null && !IsValidConditionType(elseIfConditionType))
                    {
                        reportError(elseIfCondition.Start.Line, 
                            $"Else if condition must be a boolean-compatible type, got '{elseIfConditionType}'");
                    }

                    // Enter scope for else if block
                    symbolTable.EnterScope();
                    AnalyzeBlock(bodies[i]);
                    symbolTable.ExitScope();
                }
            }

            return null;
        }

        private void AnalyzeBlock(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                visitStatement(stm);
            }
        }

        private bool IsValidConditionType(string type)
        {
            // Conditions can be bool, int, double, or char (treated as truthy/falsy)
            return type == "bool" || type == "int" || type == "double" || type == "char";
        }
    }
}

