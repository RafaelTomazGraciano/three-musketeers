using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.Logical
{
    public class LogicalCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;

        public LogicalCodeGenerator(
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

        public string VisitLogicalAndOr([NotNull] ExprParser.LogicalAndOrContext context)
        {
            string leftValue = GetExpressionValue(context.expr(0));
            string rightValue = GetExpressionValue(context.expr(1));
            
            string leftType = GetExpressionType(leftValue);
            string rightType = GetExpressionType(rightValue);
            
            // Get the operation symbol
            string op = context.GetChild(1).GetText();
            
            // Convert both operands to boolean (i1)
            string leftBool = ConvertToBool(leftValue, leftType);
            string rightBool = ConvertToBool(rightValue, rightType);
            
            string resultReg = nextRegister();
            
            if (op == "&&")
            {
                // Logical AND
                getCurrentBody().AppendLine($"  {resultReg} = and i1 {leftBool}, {rightBool}");
            }
            else if (op == "||")
            {
                // Logical OR
                getCurrentBody().AppendLine($"  {resultReg} = or i1 {leftBool}, {rightBool}");
            }
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        public string VisitLogicalNot([NotNull] ExprParser.LogicalNotContext context)
        {
            string exprValue = GetExpressionValue(context.expr());
            string exprType = GetExpressionType(exprValue);
            
            // Convert operand to boolean (i1)
            string exprBool = ConvertToBool(exprValue, exprType);
            
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = xor i1 {exprBool}, 1");
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        private string GetExpressionValue(ExprParser.ExprContext context)
        {
            string? result = visitExpression(context);
            if (result == null)
            {
                // This shouldn't happen for expressions, but provide a fallback
                return "0";
            }
            return result;
        }

        private string GetExpressionType(string value)
        {
            if (!registerTypes.ContainsKey(value))
            {
                // For literals, determine the type based on the value
                if (value == "1" || value == "true")
                {
                    registerTypes[value] = "i1";
                }
                else if (value == "0" || value == "false")
                {
                    registerTypes[value] = "i1";
                }
                else
                {
                    // Default to i32 for numeric literals
                    registerTypes[value] = "i32";
                }
            }
            return registerTypes[value];
        }

        private string ConvertToBool(string value, string currentType)
        {
            if (currentType == "i1")
                return value;
            
            string convReg = nextRegister();
            
            if (currentType == "i32")
            {
                // Convert integer to boolean: != 0
                // icmp already returns i1, no need to extend
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }
            else if (currentType == "double")
            {
                // Convert double to boolean: != 0.0
                // fcmp already returns i1, no need to extend
                getCurrentBody().AppendLine($"  {convReg} = fcmp one double {value}, 0.0");
            }
            else if (currentType == "i8")
            {
                // Convert char to boolean: != 0
                // icmp already returns i1, no need to extend
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i8 {value}, 0");
            }
            else
            {
                // Default case - treat as integer
                // icmp already returns i1, no need to extend
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }
            
            registerTypes[convReg] = "i1";
            return convReg;
        }
    }
}