using System.Text;
using Three_Musketeers.Grammar;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.CodeGeneration.CompilerDirectives
{
    public class DefineCodeGenerator
    {
        private readonly StringBuilder globalStrings;
        private readonly Dictionary<string, string> defineValues;
        private readonly Dictionary<string, string> registerTypes;
        private readonly Func<string> nextStringLabel;

        public DefineCodeGenerator(
            StringBuilder globalStrings,
            Dictionary<string, string> defineValues,
            Dictionary<string, string> registerTypes,
            Func<string> nextStringLabel)
        {
            this.globalStrings = globalStrings;
            this.defineValues = defineValues;
            this.registerTypes = registerTypes;
            this.nextStringLabel = nextStringLabel;
        }

        public void ProcessDefine(ExprParser.DefineContext define)
        {
            string defineName = "";
            string value = "";

            if (define is ExprParser.DefineIntContext defInt)
            {
                defineName = defInt.ID().GetText();
                value = defInt.INT().GetText();
                defineValues[defineName] = value;
            }
            else if (define is ExprParser.DefineDoubleContext defDouble)
            {
                defineName = defDouble.ID().GetText();
                value = defDouble.DOUBLE().GetText();
                defineValues[defineName] = value;
            }
            else if (define is ExprParser.DefineStringContext defString)
            {
                defineName = defString.ID().GetText();
                value = defString.STRING_LITERAL().GetText();
                defineValues[defineName] = value;
            }
        }

        public string? ResolveDefine(string name)
        {
            if (!defineValues.ContainsKey(name))
            {
                return null;
            }

            string defineValue = defineValues[name];

            // String literal
            if (defineValue.StartsWith("\"") && defineValue.EndsWith("\""))
            {
                // Remove quotes
                string content = defineValue.Substring(1, defineValue.Length - 2);
                
                // Process escape sequences
                var (processedString, byteCount) = EscapeSequenceProcessor.Process(content);
                
                // Create unique label
                string label = nextStringLabel();
                int length = byteCount + 1;
                
                globalStrings.AppendLine($"{label} = private unnamed_addr constant [{length} x i8] c\"{processedString}\\00\", align 1");
                registerTypes[label] = "i8*";
                
                return label;
            }
            
            // Numeric literal
            if (defineValue.Contains("."))
            {
                registerTypes[defineValue] = "double";
            }
            else
            {
                registerTypes[defineValue] = "i32";
            }
            
            return defineValue;
        }

        public bool IsDefine(string name)
        {
            return defineValues.ContainsKey(name);
        }

        public string? GetDefineValue(string name)
        {
            return defineValues.ContainsKey(name) ? defineValues[name] : null;
        }

    }
}