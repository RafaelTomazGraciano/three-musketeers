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
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Dictionary<string, StructInfo> structs,
            Func<ExprParser.ExprContext, string> visitExpression
            )
        {
            this.symbolTable = symbolTable;
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.structs = structs;
            this.visitExpression = visitExpression;
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

        public string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            var indexes = context.intIndex();
            int pointerCount = context.POINTER().Length;
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
        
            // Handle array declarations
            if (indexes.Length > 0)
            {
                List<int> dimensions = new List<int>();
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
                    dimensions.Add(dim);
                }
        
                if (hasErrors) return null;
        
                // Determine the element type for the array
                string elementType = IsStructType(type) ? $"struct_{type}" : type;
                
                // Create array symbol with pointer level
                var arraySymbol = new ArraySymbol(varName, elementType, line, dimensions, pointerCount);
                arraySymbol.isInitializated = true;
                symbolTable.AddSymbol(arraySymbol);
                
                // Return type description
                if (pointerCount > 0)
                {
                    string pointers = new string('*', pointerCount);
                    return $"array_of_{elementType}{pointers}";
                }
                return $"array_of_{elementType}";
            }
        
            // Handle pointer declarations (non-array)
            if (pointerCount > 0)
            {
                string pointeeType = IsStructType(type) ? $"struct_{type}" : type;
                var pointerSymbol = new PointerSymbol(varName, pointeeType, line, pointerCount);
                symbolTable.AddSymbol(pointerSymbol);
                
                string pointers = new string('*', pointerCount);
                return $"{pointeeType}{pointers}";
            }
        
            // Handle struct declarations (non-pointer, non-array)
            if (IsStructType(type))
            {
                var structSymbol = new StructSymbol(varName, type, line, new Dictionary<string, Symbol>())
                {
                    isInitializated = true
                };
                symbolTable.AddSymbol(structSymbol);
                return $"struct_{type}";
            }
        
            // Handle regular variable declarations
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

        public string? VisitSingleArrayAtt(ExprParser.SingleArrayAttContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;
            var symbol = symbolTable.GetSymbol(varName);
            var indexes = context.index();
            if (symbol == null)
            {
                reportError(line, $"Variable '{varName}' was not declared");
                return null;
            }
            if (indexes.Length > 0)
            {
                bool hasErrors = false;
                if (symbol is not ArraySymbol or PointerSymbol)
                {
                    reportError(line, $"Variable '{varName}' is not an array nor pointer");
                    return null;
                }

                if (symbol is ArraySymbol arraySymbol)
                {

                    foreach (var index in indexes)
                    {
                        if (index.expr() is ExprParser.IntLiteralContext intLiteralContext)
                        {
                            int value = int.Parse(intLiteralContext.INT().GetText());
                            if (value < 0 || value >= arraySymbol.dimensions.Count)
                            {
                                reportError(index.Start.Line, $"Index {value} is out of bounds for array '{varName}'");
                                hasErrors = true;
                            }
                        }
                        string typeOfExpr = visitExpression(index.expr());
                    }
                }

                var pointerSymbol 
            }

            return symbol.type;
        }

        private bool IsStructType(string typeName)
        {
            if (typeName.StartsWith("struct_"))
            {
                string structName = typeName.Substring(7);
                return structs.ContainsKey(structName);
            }
            
            return structs.ContainsKey(typeName);
        }
    }
}