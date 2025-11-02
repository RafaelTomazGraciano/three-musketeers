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
                    string? argType = getExpressionType(providedArgs[i]);
                    var (expectedType, expectedName, expectedPointerLevel) = functionInfo.parameters[i];

                    // Determine pointer level of provided argument
                    int argPointerLevel = GetArgumentPointerLevel(providedArgs[i]);

                    // Validate pointer level first
                    if (argPointerLevel != expectedPointerLevel)
                    {
                        string expectedTypeStr = $"{expectedType}{new string('*', expectedPointerLevel)}";
                        string gotTypeStr = argPointerLevel > 0
                            ? $"{argType}{new string('*', argPointerLevel)}"
                            : argType ?? "unknown";

                        reportError(line, $"Argument {i + 1} of function '{functionName}': expected '{expectedTypeStr}', but got '{gotTypeStr}'");
                        continue;
                    }

                    if (argType != expectedType && argType != null)
                    {
                        reportError(line, $"Argument {i + 1} of function '{functionName}': expected base type '{expectedType}', but got '{argType}'");
                    }
                }
            }
            return functionInfo.GetFullReturnType();
        }

        private int GetArgumentPointerLevel(ExprParser.ExprContext expr)
        {
            // Variable: check if it's a pointer
            if (expr is ExprParser.VarContext varCtx)
            {
                string varName = varCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

                if (symbol is PointerSymbol pointerSymbol)
                {
                    return pointerSymbol.pointerLevel;
                }

                return 0; // Not a pointer
            }

            // Address-of operator: increases pointer level by 1
            if (expr is ExprParser.ExprAddressContext addressCtx)
            {
                int innerLevel = GetArgumentPointerLevel(addressCtx.expr());
                return innerLevel + 1;
            }

            // Dereference operator: decreases pointer level by 1
            if (expr is ExprParser.DerrefExprContext derrefCtx)
            {
                // Extract the inner expression from derref
                var derrefNode = derrefCtx.derref();
                if (derrefNode?.expr() != null)
                {
                    int innerLevel = GetArgumentPointerLevel(derrefNode.expr());
                    return Math.Max(0, innerLevel - 1); // Can't go below 0
                }
                return 0;
            }

            // Array access: if it's a pointer array, reduces level
            if (expr is ExprParser.VarArrayContext varArrayCtx)
            {
                string varName = varArrayCtx.ID().GetText();
                Symbol? symbol = symbolTable.GetSymbol(varName);

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