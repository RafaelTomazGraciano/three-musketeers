using System.Diagnostics;
using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.SemanticAnalysis;
using Three_Musketeers.Visitors.SemanticAnalysis.Variables;
using Three_Musketeers.Visitors.SemanticAnalysis.InputOutput;
using Three_Musketeers.Visitors.SemanticAnalysis.StringConversion;
using Three_Musketeers.Visitors.SemanticAnalysis.Arithmetic;

namespace Three_Musketeers.Visitors
{
    public class SemanticAnalyzer : SemanticAnalyzerBase
    {
        private readonly VariableAssignmentSemanticAnalyzer variableAssignmentSemanticAnalyzer;
        private readonly PrintfSemanticAnalyzer printfSemanticAnalyzer;
        private readonly ScanfSemanticAnalyzer scanfSemanticAnalyzer;
        private readonly GetsSemanticAnalyzer getsSemanticAnalyzer;
        private readonly PutsSemanticAnalyzer putsSemanticAnalyzer;
        private readonly AtoiSemanticAnalyzer atoiSemanticAnalyzer;
        private readonly AtodSemanticAnalyzer atodSemanticAnalyzer;
        private readonly ItoaSemanticAnalyzer itoaSemanticAnalyzer;
        private readonly DtoaSemanticAnalyzer dtoaSemanticAnalyzer;
        private readonly ArithmeticSemanticAnalyzer arithmeticSemanticAnalyzer;

        public SemanticAnalyzer()
        {
            //variables
            variableAssignmentSemanticAnalyzer = new VariableAssignmentSemanticAnalyzer(symbolTable,
                ReportError, ReportWarning, Visit);
            // input-output
            printfSemanticAnalyzer = new PrintfSemanticAnalyzer(ReportError, ReportWarning, GetExpressionType, Visit);
            scanfSemanticAnalyzer = new ScanfSemanticAnalyzer(ReportError, symbolTable);
            getsSemanticAnalyzer = new GetsSemanticAnalyzer(ReportError, symbolTable);
            putsSemanticAnalyzer = new PutsSemanticAnalyzer(ReportError, symbolTable);
            // string conversion
            atoiSemanticAnalyzer = new AtoiSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit);
            atodSemanticAnalyzer = new AtodSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit);
            itoaSemanticAnalyzer = new ItoaSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit);
            dtoaSemanticAnalyzer = new DtoaSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit);
            // arithmetic
            arithmeticSemanticAnalyzer = new ArithmeticSemanticAnalyzer(ReportError, GetExpressionType, Visit);

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

        public override string? VisitTrueLiteral([NotNull] ExprParser.TrueLiteralContext context)
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

        public override string VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            return atoiSemanticAnalyzer.VisitAtoiConversion(context);
        }

        public override string VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            return atodSemanticAnalyzer.VisitAtodConversion(context);
        }

        public override string VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
            return itoaSemanticAnalyzer.VisitItoaConversion(context);
        }

        public override string VisitDtoaConversion([NotNull] ExprParser.DtoaConversionContext context)
        {
            return dtoaSemanticAnalyzer.VisitDtoaConversion(context);
        }

        public override string VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            return arithmeticSemanticAnalyzer.VisitAddSub(context) ?? "int";
        }

        public override string VisitMulDivMod([NotNull] ExprParser.MulDivModContext context)
        {
            return arithmeticSemanticAnalyzer.VisitMulDivMod(context) ?? "int";
        }

        public override string VisitUnaryMinus([NotNull] ExprParser.UnaryMinusContext context)
        {
            string exprType = GetExpressionType(context.expr());
            Visit(context.expr());
            return exprType; // Unary minus preserves the type
        }
    }
}

