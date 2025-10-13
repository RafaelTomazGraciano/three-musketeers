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
using Three_Musketeers.Visitors.CodeGeneration.Functions;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.CodeGeneration.IncrementDecrement;
using Three_Musketeers.Visitors.CodeGeneration.CompoundAssignment;

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
        private readonly FunctionCallCodeGenerator functionCallCodeGenerator;
        private readonly VariableResolver variableResolver;
        private readonly IncrementDecrementCodeGenerator incrementDecrementCodeGenerator;
        private readonly CompoundAssignmentCodeGenerator compoundAssignmentCodeGenerator;

        public CodeGenerator()
        {
            variableResolver = new VariableResolver(
            variables,
            () => functionCodeGenerator?.GetCurrentFunctionName()
            );

            //functions
            base.functionCodeGenerator = new FunctionCodeGenerator(functionDefinitions, registerTypes, declaredFunctions,
                variables, NextRegister, GetLLVMType, Visit, Visit, forwardDeclarations);
            base.mainFunctionCodeGenerator = new MainFunctionCodeGenerator(mainDefinition, registerTypes, variables, 
                NextRegister, GetLLVMType, Visit, Visit);
            functionCallCodeGenerator = new FunctionCallCodeGenerator(registerTypes, declaredFunctions, NextRegister,
                GetLLVMType, Visit, () => base.functionCodeGenerator!.IsInsideFunction()
                ? base.functionCodeGenerator.GetCurrentFunctionBody()! : mainDefinition);

            //variables
            variableAssignmentCodeGenerator = new VariableAssignmentCodeGenerator(
                mainDefinition, declarations, variables, registerTypes, NextRegister, GetLLVMType, Visit,
                () => functionCodeGenerator?.GetCurrentFunctionName(), GetCurrentBody);
            stringCodeGenerator = new StringCodeGenerator(globalStrings, registerTypes, NextStringLabel);
            charCodeGenerator = new CharCodeGenerator(registerTypes);

            //input-output
            printfCodeGenerator = new PrintfCodeGenerator(globalStrings, declarations, GetCurrentBody, registerTypes,
                NextRegister, NextStringLabel, Visit);
            scanfCodeGenerator = new ScanfCodeGenerator(globalStrings, declarations, GetCurrentBody, variables,
                registerTypes, NextRegister, NextStringLabel, GetLLVMType, variableResolver);
            getsCodeGenerator = new GetsCodeGenerator(declarations, GetCurrentBody, NextRegister, variableResolver);
            putsCodeGenerator = new PutsCodeGenerator(declarations, mainDefinition, registerTypes, NextRegister,
                () => functionCodeGenerator.IsInsideFunction() ? functionCodeGenerator.GetCurrentFunctionBody() : null,
                variableResolver);

            //string conversion
            atoiCodeGenerator = new AtoiCodeGenerator(declarations, GetCurrentBody, registerTypes, NextRegister, Visit);
            atodCodeGenerator = new AtodCodeGenerator(declarations, GetCurrentBody, registerTypes, NextRegister, Visit);
            itoaCodeGenerator = new ItoaCodeGenerator(declarations, GetCurrentBody, registerTypes, NextRegister, Visit);
            dtoaCodeGenerator = new DtoaCodeGenerator(declarations, GetCurrentBody, registerTypes, NextRegister, Visit);

            //arithmetic
            arithmeticCodeGenerator = new ArithmeticCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit);
            //logical
            logicalCodeGenerator = new LogicalCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit);
            //equality
            equalityCodeGenerator = new EqualityCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit);
            //comparison
            comparisonCodeGenerator = new ComparisonCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit);
            //increment/decrement
            incrementDecrementCodeGenerator = new IncrementDecrementCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, variables);
            //compound assignment
            compoundAssignmentCodeGenerator = new CompoundAssignmentCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, variables, Visit);
        }

        public override string? VisitAttRegular([NotNull] ExprParser.AttRegularContext context)
        {
            return variableAssignmentCodeGenerator.VisitAttRegular(context);
        }

        public override string? VisitAttPlusEquals([NotNull] ExprParser.AttPlusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitAttPlusEquals(context);
        }

        public override string? VisitAttMinusEquals([NotNull] ExprParser.AttMinusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitAttMinusEquals(context);
        }

        public override string? VisitSingleAtt([NotNull] ExprParser.SingleAttContext context)
        {
            return variableAssignmentCodeGenerator.VisitSingleAtt(context);
        }

        public override string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttPlusEquals(context);
        }

        public override string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttMinusEquals(context);
        }

        public override string? VisitAttMultiplyEquals([NotNull] ExprParser.AttMultiplyEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitAttMultiplyEquals(context);
        }

        public override string? VisitAttDivideEquals([NotNull] ExprParser.AttDivideEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitAttDivideEquals(context);
        }

        public override string? VisitSingleAttMultiplyEquals([NotNull] ExprParser.SingleAttMultiplyEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttMultiplyEquals(context);
        }

        public override string? VisitSingleAttDivideEquals([NotNull] ExprParser.SingleAttDivideEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttDivideEquals(context);
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

        public override string? VisitUnaryMinus([NotNull] ExprParser.UnaryMinusContext context)
        {
            string? exprValue = Visit(context.expr());
            if (exprValue == null)
            {
                return null;
            }
            string exprType;
            exprType = registerTypes[exprValue];
            string resultReg = NextRegister();

            if (exprType == "double")
            {
                GetCurrentBody().AppendLine($"  {resultReg} = fneg double {exprValue}");
            }
            else
            {
                GetCurrentBody().AppendLine($"  {resultReg} = sub i32 0, {exprValue}");
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

        public override string? VisitParens([NotNull] ExprParser.ParensContext context)
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

        public override string? VisitMainFunction([NotNull] ExprParser.MainFunctionContext context)
        {
            mainFunctionCodeGenerator!.GenerateMainFunction(context);
            return null;
        }

        public override string? VisitFunction([NotNull] ExprParser.FunctionContext context)
        {
            return base.functionCodeGenerator!.VisitFunction(context);
        }

        public override string? VisitStm([NotNull] ExprParser.StmContext context)
        {
            if (context.RETURN() != null && base.functionCodeGenerator!.IsInsideFunction())
            {
                base.functionCodeGenerator.VisitReturnStatement(context);
                return null;
            }
            
            return base.VisitStm(context);
        }
        
        public override string? VisitFunctionCall([NotNull] ExprParser.FunctionCallContext context)
        {
            return functionCallCodeGenerator.VisitFunctionCall(context);
        }


        // Increment/Decrement operators for simple variables
        public override string VisitPrefixIncrement([NotNull] ExprParser.PrefixIncrementContext context)
        {
            return incrementDecrementCodeGenerator.VisitPrefixIncrement(context);
        }

        public override string VisitPrefixDecrement([NotNull] ExprParser.PrefixDecrementContext context)
        {
            return incrementDecrementCodeGenerator.VisitPrefixDecrement(context);
        }

        public override string VisitPostfixIncrement([NotNull] ExprParser.PostfixIncrementContext context)
        {
            return incrementDecrementCodeGenerator.VisitPostfixIncrement(context);
        }

        public override string VisitPostfixDecrement([NotNull] ExprParser.PostfixDecrementContext context)
        {
            return incrementDecrementCodeGenerator.VisitPostfixDecrement(context);
        }

        // Increment/Decrement operators for array elements
        public override string VisitPrefixIncrementArray([NotNull] ExprParser.PrefixIncrementArrayContext context)
        {
            return incrementDecrementCodeGenerator.VisitPrefixIncrementArray(context);
        }

        public override string VisitPrefixDecrementArray([NotNull] ExprParser.PrefixDecrementArrayContext context)
        {
            return incrementDecrementCodeGenerator.VisitPrefixDecrementArray(context);
        }

        public override string VisitPostfixIncrementArray([NotNull] ExprParser.PostfixIncrementArrayContext context)
        {
            return incrementDecrementCodeGenerator.VisitPostfixIncrementArray(context);
        }

        public override string VisitPostfixDecrementArray([NotNull] ExprParser.PostfixDecrementArrayContext context)
        {
            return incrementDecrementCodeGenerator.VisitPostfixDecrementArray(context);
        }
    }
}

