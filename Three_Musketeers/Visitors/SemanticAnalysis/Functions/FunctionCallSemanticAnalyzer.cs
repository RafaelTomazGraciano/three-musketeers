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

        public FunctionCallSemanticAnalyzer(
            Action<int, string> reportError,
            Dictionary<string, FunctionInfo> declaredFunctions,
            Func<ExprParser.ExprContext, string?> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.declaredFunctions = declaredFunctions;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
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

            //check argument count
            if (providedArgs != null && functionInfo.parameters != null)
            {
                for (int i = 0; i < providedArgs.Length; i++)
                {
                    visitExpression(providedArgs[i]);
                    string? argType = getExpressionType(providedArgs[i]);
                    string expectedType = functionInfo.parameters[i].Type;

                    if (argType != expectedType)
                    {
                        reportError(line, $"Argument {i + 1} of function '{functionName}': expected '{expectedType}', but got '{argType}'");
                    }
                }
            }
            return functionInfo.GetFullReturnType();
        }

    }
}