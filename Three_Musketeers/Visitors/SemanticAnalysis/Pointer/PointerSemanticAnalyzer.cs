using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Pointer
{
    public class PointerSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly SymbolTable symbolTable;

        public PointerSemanticAnalyzer(Action<int, string> reportError, Action<int, string> reportWarning, SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.symbolTable = symbolTable;
        }
    }
}