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
using Three_Musketeers.Visitors.CodeGeneration.Pointer;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.CodeGeneration.IncrementDecrement;
using Three_Musketeers.Visitors.CodeGeneration.CompoundAssignment;
using Three_Musketeers.Visitors.CodeGeneration.Struct;
using Three_Musketeers.Visitors.CodeGeneration.CompilerDirectives;
using Three_Musketeers.Visitors.CodeGeneration.ControlFlow;

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
        private readonly PointerCodeGenerator pointerCodeGenerator;
        private readonly DynamicMemoryCodeGenerator dynamicMemoryCodeGenerator;
        private readonly VariableResolver variableResolver;
        private readonly IncrementDecrementCodeGenerator incrementDecrementCodeGenerator;
        private readonly CompoundAssignmentCodeGenerator compoundAssignmentCodeGenerator;
        private readonly IncludeCodeGenerator includeCodeGenerator;
        private readonly IfStatementCodeGenerator ifStatementCodeGenerator;
        private readonly SwitchStatementCodeGenerator switchStatementCodeGenerator;
        private readonly LoopStatementCodeGenerator loopStatementCodeGenerator;

        public CodeGenerator()
        {
            variableResolver = new VariableResolver(
            variables,
            GetCurrentFunctionNameIncludingMain
            );
            
            //compiler directives
            includeCodeGenerator = new IncludeCodeGenerator(declarations);
            defineCodeGenerator = new DefineCodeGenerator(globalStrings, defineValues, registerTypes, NextStringLabel);

            //struct
            structCodeGenerator = new StructCodeGenerator(structTypes, structBuilder, GetCurrentBody, registerTypes, NextRegister,
                variableResolver, Visit, GetLLVMType, GetSize, CalculateArrayPosition);

            //functions
            base.functionCodeGenerator = new FunctionCodeGenerator(functionDefinitions, registerTypes, declaredFunctions,
                variables, NextRegister, GetLLVMType, Visit, Visit, forwardDeclarations);
            base.mainFunctionCodeGenerator = new MainFunctionCodeGenerator(mainDefinition, registerTypes, variables,
                NextRegister, GetLLVMType, Visit, Visit);
            base.mainFunctionCodeGenerator = new MainFunctionCodeGenerator(mainDefinition, registerTypes, variables,
                NextRegister, GetLLVMType, Visit, Visit);
            functionCallCodeGenerator = new FunctionCallCodeGenerator(registerTypes, declaredFunctions, NextRegister,
                GetLLVMType, Visit, () => base.functionCodeGenerator!.IsInsideFunction()
                ? base.functionCodeGenerator.GetCurrentFunctionBody()! : mainDefinition);
            

            //variables
            variableAssignmentCodeGenerator = new VariableAssignmentCodeGenerator(
                declarations, variables, registerTypes, NextRegister, GetLLVMType, Visit,
                GetCurrentFunctionNameIncludingMain, GetCurrentBody, GetAlignment, CalculateArrayPosition);
            stringCodeGenerator = new StringCodeGenerator(globalStrings, registerTypes, NextStringLabel);
            charCodeGenerator = new CharCodeGenerator(registerTypes);

            //input-output
            printfCodeGenerator = new PrintfCodeGenerator(globalStrings, GetCurrentBody, registerTypes,
                NextRegister, NextStringLabel, Visit);
            scanfCodeGenerator = new ScanfCodeGenerator(globalStrings, GetCurrentBody, variables,
                NextRegister, NextStringLabel, GetLLVMType, variableResolver, CalculateArrayPosition,
                (structGetCtx) => structCodeGenerator!.VisitStructGet(structGetCtx), registerTypes, GetAlignment);
            getsCodeGenerator = new GetsCodeGenerator(GetCurrentBody, NextRegister, variableResolver,
                (structGetCtx) => structCodeGenerator!.VisitStructGet(structGetCtx), registerTypes);
            putsCodeGenerator = new PutsCodeGenerator(declarations, GetCurrentBody, registerTypes, NextRegister,
                variableResolver, defineCodeGenerator, CalculateArrayPosition, (structGetCtx) => structCodeGenerator!.VisitStructGet(structGetCtx));


            //string conversion
            atoiCodeGenerator = new AtoiCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit);
            atodCodeGenerator = new AtodCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit);
            itoaCodeGenerator = new ItoaCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit);
            dtoaCodeGenerator = new DtoaCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit);

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
            //pointers & dynamic memory
            pointerCodeGenerator = new PointerCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit, variableResolver, CalculateArrayPosition);
            dynamicMemoryCodeGenerator = new DynamicMemoryCodeGenerator(GetCurrentBody, registerTypes, NextRegister, Visit, 
                GetAlignment, GetLLVMType, GetCurrentFunctionNameIncludingMain, variableResolver, variables);
            //increment/decrement
            incrementDecrementCodeGenerator = new IncrementDecrementCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, variableResolver, Visit, CalculateArrayPosition);
            //compound assignment
            compoundAssignmentCodeGenerator = new CompoundAssignmentCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, variableResolver, Visit, CalculateArrayPosition);
            //control flow
            ifStatementCodeGenerator = new IfStatementCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit, Visit);
            switchStatementCodeGenerator = new SwitchStatementCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit, Visit);
            loopStatementCodeGenerator = new LoopStatementCodeGenerator(
                GetCurrentBody, registerTypes, NextRegister, Visit, Visit, Visit);
        }

        public override string? VisitGenericAtt([NotNull] ExprParser.GenericAttContext context)
        {
            return variableAssignmentCodeGenerator.VisitGenericAtt(context);
        }

        public override string? VisitSingleArrayAtt([NotNull] ExprParser.SingleArrayAttContext context)
        {
            return variableAssignmentCodeGenerator.VisitSingleArrayAtt(context);
        }

        public override string? VisitDerrefAtt([NotNull] ExprParser.DerrefAttContext context)
        {
            return pointerCodeGenerator.VisitDerrefAtt(context);
        }
        public override string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttPlusEquals(context);
        }

        public override string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            return compoundAssignmentCodeGenerator.VisitSingleAttMinusEquals(context);
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
            registerTypes["true"] = "i32";
            return "1";
        }

        public override string VisitFalseLiteral([NotNull] ExprParser.FalseLiteralContext context)
        {
            registerTypes["false"] = "i32";
            return "0";
        }

        public override string VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();
    
            // Check if it's a #define first
            string? defineResult = defineCodeGenerator!.ResolveDefine(varName);
            if (defineResult != null)
            {
                return defineResult;
            }

            return variableAssignmentCodeGenerator.VisitVar(context);
        }

        public override string VisitVarArray([NotNull] ExprParser.VarArrayContext context)
        {
            return variableAssignmentCodeGenerator.VisitVarArray(context);
        }

        public override string? VisitDerref([NotNull] ExprParser.DerrefContext context)
        {
            return pointerCodeGenerator.VisitDerref(context);
        }

        public override string? VisitExprAddress([NotNull] ExprParser.ExprAddressContext context)
        {
            return pointerCodeGenerator.VisitExprAddress(context);
        }

        public override string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            if (context.Parent.Parent is ExprParser.HeteregeneousDeclarationContext) return null;
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

            if (context.ifStatement() != null)
            {
                return ifStatementCodeGenerator.VisitIfStatement(context.ifStatement());
            }

            if (context.switchStatement() != null)
            {
                return switchStatementCodeGenerator.VisitSwitchStatement(context.switchStatement());
            }

            if (context.forStatement() != null)
            {
                return loopStatementCodeGenerator.VisitForStatement(context.forStatement());
            }

            if (context.whileStatement() != null)
            {
                return loopStatementCodeGenerator.VisitWhileStatement(context.whileStatement());
            }

            if (context.doWhileStatement() != null)
            {
                return loopStatementCodeGenerator.VisitDoWhileStatement(context.doWhileStatement());
            }

            if (context.BREAK() != null)
            {
                // Handle break statement - check loops first, then switches
                if (loopStatementCodeGenerator.IsInLoop())
                {
                    string? mergeLabel = loopStatementCodeGenerator.GetCurrentLoopMergeLabel();
                    if (mergeLabel != null)
                    {
                        GetCurrentBody().AppendLine($"  br label %{mergeLabel}");
                    }
                }
                else if (switchStatementCodeGenerator.IsInSwitch())
                {
                    string? mergeLabel = switchStatementCodeGenerator.GetCurrentSwitchMergeLabel();
                    if (mergeLabel != null)
                    {
                        GetCurrentBody().AppendLine($"  br label %{mergeLabel}");
                    }
                }
                else
                {
                    // Break outside switch/loop - this will be caught by semantic analyzer
                    // For now, just continue (semantic analyzer will report error)
                }
                return null;
            }

            if (context.CONTINUE() != null)
            {
                // Handle continue statement - must be in a loop
                if (loopStatementCodeGenerator.IsInLoop())
                {
                    string? continueLabel = loopStatementCodeGenerator.GetCurrentLoopContinueLabel();
                    if (continueLabel != null)
                    {
                        GetCurrentBody().AppendLine($"  br label %{continueLabel}");
                    }
                }
                return null;
            }
            
            return base.VisitStm(context);
        }

        public override string? VisitFunctionCall([NotNull] ExprParser.FunctionCallContext context)
        {
            return functionCallCodeGenerator.VisitFunctionCall(context);
        }

        public override string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            return dynamicMemoryCodeGenerator.VisitMallocAtt(context);
        }

        public override string? VisitFreeStatement([NotNull] ExprParser.FreeStatementContext context)
        {
            return dynamicMemoryCodeGenerator.VisitFreeStatment(context);
        }

        public override string? VisitStructStatement([NotNull] ExprParser.StructStatementContext context)
        {
            return structCodeGenerator!.VisitStructStatement(context);
        }

        public override string? VisitStructAtt([NotNull] ExprParser.StructAttContext context)
        {
            return structCodeGenerator!.VisitStructAtt(context);
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

        public override string VisitStructGet([NotNull] ExprParser.StructGetContext context)
        {
            return structCodeGenerator!.VisitStructGet(context);
        }

        public override string VisitVarStruct([NotNull] ExprParser.VarStructContext context)
        {
            return structCodeGenerator!.VisitVarStruct(context.structGet());
        }

        private string? GetCurrentFunctionNameIncludingMain()
        {
            if (functionCodeGenerator?.IsInsideFunction() == true)
                return functionCodeGenerator.GetCurrentFunctionName();
            if (mainFunctionCodeGenerator?.IsInsideMain() == true)
                return "main";
            return null;
        }

        public override string? VisitIncludeSystem([NotNull] ExprParser.IncludeSystemContext context)
        {
            return includeCodeGenerator.VisitIncludeSystem(context);
        }

        public override string? VisitIncludeUser([NotNull] ExprParser.IncludeUserContext context)
        {
            return includeCodeGenerator.VisitIncludeUser(context);
        }
    }
}