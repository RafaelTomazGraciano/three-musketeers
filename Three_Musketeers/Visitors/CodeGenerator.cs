using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.CodeGeneration;
using Three_Musketeers.Visitors.CodeGeneration.Variables;
using Three_Musketeers.Visitors.CodeGeneration.InputOutput;
using Three_Musketeers.Visitors.CodeGeneration.StringConversion;
using Three_Musketeers.Visitors.CodeGeneration.Arithmetic;
using Three_Musketeers.Visitors.CodeGeneration.Logical;
using Three_Musketeers.Visitors.CodeGeneration.Equality;
using Three_Musketeers.Visitors.CodeGeneration.Comparison;

namespace Three_Musketeers.Visitors
{
    public class CodeGenerator : CodeGeneratorBase
    {
        private readonly VariableAssignmentCodeGenerator variableAssignmentCodeGenerator;
        private readonly PrintfCodeGenerator printfCodeGenerator;
        private readonly ScanfCodeGenerator scanfCodeGenerator;
        private readonly StringCodeGenerator stringCodeGenerator;
        private readonly CharCodeGenerator charCodeGenerator;
        private readonly GetsCodeGenerator getsCodeGenerator;
        private readonly PutsCodeGenerator putsCodeGenerator;
        private readonly AtoiCodeGenerator atoiCodeGenerator;
        private readonly AtodCodeGenerator atodCodeGenerator;
        private readonly ItoaCodeGenerator itoaCodeGenerator;
        private readonly DtoaCodeGenerator dtoaCodeGenerator;
        private readonly ArithmeticCodeGenerator arithmeticCodeGenerator;
        private readonly LogicalCodeGenerator logicalCodeGenerator;
        private readonly EqualityCodeGenerator equalityCodeGenerator;
        private readonly ComparisonCodeGenerator comparisonCodeGenerator;

        public CodeGenerator()
        {
            //variables
            variableAssignmentCodeGenerator = new VariableAssignmentCodeGenerator(
                mainBody, declarations, variables, registerTypes, NextRegister, GetLLVMType, Visit);
            stringCodeGenerator = new StringCodeGenerator(globalStrings, registerTypes, NextStringLabel);
            charCodeGenerator = new CharCodeGenerator(registerTypes);

            //input-output
            printfCodeGenerator = new PrintfCodeGenerator(
                globalStrings, mainBody, registerTypes, NextRegister, NextStringLabel, Visit);
            scanfCodeGenerator = new ScanfCodeGenerator(globalStrings, mainBody, variables,
                registerTypes, NextRegister, NextStringLabel, GetLLVMType);
            getsCodeGenerator = new GetsCodeGenerator(declarations, mainBody, variables, NextRegister);
            putsCodeGenerator = new PutsCodeGenerator(declarations, mainBody, variables, NextRegister);

            //string conversion
            atoiCodeGenerator = new AtoiCodeGenerator(declarations, mainBody, registerTypes, NextRegister, Visit);
            atodCodeGenerator = new AtodCodeGenerator(declarations, mainBody, registerTypes, NextRegister, Visit);
            itoaCodeGenerator = new ItoaCodeGenerator(declarations, mainBody, registerTypes, NextRegister, Visit);
            dtoaCodeGenerator = new DtoaCodeGenerator(declarations, mainBody, registerTypes, NextRegister, Visit);

            //arithmetic
            arithmeticCodeGenerator = new ArithmeticCodeGenerator(
                mainBody, registerTypes, NextRegister, Visit);
            //logical
            logicalCodeGenerator = new LogicalCodeGenerator(
                mainBody, registerTypes, NextRegister, Visit);
            //equality
            equalityCodeGenerator = new EqualityCodeGenerator(
                mainBody, registerTypes, NextRegister, Visit);
            //comparison
            comparisonCodeGenerator = new ComparisonCodeGenerator(
                mainBody, registerTypes, NextRegister, Visit);
        }

        public override string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            return variableAssignmentCodeGenerator.VisitAtt(context);
        }

