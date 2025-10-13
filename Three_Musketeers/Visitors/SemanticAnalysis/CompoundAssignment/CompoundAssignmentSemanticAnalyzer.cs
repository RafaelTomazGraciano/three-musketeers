using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.CompoundAssignment
{
    public class CompoundAssignmentSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly SymbolTable symbolTable;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;

        public CompoundAssignmentSemanticAnalyzer(
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            SymbolTable symbolTable,
            Func<ExprParser.ExprContext, string> getExpressionType)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.symbolTable = symbolTable;
            this.getExpressionType = getExpressionType;
        }

        // Array element +=
        public string? VisitSingleAttPlusEquals([NotNull] ExprParser.SingleAttPlusEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            int line = context.Start.Line;

            return ValidateArrayCompoundAssignment(varName, indexes, context.expr(), line);
        }

        // Array element -=
        public string? VisitSingleAttMinusEquals([NotNull] ExprParser.SingleAttMinusEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            int line = context.Start.Line;

            return ValidateArrayCompoundAssignment(varName, indexes, context.expr(), line);
        }


        // Array element *=
        public string? VisitSingleAttMultiplyEquals([NotNull] ExprParser.SingleAttMultiplyEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            int line = context.Start.Line;

            return ValidateArrayCompoundAssignment(varName, indexes, context.expr(), line);
        }

        // Array element /=
        public string? VisitSingleAttDivideEquals([NotNull] ExprParser.SingleAttDivideEqualsContext context)
        {
            string varName = context.ID().GetText();
            var indexes = context.index();
            int line = context.Start.Line;

            return ValidateArrayCompoundAssignment(varName, indexes, context.expr(), line);
        }

        private string? ValidateCompoundAssignment(string varName, ExprParser.ExprContext exprContext, int line)
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
                reportError(line, $"Compound assignment operators can only be used with numeric types, got '{symbol.type}'");
                return null;
            }

            // Validate expression type compatibility
            string exprType = getExpressionType(exprContext);
            if (!AreTypesCompatible(symbol.type, exprType))
            {
                reportWarning(line, $"Type mismatch: '{symbol.type}' and '{exprType}' in compound assignment");
            }

            return symbol.type;
        }

        private string? ValidateArrayCompoundAssignment(string varName, ExprParser.IndexContext[] indices, ExprParser.ExprContext exprContext, int line)
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
                int indexValue = int.Parse(indexCtx.INT().GetText());
                if (indexValue < 0 || indexValue >= arraySymbol.dimensions[i])
                {
                    reportError(indexCtx.Start.Line,
                        $"Index {i} of array '{varName}' is out of bounds. Expected 0 to {arraySymbol.dimensions[i] - 1}, but got {indexValue}");
                    return null;
                }
            }

            if (!IsNumericType(arraySymbol.innerType))
            {
                reportError(line, $"Compound assignment operators can only be used with numeric types, got '{arraySymbol.innerType}'");
                return null;
            }

            // Validate expression type compatibility
            string exprType = getExpressionType(exprContext);
            if (!AreTypesCompatible(arraySymbol.innerType, exprType))
            {
                reportWarning(line, $"Type mismatch: '{arraySymbol.innerType}' and '{exprType}' in compound assignment");
            }

            return arraySymbol.innerType;
        }

        private bool IsNumericType(string type)
        {
            return type == "int" || type == "double" || type == "char" || type == "bool";
        }

        private bool AreTypesCompatible(string varType, string exprType)
        {
            // Allow implicit conversions between numeric types
            if (varType == exprType) return true;
            
            bool varIsNumeric = IsNumericType(varType);
            bool exprIsNumeric = IsNumericType(exprType);
            
            return varIsNumeric && exprIsNumeric;
        }
    }
}
