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

            symbolTable.EnterScope();

            if (forInit != null)
            {
                visitContext(forInit);
            }

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

            if (forIncrement != null)
            {
                visitContext(forIncrement);
            }

            enterLoopContext();

            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            symbolTable.ExitScope();

            exitLoopContext();

            return null;
        }

        public string? VisitWhileStatement([NotNull] ExprParser.WhileStatementContext context)
        {
            int line = context.Start.Line;
            var condition = context.expr();
            var body = context.func_body();

            string? conditionType = getExpressionType(condition);
            visitExpression(condition);

            if (conditionType != null && !IsValidConditionType(conditionType))
            {
                reportError(condition.Start.Line,
                    $"While loop condition must be a boolean-compatible type, got '{conditionType}'");
            }

            enterLoopContext();

            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            exitLoopContext();

            return null;
        }

        public string? VisitDoWhileStatement([NotNull] ExprParser.DoWhileStatementContext context)
        {
            int line = context.Start.Line;
            var condition = context.expr();
            var body = context.func_body();

            enterLoopContext();

            symbolTable.EnterScope();
            AnalyzeBlock(body);
            symbolTable.ExitScope();

            string? conditionType = getExpressionType(condition);
            visitExpression(condition);

            if (conditionType != null && !IsValidConditionType(conditionType))
            {
                reportError(condition.Start.Line,
                    $"Do-while loop condition must be a boolean-compatible type, got '{conditionType}'");
            }

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
            return type == "bool" || type == "int" || type == "double" || type == "char";
        }
    }
}

