using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Pointer
{
    public class DynamicMemorySemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;

        private readonly Action<int, string> reportWarning;

        private readonly Func<ExprParser.ExprContext, string> VisitExpr;
        private readonly LibraryDependencyTracker libraryTracker;

        public DynamicMemorySemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Func<ExprParser.ExprContext,
            string> VisitExpr,
            LibraryDependencyTracker libraryTracker)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.VisitExpr = VisitExpr;
            this.libraryTracker = libraryTracker;
        }

        public string? VisitMallocAtt(ExprParser.MallocAttContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("malloc", context.Start.Line))
            {
                return null;
            }

            var typeToken = context.type();
            string varName = context.ID().GetText();
            int amountOfPointers = context.POINTER().Length;
            int line = context.Start.Line;
            string sizeExprType = VisitExpr(context.expr());

            if (sizeExprType == "string" || sizeExprType == "double")
            {
                reportError(line, $"Expected integer type for malloc size, not '{sizeExprType}'");
                return null;
            }

            // Case 1: Malloc with type declaration (int *pointer = malloc(5))
            if (typeToken != null)
            {
                string varType = typeToken.GetText();

                // Create and register the pointer symbol
                var pointerSymbol = new PointerSymbol(varName, varType, line, amountOfPointers, isDynamic: true);
                pointerSymbol.isInitializated = true;

                if (!symbolTable.AddSymbol(pointerSymbol))
                {
                    reportError(line, $"Variable '{varName}' has already been declared");
                    return null;
                }

                return "pointer";
            }

            // Case 2: Malloc assignment to existing variable
            Symbol? symbol = symbolTable.GetSymbol(varName);

            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            // Check if it's an array element being assigned (e.g., ptrs[i] = malloc(...))
            var indexes = context.index();
            if (indexes.Length > 0)
            {
                if (symbol is ArraySymbol arraySymbol)
                {
                    // Check if array elements are pointers
                    if (arraySymbol.pointerLevel > 0)
                    {
                        // Valid: assigning to an element of array of pointers
                        return "pointer";
                    }
                    else
                    {
                        reportError(line, $"Cannot assign malloc to array element of non-pointer type");
                        return null;
                    }
                }
                else
                {
                    reportError(line, $"Variable '{varName}' is not an array");
                    return null;
                }
            }

            // Regular pointer assignment
            if (symbol is not PointerSymbol)
            {
                reportError(line, $"Variable '{varName}' is not a pointer type, cannot assign malloc result");
                return null;
            }

            PointerSymbol pointerVar = (PointerSymbol)symbol;

            // Warn about potential memory leak if reallocating
            if (pointerVar.isDynamic)
            {
                reportWarning(line, $"Possible memory leak: pointer '{varName}' is being reallocated without freeing previous allocation");
            }

            pointerVar.isDynamic = true;
            pointerVar.isInitializated = true;

            return "pointer";
        }

        public string? VisitFreeStatment(ExprParser.FreeStatementContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("free", context.Start.Line))
            {
                return null;
            } 

            string varName = context.ID().GetText();
            int line = context.Start.Line;

            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            // Handle array of pointers: free(ptrs[i])
            if (symbol is ArraySymbol arraySymbol && context.index().Length > 0)
            {
                // Check if array elements are pointers
                if (arraySymbol.pointerLevel > 0)
                {
                    // Valid: freeing an element of an array of pointers
                    return null;
                }
                else
                {
                    reportError(line, $"Cannot free array element of non-pointer type");
                    return null;
                }
            }

            // Regular case: free(ptr)
            if (symbol is not PointerSymbol)
            {
                reportError(line, $"Cannot free non-pointer variable '{varName}' (type: {symbol.type})");
                return null;
            }

            PointerSymbol pointerSymbol = (PointerSymbol)symbol;

            if (!pointerSymbol.isDynamic)
            {
                reportWarning(line, $"Freeing pointer '{varName}' that was not allocated with malloc - this may cause undefined behavior");
            }

            if (!pointerSymbol.isInitializated)
            {
                reportWarning(line, $"Attempting to free uninitialized pointer '{varName}' - this may cause undefined behavior");
            }

            pointerSymbol.isDynamic = false;
            pointerSymbol.isInitializated = false;      

            return null;
        }
            
    }
}