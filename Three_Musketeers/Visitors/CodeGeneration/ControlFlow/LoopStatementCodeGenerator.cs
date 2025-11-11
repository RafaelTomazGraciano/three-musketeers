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

        private readonly Stack<LoopContext> activeLoopContexts = new Stack<LoopContext>();

        private class LoopContext
        {
            public string MergeLabel { get; set; } = "";
            public string ContinueLabel { get; set; } = "";
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
            string condLabel = $"for_cond_{baseLabel}";
            string bodyLabel = $"for_body_{baseLabel}";
            string incrLabel = $"for_incr_{baseLabel}";
            string mergeLabel = $"for_merge_{baseLabel}";

            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = incrLabel
            };
            activeLoopContexts.Push(loopContext);

            if (forInit != null)
            {
                visitContext(forInit);
            }

            // Branch to condition
            getCurrentBody().AppendLine($"  br label %{condLabel}");

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
                getCurrentBody().AppendLine($"  br label %{bodyLabel}");
            }

            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, incrLabel);

            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{incrLabel}");
            }

            getCurrentBody().AppendLine($"{incrLabel}:");
            if (forIncrement != null)
            {
                visitContext(forIncrement);
            }
            getCurrentBody().AppendLine($"  br label %{condLabel}");

            getCurrentBody().AppendLine($"{mergeLabel}:");

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

            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = condLabel
            };
            activeLoopContexts.Push(loopContext);

            getCurrentBody().AppendLine($"  br label %{condLabel}");
            
            getCurrentBody().AppendLine($"{condLabel}:");
            string? conditionValue = visitExpression(condition);
            if (conditionValue == null)
            {
                activeLoopContexts.Pop();
                return null;
            }
            string conditionBool = ConvertToBool(conditionValue);
            getCurrentBody().AppendLine($"  br i1 {conditionBool}, label %{bodyLabel}, label %{mergeLabel}");

            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, condLabel);

            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{condLabel}");
            }

            getCurrentBody().AppendLine($"{mergeLabel}:");

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

            LoopContext loopContext = new LoopContext
            {
                MergeLabel = mergeLabel,
                ContinueLabel = condLabel
            };
            activeLoopContexts.Push(loopContext);

            getCurrentBody().AppendLine($"  br label %{bodyLabel}");
            
            getCurrentBody().AppendLine($"{bodyLabel}:");
            bool hasBreak = GenerateBlock(body, mergeLabel, condLabel);

            if (!hasBreak)
            {
                getCurrentBody().AppendLine($"  br label %{condLabel}");
            }

            getCurrentBody().AppendLine($"{condLabel}:");
            string? conditionValue = visitExpression(condition);
            if (conditionValue == null)
            {
                activeLoopContexts.Pop();
                return null;
            }
            string conditionBool = ConvertToBool(conditionValue);
            getCurrentBody().AppendLine($"  br i1 {conditionBool}, label %{bodyLabel}, label %{mergeLabel}");

            getCurrentBody().AppendLine($"{mergeLabel}:");

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
            bool foundControlFlow = false;
            
            foreach (var stm in statements)
            {
                if (stm.BREAK() != null)
                {
                    getCurrentBody().AppendLine($"  br label %{breakLabel}");
                    foundControlFlow = true;
                    break;
                }

                if (stm.CONTINUE() != null)
                {
                    getCurrentBody().AppendLine($"  br label %{continueLabel}");
                    foundControlFlow = true;
                    break;
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
                return value; 
            }

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

        private string GetExpressionType(string value)
        {
            if (!registerTypes.ContainsKey(value))
            {
                registerTypes[value] = "i32";
            }
            return registerTypes[value];
        }
    }
}

