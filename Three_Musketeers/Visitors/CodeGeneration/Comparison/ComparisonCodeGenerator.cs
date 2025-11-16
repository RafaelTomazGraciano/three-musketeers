using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.Comparison
{
    public class ComparisonCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public ComparisonCodeGenerator(
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

        public string VisitComparison([NotNull] ExprParser.ComparisonContext context)
        {
            string leftValue = GetExpressionValue(context.expr(0));
            string rightValue = GetExpressionValue(context.expr(1));
            string op = context.GetChild(1).GetText();
            
            string leftType = GetExpressionType(leftValue);
            string rightType = GetExpressionType(rightValue);
            
            string resultReg = nextRegister();
            
            bool isPointerComparison = leftType.Contains('*') || rightType.Contains('*');
            
            if (isPointerComparison)
            {
                // Pointer comparison - use the pointer type
                string pointerType = leftType.Contains('*') ? leftType : rightType;
                
                // Convert integer literals to pointers if needed
                string leftForComparison = leftValue;
                string rightForComparison = rightValue;
                
                if (!leftType.Contains('*') && leftValue == "0")
                {
                    leftForComparison = "null";
                }
                
                if (!rightType.Contains('*') && rightValue == "0")
                {
                    rightForComparison = "null";
                }
                
                string llvmOp = op switch
                {
                    ">" => "icmp ugt",   
                    "<" => "icmp ult",   
                    ">=" => "icmp uge",
                    "<=" => "icmp ule",
                    "==" => "icmp eq",
                    "!=" => "icmp ne",
                    _ => "icmp eq"
                };
                
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} {pointerType} {leftForComparison}, {rightForComparison}");
            }
            else if (leftType == "double" || rightType == "double")
            {
                // Floating point comparison
                string leftConverted = ConvertToComparisonType(leftValue, leftType, "double");
                string rightConverted = ConvertToComparisonType(rightValue, rightType, "double");
                
                string llvmOp = op switch
                {
                    ">" => "fcmp ogt",
                    "<" => "fcmp olt",
                    ">=" => "fcmp oge",
                    "<=" => "fcmp ole",
                    "==" => "fcmp oeq",
                    "!=" => "fcmp one",
                    _ => "fcmp oeq"
                };
                
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} double {leftConverted}, {rightConverted}");
            }
            else
            {
                // Integer comparison
                string leftConverted = ConvertToComparisonType(leftValue, leftType, "i32");
                string rightConverted = ConvertToComparisonType(rightValue, rightType, "i32");
                
                string llvmOp = op switch
                {
                    ">" => "icmp sgt",
                    "<" => "icmp slt",
                    ">=" => "icmp sge",
                    "<=" => "icmp sle",
                    "==" => "icmp eq",
                    "!=" => "icmp ne",
                    _ => "icmp eq"
                };
                
                getCurrentBody().AppendLine($"  {resultReg} = {llvmOp} i32 {leftConverted}, {rightConverted}");
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

        private string ConvertToComparisonType(string value, string currentType, string targetType)
        {
            // If already the target type, return as is
            if (currentType == targetType)
            {
                return value;
            }
            
            // Convert to target type
            if (targetType == "double")
            {
                if (currentType == "i32" || currentType == "i1" || currentType == "i8")
                {
                    string convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = sitofp {currentType} {value} to double");
                    registerTypes[convReg] = "double";
                    return convReg;
                }
            }
            else if (targetType == "i32")
            {
                if (currentType == "i1")
                {
                    // Convert boolean to integer
                    string convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = zext i1 {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                else if (currentType == "i8")
                {
                    // Convert char to integer
                    string convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = sext i8 {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                else if (currentType == "double")
                {
                    // Convert double to integer
                    string convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = fptosi double {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
            }
            
            // If no conversion needed or supported, return as is
            return value;
        }
    }
}