        public override string? VisitSingleAtt([NotNull] ExprParser.SingleAttContext context)
        {
            return variableAssignmentCodeGenerator.VisitSingleAtt(context);
        }

        public override string VisitIntLiteral([NotNull] ExprParser.IntLiteralContext context)
        {
            string value = context.INT().GetText();
            registerTypes[value] = "i32";
            return value;
        }

        public override string VisitDoubleLiteral([NotNull] ExprParser.DoubleLiteralContext context)
        {
            string value = context.DOUBLE().GetText();
            registerTypes[value] = "double";
            return value;
        }

        public override string VisitCharLiteral([NotNull] ExprParser.CharLiteralContext context)
        {
            return charCodeGenerator.VisitCharLiteral(context);
        }

        public override string VisitTrueLiteral([NotNull] ExprParser.TrueLiteralContext context)
        {
            registerTypes["true"] = "i1";
            return "1";
        }

        public override string VisitFalseLiteral([NotNull] ExprParser.FalseLiteralContext context)
        {
            registerTypes["false"] = "i1";
            return "0";
        }

        public override string VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentCodeGenerator.VisitVar(context);
        }

        public override string VisitVarArray([NotNull] ExprParser.VarArrayContext context)
        {
            return variableAssignmentCodeGenerator.VisitVarArray(context);
        }

        public override string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            return variableAssignmentCodeGenerator.VisitDec(context);
        }

        public override string VisitStringLiteral([NotNull] ExprParser.StringLiteralContext context)
        {
            return stringCodeGenerator.VisitStringLiteral(context);
        }

        public override string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            return printfCodeGenerator.VisitPrintfStatement(context);
        }

        public override string? VisitScanfStatement([NotNull] ExprParser.ScanfStatementContext context)
        {
            return scanfCodeGenerator.VisitScanfStatement(context);
        }

        public override string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            return getsCodeGenerator.VisitGetsStatement(context);
        }

        public override string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            return putsCodeGenerator.VisitPutsStatement(context);
        }

        public override string VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            return atoiCodeGenerator.VisitAtoiConversion(context);
        }

        public override string VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            return atodCodeGenerator.VisitAtodConversion(context);
        }

        public override string VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
            return itoaCodeGenerator.VisitItoaConversion(context);
        }

        public override string VisitDtoaConversion([NotNull] ExprParser.DtoaConversionContext context)
        {
            return dtoaCodeGenerator.VisitDtoaConversion(context);
        }

        public override string VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            return arithmeticCodeGenerator.VisitAddSub(context);
        }

        public override string VisitMulDivMod([NotNull] ExprParser.MulDivModContext context)
        {
            return arithmeticCodeGenerator.VisitMulDivMod(context);
        }

        public override string VisitUnaryMinus([NotNull] ExprParser.UnaryMinusContext context)
        {
            string exprValue = Visit(context.expr());
            string exprType = registerTypes[exprValue];
            string resultReg = NextRegister();
            
            if (exprType == "double")
            {
                mainBody.AppendLine($"  {resultReg} = fneg double {exprValue}");
            }
            else
            {
                mainBody.AppendLine($"  {resultReg} = sub i32 0, {exprValue}");
            }
            
            registerTypes[resultReg] = exprType;
            return resultReg;
        }

        public override string VisitLogicalAndOr([NotNull] ExprParser.LogicalAndOrContext context)
        {
            return logicalCodeGenerator.VisitLogicalAndOr(context);
        }

        public override string VisitLogicalNot([NotNull] ExprParser.LogicalNotContext context)
        {
            return logicalCodeGenerator.VisitLogicalNot(context);
        }

        public override string VisitParens([NotNull] ExprParser.ParensContext context)
        {
            // For parentheses, just visit the inner expression
            return Visit(context.expr());
        }

        public override string VisitEquality([NotNull] ExprParser.EqualityContext context)
        {
            return equalityCodeGenerator.VisitEquality(context);
        }

        public override string VisitComparison([NotNull] ExprParser.ComparisonContext context)
        {
            return comparisonCodeGenerator.VisitComparison(context);
        }
    }
}

