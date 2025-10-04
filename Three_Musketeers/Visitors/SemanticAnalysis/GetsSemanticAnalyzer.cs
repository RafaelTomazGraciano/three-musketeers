using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis
{
    public class GetsSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        
        public GetsSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            string varName = context.ID().GetText();

            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(context.Start.Line,
                    $"Variable '{varName}' not declared before use in gets()");
                return null;
            }

            if (symbol.type != "string")
            {
                reportError(context.Start.Line,
                    $"gets() can only be used with string variables, but '{varName}' is '{symbol.type}'");
                return null;
            }

            symbolTable.MarkInitializated(varName);

            return null;
        }
    }
}