using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Variables
{
    public class VariableAssignmentSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning
            )
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
        }

        public string? VisitAtt([NotNull] ExprParser.GenericAttContext context)
        {
            var typeToken = context.type();
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            if (typeToken == null)
            {
                var existingSymbol = symbolTable.GetSymbol(varName);
                if (existingSymbol == null)
                {

                    reportError(context.Start.Line, $"Variable {varName} does not have a type");
                    return null;
                }
                existingSymbol.isInitializated = true;
                return existingSymbol.type;
            }

            string type = typeToken.GetText();

            if (type == "void")
            {
                reportError(line, $"Variable '{varName}' cannot be void");
            }

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);
            symbolTable.MarkInitializated(varName);
            return null;
        }

        public string? VisitDeclaration([NotNull] ExprParser.BaseDecContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            var indexes = context.index();
            int line = context.Start.Line;

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            if (indexes.Length > 0)
            {
                List<int> dimension = new ArrayList<int>();
                bool hasErrors = false;
                foreach (var index in indexes)
                {
                    int dim = int.Parse(index.INT().GetText());
                    if (dim < 1)
                    {
                        reportError(index.Start.Line, "Array dimension must be greater than 0");
                        hasErrors = true;
                        continue;
                    }
                    dimension.Add(dim);
                }

                if (hasErrors) return null;

                symbolTable.AddSymbol(new ArraySymbol(varName, type, line, dimension));
                return "array";
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);

            return type;
        }

        public string? VisitVar([NotNull] ExprParser.VarContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            if (!symbol.isInitializated)
            {
                reportError(line, $"Variable '{varName}' is empty");
                return null;
            }

            return symbol.type;
        }

        public string? VisitSingleAtt(ExprParser.SingleAttContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;

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

            var indices = context.index();
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

            return arraySymbol.innerType;
        }
    }
}

