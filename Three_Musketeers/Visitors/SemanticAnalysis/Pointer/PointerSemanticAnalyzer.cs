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
            var exprContext = context.expr();
            int line = context.Start.Line;
            if (exprContext is ExprParser.VarContext varContext)
            {
                string name = varContext.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varContext.ID().GetText());
                if (symbol == null)
                {
                    reportError(line, $"Variable '{name}' was not declared");
                    return null;
                }

                if (symbol.type != "pointer" || symbol.type != "array")
                {
                    reportError(line, $"Cannot derref a variable of type {symbol.type}");
                    return null;
                }
                return symbol.type;
            }
            
            return Visit(context.expr());
        }

        public string? VisitExprAddress([NotNull] ExprParser.ExprAddressContext context)
        {
            var exprContext = context.expr();
            if (exprContext is ExprParser.ExprAddressContext)
            {
                reportError(context.Start.Line, "Cannot get an address of an andress");
                return null;
            }

            return "pointer";
        }
        
        public string? VisitFreeStatement([NotNull] ExprParser.FreeStatementContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;
            Symbol? symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }
            if (symbol is not PointerSymbol)
            {
                reportError(line, $"Variable '{varName}' is not a pointer");
                return null;
            }
            PointerSymbol pointer = (PointerSymbol)symbol;
            pointer.isDynamic = false;
            pointer.isInitializated = false;
            return null;
        }
    }
}