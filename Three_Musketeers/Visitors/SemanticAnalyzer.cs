using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.SemanticAnalysis;
using Three_Musketeers.Visitors.SemanticAnalysis.Variables;
using Three_Musketeers.Visitors.SemanticAnalysis.InputOutput;
using Three_Musketeers.Visitors.SemanticAnalysis.StringConversion;
using Three_Musketeers.Visitors.SemanticAnalysis.Arithmetic;
using Three_Musketeers.Visitors.SemanticAnalysis.Logical;
using Three_Musketeers.Visitors.SemanticAnalysis.Equality;
using Three_Musketeers.Visitors.SemanticAnalysis.Comparison;
using Three_Musketeers.Visitors.SemanticAnalysis.Functions;
using Three_Musketeers.Visitors.SemanticAnalysis.Pointer;
using Three_Musketeers.Visitors.SemanticAnalysis.IncrementDecrement;
using Three_Musketeers.Visitors.SemanticAnalysis.CompoundAssignment;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct;
using Three_Musketeers.Visitors.SemanticAnalysis.CompilerDirectives;
using Three_Musketeers.Visitors.SemanticAnalysis.ControlFlow;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors
{
    public enum ControlFlowContext
    {
        Loop,
        Switch
    }

    public class SemanticAnalyzer : SemanticAnalyzerBase
    {
        // Track active control flow contexts for break/continue validation
        private readonly Stack<ControlFlowContext> activeControlFlowContexts = new Stack<ControlFlowContext>();
        private readonly VariableAssignmentSemanticAnalyzer variableAssignmentSemanticAnalyzer;
        private readonly PointerSemanticAnalyzer pointerSemanticAnalyzer;
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
        private readonly MainFunctionSemanticAnalyzer mainFunctionSemanticAnalyzer;
        private readonly FunctionSemanticAnalyzer functionSemanticAnalyzer;
        private readonly FunctionCallSemanticAnalyzer functionCallSemanticAnalyzer;
        private readonly DynamicMemorySemanticAnalyzer dynamicMemorySemanticAnalyzer;
        private readonly IncrementDecrementSemanticAnalyzer incrementDecrementSemanticAnalyzer;
        private readonly CompoundAssignmentSemanticAnalyzer compoundAssignmentSemanticAnalyzer;
        private readonly IfStatementSemanticAnalyzer ifStatementSemanticAnalyzer;
        private readonly SwitchStatementSemanticAnalyzer switchStatementSemanticAnalyzer;
        private readonly LoopStatementSemanticAnalyzer loopStatementSemanticAnalyzer;

        private readonly DefineSemanticAnalyzer defineSemanticAnalyzer;
        private readonly IncludeSemanticAnalyzer includeSemanticAnalyzer;

        public SemanticAnalyzer(string currentFilePath = "")
        {
            //compiler directives
            includeSemanticAnalyzer = new IncludeSemanticAnalyzer(ReportError, ReportWarning, currentFilePath, libraryTracker);
            defineSemanticAnalyzer = new DefineSemanticAnalyzer(symbolTable, ReportError, ReportWarning);

            //structs and unions
            structSemanticAnalyzer = new StructSemanticAnalyzer(symbolTable, structures, ReportError);
            
            //variables
            variableAssignmentSemanticAnalyzer = new VariableAssignmentSemanticAnalyzer(symbolTable, ReportError, ReportWarning, structures, Visit);
            pointerSemanticAnalyzer = new PointerSemanticAnalyzer(ReportError, ReportWarning, Visit, symbolTable);

            
            // input-output
            printfSemanticAnalyzer = new PrintfSemanticAnalyzer(ReportError, ReportWarning, GetExpressionType, Visit, libraryTracker);
            scanfSemanticAnalyzer = new ScanfSemanticAnalyzer(ReportError, symbolTable, libraryTracker, structSemanticAnalyzer);
            getsSemanticAnalyzer = new GetsSemanticAnalyzer(ReportError, symbolTable, libraryTracker);
            putsSemanticAnalyzer = new PutsSemanticAnalyzer(ReportError, symbolTable, libraryTracker);
            
            // string conversion
            atoiSemanticAnalyzer = new AtoiSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit, libraryTracker);
            atodSemanticAnalyzer = new AtodSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit, libraryTracker);
            itoaSemanticAnalyzer = new ItoaSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit, libraryTracker);
            dtoaSemanticAnalyzer = new DtoaSemanticAnalyzer(ReportError, symbolTable, GetExpressionType, Visit, libraryTracker);
            // arithmetic
            arithmeticSemanticAnalyzer = new ArithmeticSemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // logical
            logicalSemanticAnalyzer = new LogicalSemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // equality
            equalitySemanticAnalyzer = new EqualitySemanticAnalyzer(ReportError, GetExpressionType, Visit);
            // comparison
            comparisonSemanticAnalyzer = new ComparisonSemanticAnalyzer(ReportError, GetExpressionType, Visit);
            //functions
            mainFunctionSemanticAnalyzer = new MainFunctionSemanticAnalyzer(symbolTable, ReportError, Visit);
            functionSemanticAnalyzer = new FunctionSemanticAnalyzer(symbolTable, declaredFunctions, ReportError, ReportWarning,
                GetExpressionType, Visit);
            functionCallSemanticAnalyzer = new FunctionCallSemanticAnalyzer(ReportError, declaredFunctions, GetExpressionType,
                Visit, symbolTable);
            //Dynamic memory
            dynamicMemorySemanticAnalyzer = new DynamicMemorySemanticAnalyzer(symbolTable, ReportError, ReportWarning, Visit, libraryTracker);
            // increment/decrement
            incrementDecrementSemanticAnalyzer = new IncrementDecrementSemanticAnalyzer(ReportError, ReportWarning, symbolTable);
            // compound assignment
            compoundAssignmentSemanticAnalyzer = new CompoundAssignmentSemanticAnalyzer(ReportError, ReportWarning, symbolTable, GetExpressionType);
            compoundAssignmentSemanticAnalyzer = new CompoundAssignmentSemanticAnalyzer(ReportError, ReportWarning, symbolTable,
                GetExpressionType);
            // control flow
            ifStatementSemanticAnalyzer = new IfStatementSemanticAnalyzer(symbolTable, ReportError, GetExpressionType, Visit, Visit);
            switchStatementSemanticAnalyzer = new SwitchStatementSemanticAnalyzer(
                symbolTable, ReportError, GetExpressionType, Visit, Visit,
                () => activeControlFlowContexts.Push(ControlFlowContext.Switch),
                () => activeControlFlowContexts.Pop());
            loopStatementSemanticAnalyzer = new LoopStatementSemanticAnalyzer(
                symbolTable, ReportError, GetExpressionType, Visit, Visit, Visit,
                () => activeControlFlowContexts.Push(ControlFlowContext.Loop),
                () => activeControlFlowContexts.Pop());
        }

        public override string? VisitStart([NotNull] ExprParser.StartContext context)
        {
            return base.VisitStart(context);
        }

        public override string? VisitProg([NotNull] ExprParser.ProgContext context)
        {
            return base.VisitProg(context);
        }

        public override string? VisitGenericAtt([NotNull] ExprParser.GenericAttContext context)
        {
            string? type = variableAssignmentSemanticAnalyzer.VisitAtt(context);
            string? exprType = Visit(context.expr());
            if (type == null || exprType == null) return null;

            if (!CastTypes.TwoTypesArePermitedToCast(type, exprType))
            {
                ReportError(context.Start.Line,
                    $"Cannot assign value of type '{exprType}' to variable of type '{type}'");
                return null;
            }

            return type;
        }

        public override string? VisitSingleArrayAtt([NotNull] ExprParser.SingleArrayAttContext context)
        {
            var result = variableAssignmentSemanticAnalyzer.VisitSingleArrayAtt(context);
            var exprType = Visit(context.expr());
            if (result == null || exprType == null) return null;
            if(!TwoTypesArePermitedToCast(result, exprType))
            {
                ReportError(context.Start.Line,
                    $"Cannot assign value of type '{exprType}' to array element of type '{result}'");
                return null;
            }
            return null;
        }

        public override string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            if (context.Parent.Parent is ExprParser.HeteregeneousDeclarationContext) return null;
            
            return variableAssignmentSemanticAnalyzer.VisitDeclaration(context);
        }

        public override string? VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentSemanticAnalyzer.VisitVar(context);
        }

        public override string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            string? type = variableAssignmentSemanticAnalyzer.VisitMallocAtt(context);
            // Then validate the malloc call
            dynamicMemorySemanticAnalyzer.VisitMallocAtt(context);
            return type;
        }
        
        public override string? VisitFreeStatement([NotNull] ExprParser.FreeStatementContext context)
        {
            return dynamicMemorySemanticAnalyzer.VisitFreeStatment(context);            
        }

        public override string? VisitExprAddress([NotNull] ExprParser.ExprAddressContext context)
        {
            return pointerSemanticAnalyzer.VisitExprAddress(context);
        }

        public override string VisitDerrefAtt([NotNull] ExprParser.DerrefAttContext context)
        {
            string? variableType = pointerSemanticAnalyzer.VisitDerref(context);
            string? expr = Visit(context.expr());
            return "int";
        }
        public override string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttPlusEquals(context);
        }

        public override string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            return compoundAssignmentSemanticAnalyzer.VisitSingleAttMinusEquals(context);
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

        public override string? VisitAtoiConversion([NotNull] ExprParser.AtoiConversionContext context)
        {
            return atoiSemanticAnalyzer.VisitAtoiConversion(context);
        }

        public override string? VisitAtodConversion([NotNull] ExprParser.AtodConversionContext context)
        {
            return atodSemanticAnalyzer.VisitAtodConversion(context);
        }

        public override string? VisitItoaConversion([NotNull] ExprParser.ItoaConversionContext context)
        {
            return itoaSemanticAnalyzer.VisitItoaConversion(context);
        }

        public override string? VisitDtoaConversion([NotNull] ExprParser.DtoaConversionContext context)
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

        public override string? VisitMainFunction([NotNull] ExprParser.MainFunctionContext context)
        {
            symbolTable.EnterScope();
            mainFunctionSemanticAnalyzer.AnalyzeMainFunction(context);
            symbolTable.ExitScope();
            return null;
        }

        public override string? VisitFunction([NotNull] ExprParser.FunctionContext context)
        {
            symbolTable.EnterScope();
            functionSemanticAnalyzer.AnalyzeFunction(context);
            symbolTable.ExitScope();
            return null;
        }

        public override string? VisitStm([NotNull] ExprParser.StmContext context)
        {
            if (context.RETURN() != null)
            {
                functionSemanticAnalyzer.AnalyzeReturnStatement(context);
                return null;
            }

            if (context.ifStatement() != null)
            {
                return ifStatementSemanticAnalyzer.VisitIfStatement(context.ifStatement());
            }

            if (context.switchStatement() != null)
            {
                return switchStatementSemanticAnalyzer.VisitSwitchStatement(context.switchStatement());
            }

            if (context.forStatement() != null)
            {
                return loopStatementSemanticAnalyzer.VisitForStatement(context.forStatement());
            }

            if (context.whileStatement() != null)
            {
                return loopStatementSemanticAnalyzer.VisitWhileStatement(context.whileStatement());
            }

            if (context.doWhileStatement() != null)
            {
                return loopStatementSemanticAnalyzer.VisitDoWhileStatement(context.doWhileStatement());
            }

            if (context.BREAK() != null)
            {
                // Break statement semantic check - must be inside a switch or loop
                if (activeControlFlowContexts.Count == 0)
                {
                    ReportError(context.Start.Line, 
                        "'break' statement must be inside a loop or switch statement");
                }
                return null;
            }

            if (context.CONTINUE() != null)
            {
                // Continue statement semantic check - must be inside a loop
                if (activeControlFlowContexts.Count == 0 || 
                    activeControlFlowContexts.Peek() != ControlFlowContext.Loop)
                {
                    ReportError(context.Start.Line, 
                        "'continue' statement must be inside a loop");
                }
                return null;
            }

            return base.VisitStm(context);
        }

        public override string? VisitFunctionCall([NotNull] ExprParser.FunctionCallContext context)
        {
            return functionCallSemanticAnalyzer.VisitFunctionCall(context);
        }

        public override string? VisitStructGet([NotNull] ExprParser.StructGetContext context)
        {
            return structSemanticAnalyzer.VisitStructGet(context);
        }

        public override string? VisitVarStruct([NotNull] ExprParser.VarStructContext context)
        {
            return structSemanticAnalyzer.VisitStructGet(context.structGet());
        }

        public override string? VisitStructAtt([NotNull] ExprParser.StructAttContext context)
        {
            int line = context.Start.Line;
            string? fieldType = structSemanticAnalyzer.VisitStructGet(context.structGet());
            if (fieldType == null)
            {
                return null;
            }

            string? exprType = Visit(context.expr());

            if (exprType == null)
            {
                return null;
            }

            // Check type compatibility
            if (!TwoTypesArePermitedToCast(fieldType, exprType))
            {
                ReportError(line, $"Cannot assign value of type '{exprType}' to struct field of type '{fieldType}'");
                return null;
            }

            return fieldType;
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

        public override string? VisitIncludeSystem([NotNull] ExprParser.IncludeSystemContext context)
        {
            return includeSemanticAnalyzer.VisitIncludeSystem(context);
        }

        public override string? VisitIncludeUser([NotNull] ExprParser.IncludeUserContext context)
        {
            return includeSemanticAnalyzer.VisitIncludeUser(context);
        }

        public override string? VisitDefineInt([NotNull] ExprParser.DefineIntContext context)
        {
            return defineSemanticAnalyzer.VisitDefineInt(context);
        }

        public override string? VisitDefineDouble([NotNull] ExprParser.DefineDoubleContext context)
        {
            return defineSemanticAnalyzer.VisitDefineDouble(context);
        }

        public override string? VisitDefineString([NotNull] ExprParser.DefineStringContext context)
        {
            return defineSemanticAnalyzer.VisitDefineString(context);
        }
    }
}

