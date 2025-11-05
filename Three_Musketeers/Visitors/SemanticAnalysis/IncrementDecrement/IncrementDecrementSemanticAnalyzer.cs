using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.IncrementDecrement
{
    public class IncrementDecrementSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly SymbolTable symbolTable;

        public IncrementDecrementSemanticAnalyzer(
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.symbolTable = symbolTable;
        }

        // Prefix increment: ++x
        public string? VisitPrefixIncrement([NotNull] ExprParser.PrefixIncrementContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateIncrementDecrement(varName, line);
        }

        // Prefix decrement: --x
        public string? VisitPrefixDecrement([NotNull] ExprParser.PrefixDecrementContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateIncrementDecrement(varName, line);
        }

        // Postfix increment: x++
        public string? VisitPostfixIncrement([NotNull] ExprParser.PostfixIncrementContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateIncrementDecrement(varName, line);
        }

        // Postfix decrement: x--
        public string? VisitPostfixDecrement([NotNull] ExprParser.PostfixDecrementContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateIncrementDecrement(varName, line);
        }

        // Prefix increment array: ++arr[0]
        public string? VisitPrefixIncrementArray([NotNull] ExprParser.PrefixIncrementArrayContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateArrayIncrementDecrement(varName, context.index(), line);
        }

        // Prefix decrement array: --arr[0]
        public string? VisitPrefixDecrementArray([NotNull] ExprParser.PrefixDecrementArrayContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateArrayIncrementDecrement(varName, context.index(), line);
        }

        // Postfix increment array: arr[0]++
        public string? VisitPostfixIncrementArray([NotNull] ExprParser.PostfixIncrementArrayContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateArrayIncrementDecrement(varName, context.index(), line);
        }

        // Postfix decrement array: arr[0]--
        public string? VisitPostfixDecrementArray([NotNull] ExprParser.PostfixDecrementArrayContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            return ValidateArrayIncrementDecrement(varName, context.index(), line);
        }

        private string? ValidateIncrementDecrement(string varName, int line)
        {
            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if (!symbol.isInitializated)
            {
                reportError(line, $"Variable '{varName}' is not initialized");
                return null;
            }

            if (!IsNumericType(symbol.type))
            {
                reportError(line, $"Increment/decrement operators can only be used with numeric types, got '{symbol.type}'");
                return null;
            }

            return symbol.type;
        }

        private string? ValidateArrayIncrementDecrement(string varName, ExprParser.IndexContext[] indices, int line)
        {
            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if (symbol is not ArraySymbol arraySymbol)
            {
                reportError(line, $"Variable '{varName}' is not an array");
                return null;
            }

            if (indices.Length != arraySymbol.dimensions.Count)
            {
                reportError(line, $"Array '{varName}' expects {arraySymbol.dimensions.Count} indices, but got {indices.Length}");
                return null;
            }

            for (int i = 0; i < indices.Length; i++)
            {
                var indexCtx = indices[i];
                /*int indexValue = int.Parse(indexCtx.INT().GetText());
                if (indexValue < 0 || indexValue >= arraySymbol.dimensions[i])
                {
                    reportError(indexCtx.Start.Line,
                        $"Index {i} of array '{varName}' is out of bounds. Expected 0 to {arraySymbol.dimensions[i] - 1}, but got {indexValue}");
                    return null;
                }*/
            }

            if (!IsNumericType(arraySymbol.elementType))
            {
                reportError(line, $"Increment/decrement operators can only be used with numeric types, got '{arraySymbol.elementType}'");
                return null;
            }

            return arraySymbol.elementType;
        }

        private bool IsNumericType(string type)
        {
            return type == "int" || type == "double" || type == "char" || type == "bool";
        }
    }
}
