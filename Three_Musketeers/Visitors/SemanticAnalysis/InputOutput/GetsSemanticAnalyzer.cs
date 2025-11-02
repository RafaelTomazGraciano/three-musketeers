using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class GetsSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly LibraryDependencyTracker libraryTracker;
        
        public GetsSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable,
            LibraryDependencyTracker libraryTracker)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
            this.libraryTracker = libraryTracker;
        }

        public string? VisitGetsStatement([NotNull] ExprParser.GetsStatementContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("gets", context.Start.Line))
            {
                return null;
            }

            string varName = context.ID().GetText();

            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(context.Start.Line,
                    $"Variable '{varName}' not declared before use in gets()");
                return null;
            }

            if (symbol.isConstant)
            {
                reportError(context.Start.Line,
                    $"Cannot use #define constant '{varName}' in gets(). Constants are read-only");
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