using System.Diagnostics;
using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.SemanticAnalysis;
using Three_Musketeers.Visitors.SemanticAnalysis.Variables;
using Three_Musketeers.Visitors.SemanticAnalysis.InputOutput;
using Three_Musketeers.Visitors.SemanticAnalysis.StringConversion;
using Three_Musketeers.Visitors.SemanticAnalysis.Arithmetic;
using Three_Musketeers.Visitors.SemanticAnalysis.Logical;
using Three_Musketeers.Visitors.SemanticAnalysis.Equality;
using Three_Musketeers.Visitors.SemanticAnalysis.Comparison;
using Three_Musketeers.Visitors.SemanticAnalysis.IncrementDecrement;
using Three_Musketeers.Visitors.SemanticAnalysis.CompoundAssignment;

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
        private readonly LogicalSemanticAnalyzer logicalSemanticAnalyzer;
        private readonly EqualitySemanticAnalyzer equalitySemanticAnalyzer;
        private readonly ComparisonSemanticAnalyzer comparisonSemanticAnalyzer;
        private readonly IncrementDecrementSemanticAnalyzer incrementDecrementSemanticAnalyzer;
        private readonly CompoundAssignmentSemanticAnalyzer compoundAssignmentSemanticAnalyzer;

        public SemanticAnalyzer()
        {
            //variables
            variableAssignmentSemanticAnalyzer = new VariableAssignmentSemanticAnalyzer(symbolTable,
                ReportError, ReportWarning);
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
            // logical
            logicalSemanticAnalyzer = new LogicalSemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // equality
            equalitySemanticAnalyzer = new EqualitySemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // comparison
            comparisonSemanticAnalyzer = new ComparisonSemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // increment/decrement
            incrementDecrementSemanticAnalyzer = new IncrementDecrementSemanticAnalyzer(ReportError, ReportWarning, symbolTable);
            // compound assignment
            compoundAssignmentSemanticAnalyzer = new CompoundAssignmentSemanticAnalyzer(ReportError, ReportWarning, symbolTable, GetExpressionType);
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

        public override string? VisitAttRegular([NotNull] ExprParser.AttRegularContext context)
        {
            string? type = variableAssignmentSemanticAnalyzer.VisitAttRegular(context);
            string? exprType = Visit(context.expr());
            if (type == null || exprType == null) return null;

            if (!TwoTypesArePermitedToCast(type, exprType))
            {
                ReportError(context.Start.Line,
                    $"Cannot assign value of type '{exprType}' to variable of type '{type}'");
                return null;
            }

            return type;
        }

        public override string? VisitAttPlusEquals([NotNull] ExprParser.AttPlusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitAttPlusEquals(context);
        }

        public override string? VisitAttMinusEquals([NotNull] ExprParser.AttMinusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitAttMinusEquals(context);
        }

        public override string? VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitVar(context);
        }

        public override string? VisitSingleAtt([NotNull] ExprParser.SingleAttContext context)
        {
            string? arrayType = variableAssignmentSemanticAnalyzer.VisitSingleAtt(context);
            string? exprType = Visit(context.expr());
            if (arrayType == null || exprType == null) return null;

            if (!TwoTypesArePermitedToCast(arrayType, exprType))
            {
                ReportError(context.Start.Line,
                    $"Cannot assign value of type '{exprType}' to array element of type '{arrayType}'");
                return null;
            }

            return arrayType;
        }

        public override string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttPlusEquals(context);
        }

        public override string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttMinusEquals(context);
        }

        public override string? VisitAttMultiplyEquals([NotNull] ExprParser.AttMultiplyEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitAttMultiplyEquals(context);
        }

        public override string? VisitAttDivideEquals([NotNull] ExprParser.AttDivideEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitAttDivideEquals(context);
        }

        public override string? VisitSingleAttMultiplyEquals([NotNull] ExprParser.SingleAttMultiplyEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttMultiplyEquals(context);
        }

        public override string? VisitSingleAttDivideEquals([NotNull] ExprParser.SingleAttDivideEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttDivideEquals(context);
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

        public override string VisitLogicalAndOr([NotNull] ExprParser.LogicalAndOrContext context)
        {
            return logicalSemanticAnalyzer.VisitLogicalAndOr(context);
        }

        public override string VisitLogicalNot([NotNull] ExprParser.LogicalNotContext context)
        {
            return logicalSemanticAnalyzer.VisitLogicalNot(context);
        }

        public override string VisitEquality([NotNull] ExprParser.EqualityContext context)
        {
            return equalitySemanticAnalyzer.VisitEquality(context);
        }

        public override string VisitComparison([NotNull] ExprParser.ComparisonContext context)
        {
            return comparisonSemanticAnalyzer.VisitComparison(context);
        }

        // Increment/Decrement operators for simple variables
        public override string VisitPrefixIncrement([NotNull] ExprParser.PrefixIncrementContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPrefixIncrement(context) ?? "int";
        }

        public override string VisitPrefixDecrement([NotNull] ExprParser.PrefixDecrementContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPrefixDecrement(context) ?? "int";
        }

        public override string VisitPostfixIncrement([NotNull] ExprParser.PostfixIncrementContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPostfixIncrement(context) ?? "int";
        }

        public override string VisitPostfixDecrement([NotNull] ExprParser.PostfixDecrementContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPostfixDecrement(context) ?? "int";
        }

        // Increment/Decrement operators for array elements
        public override string VisitPrefixIncrementArray([NotNull] ExprParser.PrefixIncrementArrayContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPrefixIncrementArray(context) ?? "int";
        }

        public override string VisitPrefixDecrementArray([NotNull] ExprParser.PrefixDecrementArrayContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPrefixDecrementArray(context) ?? "int";
        }

        public override string VisitPostfixIncrementArray([NotNull] ExprParser.PostfixIncrementArrayContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPostfixIncrementArray(context) ?? "int";
        }

        public override string VisitPostfixDecrementArray([NotNull] ExprParser.PostfixDecrementArrayContext context)
        {
            return incrementDecrementSemanticAnalyzer.VisitPostfixDecrementArray(context) ?? "int";
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

