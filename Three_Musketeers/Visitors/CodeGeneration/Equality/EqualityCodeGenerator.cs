using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.Equality
{
    public class EqualityCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public EqualityCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
        }

        public string VisitEquality([NotNull] ExprParser.EqualityContext context)
        {
            string leftValue = GetExpressionValue(context.expr(0));
            string rightValue = GetExpressionValue(context.expr(1));
            
            string leftType = GetExpressionType(leftValue);
            string rightType = GetExpressionType(rightValue);
            
            // Get the operation symbol
            string op = context.GetChild(1).GetText();
            
            // Convert both operands to the same type for comparison
            string leftConverted = ConvertToComparisonType(leftValue, leftType);
            string rightConverted = ConvertToComparisonType(rightValue, rightType);
            string comparisonType = GetComparisonType(leftType, rightType);
            
            string resultReg = nextRegister();
            
            if (op == "==")
            {
                // Equality comparison
                if (comparisonType == "double")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = fcmp oeq double {leftConverted}, {rightConverted}");
                }
                else if (comparisonType == "i32")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp eq i32 {leftConverted}, {rightConverted}");
                }
                else if (comparisonType == "i8")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp eq i8 {leftConverted}, {rightConverted}");
                }
                else
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp eq i32 {leftConverted}, {rightConverted}");
                }
            }
            else if (op == "!=")
            {
                // Inequality comparison
                if (comparisonType == "double")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = fcmp one double {leftConverted}, {rightConverted}");
                }
                else if (comparisonType == "i32")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp ne i32 {leftConverted}, {rightConverted}");
                }
                else if (comparisonType == "i8")
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp ne i8 {leftConverted}, {rightConverted}");
                }
                else
                {
                    getCurrentBody().AppendLine($"  {resultReg} = icmp ne i32 {leftConverted}, {rightConverted}");
                }
            }
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        private string GetExpressionValue(ExprParser.ExprContext context)
        {
            string? result = visitExpression(context);
            if (result == null)
            {
                return "0";
            }
            return result;
        }

        private string GetExpressionType(string value)
        {
            if (!registerTypes.ContainsKey(value))
            {
                registerTypes[value] = "i32";
            }
            return registerTypes[value];
        }

        private string ConvertToComparisonType(string value, string currentType)
        {
            if (currentType == "i1")
            {
                // Convert boolean to integer for comparison
                string convReg = nextRegister();
                getCurrentBody().AppendLine($"  {convReg} = zext i1 {value} to i32");
                registerTypes[convReg] = "i32";
                return convReg;
            }
            else if (currentType == "i8")
            {
                // Convert char to integer for comparison
                string convReg = nextRegister();
                getCurrentBody().AppendLine($"  {convReg} = zext i8 {value} to i32");
                registerTypes[convReg] = "i32";
                return convReg;
            }
            else if (currentType == "double")
            {
                // For double, keep as double
                return value;
            }
            else
            {
                // For i32, return as is
                return value;
            }
        }

        private string GetComparisonType(string type1, string type2)
        {
            // If either is double, use double comparison
            if (type1 == "double" || type2 == "double")
                return "double";
            
            // If either is char, use i8 comparison
            if (type1 == "i8" || type2 == "i8")
                return "i8";
            
            // Default to i32
            return "i32";
        }
    }
}
