using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.CodeGeneration;

namespace Three_Musketeers.Visitors
{
    public class CodeGenerator : CodeGeneratorBase
    {
        private readonly VariableAssignmentCodeGenerator variableAssignmentCodeGenerator;
        private readonly PrintfCodeGenerator printfCodeGenerator;
        private readonly ScanfCodeGenerator scanfCodeGenerator;
        private readonly StringCodeGenerator stringCodeGenerator;
        private readonly GetsCodeGenerator getsCodeGenerator;
        private readonly PutsCodeGenerator putsCodeGenerator;

        public CodeGenerator()
        {
            variableAssignmentCodeGenerator = new VariableAssignmentCodeGenerator(
                mainBody, declarations, variables, registerTypes, NextRegister, GetLLVMType, Visit);

            printfCodeGenerator = new PrintfCodeGenerator(
                globalStrings, mainBody, registerTypes, NextRegister, NextStringLabel, Visit);

            scanfCodeGenerator = new ScanfCodeGenerator(globalStrings, mainBody, variables,
                registerTypes, NextRegister, NextStringLabel, GetLLVMType);

            stringCodeGenerator = new StringCodeGenerator(globalStrings, registerTypes, NextStringLabel);

            getsCodeGenerator = new GetsCodeGenerator(declarations, mainBody, variables, NextRegister);
            putsCodeGenerator = new PutsCodeGenerator(declarations, mainBody, variables, NextRegister);
        }

        public override string? VisitAtt([NotNull] ExprParser.AttContext context)
        {
            return variableAssignmentCodeGenerator.VisitAtt(context);
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

        public override string VisitVar([NotNull] ExprParser.VarContext context)
        {
            return variableAssignmentCodeGenerator.VisitVar(context);
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

    }
}

