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
        private readonly Dictionary<string, StructInfo> structs;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Dictionary<string, StructInfo> structs)
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.structs = structs;
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

            if (symbolTable.Contains(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            // Valida se tipo struct existe
            if (IsStructType(type) && !structs.ContainsKey(type))
            {
                reportError(line, $"Struct type '{type}' is not defined");
                return null;
            }

            var symbol = new Symbol(varName, type, line);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
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

            // Validate if struct type exists
            if (IsStructType(type) && !structs.ContainsKey(type))
            {
                reportError(line, $"Struct type '{type}' is not defined");
                return null;
            }

            if (indexes.Length > 0)
            {
                List<int> dimension = new List<int>();
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

                if (IsStructType(type))
                {
                    var arraySymbol = new ArraySymbol(varName, $"struct_{type}", line, dimension);
                    arraySymbol.isInitializated = true;
                    symbolTable.AddSymbol(arraySymbol);
                    return "array";
                }

                symbolTable.AddSymbol(new ArraySymbol(varName, type, line, dimension));
                return "array";
            }

            if (IsStructType(type))
            {
                var structSymbol = new StructSymbol(varName, type, line, new Dictionary<string, Symbol>());
                structSymbol.isInitializated = true;
                symbolTable.AddSymbol(structSymbol);
                return $"struct_{type}";
            }

            var symbol = new Symbol(varName, type, line);
            symbolTable.AddSymbol(symbol);

            return type;
        }
        public string? VisitDeclaration([NotNull] ExprParser.PointerDecContext context)
        {
            var typeToken = context.type();
            string varName = context.ID().GetText();
            int pointers = context.POINTER().Length;
            int line = context.Start.Line;

            if (typeToken == null)
            {
                var variable = symbolTable.GetSymbol(varName);
                if (variable == null)
                {
                    reportError(line, $"Variable '{varName}' was not declared");
                    return null;
                }
                return variable.type;
            }

            string type = typeToken.GetText();

            // Valida se tipo struct existe
            if (IsStructType(type) && !structs.ContainsKey(type))
            {
                reportError(line, $"Struct type '{type}' is not defined");
                return null;
            }

            // Ponteiro para struct
            symbolTable.AddSymbol(new PointerSymbol(varName, type, line, pointers));
            return "pointer";
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
            var indices = context.index();

            var symbol = symbolTable.GetSymbol(varName);
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }

            string type = symbol.type;

            if (indices.Length == 0)
            {
                return type;
            }

            if (symbol is not ArraySymbol && symbol is not PointerSymbol)
            {
                reportError(line, $"Variable '{varName}' is not an array nor Pointer");
                return null;
            }

            if (symbol is ArraySymbol arraySymbol)
            {
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

                // Retorna o tipo interno do array (pode ser struct)
                return arraySymbol.innerType;
            }

            var pointerSymbol = (PointerSymbol)symbol;

            if (indices.Length > pointerSymbol.amountOfPointers)
            {
                reportError(line, $"Variable '{varName}' have more indices. Expected up to {pointerSymbol.amountOfPointers} got {indices.Length}");
                return null;
            }

            // Retorna o tipo interno do ponteiro (pode ser struct)
            return pointerSymbol.innerType;
        }

        // Método auxiliar para verificar se um tipo é struct
        private bool IsStructType(string typeName)
        {
            // Verifica se é um tipo struct conhecido
            // Pode ser "Point" ou já formatado como "struct_Point"
            if (typeName.StartsWith("struct_"))
            {
                string structName = typeName.Substring(7);
                return structs.ContainsKey(structName);
            }
            
            return structs.ContainsKey(typeName);
        }

        // Método auxiliar para obter informações de struct
        public StructInfo? GetStructInfo(string typeName)
        {
            if (typeName.StartsWith("struct_"))
            {
                string structName = typeName.Substring(7);
                return structs.ContainsKey(structName) ? structs[structName] : null;
            }
            
            return structs.ContainsKey(typeName) ? structs[typeName] : null;
        }
    }
}