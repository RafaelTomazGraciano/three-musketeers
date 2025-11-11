using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.Variables
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
            
            var (processedString, byteCount) = EscapeSequenceProcessor.Process(content);
            
            string strLabel = nextStringLabel();
            int strLen = byteCount + 1; // +1 for null terminator
            
            globalStrings.AppendLine($"{strLabel} = private unnamed_addr constant [{strLen} x i8] c\"{processedString}\\00\", align 1");
            
            registerTypes[strLabel] = "i8*";
            
            return strLabel;
        }
    }
}