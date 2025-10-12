using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Pointer
{
    public class DynamicMemorySemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;

        private readonly Action<int, string> reportWarning;

        public DynamicMemorySemanticAnalyzer(SymbolTable symbolTable, Action<int, string> reportError, Action<int, string> reportWarning)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
        }

        public string? VisitMallocAtt(ExprParser.MallocAttContext context)
        {
            var typeToken = context.type();
            string varName = context.ID().GetText();
            int amountOfPointer = context.POINTER().Length;
            int line = context.Start.Line;

            if (typeToken == null)
            {
                var entry = symbolTable.GetSymbol(varName);
                if (entry == null)
                {
                    reportError(line, $"Variable '{varName}' was no declared");
                    return null;
                }
                var symbol = (PointerSymbol)entry;
                if (symbol.isDynamic)
                {
                    reportWarning(line, $"Possible memory leak on line {line}");
                }
                symbol.isInitializated = true;
                symbol.isDynamic = true;
                return "pointer";
            }

            var pointer = new PointerSymbol(varName, typeToken.GetText(), amountOfPointer, line, true);
            pointer.isInitializated = true;
            symbolTable.AddSymbol(pointer);

            return "pointer";
        }
    }
}