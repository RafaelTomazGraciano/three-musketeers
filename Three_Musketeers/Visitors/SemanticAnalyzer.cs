using System.Diagnostics;
using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.SemanticAnalysis;

namespace Three_Musketeers.Visitors
{
    public class SemanticAnalyzer : SemanticAnalyzerBase
    {
        private readonly VariableAssignmentSemanticAnalyzer variableAssignmentSemanticAnalyzer;
        private readonly PrintfSemanticAnalyzer printfSemanticAnalyzer;
        private readonly ScanfSemanticAnalyzer scanfSemanticAnalyzer;
        private readonly GetsSemanticAnalyzer getsSemanticAnalyzer;

        public SemanticAnalyzer()
        {
            variableAssignmentSemanticAnalyzer = new VariableAssignmentSemanticAnalyzer(symbolTable,
                ReportError, ReportWarning, Visit);
            printfSemanticAnalyzer = new PrintfSemanticAnalyzer(ReportError, ReportWarning, GetExpressionType, Visit);
            scanfSemanticAnalyzer = new ScanfSemanticAnalyzer(ReportError, symbolTable);
            getsSemanticAnalyzer = new GetsSemanticAnalyzer(ReportError, symbolTable);
        }

        public override object? VisitStart([NotNull] ExprParser.StartContext context)
        {
            return base.VisitStart(context);
        }

        public override object? VisitProg([NotNull] ExprParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override object? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitAtt(context);
        }

        public override object? VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitVar(context);
        }

        public override object VisitStringLiteral([NotNull] ExprParser.StringLiteralContext context)
        {
            return "string";
        }

        public override object? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            return printfSemanticAnalyzer.VisitPrintfStatement(context);
        }

        public override object? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            return scanfSemanticAnalyzer.VisitScanfStatement(context);
        }

        public override object VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            return getsSemanticAnalyzer.VisitGetsStatement(context);
        }
    }
}

