using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Pointer
{
    public class DynamicMemorySemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;

        private readonly Action<int, string> reportWarning;

        private readonly Func<ExprParser.ExprContext, string> VisitExpr;

        public DynamicMemorySemanticAnalyzer(SymbolTable symbolTable, Action<int, string> reportError, Action<int, string> reportWarning, Func<ExprParser.ExprContext, string> VisitExpr)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.VisitExpr = VisitExpr;
        }

        public string? VisitMallocAtt(ExprParser.MallocAttContext context)
        {
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

            if (VisitExpr(context.expr()) == "string")
            {
                reportError(line, $"Expected char, int or double not string");
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

            // Case 2: Malloc assignment to existing variable (pointer = malloc(5))
            Symbol? symbol = symbolTable.GetSymbol(varName);
            
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

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
    }
}