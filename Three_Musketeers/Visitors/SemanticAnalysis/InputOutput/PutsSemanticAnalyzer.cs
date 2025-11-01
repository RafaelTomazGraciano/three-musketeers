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
            int line = context.Start.Line;

            // puts(ID) or puts(ID[index])
            if (context.ID() != null)
            {
                string varName = context.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol == null)
                {
                    reportError(line, $"Variable '{varName}' not declared before use in puts()");
                    return null;
                }

                bool hasIndexAccess = context.index() != null;

                if (hasIndexAccess)
                {
                    ValidateArrayElementAccess(symbol, varName, line);
                }
                else
                {
                    ValidateWholeVariableAccess(symbol, varName, line);
                }

                if (!symbol.isInitializated)
                {
                    reportError(line, $"Variable '{varName}' may not have been initialized before use in puts()");
                }

                return null;
            }

            // STRING_LITERAL
            if (context.STRING_LITERAL() != null)
            {
                // string are always valid
            }

            return null;
        }

        private void ValidateArrayElementAccess(Symbol symbol, string varName, int line)
        {
            if (symbol is ArraySymbol arraySymbol)
            {
                if (arraySymbol.elementType != "string" && arraySymbol.elementType != "char")
                {
                    reportError(line,
                        $"puts() can only print string or char array elements, but '{varName}' is '{arraySymbol.elementType}[]'");
                }
            }
            else
            {
                reportError(line,
                    $"Variable '{varName}' is not an array, cannot use index access in puts()");
            }
        }

        private void ValidateWholeVariableAccess(Symbol symbol, string varName, int line)
        {
            if (symbol is ArraySymbol arraySymbol)
            {
                if (arraySymbol.elementType == "char")
                {
                    // char array can be printed as a whole (it's a string)
                    return;
                }
                else if (arraySymbol.elementType == "string")
                {
                    // string array without index - not allowed
                    reportError(line,
                        $"Cannot print entire string array '{varName}', use index access: puts({varName}[index])");
                    return;
                }
                else
                {
                    reportError(line,
                        $"puts() cannot print array of type '{arraySymbol.elementType}'");
                    return;
                }
            }

            if (symbol.type != "string")
            {
                reportError(line,
                    $"puts() can only be used with string variables, but '{varName}' is '{symbol.type}'");
            }
        }
    }
}