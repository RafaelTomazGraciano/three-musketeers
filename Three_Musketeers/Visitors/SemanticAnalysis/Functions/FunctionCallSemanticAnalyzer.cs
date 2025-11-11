using Three_Musketeers.Grammar;
using Three_Musketeers.Models;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Functions
{
    public class FunctionCallSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Dictionary<string, FunctionInfo> declaredFunctions;
        private readonly Func<ExprParser.ExprContext, string?> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly SymbolTable symbolTable;

        public FunctionCallSemanticAnalyzer(
            Action<int, string> reportError,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Func<ExprParser.ExprContext, string?> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression,
            SymbolTable symbolTable)
        {
            this.reportError = reportError;
            this.declaredFunctions = declaredFunctions;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
            this.symbolTable = symbolTable;
        }

        public string? VisitFunctionCall(ExprParser.FunctionCallContext context)
        {
            int line = context.Start.Line;
            string functionName = context.ID().GetText();

            if (!declaredFunctions.ContainsKey(functionName))
            {
                reportError(line, $"Function '{functionName}' has not been declared");
                return null;
            }

            FunctionInfo functionInfo = declaredFunctions[functionName];
            var providedArgs = context.expr();
            int providedCount = providedArgs?.Length ?? 0;
            int expectedCount = functionInfo.parameters?.Count ?? 0;

            if (providedCount != expectedCount)
            {
                reportError(line, $"Function '{functionName}' expects {expectedCount} arguments, but {providedCount} were provided");
                return functionInfo.GetFullReturnType();
            }

            //validate each argument
            if (providedArgs != null && functionInfo.parameters != null)
            {
                for (int i = 0; i < providedArgs.Length; i++)
                {
                    visitExpression(providedArgs[i]);

                    var (expectedType, expectedName, expectedPointerLevel) = functionInfo.parameters[i];

                    // Determine pointer level and base type of provided argument
                    int argPointerLevel = GetArgumentPointerLevel(providedArgs[i]);
                    string? argBaseType = GetArgumentBaseType(providedArgs[i]);

                    // Validate pointer level first
                    if (argPointerLevel != expectedPointerLevel)
                    {
                        string expectedTypeStr = expectedPointerLevel > 0
                            ? $"{expectedType}{new string('*', expectedPointerLevel)}"
                            : expectedType;
                        string gotTypeStr = argPointerLevel > 0
                            ? $"{argBaseType ?? "unknown"}{new string('*', argPointerLevel)}"
                            : argBaseType ?? "unknown";

                        reportError(line, $"Argument {i + 1} of function '{functionName}': expected '{expectedTypeStr}', but got '{gotTypeStr}'");
                        continue;
                    }

                    // Validate base type
                    if (argBaseType != expectedType && argBaseType != null)
                    {
                        string expectedTypeStr = expectedPointerLevel > 0
                            ? $"{expectedType}{new string('*', expectedPointerLevel)}"
                            : expectedType;
                        string gotTypeStr = argPointerLevel > 0
                            ? $"{argBaseType}{new string('*', argPointerLevel)}"
                            : argBaseType;

                        reportError(line, $"Argument {i + 1} of function '{functionName}': expected '{expectedTypeStr}', but got '{gotTypeStr}'");
                    }
                }
            }
            return functionInfo.GetFullReturnType();
        }

        private string? GetArgumentBaseType(ExprParser.ExprContext expr)
        {
            // Variable: get its base type
            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol is PointerSymbol pointerSymbol)
                {
                    return pointerSymbol.pointeeType;
                }

                if (symbol is ArraySymbol arraySymbol)
                {
                    return arraySymbol.elementType;
                }

                return symbol?.type;
            }

            // Address-of operator: get type of inner expression
            if (expr is ExprParser.ExprAddressContext addressCtx)
            {
                if (addressCtx.expr() is ExprParser.VarArrayContext arrayAccess)
                {
                    string varName = arrayAccess.ID().GetText();
                    Symbol? symbol = symbolTable.GetSymbol(varName);

                    if (symbol is ArraySymbol arraySymbol)
                    {
                        return arraySymbol.elementType;
                    }
                }

                return GetArgumentBaseType(addressCtx.expr());
            }

            // Dereference operator: get type of what's pointed to
            if (expr is ExprParser.DerrefExprContext derrefCtx)
            {
                var derrefNode = derrefCtx.derref();
                if (derrefNode?.expr() != null)
                {
                    return GetArgumentBaseType(derrefNode.expr());
                }
                return null;
            }

            // Array access: get element type
            if (expr is ExprParser.VarArrayContext varArrayCtx)
            {
                string varName = varArrayCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol is ArraySymbol arraySymbol)
                {
                    return arraySymbol.elementType;
                }

                if (symbol is PointerSymbol pointerSymbol)
                {
                    return pointerSymbol.pointeeType;
                }

                return symbol?.type;
            }

            // Function call: check return type
            if (expr is ExprParser.FunctionCallContext funcCallCtx)
            {
                string funcName = funcCallCtx.ID().GetText();
                if (declaredFunctions.ContainsKey(funcName))
                {
                    return declaredFunctions[funcName].returnType;
                }
                return null;
            }

            // Literal or expression: use getExpressionType
            return getExpressionType(expr);
        }

        private int GetArgumentPointerLevel(ExprParser.ExprContext expr)
        {
            // Variable: check if it's a pointer or array
            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol is PointerSymbol pointerSymbol)
                {
                    return pointerSymbol.pointerLevel;
                }

                // Arrays decay to pointers when passed to functions
                if (symbol is ArraySymbol arraySymbol)
                {
                    // Array passed to function = pointer to first element
                    return arraySymbol.pointerLevel > 0 ? arraySymbol.pointerLevel : 1;
                }

                return 0; // Not a pointer
            }

            // Address-of operator: increases pointer level by 1
            if (expr is ExprParser.ExprAddressContext addressCtx)
            {
                // Special case: &arr[i] where arr is an array
                if (addressCtx.expr() is ExprParser.VarArrayContext arrayAccess)
                {
                    string varName = arrayAccess.ID().GetText();
                    Symbol? symbol = symbolTable.GetSymbol(varName);

                    if (symbol is ArraySymbol)
                    {
                        // &arr[i] gives pointer to element
                        return 1;
                    }
                }

                int innerLevel = GetArgumentPointerLevel(addressCtx.expr());
                return innerLevel + 1;
            }

            // Dereference operator: decreases pointer level by 1
            if (expr is ExprParser.DerrefExprContext derrefCtx)
            {
                var derrefNode = derrefCtx.derref();
                if (derrefNode?.expr() != null)
                {
                    int innerLevel = GetArgumentPointerLevel(derrefNode.expr());
                    return Math.Max(0, innerLevel - 1);
                }
                return 0;
            }

            // Array access: arr[i] where arr is array or pointer
            if (expr is ExprParser.VarArrayContext varArrayCtx)
            {
                string varName = varArrayCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                // Array element access returns the element, not a pointer
                if (symbol is ArraySymbol)
                {
                    return 0; // arr[i] is not a pointer, it's an element
                }

                // Pointer array access: ptr[i]
                if (symbol is PointerSymbol pointerSymbol)
                {
                    return Math.Max(0, pointerSymbol.pointerLevel - 1);
                }

                return 0;
            }

            // Function call: check return type
            if (expr is ExprParser.FunctionCallContext funcCallCtx)
            {
                string funcName = funcCallCtx.ID().GetText();
                if (declaredFunctions.ContainsKey(funcName))
                {
                    return declaredFunctions[funcName].returnPointerLevel;
                }
                return 0;
            }

            // Default: not a pointer
            return 0;
        }

    }
}