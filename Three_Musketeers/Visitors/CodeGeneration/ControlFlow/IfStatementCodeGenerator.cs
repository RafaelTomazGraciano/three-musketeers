using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.ControlFlow
{
    public class IfStatementCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private int labelCounter = 0;

        public IfStatementCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression,
            Func<ExprParser.StmContext, string?> visitStatement)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.visitStatement = visitStatement;
        }

        public string? VisitIfStatement([NotNull] ExprParser.IfStatementContext context)
        {
            var expressions = context.expr();
            var bodies = context.func_body();
            var elseTokens = context.ELSE();

            int baseLabel = labelCounter++;
            string mergeLabel = $"merge_{baseLabel}";

            // Evaluate if condition
            string? ifConditionValue = visitExpression(expressions[0]);
            if (ifConditionValue == null)
            {
                return null;
            }

            string ifConditionBool = ConvertToBool(ifConditionValue);
            string ifLabel = $"if_{baseLabel}";

            // Determine if there are else if chains and/or final else
            int elseIfCount = expressions.Length - 1;
            // Final else exists if we have more bodies than expressions (each expression has a body, plus optional final else body)
            bool hasFinalElse = bodies.Length > expressions.Length;

            // Generate conditional branch from if to first else-if or else block
            if (elseIfCount > 0 || hasFinalElse)
            {
                string nextLabel = elseIfCount > 0 ? $"else_if_cond_{baseLabel}_0" : $"else_{baseLabel}";
                getCurrentBody().AppendLine($"  br i1 {ifConditionBool}, label %{ifLabel}, label %{nextLabel}");
            }
            else
            {
                // No else/else-if, branch directly to merge
                getCurrentBody().AppendLine($"  br i1 {ifConditionBool}, label %{ifLabel}, label %{mergeLabel}");
            }

            // Generate if block
            getCurrentBody().AppendLine($"{ifLabel}:");
            GenerateBlock(bodies[0]);
            getCurrentBody().AppendLine($"  br label %{mergeLabel}");

            // Generate else-if chains
            for (int i = 0; i < elseIfCount; i++)
            {
                string condLabel = $"else_if_cond_{baseLabel}_{i}";
                string blockLabel = $"else_if_{baseLabel}_{i}";

                // Generate condition check label first
                getCurrentBody().AppendLine($"{condLabel}:");

                // Now evaluate the condition in this basic block
                string? elseIfConditionValue = visitExpression(expressions[i + 1]);
                
                if (elseIfConditionValue == null)
                {
                    continue;
                }

                string elseIfConditionBool = ConvertToBool(elseIfConditionValue);

                // Branch to next else-if, else, or merge
                string nextLabel;
                if (i < elseIfCount - 1)
                {
                    nextLabel = $"else_if_cond_{baseLabel}_{i + 1}";
                }
                else if (hasFinalElse)
                {
                    nextLabel = $"else_{baseLabel}";
                }
                else
                {
                    nextLabel = mergeLabel;
                }

                getCurrentBody().AppendLine($"  br i1 {elseIfConditionBool}, label %{blockLabel}, label %{nextLabel}");

                // Generate else-if block
                getCurrentBody().AppendLine($"{blockLabel}:");
                GenerateBlock(bodies[i + 1]);
                getCurrentBody().AppendLine($"  br label %{mergeLabel}");
            }

            // Generate final else block (if present)
            if (hasFinalElse)
            {
                string elseLabel = $"else_{baseLabel}";
                getCurrentBody().AppendLine($"{elseLabel}:");
                GenerateBlock(bodies[bodies.Length - 1]);
                getCurrentBody().AppendLine($"  br label %{mergeLabel}");
            }

            // Generate merge label
            getCurrentBody().AppendLine($"{mergeLabel}:");

            return null;
        }

        private void GenerateBlock(ExprParser.Func_bodyContext context)
        {
            var statements = context.stm();
            foreach (var stm in statements)
            {
                visitStatement(stm);
            }
        }

        private string ConvertToBool(string value)
        {
            string currentType = GetExpressionType(value);
            
            if (currentType == "i1")
            {
                // Already boolean, return as is
                return value;
            }

            // Need to convert to boolean
            string convReg = nextRegister();

            if (currentType == "i32")
            {
                // Convert integer to boolean: != 0
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }
            else if (currentType == "double")
            {
                // Convert double to boolean: != 0.0
                getCurrentBody().AppendLine($"  {convReg} = fcmp one double {value}, 0.0");
            }
            else if (currentType == "i8")
            {
                // Convert char to boolean: != 0
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i8 {value}, 0");
            }
            else
            {
                // Default case - treat as integer
                getCurrentBody().AppendLine($"  {convReg} = icmp ne i32 {value}, 0");
            }

            registerTypes[convReg] = "i1";
            return convReg;
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
    }
}

