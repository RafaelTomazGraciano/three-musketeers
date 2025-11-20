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
        private int labelCounter = 0;

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
            string op = context.GetChild(1).GetText();
            
            if (op == "&&")
            {
                return VisitLogicalAnd(context);
            }
            else if (op == "||")
            {
                return VisitLogicalOr(context);
            }
            
            // Fallback (should not happen)
            return VisitLogicalAndSimple(context);
        }

        private string VisitLogicalAnd([NotNull] ExprParser.LogicalAndOrContext context)
        {
            int baseLabel = labelCounter++;
            string rhsLabel = $"and_rhs_{baseLabel}";
            string mergeLabel = $"and_merge_{baseLabel}";
            
            // Get current block label BEFORE evaluating left expression
            string leftBlockLabel = GetCurrentBlockLabel();
            
            // Evaluate left side
            string leftValue = GetExpressionValue(context.expr(0));
            string leftType = GetExpressionType(leftValue);
            string leftBool = ConvertToBool(leftValue, leftType);
            
            // Get the actual block we're in after evaluating left (might have changed due to nested operations)
            string actualLeftBlock = GetCurrentBlockLabel();
            
            // Short-circuit: if left is false, jump to merge
            // If left is true, evaluate right side
            getCurrentBody().AppendLine($"  br i1 {leftBool}, label %{rhsLabel}, label %{mergeLabel}");
            
            // Right-hand side block
            getCurrentBody().AppendLine($"{rhsLabel}:");
            string rightValue = GetExpressionValue(context.expr(1));
            string rightType = GetExpressionType(rightValue);
            string rightBool = ConvertToBool(rightValue, rightType);
            
            // Get the actual block we're in after evaluating right (for nested operations)
            string actualRightBlock = GetCurrentBlockLabel();
            
            getCurrentBody().AppendLine($"  br label %{mergeLabel}");
            
            // Merge block with PHI node
            getCurrentBody().AppendLine($"{mergeLabel}:");
            string resultReg = nextRegister();

            // PHI node should reference the actual predecessor blocks
            getCurrentBody().AppendLine($"  {resultReg} = phi i1 [ false, %{actualLeftBlock} ], [ {rightBool}, %{actualRightBlock} ]");
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        private string VisitLogicalOr([NotNull] ExprParser.LogicalAndOrContext context)
        {
            int baseLabel = labelCounter++;
            string rhsLabel = $"or_rhs_{baseLabel}";
            string mergeLabel = $"or_merge_{baseLabel}";
            
            // Get current block label BEFORE evaluating left expression
            string leftBlockLabel = GetCurrentBlockLabel();
            
            // Evaluate left side
            string leftValue = GetExpressionValue(context.expr(0));
            string leftType = GetExpressionType(leftValue);
            string leftBool = ConvertToBool(leftValue, leftType);
            
            // Get the actual block we're in after evaluating left (might have changed due to nested operations)
            string actualLeftBlock = GetCurrentBlockLabel();
            
            // Short-circuit: if left is true, jump to merge
            // If left is false, evaluate right side
            getCurrentBody().AppendLine($"  br i1 {leftBool}, label %{mergeLabel}, label %{rhsLabel}");
            
            // Right-hand side block
            getCurrentBody().AppendLine($"{rhsLabel}:");
            string rightValue = GetExpressionValue(context.expr(1));
            string rightType = GetExpressionType(rightValue);
            string rightBool = ConvertToBool(rightValue, rightType);
            
            // Get the actual block we're in after evaluating right (for nested operations)
            string actualRightBlock = GetCurrentBlockLabel();
            
            getCurrentBody().AppendLine($"  br label %{mergeLabel}");
            
            // Merge block with PHI node
            getCurrentBody().AppendLine($"{mergeLabel}:");
            string resultReg = nextRegister();
            
            // PHI node should reference the actual predecessor blocks
            getCurrentBody().AppendLine($"  {resultReg} = phi i1 [ true, %{actualLeftBlock} ], [ {rightBool}, %{actualRightBlock} ]");
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        private string VisitLogicalAndSimple([NotNull] ExprParser.LogicalAndOrContext context)
        {
            string leftValue = GetExpressionValue(context.expr(0));
            string rightValue = GetExpressionValue(context.expr(1));
            
            string leftType = GetExpressionType(leftValue);
            string rightType = GetExpressionType(rightValue);
            
            string op = context.GetChild(1).GetText();
            
            string leftBool = ConvertToBool(leftValue, leftType);
            string rightBool = ConvertToBool(rightValue, rightType);
            
            string resultReg = nextRegister();
            
            if (op == "&&")
            {
                getCurrentBody().AppendLine($"  {resultReg} = and i1 {leftBool}, {rightBool}");
            }
            else if (op == "||")
            {
                getCurrentBody().AppendLine($"  {resultReg} = or i1 {leftBool}, {rightBool}");
            }
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        public string VisitLogicalNot([NotNull] ExprParser.LogicalNotContext context)
        {
            string exprValue = GetExpressionValue(context.expr());
            string exprType = GetExpressionType(exprValue);
            
            string exprBool = ConvertToBool(exprValue, exprType);
            
            string resultReg = nextRegister();
            getCurrentBody().AppendLine($"  {resultReg} = xor i1 {exprBool}, 1");
            
            registerTypes[resultReg] = "i1";
            return resultReg;
        }

        private string GetCurrentBlockLabel()
        {
            string currentBody = getCurrentBody().ToString();
            string[] lines = currentBody.Split('\n');
            
            // Search backwards for the most recent label
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                // Check if this is a label (ends with : and doesn't start with %)
                if (line.EndsWith(":") && !line.StartsWith("%"))
                {
                    return line.TrimEnd(':');
                }
            }
            
            return "entry";
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

        private string ConvertToBool(string value, string currentType)
        {
            if (currentType == "i1")
                return value;
            
            string convReg = nextRegister();
            
            if (currentType == "i32")
            {
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }
            else if (currentType == "double")
            {
                getCurrentBody().AppendLine($"  {convReg} = fcmp one double {value}, 0.0");
            }
            else if (currentType == "i8")
            {
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i8 {value}, 0");
            }
            else
            {
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }
            
            registerTypes[convReg] = "i1";
            return convReg;
        }
    }
}