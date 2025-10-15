using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.CompilerDirectives
{
    public class DefineSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;

        public DefineSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
        }

        public string? VisitDefineInt([NotNull] ExprParser.DefineIntContext context)
        {
            string defineName = context.ID().GetText();
            string value = context.INT().GetText();
            int line = context.Start.Line;

            if (symbolTable.Contains(defineName))
            {
                reportWarning(line, $"#define '{defineName}' shadows an existing variable");
            }

            // Cria símbolo como constante e adiciona à SymbolTable
            var symbol = new Symbol(defineName, "int", line, value);
            symbolTable.AddSymbol(symbol);
            
            return defineName;
        }

        public string? VisitDefineDouble([NotNull] ExprParser.DefineDoubleContext context)
        {
            string defineName = context.ID().GetText();
            string value = context.DOUBLE().GetText();
            int line = context.Start.Line;

            if (symbolTable.Contains(defineName))
            {
                reportWarning(line, $"#define '{defineName}' shadows an existing variable");
            }

            // Cria símbolo como constante e adiciona à SymbolTable
            var symbol = new Symbol(defineName, "double", line, value);
            symbolTable.AddSymbol(symbol);
            
            return defineName;
        }

        public string? VisitDefineString([NotNull] ExprParser.DefineStringContext context)
        {
            string defineName = context.ID().GetText();
            string value = context.STRING_LITERAL().GetText();
            int line = context.Start.Line;

            if (symbolTable.Contains(defineName))
            {
                reportWarning(line, $"#define '{defineName}' shadows an existing variable");
            }

            // Cria símbolo como constante e adiciona à SymbolTable
            var symbol = new Symbol(defineName, "string", line, value);
            symbolTable.AddSymbol(symbol);
            
            return defineName;
        }
    }
}