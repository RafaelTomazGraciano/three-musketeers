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

            int line = context.Start.Line;
            var structGet = context.structGet();
            if (structGet != null)
            {
                return null;
            }

            // Handle regular variable or array element
            string varName = context.ID().GetText();
            var indexes = context.index();
            
            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            // If there are indices, validate array access
            if (indexes.Length > 0)
            {
                if (symbol is ArraySymbol arraySymbol)
                {
                    if (indexes.Length > arraySymbol.dimensions.Count)
                    {
                        reportError(line, $"Too many indices for array '{varName}': expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                        return null;
                    }

                    // Validate each index
                    foreach (var index in indexes)
                    {
                        string? indexType = VisitExpr(index.expr()); 
                        if (indexType != null && indexType != "int")
                        {
                            reportError(index.Start.Line, $"Array index must be of type 'int', got '{indexType}'");
                            return null;
                        }
                    }

                    // Check if the element type is a pointer
                    string elementType = arraySymbol.elementType;
                    if (!elementType.EndsWith("*") && !(arraySymbol.pointerLevel > 0))
                    {
                        reportError(line, $"Cannot free non-pointer array element of type '{elementType}'");
                        return null;
                    }
                }
                else if (symbol is PointerSymbol pointerSymbol)
                {
                    // Pointer with index access (e.g., ptr[0])
                    foreach (var index in indexes)
                    {
                        string? indexType = VisitExpr(index.expr());
                        if (indexType != null && indexType != "int")
                        {
                            reportError(index.Start.Line, $"Pointer index must be of type 'int', got '{indexType}'");
                            return null;
                        }
                    }

                    if (pointerSymbol.pointerLevel < indexes.Length)
                    {
                        reportError(line, $"Too many dereferences for pointer '{varName}'");
                        return null;
                    }
                }
                else
                {
                    reportError(line, $"Variable '{varName}' is not an array or pointer");
                    return null;
                }
            }
            else
            {
                // No indices - must be a pointer
                if (!(symbol is PointerSymbol))
                {
                    reportError(line, $"Cannot free non-pointer variable '{varName}'");
                    return null;
                }

                var pointerSymbol = symbol as PointerSymbol;
                if (!pointerSymbol!.isDynamic)
                {
                    reportWarning(line, $"Freeing pointer '{varName}' that may not have been dynamically allocated");
                }
            }

            return null;
        }

        public void VisitMallocStructAtt(ExprParser.MallocStructAttContext context)
        {
            if (!libraryTracker.CheckFunctionDependency("free", context.Start.Line))
            {
                return;
            } 
            
            int line = context.Start.Line;
            // Validate the size expression
            var sizeExpr = context.expr();
            if (sizeExpr == null)
            {
                reportError(line, "malloc requires a size argument");
                return;
            }
            
            // Visit the size expression to check its type
            string? sizeType = VisitExpr(sizeExpr);
            if (sizeType == null)
            {
                return;
            }
            
            // Size must be an integer
            if (sizeType != "int")
            {
                reportError(line, $"malloc size must be of type 'int', got '{sizeType}'");
                return;
            }
            
            // Check for zero or negative size if it's a literal
            if (sizeExpr is ExprParser.IntLiteralContext intLiteral)
            {
                int size = int.Parse(intLiteral.INT().GetText());
                if (size <= 0)
                {
                    reportWarning(line, $"malloc with size {size} may cause undefined behavior");
                }
            }
        }
            
    }
}