using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.ControlFlow
{
    public class SwitchStatementCodeGenerator
    {
        private readonly Func<StringBuilder> getCurrentBody;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextRegister;
        private readonly Func<ExprParser.ExprContext, string?> visitExpression;
        private readonly Func<ExprParser.StmContext, string?> visitStatement;
        private int labelCounter = 0;

        private readonly Stack<string> activeSwitchMergeLabels = new Stack<string>();

        public SwitchStatementCodeGenerator(
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

        public string? VisitSwitchStatement([NotNull] ExprParser.SwitchStatementContext context)
        {
            var switchExpr = context.expr();
            var caseLabels = context.caseLabel();
            var defaultLabel = context.defaultLabel();

            int baseLabel = labelCounter++;
            string mergeLabel = $"switch_merge_{baseLabel}";

            // Track this switch's merge label for break statements
            activeSwitchMergeLabels.Push(mergeLabel);

            // Evaluate switch expression
            string? switchValue = visitExpression(switchExpr);
            if (switchValue == null)
            {
                activeSwitchMergeLabels.Pop();
                return null;
            }

            // Convert switch value to i32 if needed
            string switchValueI32 = ConvertToI32(switchValue);

            // Build switch cases list for LLVM switch instruction
            List<string> casePairs = new List<string>();
            Dictionary<string, string> caseLabelMap = new Dictionary<string, string>();

            // Generate labels for each case
            for (int i = 0; i < caseLabels.Length; i++)
            {
                var caseLabel = caseLabels[i];
                string caseValue = GetCaseValue(caseLabel);
                string caseLabelName = $"case_{baseLabel}_{i}";
                caseLabelMap[caseValue] = caseLabelName;
                casePairs.Add($"i32 {caseValue}, label %{caseLabelName}");
            }

            // Generate default label if present
            string defaultLabelName = defaultLabel != null ? $"default_{baseLabel}" : mergeLabel;

            // Generate LLVM switch instruction
            if (casePairs.Count > 0)
            {
                string casesList = string.Join(" ", casePairs);
                getCurrentBody().AppendLine($"  switch i32 {switchValueI32}, label %{defaultLabelName} [{casesList}]");
            }
            else
            {
                // No cases, just branch to default or merge
                getCurrentBody().AppendLine($"  br label %{defaultLabelName}");
            }

            // Generate case blocks
            for (int i = 0; i < caseLabels.Length; i++)
            {
                var caseLabel = caseLabels[i];
                string caseLabelName = $"case_{baseLabel}_{i}";

                getCurrentBody().AppendLine($"{caseLabelName}:");
                bool hasBreak = GenerateBlock(caseLabel.func_body(), mergeLabel);

                // If no break statement, fall through to next case or default
                if (!hasBreak)
                {
                    if (i < caseLabels.Length - 1)
                    {
                        // Fall through to next case
                        string nextCaseLabel = $"case_{baseLabel}_{i + 1}";
                        getCurrentBody().AppendLine($"  br label %{nextCaseLabel}");
                    }
                    else if (defaultLabel != null)
                    {
                        // Fall through to default
                        getCurrentBody().AppendLine($"  br label %{defaultLabelName}");
                    }
                    else
                    {
                        // Fall through to merge
                        getCurrentBody().AppendLine($"  br label %{mergeLabel}");
                    }
                }
            }

            // Generate default block if present
            if (defaultLabel != null)
            {
                getCurrentBody().AppendLine($"{defaultLabelName}:");
                bool hasBreak = GenerateBlock(defaultLabel.func_body(), mergeLabel);
                // Default always branches to merge (even if no break, though break is recommended)
                if (!hasBreak)
                {
                    getCurrentBody().AppendLine($"  br label %{mergeLabel}");
                }
            }

            // Generate merge label
            getCurrentBody().AppendLine($"{mergeLabel}:");

            // Remove from active switch stack
            activeSwitchMergeLabels.Pop();

            return null;
        }

        public bool IsInSwitch()
        {
            return activeSwitchMergeLabels.Count > 0;
        }

        public string? GetCurrentSwitchMergeLabel()
        {
            return activeSwitchMergeLabels.Count > 0 ? activeSwitchMergeLabels.Peek() : null;
        }

        private bool GenerateBlock(ExprParser.Func_bodyContext context, string mergeLabel)
        {
            var statements = context.stm();
            bool foundBreak = false;
            
            foreach (var stm in statements)
            {
                if (stm.BREAK() != null)
                {
                    getCurrentBody().AppendLine($"  br label %{mergeLabel}");
                    foundBreak = true;
                    break;
                }

                visitStatement(stm);
            }

            return foundBreak;
        }

        private string GetCaseValue(ExprParser.CaseLabelContext context)
        {
            var intToken = context.INT();
            var charToken = context.CHAR_LITERAL();

            if (intToken != null)
            {
                return intToken.GetText();
            }
            else if (charToken != null)
            {
                
                string charLiteral = charToken.GetText();
                char charValue = charLiteral[1];
                if (charLiteral[1] == '\\')
                {
                    switch (charLiteral[2])
                    {
                        case 'n': charValue = '\n'; break;
                        case 't': charValue = '\t'; break;
                        case 'r': charValue = '\r'; break;
                        case '0': charValue = '\0'; break;
                        case '\\': charValue = '\\'; break;
                        case '\'': charValue = '\''; break;
                        default: charValue = charLiteral[2]; break;
                    }
                }
                return ((int)charValue).ToString();
            }
            else
            {
                return "0";
            }
        }

        private string ConvertToI32(string value)
        {
            string currentType = GetExpressionType(value);

            if (currentType == "i32")
            {
                return value;
            }
            else if (currentType == "i1")
            {
                string convReg = nextRegister();
                getCurrentBody().AppendLine($"  {convReg} = zext i1 {value} to i32");
                registerTypes[convReg] = "i32";
                return convReg;
            }
            else if (currentType == "i8")
            {
                string convReg = nextRegister();
                getCurrentBody().AppendLine($"  {convReg} = zext i8 {value} to i32");
                registerTypes[convReg] = "i32";
                return convReg;
            }
            else
            {
                if (currentType == "double")
                {
                    string convReg = nextRegister();
                    getCurrentBody().AppendLine($"  {convReg} = fptosi double {value} to i32");
                    registerTypes[convReg] = "i32";
                    return convReg;
                }
                return value;
            }
        }

        private string GetExpressionType(string value)
        {
            if (!registerTypes.ContainsKey(value))
            {
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
                    registerTypes[value] = "i32";
                }
            }
            return registerTypes[value];
        }
    }
}

