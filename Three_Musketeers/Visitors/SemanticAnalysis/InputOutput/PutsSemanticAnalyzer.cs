using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct;

namespace Three_Musketeers.Visitors.SemanticAnalysis.InputOutput
{
    public class PutsSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly LibraryDependencyTracker libraryTracker;
        private readonly StructSemanticAnalyzer structSemanticAnalyzer;

        public PutsSemanticAnalyzer(
            Action<int, string> reportError,
            SymbolTable symbolTable,
            LibraryDependencyTracker libraryTracker,
            StructSemanticAnalyzer structSemanticAnalyzer)
        {
            this.reportError = reportError;
            this.symbolTable = symbolTable;
            this.libraryTracker = libraryTracker;
            this.structSemanticAnalyzer = structSemanticAnalyzer;
        }

        public string? VisitPutsStatement([NotNull] ExprParser.PutsStatementContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("puts", context.Start.Line))
            {
                return null;
            }
    
            int line = context.Start.Line;

            // puts(STRING_LITERAL)
            if (context.STRING_LITERAL() != null)
            {
                // String literals are always valid
                return null;
            }

            // puts(structGet)
            if (context.structGet() != null)
            {
                ValidateStructGetAccess(context.structGet(), line);
                return null;
            }

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

            return null;
        }

        private void ValidateStructGetAccess(ExprParser.StructGetContext structGetCtx, int line)
        {
            string varName = structGetCtx.ID().GetText();
            
            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' not declared before use in puts()");
                return;
            }

            if (!symbol.isInitializated)
            {
                reportError(line, $"Variable '{varName}' may not have been initialized before use in puts()");
            }

            // Use the struct analyzer to validate and get the final member type
            string? finalType = structSemanticAnalyzer.VisitStructGet(structGetCtx);
            
            if (finalType == null)
            {
                // Error already reported by StructSemanticAnalyzer
                return;
            }

            // Clean up type name if it has "struct_" or "union_" prefix
            if (finalType.StartsWith("struct_"))
            {
                finalType = finalType.Substring(7);
            }
            else if (finalType.StartsWith("union_"))
            {
                finalType = finalType.Substring(6);
            }

            // Validate that the final type is string or char
            if (finalType != "string")
            {
                reportError(line, 
                    $"puts() can only print string members from structs/unions, but member is '{finalType}'. Use individual character access for char arrays.");
            }
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