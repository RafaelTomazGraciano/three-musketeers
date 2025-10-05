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
            return "bool";
        }

        public override string VisitFalseLiteral([NotNull] ExprParser.FalseLiteralContext context)
        {
            return "false";
        }

        public override string? VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            string leftType = Visit(context.expr(0)) ?? "?";
            string rightType = Visit(context.expr(1)) ?? "?";
            bool isMinus = context.GetText().Contains('-');

            if (leftType == "string" || rightType == "string")
            {
                if (isMinus)
                {
                    ReportError(context.Start.Line, "Cannot perform '-' with strings");
                    return null;
                }
                return "string";
            }

            if (!TwoTypesArePermitedToCast(leftType, rightType))
            {
                ReportError(context.Start.Line,
                    $"Cannot perform '{(isMinus ? '-' : '+')}' between '{leftType}' and '{rightType}'");
                return null;
            }

            if (leftType == "double" || rightType == "double")
            {
                return "double";
            }

            if (leftType == "int" || rightType == "int")
            {
                return "int";
            }

            if (leftType == "char" || rightType == "char")
            {
                return "char";
            }

            return "bool";
        }

        public override string? VisitMulDiv([NotNull] ExprParser.MulDivContext context)
        {
            string leftType = Visit(context.expr(0)) ?? "?";
            string rightType = Visit(context.expr(1)) ?? "?";
            bool isDiv = context.GetText().Contains('/');

            if (leftType == "string" || rightType == "string")
            {
                ReportError(context.Start.Line, $"Cannot perform '{(isDiv ? '/' : '*')}' with strings");
                return null;
            }

            if (!TwoTypesArePermitedToCast(leftType, rightType))
            {
                ReportError(context.Start.Line,
                    $"Cannot perform '{(isDiv ? '/' : '*')}' between '{leftType}' and '{rightType}'");
                return null;
            }

            if (leftType == "double" || rightType == "double")
            {
                return "double";
            }

            if (leftType == "int" || rightType == "int")
            {
                return "int";
            }

            if (leftType == "char" || rightType == "char")
            {
                return "char";
            }

            return "bool";
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

        private static bool TwoTypesArePermitedToCast(string type1, string type2)
        {
            bool anyIsDouble = type1 == "double" || type2 == "double";
            bool anyIsChar = type1 == "char" || type2 == "char";
            bool anyIsInt = type1 == "int" || type2 == "int";
            bool anyIsBool = type1 == "bool" || type2 == "bool";
            bool anyIsString = type1 == "string" || type2 == "string";
            if (type1 == type2) return true;
            if (anyIsDouble && (anyIsChar || anyIsInt || anyIsBool)) return true;
            if (anyIsInt && (anyIsChar || anyIsBool)) return true;
            if (anyIsChar && (anyIsBool || !anyIsString)) return true;
            return false;
        }
    }
}

