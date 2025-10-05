using Antlr4.Runtime.Misc;
using System;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.SemanticAnalysis.Arithmetic
{
    public class ArithmeticSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Func<ExprParser.ExprContext, string> getExpressionType;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public ArithmeticSemanticAnalyzer(
            Action<int, string> reportError,
            Func<ExprParser.ExprContext, string> getExpressionType,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.reportError = reportError;
            this.getExpressionType = getExpressionType;
            this.visitExpression = visitExpression;
        }

        public string VisitAddSub([NotNull] ExprParser.AddSubContext context)
        {
            var leftType = getExpressionType(context.expr(0));
            var rightType = getExpressionType(context.expr(1));
            
            // Visit both expressions to ensure they're valid
            visitExpression(context.expr(0));
            visitExpression(context.expr(1));
            
            // Type checking for arithmetic operations
            if (!IsValidArithmeticType(leftType) || !IsValidArithmeticType(rightType))
            {
                reportError(context.Start.Line, 
                    $"Arithmetic operations require numeric types, got '{leftType}' and '{rightType}'");
                return "int"; // Return default type instead of null
            }
            
            // Return the promoted type (double if either operand is double, otherwise int)
            return PromoteTypes(leftType, rightType);
        }

        public string VisitMulDivMod([NotNull] ExprParser.MulDivModContext context)
        {
            var leftType = getExpressionType(context.expr(0));
            var rightType = getExpressionType(context.expr(1));
            
            // Visit both expressions to ensure they're valid
            visitExpression(context.expr(0));
            visitExpression(context.expr(1));
            
            // Get the operation symbol
            string op = context.GetChild(1).GetText();
            
            // Modulo operation only works with integers
            if (op == "%")
            {
                if (!IsValidIntegerType(leftType) || !IsValidIntegerType(rightType))
                {
                    reportError(context.Start.Line, 
                        $"Modulo operation requires integer types, got '{leftType}' and '{rightType}'");
                    return "int";
                }
                return "int"; // Modulo always returns int
            }
            
            // For multiplication and division, use the same logic as before
            if (!IsValidArithmeticType(leftType) || !IsValidArithmeticType(rightType))
            {
                reportError(context.Start.Line, 
                    $"Arithmetic operations require numeric types, got '{leftType}' and '{rightType}'");
                return "int";
            }
            
            // Return the promoted type (double if either operand is double, otherwise int)
            return PromoteTypes(leftType, rightType);
        }

        private bool IsValidArithmeticType(string type)
        {
            return type == "int" || type == "double" || type == "char" || type == "bool";
        }

        private bool IsValidIntegerType(string type)
        {
            return type == "int" || type == "char" || type == "bool";
        }

        private string PromoteTypes(string type1, string type2)
        {
            // If either is double, result is double
            if (type1 == "double" || type2 == "double")
                return "double";
            
            // Otherwise, result is int
            return "int";
        }
    }
}