using System.Diagnostics.CodeAnalysis;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Pointer
{
    public class PointerSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;

        private readonly Func<ExprParser.ExprContext, string?> Visit;

        private readonly SymbolTable symbolTable;

        public PointerSemanticAnalyzer(Action<int, string> reportError, Action<int, string> reportWarning, Func<ExprParser.ExprContext, string> Visit, SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.Visit = Visit;
            this.symbolTable = symbolTable;
        }

        public string? VisitDerref([NotNull] ExprParser.DerrefAttContext context)
        {
            var derrefContext = context.derref();
            var exprContext = derrefContext.expr();
            int line = context.Start.Line;

            if (exprContext is ExprParser.VarContext varContext)
            {
                string name = varContext.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(name);

                if (symbol == null)
                {
                    reportError(line, $"Variable '{name}' was not declared");
                    return null;
                }

                if (symbol is PointerSymbol pointerSymbol)
                {
                    // Valid dereference of pointer
                    if (!pointerSymbol.isInitializated)
                    {
                        reportWarning(line, $"Dereferencing potentially uninitialized pointer '{name}'");
                    }
                    return pointerSymbol.pointeeType;
                }

                reportError(line, $"Cannot dereference a variable of type '{symbol.type}' - must be a pointer");
                return null;
            }

            return Visit(exprContext);
        }

        public string? VisitExprAddress([NotNull] ExprParser.ExprAddressContext context)
        {
            var exprContext = context.expr();
            int line = context.Start.Line;

            if (exprContext is ExprParser.VarContext varContext)
            {
                string varName = varContext.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);
                
                if (symbol == null)
                {
                    reportError(line, $"Variable '{varName}' was not declared");
                    return null;
                }

                return "pointer";
            }
            
            if (exprContext is ExprParser.ExprAddressContext)
            {
                reportError(context.Start.Line, "Cannot get an address of an andress");
                return null;
            }

            return Visit(context.expr());
        }
    }
}