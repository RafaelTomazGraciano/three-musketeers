using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public class StringCodeGenerator
    {
        private readonly StringBuilder globalStrings;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextStringLabel;

        public StringCodeGenerator(
            StringBuilder globalStrings,
            Dictionary<string, string> registerTypes,
            Func<string> nextStringLabel)
        {
            this.globalStrings = globalStrings;
            this.registerTypes = registerTypes;
            this.nextStringLabel = nextStringLabel;
        }

        public string VisitStringLiteral([NotNull] ExprParser.StringLiteralContext context)
        {
            string rawString = context.STRING_LITERAL().GetText();
            string content = rawString.Substring(1, rawString.Length - 2);
            content = ProcessEscapeSequences(content);
            
            string strLabel = nextStringLabel();
            int strLen = content.Length + 1;
            
            globalStrings.AppendLine($"{strLabel} = private unnamed_addr constant [{strLen} x i8] c\"{content}\\00\", align 1");
            
            registerTypes[strLabel] = $"[{strLen} x i8]*";;
            
            return strLabel;
        }

        private string ProcessEscapeSequences(string str)
        {
            return str.Replace("\\n", "\\0A")
                      .Replace("\\t", "\\09")
                      .Replace("\\r", "\\0D")
                      .Replace("\\\"", "\\22")
                      .Replace("\\\\", "\\5C");
        }
    }
}