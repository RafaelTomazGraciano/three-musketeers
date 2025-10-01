using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors.CodeGeneration;

namespace Three_Musketeers.Visitors
{
    public class CodeGenerator : CodeGeneratorBase
    {
        private readonly VariableAssignmentCodeGenerator variableAssignmentCodeGenerator;
        private readonly PrintfCodeGenerator printfCodeGenerator;

        public CodeGenerator()
        {
            variableAssignmentCodeGenerator = new VariableAssignmentCodeGenerator(
                mainBody, variables, registerTypes, NextRegister, GetLLVMType, Visit);
            printfCodeGenerator = new PrintfCodeGenerator(
                globalStrings, mainBody, registerTypes, NextRegister, NextStringLabel, Visit);
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

        public override string? VisitPrintfStatement([NotNull] ExprParser.PrintfStatementContext context)
        {
            return printfCodeGenerator.VisitPrintfStatement(context);
        }
    }
}

