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
        private readonly Dictionary<string, HeterogenousInfo> structs;
        private readonly Func<ExprParser.ExprContext, string> visitExpression;

        public VariableAssignmentSemanticAnalyzer(
            SymbolTable symbolTable,
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            Dictionary<string, HeterogenousInfo> structs,
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
            var pointers = context.POINTER();
            Symbol symbol;
            
            if (typeToken == null)
            {
                var existingSymbol = symbolTable.GetSymbol(varName);
                if (existingSymbol == null)
                {
                    reportError(line, $"Variable '{varName}' was not declared");
                    return null;
                }
                existingSymbol.isInitializated = true;
                return existingSymbol.type;
            }

            // Case 2: Type specified 
            string type = typeToken.GetText();

            // Check if already declared in current scope only
            if (symbolTable.ContainsInCurrentScopeOnly(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            if (IsStructType(type) && !structs.ContainsKey(type))
            {
                reportError(line, $"Struct type '{type}' is not defined");
                return null;
            }
            
            if (pointers != null && pointers.Length > 0)
            {
                symbol = new PointerSymbol(varName, type, line, pointers.Length);
            }
            else
            {
                symbol = new Symbol(varName, type, line);
            }
            
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            
            return type;
        }

        public string? VisitDeclaration([NotNull] ExprParser.DeclarationContext context)
        {
            string type = context.type().GetText();
            string varName = context.ID().GetText();
            var indexes = context.intIndex();
            int pointerCount = context.POINTER().Length;
            int line = context.Start.Line;


            if (symbolTable.ContainsInCurrentScopeOnly(varName))
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
                
                // ✅ DEBUG AQUI
                Console.WriteLine($"[DEBUG] Creating ArraySymbol: name={varName}, elementType={elementType}, type={type}");
   
                
                // Create array symbol with pointer level
                var arraySymbol = new ArraySymbol(varName, elementType, line, dimensions, pointerCount);
                arraySymbol.isInitializated = true;

                // ✅ DEBUG AQUI TAMBÉM
                Console.WriteLine($"[DEBUG] ArraySymbol created: arraySymbol.type={arraySymbol.type}");
    
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

            if (symbol is not ArraySymbol && symbol is not PointerSymbol && indexes.Length > 0)
            {
                reportError(line, $"Variable '{varName}' is not an array or pointer");
                return null;
            }

            bool hasErrors = false;

            if (indexes.Length == 0)
            {
                string exprType = visitExpression(context.expr());
                if (exprType == null)
                {
                    return null;
                }

                if (!AreTypesCompatible(symbol.type, exprType))
                {
                    reportError(line, $"Type mismatch: cannot assign '{exprType}' to '{symbol.type}'");
                    return null;
                }

                return symbol.type;
            }

            if (symbol is ArraySymbol arraySymbol)
            {
                // ✅ DEBUG
                Console.WriteLine($"[DEBUG] SingleArrayAtt: varName={varName}, arraySymbol.type={arraySymbol.type}, indexes.Length={indexes.Length}, dimensions.Count={arraySymbol.dimensions.Count}");
    
                if (indexes.Length > arraySymbol.dimensions.Count)
                {
                    reportError(line, $"Too many indices for array '{varName}': expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                    hasErrors = true;
                }

                for (int i = 0; i < indexes.Length && i < arraySymbol.dimensions.Count; i++)
                {
                    string indexType = visitExpression(indexes[i].expr());

                    if (indexType == null)
                    {
                        hasErrors = true;
                        continue;
                    }

                    if (indexType != "int")
                    {
                        reportError(indexes[i].Start.Line, $"Array index must be of type 'int', got '{indexType}'");
                        hasErrors = true;
                    }

                    if (indexes[i].expr() is ExprParser.IntLiteralContext intLiteralContext)
                    {
                        int value = int.Parse(intLiteralContext.INT().GetText());
                        if (value < 0)
                        {
                            reportError(indexes[i].Start.Line, $"Array index cannot be negative: {value}");
                            hasErrors = true;
                        }
                        else if (value >= arraySymbol.dimensions[i])
                        {
                            reportError(indexes[i].Start.Line, $"Index {value} is out of bounds for dimension {i} (size: {arraySymbol.dimensions[i]})");
                            hasErrors = true;
                        }
                    }
                }

                if (hasErrors)
                {
                    return null;
                }

                string exprType = visitExpression(context.expr());
                if (exprType == null)
                {
                    return null;
                }

                string elementType = arraySymbol.elementType;

                // ✅ DEBUG
                Console.WriteLine($"[DEBUG] elementType={elementType}, exprType={exprType}");
    

                if (indexes.Length == arraySymbol.dimensions.Count)
                {
                    if (!AreTypesCompatible(elementType, exprType))
                    {
                        reportError(line, $"Type mismatch: cannot assign '{exprType}' to array element of type '{elementType}'");
                        return null;
                    }

                    arraySymbol.isInitializated = true;
                    return elementType;
                }
                else
                {
                    reportWarning(line, $"Partial array access on '{varName}'");
                    return $"array_of_{elementType}";
                }
            }

            if (symbol is not ArraySymbol && symbol is not PointerSymbol)
            {
                reportError(line, $"Variable '{varName}' is not an array nor Pointer");
                return null;
            }

            if (symbol is PointerSymbol pointerSymbol)
            {
                if (!pointerSymbol.isInitializated)
                {
                    reportWarning(line, $"Pointer '{varName}' may not be initialized");
                }

                foreach (var index in indexes)
                {
                    string indexType = visitExpression(index.expr());

                    if (indexType == null)
                    {
                        hasErrors = true;
                        continue;
                    }

                    if (indexType != "int")
                    {
                        reportError(index.Start.Line, $"Pointer index must be of type 'int', got '{indexType}'");
                        hasErrors = true;
                    }
                }

                if (hasErrors)
                {
                    return null;
                }

                string exprType = visitExpression(context.expr());
                if (exprType == null)
                {
                    return null;
                }

                string pointedType = pointerSymbol.pointeeType;
                int effectivePointerLevel = pointerSymbol.pointerLevel - indexes.Length;

                if (effectivePointerLevel < 0)
                {
                    reportError(line, $"Too many dereferences for pointer '{varName}'");
                    return null;
                }

                string expectedType = effectivePointerLevel > 0 
                    ? pointedType + new string('*', effectivePointerLevel)
                    : pointedType;

                if (!AreTypesCompatible(expectedType, exprType))
                {
                    reportError(line, $"Type mismatch: cannot assign '{exprType}' to '{expectedType}'");
                    return null;
                }

                return expectedType;
            }

            return null;
        }
        
        public string? VisitMallocAtt([NotNull] ExprParser.MallocAttContext context)
        {
            string varName = context.ID().GetText();
            int line = context.Start.Line;
            var typeToken = context.type();
            var pointers = context.POINTER();

            // If no type specified, variable must exist
            if (typeToken == null)
            {
                var existingSymbol = symbolTable.GetSymbol(varName);
                if (existingSymbol == null)
                {
                    reportError(line, $"Variable '{varName}' was not declared");
                    return null;
                }
                existingSymbol.isInitializated = true;
                return existingSymbol.type;
            }

            // Type specified - new variable declaration
            string type = typeToken.GetText();
            int pointerCount = pointers?.Length ?? 0;

            if (symbolTable.ContainsInCurrentScopeOnly(varName))
            {
                reportError(line, $"Variable '{varName}' has already been declared");
                return null;
            }

            // malloc always returns a pointer, so add at least one level
            if (pointerCount == 0)
            {
                pointerCount = 1;
            }

            var symbol = new PointerSymbol(varName, type, line, pointerCount, isDynamic: true);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);

            return "pointer";
        }

        private bool AreTypesCompatible(string targetType, string sourceType)
        {
            if (targetType == sourceType)
            {
                return true;
            }

            // Permitir conversões numéricas implícitas
            if ((targetType == "double" && sourceType == "int") ||
                (targetType == "int" && sourceType == "double"))
            {
                return true;
            }

            // Comparação de tipos struct
            if (targetType.StartsWith("struct_") && sourceType.StartsWith("struct_"))
            {
                return targetType == sourceType;
            }

            return false;
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