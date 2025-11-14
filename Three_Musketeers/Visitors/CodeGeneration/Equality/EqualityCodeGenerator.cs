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
            
            // Determine comparison type FIRST
            string comparisonType = GetComparisonType(leftType, rightType);

            // Handle pointer comparisons
            if (comparisonType.Contains('*'))
            {
                // Convert 0 to null for pointer comparisons
                if (leftValue == "0" && !leftType.Contains('*'))
                    leftValue = "null";
                if (rightValue == "0" && !rightType.Contains('*'))
                    rightValue = "null";
                
                string resultRegister = nextRegister();
                string llvmOp = op == "==" ? "icmp eq" : "icmp ne";
                getCurrentBody().AppendLine($"  {resultRegister} = {llvmOp} {comparisonType} {leftValue}, {rightValue}");
                registerTypes[resultRegister] = "i1";
                return resultRegister;
            }   
            
            // Convert both operands to the same type for comparison
            string leftConverted = ConvertToType(leftValue, leftType, comparisonType);
            string rightConverted = ConvertToType(rightValue, rightType, comparisonType);
            
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

        private string ConvertToType(string value, string fromType, string toType)
        {
            if (fromType == toType)
                return value;

            string convReg = nextRegister();

            // From i1 (bool)
            if (fromType == "i1")
            {
                if (toType == "i32")
                {
                    getCurrentBody().AppendLine($"  {convReg} = zext i1 {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                else if (toType == "i8")
                {
                    getCurrentBody().AppendLine($"  {convReg} = zext i1 {value} to i8");
                    registerTypes[convReg] = "i8";
                    return convReg;
                }
                else if (toType == "double")
                {
                    string tempReg = nextRegister();
                    getCurrentBody().AppendLine($"  {tempReg} = zext i1 {value} to i32");
                    registerTypes[tempReg] = "i32";

                    convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {tempReg} to double");
                    registerTypes[convReg] = "double";
                    return convReg;
                }
            }
            // From i8 (char)
            else if (fromType == "i8")
            {
                if (toType == "i32")
                {
                    getCurrentBody().AppendLine($"  {convReg} = zext i8 {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                else if (toType == "double")
                {
                    string tempReg = nextRegister();
                    getCurrentBody().AppendLine($"  {tempReg} = zext i8 {value} to i32");
                    registerTypes[tempReg] = "i32";

                    convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {tempReg} to double");
                    registerTypes[convReg] = "double";
                    return convReg;
                }
            }
            // From i32 (int)
            else if (fromType == "i32")
            {
                if (toType == "i8")
                {
                    getCurrentBody().AppendLine($"  {convReg} = trunc i32 {value} to i8");
                    registerTypes[convReg] = "i8";
                    return convReg;
                }
                else if (toType == "double")
                {
                    getCurrentBody().AppendLine($"  {convReg} = sitofp i32 {value} to double");
                    registerTypes[convReg] = "double";
                    return convReg;
                }
            }
            // From double
            else if (fromType == "double")
            {
                if (toType == "i32")
                {
                    getCurrentBody().AppendLine($"  {convReg} = fptosi double {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                else if (toType == "i8")
                {
                    string tempReg = nextRegister();
                    getCurrentBody().AppendLine($"  {tempReg} = fptosi double {value} to i32");
                    registerTypes[tempReg] = "i32";

                    convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = trunc i32 {tempReg} to i8");
                    registerTypes[convReg] = "i8";
                    return convReg;
                }
            }

            return value;
        }

        private string GetComparisonType(string type1, string type2)
        {
            // Check for pointer types first
            if (type1.Contains('*') || type2.Contains('*'))
            {
                // Return the pointer type (preferably the one that has *)
                return type1.Contains('*') ? type1 : type2;
            }
            
            // Existing logic
            if (type1 == "double" || type2 == "double")
                return "double";
            if (type1 == "i8" || type2 == "i8")
                return "i8";
            return "i32";
        }
    }
}
