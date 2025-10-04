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
        private readonly PutsSemanticAnalyzer putsSemanticAnalyzer;

        public SemanticAnalyzer()
        {
            variableAssignmentSemanticAnalyzer = new VariableAssignmentSemanticAnalyzer(symbolTable,
                ReportError, ReportWarning, Visit);
            printfSemanticAnalyzer = new PrintfSemanticAnalyzer(ReportError, ReportWarning, GetExpressionType, Visit);
            scanfSemanticAnalyzer = new ScanfSemanticAnalyzer(ReportError, symbolTable);
            getsSemanticAnalyzer = new GetsSemanticAnalyzer(ReportError, symbolTable);
            putsSemanticAnalyzer = new PutsSemanticAnalyzer(ReportError, symbolTable);
        }

        public override string? VisitStart([NotNull] ExprParser.StartContext context)
        {
            return base.VisitStart(context);
        }

        public override string? VisitProg([NotNull] ExprParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitDeclaration(context);
        }

        public override string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitAtt(context);
        }

        public override string? VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitVar(context);
        }

        public override string VisitStringLiteral([NotNull] ExprParser.StringLiteralContext context)
        {
            return "string";
        }

        public override string VisitIntLiteral([NotNull] ExprParser.IntLiteralContext context)
        {
            return "int";
        }

        public override string VisitDoubleLiteral([NotNull] ExprParser.DoubleLiteralContext context)
        {
            return "double";
        }

        public override string VisitCharLiteral([NotNull] ExprParser.CharLiteralContext context)
        {
            return "char";
        }

        public override string VisitTrueLiteral([NotNull] ExprParser.TrueLiteralContext context)
        {
            return base.VisitTrueLiteral(context);
        }

        public override string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            return printfSemanticAnalyzer.VisitPrintfStatement(context);
        }

        public override string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            return scanfSemanticAnalyzer.VisitScanfStatement(context);
        }

        public override string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            return getsSemanticAnalyzer.VisitGetsStatement(context);
        }

        public override string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            return putsSemanticAnalyzer.VisitPutsStatement(context);
        }

    }
}

