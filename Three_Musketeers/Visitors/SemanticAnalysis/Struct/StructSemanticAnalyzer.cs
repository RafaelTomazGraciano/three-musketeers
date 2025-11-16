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
            var typeContext = context.type();
            string id = context.ID().GetText();
            var indexes = context.intIndex();
            int pointerCount = context.POINTER().Length;
            string elementType;
            
            // Check if it's a struct or union type
            if (typeContext.GetChild(0).GetText() == "struct" && typeContext.ID() != null)
            {
                string structName = typeContext.ID().GetText();
                elementType = $"struct_{structName}";
            }
            else if (typeContext.GetChild(0).GetText() == "union" && typeContext.ID() != null)
            {
                string unionName = typeContext.ID().GetText();
                elementType = $"union_{unionName}";
            }
            else
            {
                string type = typeContext.GetText();
                elementType = type;
                
                // Check if type exists in heterogenousInfo
                if (heterogenousInfo.ContainsKey(type))
                {
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
            }

            if (indexes.Length > 0)
            {
                if (elementType == "string")
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
                return new ArraySymbol(id, elementType, line, dimensions, pointerCount);
            }

            // Handle standalone pointers (non-array)
            if (pointerCount > 0)
            {
                return new PointerSymbol(id, elementType, line, pointerCount);
            }

            // Handle struct members (non-pointer, non-array)
            // Extract just the name without prefix for StructSymbol
            string typeName = elementType;
            if (elementType.StartsWith("struct_"))
            {
                typeName = elementType.Substring(7);
            }
            else if (elementType.StartsWith("union_"))
            {
                typeName = elementType.Substring(6);
            }
            
            if (heterogenousInfo.ContainsKey(typeName))
            {
                // Nested struct member
                return new StructSymbol(id, typeName, line, new Dictionary<string, Symbol>());
            }

            // Handle simple variable members
            return new Symbol(id, elementType, line);
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
                if (exprContext is ExprParser.VarContext varCtx)
                {
                    string varName = varCtx.ID().GetText();
                    baseSymbol = symbolTable.GetSymbol(varName);
                    
                    if (baseSymbol == null)
                    {
                        reportError(line, $"Variable '{varName}' not found");
                        return null;
                    }
                    
                    if (!baseSymbol.isInitializated)
                    {
                        reportError(line, $"Variable '{varName}' is used before being initialized");
                    }
                    
                    if (!(baseSymbol is PointerSymbol pointerSymbol))
                    {
                        reportError(line, $"Cannot dereference non-pointer variable '{varName}'");
                        return null;
                    }
                    
                    currentType = pointerSymbol.pointeeType;
                }
                else if (exprContext is ExprParser.VarStructContext varStructCtx)
                {
                    string? structMemberType = VisitStructGet(varStructCtx.structGet());
                    if (structMemberType == null) return null;
                    currentType = structMemberType;
                    
                    // If the type is a pointer (ends with *), we need to dereference it
                    if (currentType.EndsWith("*"))
                    {
                        currentType = currentType.Substring(0, currentType.Length - 1);
                        
                        // Add back the struct_ prefix if it's a struct type
                        if (heterogenousInfo.ContainsKey(currentType))
                        {
                            var hetInfo = heterogenousInfo[currentType];
                            if (hetInfo is StructInfo)
                            {
                                currentType = $"struct_{currentType}";
                            }
                            else if (hetInfo is UnionInfo)
                            {
                                currentType = $"union_{currentType}";
                            }
                        }
                    }
                }
                else if (exprContext is ExprParser.DerrefExprContext nestedDerrefCtx)
                {
                    string? innerType = VisitDerrefExpr(nestedDerrefCtx);
                    if (innerType == null) return null;
                    currentType = innerType;
                    
                    // If the type is a pointer (ends with *), we need to dereference it
                    if (currentType.EndsWith("*"))
                    {
                        currentType = currentType.Substring(0, currentType.Length - 1);
                        
                        // Add back the struct_ prefix if it's a struct type
                        if (heterogenousInfo.ContainsKey(currentType))
                        {
                            var hetInfo = heterogenousInfo[currentType];
                            if (hetInfo is StructInfo)
                            {
                                currentType = $"struct_{currentType}";
                            }
                            else if (hetInfo is UnionInfo)
                            {
                                currentType = $"union_{currentType}";
                            }
                        }
                    }
                }
                else
                {
                    reportError(line, "Complex expressions in dereference not yet supported");
                    return null;
                }
                
                // Clean up type name BEFORE checking if it's a struct
                if (currentType.StartsWith("struct_"))
                {
                    currentType = currentType.Substring(7);
                }
                else if (currentType.StartsWith("union_"))
                {
                    currentType = currentType.Substring(6);
                }
                
                if (!heterogenousInfo.ContainsKey(currentType))
                {
                    reportError(line, $"Cannot dereference pointer to non-struct type '{currentType}'");
                    return null;
                }
            }
            else
            {
                // Original code for regular struct access (ID.member or ID->member)
                string id = context.ID().GetText();

                baseSymbol = symbolTable.GetSymbol(id);
                if (baseSymbol == null)
                {
                    reportError(line, $"Variable '{id}' not found");
                    return null;
                }

                if (!baseSymbol.isInitializated)
                {
                    reportError(line, $"Variable '{id}' is used before being initialized");
                }

                currentType = baseSymbol.type;
                
                if (indexes.Length > 0)
                {
                    if (baseSymbol is ArraySymbol arraySymbol)
                    {
                        if (indexes.Length > arraySymbol.dimensions.Count)
                        {
                            reportError(line, $"Too many indices for array '{id}'. Expected {arraySymbol.dimensions.Count}, got {indexes.Length}");
                            return null;
                        }
                        currentType = arraySymbol.elementType;
                    }
                    else if (baseSymbol is PointerSymbol pointerSymbol)
                    {
                        // Handle pointer indexing 
                        currentType = pointerSymbol.pointeeType;
                        
                        if (pointerSymbol.pointerLevel > 1)
                        {
                            currentType += new string('*', pointerSymbol.pointerLevel - 1);
                        }
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
                
                // Clean up type name
                if (currentType.StartsWith("struct_"))
                {
                    currentType = currentType.Substring(7);
                }
                else if (currentType.StartsWith("union_"))
                {
                    currentType = currentType.Substring(6);
                }

                if (!heterogenousInfo.ContainsKey(currentType))
                {
                    string? varIdentifier = baseSymbol?.name ?? "dereferenced pointer";
                    reportError(line, $"Variable '{varIdentifier}' is not a struct or union type. Cannot access members.");
                    return null;
                }
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

                // If this is a pointer field and we're not using arrow operator, return pointer type
                if (!isArrowOp && fieldSymbol is PointerSymbol pointerFieldNested)
                {
                    string fullPointerType = pointerFieldNested.pointeeType;
                    if (pointerFieldNested.pointerLevel > 0)
                    {
                        fullPointerType += new string('*', pointerFieldNested.pointerLevel);
                    }
                    
                    // Clean up the struct_ prefix before returning
                    if (fullPointerType.StartsWith("struct_"))
                    {
                        fullPointerType = fullPointerType.Substring(7);
                    }
                    else if (fullPointerType.StartsWith("union_"))
                    {
                        fullPointerType = fullPointerType.Substring(6);
                    }
                    
                    return fullPointerType;
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
                if (fieldSymbol is PointerSymbol pointerField)
                {
                    // If it's a pointer, return the full pointer type
                    string pointerType = pointerField.pointeeType;
                    if (pointerField.pointerLevel > 0)
                    {
                        pointerType += new string('*', pointerField.pointerLevel);
                    }
                    return pointerType;
                }
                return fieldType;
            }
        }

        private string? VisitDerrefExpr(ExprParser.DerrefExprContext context)
        {
            var derrefNode = context.derref();
            if (derrefNode?.expr() == null) return null;
            
            var exprContext = derrefNode.expr();
            
            if (exprContext is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                var symbol = symbolTable.GetSymbol(varName);
                
                if (symbol is PointerSymbol pointerSymbol)
                {
                    return pointerSymbol.pointeeType;
                }
            }
            else if (exprContext is ExprParser.VarStructContext varStructCtx)
            {
                // Get the type from struct access
                string? structMemberType = VisitStructGet(varStructCtx.structGet());
                if (structMemberType == null) return null;
                
                // If the type ends with *, it's a pointer - remove one level of indirection
                if (structMemberType.EndsWith("*"))
                {
                    // Remove the trailing *
                    string dereferencedType = structMemberType.Substring(0, structMemberType.Length - 1);
                    
                    // If there are still more *, return with reduced pointer level
                    if (dereferencedType.EndsWith("*"))
                    {
                        return dereferencedType;
                    }
                    
                    // Otherwise, add back the struct_ prefix if it's a struct type
                    if (heterogenousInfo.ContainsKey(dereferencedType))
                    {
                        var hetInfo = heterogenousInfo[dereferencedType];
                        if (hetInfo is StructInfo)
                        {
                            return $"struct_{dereferencedType}";
                        }
                        else if (hetInfo is UnionInfo)
                        {
                            return $"union_{dereferencedType}";
                        }
                    }
                    
                    return dereferencedType;
                }
                
                return structMemberType;
            }
            else if (exprContext is ExprParser.DerrefExprContext nestedDerref)
            {
                string? innerType = VisitDerrefExpr(nestedDerref);
                if (innerType == null) return null;
                
                // If the type ends with *, remove one level of indirection
                if (innerType.EndsWith("*"))
                {
                    string dereferencedType = innerType.Substring(0, innerType.Length - 1);
                    
                    // If there are still more *, return with reduced pointer level
                    if (dereferencedType.EndsWith("*"))
                    {
                        return dereferencedType;
                    }
                    
                    // Otherwise, add back the struct_ prefix if it's a struct type
                    if (heterogenousInfo.ContainsKey(dereferencedType))
                    {
                        var hetInfo = heterogenousInfo[dereferencedType];
                        if (hetInfo is StructInfo)
                        {
                            return $"struct_{dereferencedType}";
                        }
                        else if (hetInfo is UnionInfo)
                        {
                            return $"union_{dereferencedType}";
                        }
                    }
                    
                    return dereferencedType;
                }
                
                return innerType;
            }
            
            return null;
        }
    }
}