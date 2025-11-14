using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Struct
{
    public class StructSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Dictionary<string, HeterogenousInfo> heterogenousInfo;
        private readonly Action<int, string> reportError;

        public StructSemanticAnalyzer(SymbolTable symbolTable, Dictionary<string, HeterogenousInfo> heterogenousInfo, Action<int, string> reportError)
        {
            this.symbolTable = symbolTable;
            this.heterogenousInfo = heterogenousInfo;
            this.reportError = reportError;
        }

        public void VisitStructStatement(ExprParser.StructStatementContext context)
        {
            string structName = context.ID().GetText();
            int line = context.Start.Line;

            // Register the struct in the dictionary
            if (heterogenousInfo.ContainsKey(structName))
            {
                reportError(line, $"Struct '{structName}' already declared");
                return;
            }

            var members = new Dictionary<string, Symbol>();
            heterogenousInfo[structName] = new StructInfo(structName, members, line);

            var declarationContexts = context.declaration();
            foreach (var declarationContext in declarationContexts)
            {
                Symbol? symbol = ProcessDeclaration(declarationContext, declarationContext.Start.Line);

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
        }
        
        public void VisitUnionStatement(ExprParser.UnionStatementContext context)
        {
            string unionName = context.ID().GetText();
            int line = context.Start.Line;
            var declarationContexts = context.declaration();
            var members = new Dictionary<string, Symbol>();
            
            foreach (var declaration in declarationContexts)
            {
                Symbol? symbol = ProcessDeclaration(declaration, declaration.Start.Line);
                if (symbol != null)
                {
                    // Check for duplicate members
                    if (members.ContainsKey(symbol.name))
                    {
                        reportError(line, $"Member '{symbol.name}' in union '{unionName}' was found duplicated");
                        continue;
                    }

                    members[symbol.name] = symbol;
                }
            }
            
            // Register the union in the dictionary
            if (!heterogenousInfo.ContainsKey(unionName))
            {
                heterogenousInfo[unionName] = new UnionInfo(unionName, members, line);
                return;
            }
            reportError(line, $"Union '{unionName}' already declared");
            return;
        }

        private Symbol? ProcessDeclaration(ExprParser.DeclarationContext context, int line)
        {
            string type = context.type().GetText();
            string id = context.ID().GetText();
            var indexes = context.intIndex();
            int pointerCount = context.POINTER().Length;

            string elementType = type;
            if (heterogenousInfo.ContainsKey(type))
            {
                // Check if it's a struct or union
                var hetInfo = heterogenousInfo[type];
                if (hetInfo is StructInfo)
                {
                    elementType = $"struct_{type}";
                }
                else if (hetInfo is UnionInfo)
                {
                    elementType = $"union_{type}";
                }
            }

            if (indexes.Length > 0)
            {
                if (type == "string")
                {
                    reportError(line, "Cannot declare arrays of type 'string'");
                    return null;
                }
                
                List<int> dimensions = [];
                foreach (var index in indexes)
                {
                    int intIndex = int.Parse(index.INT().GetText());
                    if (intIndex < 1)
                    {
                        reportError(line, $"Array dimension must be greater than 0 at line {index.Start.Line}");
                        return null;
                    }
                    dimensions.Add(intIndex);
                }

                // Create array symbol with pointer level
                // Examples: int[10], int*[10], struct MyStruct**[5][3]
                return new ArraySymbol(id, elementType, line, dimensions, pointerCount);
            }

            // Handle standalone pointers (non-array)
            if (pointerCount > 0)
            {
                // Examples: int*, struct MyStruct**, char***
                return new PointerSymbol(id, elementType, line, pointerCount);
            }

            // Handle struct members (non-pointer, non-array)
            if (heterogenousInfo.ContainsKey(type))
            {
                // Nested struct member
                return new StructSymbol(id, type, line, new Dictionary<string, Symbol>());
            }

            // Handle simple variable members
            // Examples: int, float, char
            return new Symbol(id, type, line);
        }

        public string? VisitStructGet(ExprParser.StructGetContext context)
        {
            int line = context.Start.Line;
            string currentType;
            Symbol? baseSymbol = null;
            var indexes = context.index();
            
            // Check if this is a dereferenced pointer access: (*ptr).member
            if (context.GetChild(0).GetText() == "(")
            {
                // This is: '(' POINTER expr ')' '.' structContinue
                var pointerToken = context.POINTER();
                if (pointerToken == null)
                {
                    reportError(line, "Invalid dereference syntax in struct access");
                    return null;
                }
                
                var exprContext = context.expr();
                if (exprContext == null)
                {
                    reportError(line, "Missing expression in dereference");
                    return null;
                }
                
                // Get the variable being dereferenced
                string? varName = null;
                if (exprContext is ExprParser.VarContext varCtx)
                {
                    varName = varCtx.ID().GetText();
                }
                else
                {
                    reportError(line, "Complex expressions in dereference not yet supported");
                    return null;
                }
                
                // Get the symbol
                baseSymbol = symbolTable.GetSymbol(varName);
                if (baseSymbol == null)
                {
                    reportError(line, $"Variable '{varName}' not found");
                    return null;
                }
                
                // Check if variable has been initialized
                if (!baseSymbol.isInitializated)
                {
                    reportError(line, $"Variable '{varName}' is used before being initialized");
                }
                
                // Must be a pointer
                if (!(baseSymbol is PointerSymbol pointerSymbol))
                {
                    reportError(line, $"Cannot dereference non-pointer variable '{varName}'");
                    return null;
                }
                
                // Get the pointee type
                currentType = pointerSymbol.pointeeType;
            }
            else
            {
                // Original code for regular struct access (ID.member or ID->member)
                string id = context.ID().GetText();

                // Get the base variable
                baseSymbol = symbolTable.GetSymbol(id);
                if (baseSymbol == null)
                {
                    reportError(line, $"Variable '{id}' not found");
                    return null;
                }

                // Check if variable has been initialized
                if (!baseSymbol.isInitializated)
                {
                    reportError(line, $"Variable '{id}' is used before being initialized");
                }

                // Determine the current type after array indexing (if any)
                currentType = baseSymbol.type;
                
                // Handle array indexing on base variable
                if (indexes.Length > 0)
                {
                    if (baseSymbol is ArraySymbol arraySymbol)
                    {
                        // Verify index count
                        if (indexes.Length > arraySymbol.dimensions.Count)
                        {
                            reportError(line, $"Too many indices for array '{id}'. Expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                            return null;
                        }
                        
                        // After indexing, the type is the inner type
                        currentType = arraySymbol.elementType;
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
                    if (baseSymbol is PointerSymbol pointerSymbol)
                    {
                        currentType = pointerSymbol.pointeeType;
                    }
                    else
                    {
                        reportError(line, $"Arrow operator '->' used on non-pointer variable '{id}'");
                        return null;
                    }
                }
            }

            // Clean up type name if it has "struct_" or "union_" prefix
            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7);
            }
            else if (currentType.StartsWith("union_"))
            {
                currentType = currentType.Substring(6);
            }

            // Verify the type is a struct or union
            if (!heterogenousInfo.ContainsKey(currentType))
            {
                string? varIdentifier = baseSymbol?.name ?? "dereferenced pointer";
                reportError(line, $"Variable '{varIdentifier}' is not a struct or union type. Cannot access members.");
                return null;
            }

            // Navigate through the struct chain and return the final field type
            string? finalType = NavigateStructChain(context.structContinue(), currentType, line);

            if (finalType != null)
            {
                if (finalType.StartsWith("struct_"))
                {
                    finalType = finalType.Substring(7);
                }
                else if (finalType.StartsWith("union_"))
                {
                    finalType = finalType.Substring(6);
                }
            }
            
            return finalType;
        }

        private string? NavigateStructChain(ExprParser.StructContinueContext context, string currentType, int line)
        {
            // Clean up type name
            if (currentType.StartsWith("struct_"))
            {
                currentType = currentType.Substring(7);
            }
            else if (currentType.StartsWith("union_"))
            {
                currentType = currentType.Substring(6);
            }

            // Get the struct/union definition
            if (!heterogenousInfo.TryGetValue(currentType, out HeterogenousInfo? currentHetType))
            {
                reportError(line, $"Struct or Union type '{currentType}' not found");
                return null;
            }

            // Check if this structContinue is a nested structGet 
            var nestedStructGet = context.structGet();
            if (nestedStructGet != null)
            {
                string memberId = nestedStructGet.ID().GetText();
                var indexes = nestedStructGet.index();

                // Check if the field exists
                if (!currentHetType.HasMember(memberId))
                {
                    var availableMembers = string.Join(", ", currentHetType.members.Keys);
                    reportError(line, $"Struct or Union '{currentType}' does not have a member '{memberId}'. Available members: {availableMembers}");
                    return null;
                }

                Symbol fieldSymbol = currentHetType.GetMember(memberId)!;
                string fieldType = fieldSymbol.type;

                // Handle array indexing on the field
                if (indexes.Length > 0)
                {
                    if (fieldSymbol is ArraySymbol arrayField)
                    {
                        // Verify index count
                        if (indexes.Length > arrayField.dimensions.Count)
                        {
                            reportError(line, $"Too many indices for field '{memberId}'. Expected {arrayField.dimensions.Count}, got {indexes.Length}");
                            return null;
                        }
                        
                        // After indexing, type is the inner type
                        fieldType = arrayField.elementType;
                    }
                    else
                    {
                        reportError(line, $"Field '{memberId}' is not an array but is being indexed");
                        return null;
                    }
                }

                // Check for arrow operator
                bool isArrowOp = false;
                int childIndex = indexes.Length + 1;
                if (nestedStructGet.ChildCount > childIndex)
                {
                    var child = nestedStructGet.GetChild(childIndex);
                    isArrowOp = child?.GetText() == "->";
                }

                if (isArrowOp)
                {
                    // Field should be a pointer to struct/union
                    if (fieldSymbol is PointerSymbol pointerField)
                    {
                        fieldType = pointerField.pointeeType;
                    }
                    else
                    {
                        reportError(line, $"Arrow operator '->' used on non-pointer field '{memberId}'");
                        return null;
                    }
                }

                // Get the nested structContinue
                var nestedStructContinue = nestedStructGet.structContinue();
                if (nestedStructContinue == null)
                {
                    reportError(line, $"Invalid struct/union access syntax");
                    return null;
                }

                // Clean up field type prefix before recursion
                if (fieldType.StartsWith("struct_"))
                {
                    fieldType = fieldType.Substring(7);
                }
                else if (fieldType.StartsWith("union_"))
                {
                    fieldType = fieldType.Substring(6);
                }

                // Check if the field type is a struct/union for nested access
                if (!heterogenousInfo.ContainsKey(fieldType))
                {
                    reportError(line, $"Field '{memberId}' is not a struct or union type. Cannot access nested members.");
                    return null;
                }

                // Recursive call with the nested struct continue
                return NavigateStructChain(nestedStructContinue, fieldType, line);
            }
            else
            {
                // This is a terminal ID 
                string fieldId = context.ID().GetText();
                var indexes = context.index();

                // Check if the field exists
                if (!currentHetType.HasMember(fieldId))
                {
                    var availableMembers = string.Join(", ", currentHetType.members.Keys);
                    reportError(line, $"Struct or Union '{currentType}' does not have a member '{fieldId}'. Available members: {availableMembers}");
                    return null;
                }

                Symbol fieldSymbol = currentHetType.GetMember(fieldId)!;
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
                        fieldType = arrayField.elementType;
                    }
                    else
                    {
                        reportError(line, $"Field '{fieldId}' is not an array but is being indexed");
                        return null;
                    }
                }

                // Terminal case - return the final field type
                return fieldType;
            }
        }
    }
}