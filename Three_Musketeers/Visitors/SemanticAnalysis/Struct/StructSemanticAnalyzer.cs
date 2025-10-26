// ============================================================================
// Complete implementation for StructSemanticAnalyzer.cs
// ============================================================================

using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Struct
{
    public class StructSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Dictionary<string, StructInfo> structInfo;
        private readonly Action<int, string> reportError;

        public StructSemanticAnalyzer(SymbolTable symbolTable, Dictionary<string, StructInfo> structInfo, Action<int, string> reportError)
        {
            this.symbolTable = symbolTable;
            this.structInfo = structInfo;
            this.reportError = reportError;
        }

        public string? VisitStructStatement(ExprParser.StructStatementContext context)
        {
            string structName = context.ID().GetText();
            int line = context.Start.Line;
            var declarationContexts = context.declaration();
            var members = new Dictionary<string, Symbol>();

            foreach (var declarationContext in declarationContexts)
            {
                Symbol? symbol = ProcessDeclaration(declarationContext);
                
                if (symbol != null)
                {
                    // Check for duplicate members
                    if (members.ContainsKey(symbol.name))
                    {
                        reportError(line, $"Member '{symbol.name}' in struct '{structName}' was found duplicated");
                        continue;
                    }
                    
                    members[symbol.name] = symbol;
                }
            }

            // Register the struct in the dictionary
            if (!structInfo.ContainsKey(structName))
            {
                structInfo[structName] = new StructInfo(structName, members, line);
                return null;
            }
            reportError(line, $"Struct '{structName}' already declared");
            return null;
        }

        private Symbol? ProcessDeclaration(ExprParser.DeclarationContext declarationContext)
        {
            int line = declarationContext.Start.Line;

            if (declarationContext is ExprParser.BaseDecContext baseDecContext)
            {
                return ProcessBaseDeclaration(baseDecContext, line);
            }
            
            if (declarationContext is ExprParser.PointerDecContext pointerDecContext)
            {
                return ProcessPointerDeclaration(pointerDecContext, line);
            }

            return null;
        }

        private Symbol ProcessBaseDeclaration(ExprParser.BaseDecContext context, int line)
        {
            string type = context.type().GetText();
            string id = context.ID().GetText();
            var indexes = context.index();

            // No indexes - simple variable or simple struct
            if (indexes.Length == 0)
            {
                // Check if it's a struct type
                if (structInfo.ContainsKey(type))
                {
                    return new StructSymbol(id, type, line, new Dictionary<string, Symbol>());
                }
                
                return new Symbol(id, type, line);
            }

            // With indexes - array
            List<int> dimensions = new List<int>();
            foreach (var index in indexes)
            {
                int intIndex = int.Parse(index.INT().GetText());
                dimensions.Add(intIndex);
            }

            // Struct array
            if (structInfo.ContainsKey(type))
            {
                return new ArraySymbol(id, $"struct_{type}", line, dimensions);
            }

            // Normal array
            return new ArraySymbol(id, type, line, dimensions);
        }

        private static PointerSymbol ProcessPointerDeclaration(ExprParser.PointerDecContext context, int line)
        {
            string type = context.type().GetText();
            string id = context.ID().GetText();
            int pointerCount = context.POINTER().Length;

            return new PointerSymbol(id, type, line, pointerCount);
        }

        public string? VisitStructGet(ExprParser.StructGetContext context)
        {
            string id = context.ID().GetText();
            int line = context.Start.Line;
            var indexes = context.index();

            // Get the base variable
            var symbol = symbolTable.GetSymbol(id);
            if (symbol == null)
            {
                reportError(line, $"Variable '{id}' not found");
                return null;
            }

            // Check if variable has been initialized
            if (!symbol.isInitializated)
            {
                reportError(line, $"Variable '{id}' is used before being initialized");
            }

            // Determine the current type after array indexing (if any)
            string currentType = symbol.type;
            
            // Handle array indexing on base variable
            if (indexes.Length > 0)
            {
                if (symbol is ArraySymbol arraySymbol)
                {
                    // Verify index count
                    if (indexes.Length > arraySymbol.dimensions.Count)
                    {
                        reportError(line, $"Too many indices for array '{id}'. Expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                        return null;
                    }
                    
                    // After indexing, the type is the inner type
                    currentType = arraySymbol.innerType;
                }
                else
                {
                    reportError(line, $"Variable '{id}' is not an array but is being indexed");
                    return null;
                }
            }

            bool isArrowOp = context.GetChild(indexes.Length + 1).GetText() == "->";
            
            if (isArrowOp)
            {
                if (symbol is PointerSymbol pointerSymbol)
                {
                    currentType = pointerSymbol.innerType;
                }
                else
                {
                    reportError(line, $"Arrow operator '->' used on non-pointer variable '{id}'");
                    return null;
                }
            }

            // Clean up type name if it has "struct_" prefix
            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7);
            }

            // Verify the type is a struct
            if (!structInfo.ContainsKey(currentType))
            {
                reportError(line, $"Variable '{id}' is not a struct type. Cannot access members.");
                return null;
            }

            // Navigate through the struct chain and return the final field type
            return NavigateStructChain(context.structContinue(), currentType, line);
        }

        private string? NavigateStructChain(ExprParser.StructContinueContext context, string currentType, int line)
        {
            string fieldId = context.ID().GetText();
            var indexes = context.index();

            // Clean up type name
            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7);
            }

            // Get the struct definition
            if (!structInfo.TryGetValue(currentType, out StructInfo? currentStruct))
            {
                reportError(line, $"Struct type '{currentType}' not found");
                return null;
            }

            // Check if the field exists
            if (!currentStruct.HasMember(fieldId))
            {
                var availableMembers = string.Join(", ", currentStruct.members.Keys);
                reportError(line, $"Struct '{currentType}' does not have a member '{fieldId}'. Available members: {availableMembers}");
                return null;
            }

            Symbol fieldSymbol = currentStruct.GetMember(fieldId)!;
            string fieldType = fieldSymbol.type;

            // Handle array indexing on the field
            if (indexes.Length > 0)
            {
                if (fieldSymbol is ArraySymbol arrayField)
                {
                    // Verify index count
                    if (indexes.Length > arrayField.dimensions.Count)
                    {
                        reportError(line, $"Too many indices for field '{fieldId}'. Expected {arrayField.dimensions.Count}, got {indexes.Length}");
                        return null;
                    }
                    
                    // After indexing, type is the inner type
                    fieldType = arrayField.innerType;
                }
                else
                {
                    reportError(line, $"Field '{fieldId}' is not an array but is being indexed");
                    return null;
                }
            }

            // Check if there's nested struct access
            var nestedStructGet = context.structGet();
            if (nestedStructGet != null)
            {
                var nestedIndexes = nestedStructGet.index();
                bool isArrowOp = nestedStructGet.GetChild(nestedIndexes.Length + 1)?.GetText() == "->";

                if (isArrowOp)
                {
                    // Field should be a pointer to struct
                    if (fieldSymbol is PointerSymbol pointerField)
                    {
                        fieldType = pointerField.innerType;
                    }
                    else
                    {
                        reportError(line, $"Arrow operator '->' used on non-pointer field '{fieldId}'");
                        return null;
                    }
                }

                if (fieldType.StartsWith("struct_"))
                {
                    fieldType = fieldType.Substring(7);
                }

                if (!structInfo.ContainsKey(fieldType))
                {
                    reportError(line, $"Field '{fieldId}' is not a struct type. Cannot access nested members.");
                    return null;
                }

                return NavigateStructChain(nestedStructGet.structContinue(), fieldType, line);
            }

            return fieldType;
        }
    }
}