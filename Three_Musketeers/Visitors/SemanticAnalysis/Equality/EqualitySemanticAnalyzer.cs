using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Equality
{
    public class EqualitySemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public EqualitySemanticAnalyzer(
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public string VisitEquality([NotNull] ExprParser.EqualityContext context)
        {
            var leftType = getExpressionType(context.expr(0));
            var rightType = getExpressionType(context.expr(1));
            
            // Visit both expressions to ensure they're valid
            visitExpression(context.expr(0));
            visitExpression(context.expr(1));
            
            // Equality operations can work with any types, but they should be compatible
            if (!AreCompatibleTypes(leftType, rightType))
            {
                reportError(context.Start.Line, 
                    $"Cannot compare incompatible types '{leftType}' and '{rightType}'");
                return "bool";
            }
            
            // Equality operations always return bool
            return "bool";
        }

        private bool AreCompatibleTypes(string type1, string type2)
        {
            // All types can be compared for equality
            // Numeric types can be compared with each other
            if (IsNumericType(type1) && IsNumericType(type2))
                return true;
            
            // Same types can always be compared
            if (type1 == type2)
                return true;
            
            // String and char can be compared
            if ((type1 == "string" && type2 == "char") || (type1 == "char" && type2 == "string"))
                return true;
            
            // Bool can be compared with numeric types (0/1)
            if ((type1 == "bool" && IsNumericType(type2)) || (type2 == "bool" && IsNumericType(type1)))
                return true;
            
            return false;
        }

        private bool IsNumericType(string type)
        {
            return type == "int" || type == "double" || type == "char";
        }
    }
}
