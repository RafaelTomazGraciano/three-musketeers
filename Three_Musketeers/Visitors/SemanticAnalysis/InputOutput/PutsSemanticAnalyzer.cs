using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class PutsSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;

        public PutsSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
        }

        public string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            if (context.ID() != null)
            {
                string varName = context.ID().GetText();

                Symbol? symbol = symbolTable.GetSymbol(varName);
                if (symbol == null)
                {
                    reportError(context.Start.Line,
                        $"Variable '{varName}' not declarated before use in puts()");
                    return null;
                }

                if (symbol.type != "string")
                {
                    reportError(context.Start.Line,
                        $"puts() can only be used with string variables, but '{varName}' is '{symbol.type}'");
                    return null;
                }

                if (!symbol.isInitializated)
                {
                    reportError(context.Start.Line,
                    $"Variable '{varName}' may not have been  initializated before use in puts()");
                }
                return null;
            }

            if (context.STRING_LITERAL() != null)
            {
                //for string is always valid
            }
            return null;
        }

    }
}