using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.ControlFlow
{
    public class LoopStatementSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string?> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private readonly Func<IParseTree, string?> visitContext;
        private readonly Action enterLoopContext;
        private readonly Action exitLoopContext;

        public LoopStatementSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string?> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression,
            Func<ExprParser.StmContext, string?> visitStatement,
            Func<IParseTree, string?> visitContext,
            Action enterLoopContext,
            Action exitLoopContext)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
            this.visitStatement = visitStatement;
            this.visitContext = visitContext;
            this.enterLoopContext = enterLoopContext;
            this.exitLoopContext = exitLoopContext;
        }

        public string? VisitForStatement([NotNull] ExprParser.ForStatementContext context)
        {
            int line = context.Start.Line;
            var forInit = context.forInit();
            var forCondition = context.forCondition();
            var forIncrement = context.forIncrement();
            var body = context.func_body();

            // Enter scope for the entire for loop (init variables are in loop scope)
            symbolTable.EnterScope();

            // Analyze initialization (if present)
            if (forInit != null)
            {
                visitContext(forInit);
            }

            // Analyze condition (if present)
            if (forCondition != null)
            {
                string? conditionType = getExpressionType(forCondition.expr());
                visitExpression(forCondition.expr());

                if (conditionType != null && !IsValidConditionType(conditionType))
                {
                    reportError(forCondition.Start.Line,
                        $"For loop condition must be a boolean-compatible type, got '{conditionType}'");
                }
            }

            // Analyze increment (if present) - just visit it, type checking happens in visitExpression/visitAtt/visitAttVar
            if (forIncrement != null)
            {
                visitContext(forIncrement);
            }

            // Enter loop context for break/continue validation
            enterLoopContext();

            // Enter scope for loop body
            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            // Exit loop scope
            symbolTable.ExitScope();

            // Exit loop context
            exitLoopContext();

            return null;
        }

        public string? VisitWhileStatement([NotNull] ExprParser.WhileStatementContext context)
        {
            int line = context.Start.Line;
            var condition = context.expr();
            var body = context.func_body();

            // Validate condition
            string? conditionType = getExpressionType(condition);
            visitExpression(condition);

            if (conditionType != null && !IsValidConditionType(conditionType))
            {
                reportError(condition.Start.Line,
                    $"While loop condition must be a boolean-compatible type, got '{conditionType}'");
            }

            // Enter loop context for break/continue validation
            enterLoopContext();

            // Enter scope for loop body
            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            // Exit loop context
            exitLoopContext();

            return null;
        }

        public string? VisitDoWhileStatement([NotNull] ExprParser.DoWhileStatementContext context)
        {
            int line = context.Start.Line;
            var condition = context.expr();
            var body = context.func_body();

            // Enter loop context for break/continue validation
            enterLoopContext();

            // Enter scope for loop body
            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            // Validate condition
            string? conditionType = getExpressionType(condition);
            visitExpression(condition);

            if (conditionType != null && !IsValidConditionType(conditionType))
            {
                reportError(condition.Start.Line,
                    $"Do-while loop condition must be a boolean-compatible type, got '{conditionType}'");
            }

            // Exit loop context
            exitLoopContext();

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

