using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.ControlFlow
{
    public class LoopStatementCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private readonly Func<IParseTree, string?> visitContext;
        private int labelCounter = 0;

        // Track active loop contexts for break/continue statements
        private readonly Stack<LoopContext> activeLoopContexts = new Stack<LoopContext>();

        private class LoopContext
        {
            public string MergeLabel { get; set; } = "";
            public string ContinueLabel { get; set; } = ""; // For continue: where to branch (condition check or loop start)
        }

        public LoopStatementCodeGenerator(
            Func<StringBuilder> getCurrentBody,
            Dictionary<string, string> registerTypes,
            Func<string> nextRegister,
            Func<ExprParser.ExprContext, string?> visitExpression,
            Func<ExprParser.StmContext, string?> visitStatement,
            Func<IParseTree, string?> visitContext)
        {
            this.getCurrentBody = getCurrentBody;
            this.registerTypes = registerTypes;
            this.nextRegister = nextRegister;
            this.visitExpression = visitExpression;
            this.visitStatement = visitStatement;
            this.visitContext = visitContext;
        }

        public string? VisitForStatement([NotNull] ExprParser.ForStatementContext context)
        {
            var forInit = context.forInit();
            var forCondition = context.forCondition();
            var forIncrement = context.forIncrement();
            var body = context.func_body();

            int baseLabel = labelCounter++;
            string initLabel = $"for_init_{baseLabel}";
            string condLabel = $"for_cond_{baseLabel}";
            string bodyLabel = $"for_body_{baseLabel}";
            string incrLabel = $"for_incr_{baseLabel}";
            string mergeLabel = $"for_merge_{baseLabel}";

            // Track this loop's context for break/continue
            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = incrLabel // For continue, branch to increment block
            };
            activeLoopContexts.Push(loopContext);

            // Branch to init block if present, otherwise to condition check
            if (forInit != null)
            {
                getCurrentBody().AppendLine($"  br label %{initLabel}");
                // Generate init block
                getCurrentBody().AppendLine($"{initLabel}:");
                visitContext(forInit);
                // After init, branch to condition check
                getCurrentBody().AppendLine($"  br label %{condLabel}");
            }

            // Generate condition check
            getCurrentBody().AppendLine($"{condLabel}:");
            
            if (forCondition != null)
            {
                string? conditionValue = visitExpression(forCondition.expr());
                if (conditionValue == null)
                {
                    activeLoopContexts.Pop();
                    return null;
                }
                string conditionBool = ConvertToBool(conditionValue);
                getCurrentBody().AppendLine($"  br i1 {conditionBool}, label %{bodyLabel}, label %{mergeLabel}");
            }
            else
            {
                // No condition means infinite loop
                getCurrentBody().AppendLine($"  br label %{bodyLabel}");
            }

            // Generate loop body
            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, incrLabel);

            // If no break, branch to increment
            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{incrLabel}");
            }

            // Generate increment block (if present)
            getCurrentBody().AppendLine($"{incrLabel}:");
            if (forIncrement != null)
            {
                visitContext(forIncrement);
            }
            // Always branch back to condition check
            getCurrentBody().AppendLine($"  br label %{condLabel}");

            // Generate merge label
            getCurrentBody().AppendLine($"{mergeLabel}:");

            // Remove from active loop stack
            activeLoopContexts.Pop();

            return null;
        }

        public string? VisitWhileStatement([NotNull] ExprParser.WhileStatementContext context)
        {
            var condition = context.expr();
            var body = context.func_body();

            int baseLabel = labelCounter++;
            string condLabel = $"while_cond_{baseLabel}";
            string bodyLabel = $"while_body_{baseLabel}";
            string mergeLabel = $"while_merge_{baseLabel}";

            // Track this loop's context for break/continue
            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = condLabel // For continue, branch to condition check
            };
            activeLoopContexts.Push(loopContext);

            // Branch to condition check
            getCurrentBody().AppendLine($"  br label %{condLabel}");
            
            // Generate condition check
            getCurrentBody().AppendLine($"{condLabel}:");
            string? conditionValue = visitExpression(condition);
            if (conditionValue == null)
            {
                activeLoopContexts.Pop();
                return null;
            }
            string conditionBool = ConvertToBool(conditionValue);
            getCurrentBody().AppendLine($"  br i1 {conditionBool}, label %{bodyLabel}, label %{mergeLabel}");

            // Generate loop body
            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, condLabel);

            // If no break, branch back to condition check
            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{condLabel}");
            }

            // Generate merge label
            getCurrentBody().AppendLine($"{mergeLabel}:");

            // Remove from active loop stack
            activeLoopContexts.Pop();

            return null;
        }

        public string? VisitDoWhileStatement([NotNull] ExprParser.DoWhileStatementContext context)
        {
            var condition = context.expr();
            var body = context.func_body();

            int baseLabel = labelCounter++;
            string bodyLabel = $"dowhile_body_{baseLabel}";
            string condLabel = $"dowhile_cond_{baseLabel}";
            string mergeLabel = $"dowhile_merge_{baseLabel}";

            // Track this loop's context for break/continue
            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = condLabel // For continue, branch to condition check
            };
            activeLoopContexts.Push(loopContext);

            // Branch to loop body (executes at least once)
            getCurrentBody().AppendLine($"  br label %{bodyLabel}");
            
            // Generate loop body
            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, condLabel);

            // If no break, branch to condition check
            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{condLabel}");
            }

            // Generate condition check
            getCurrentBody().AppendLine($"{condLabel}:");
            string? conditionValue = visitExpression(condition);
            if (conditionValue == null)
            {
                activeLoopContexts.Pop();
                return null;
            }
            string conditionBool = ConvertToBool(conditionValue);
            getCurrentBody().AppendLine($"  br i1 {conditionBool}, label %{bodyLabel}, label %{mergeLabel}");

            // Generate merge label
            getCurrentBody().AppendLine($"{mergeLabel}:");

            // Remove from active loop stack
            activeLoopContexts.Pop();

            return null;
        }

        public bool IsInLoop()
        {
            return activeLoopContexts.Count > 0;
        }

        public string? GetCurrentLoopMergeLabel()
        {
            return activeLoopContexts.Count > 0 ? activeLoopContexts.Peek().MergeLabel : null;
        }

        public string? GetCurrentLoopContinueLabel()
        {
            return activeLoopContexts.Count > 0 ? activeLoopContexts.Peek().ContinueLabel : null;
        }

        private bool GenerateBlock(ExprParser.Func_bodyContext context, string breakLabel, string continueLabel)
        {
            var statements = context.stm();
            bool foundControlFlow = false; // Track if break or continue was found
            
            foreach (var stm in statements)
            {
                // Check if this is a break statement
                if (stm.BREAK() != null)
                {
                    // Generate branch to merge label and stop processing this block
                    getCurrentBody().AppendLine($"  br label %{breakLabel}");
                    foundControlFlow = true;
                    break; // Exit loop - break prevents further execution
                }

                // Check if this is a continue statement
                if (stm.CONTINUE() != null)
                {
                    // Generate branch to continue label and stop processing this block
                    getCurrentBody().AppendLine($"  br label %{continueLabel}");
                    foundControlFlow = true; // Continue also ends this block
                    break; // Exit loop - continue prevents further execution
                }

                visitStatement(stm);
            }

            return foundControlFlow;
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

