using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Struct_Unions
{
    public class UnionSemanticAnalyzer
    {
        private readonly SymbolTable symbolTable;
        private readonly Dictionary<string, HeterogenousInfo> heterogenousInfo;
        private readonly Action<int, string> reportError;

        public UnionSemanticAnalyzer(
            SymbolTable symbolTable, 
            Dictionary<string, HeterogenousInfo> heterogenousInfo, 
            Action<int, string> reportError)
        {
            this.symbolTable = symbolTable;
            this.heterogenousInfo = heterogenousInfo;
            this.reportError = reportError;
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

            // Handle union members (non-pointer, non-array)
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
                // Nested struct/union member
                return new StructSymbol(id, typeName, line, new Dictionary<string, Symbol>());
            }

            // Handle simple variable members
            return new Symbol(id, elementType, line);
        }
    }
}