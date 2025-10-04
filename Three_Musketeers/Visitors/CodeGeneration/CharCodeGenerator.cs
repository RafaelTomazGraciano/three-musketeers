using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration
{
    public class CharCodeGenerator
    {
        private readonly Dictionary<string, string> registerTypes;
        
        public CharCodeGenerator(Dictionary<string, string> registerTypes)
        {
            this.registerTypes = registerTypes;
        }

        public string VisitCharLiteral([NotNull] ExprParser.CharLiteralContext context)
        {
            string value = context.CHAR_LITERAL().GetText().Replace("'", "");
            registerTypes[value] = "i8";
            if (value.Contains('\\'))
            {
                return ProcessEscapeSequences(value);
            }

            Console.WriteLine($"{(int)value[0]} {value[0]}");
            return ((int)value[0]).ToString();
        }
        
        private string ProcessEscapeSequences(string str)
        {
            return str switch
            {
                "\\t" => "9",
                "\\n" => "10",
                "\\r" => "13",
                "\\\'" => "39",
                _ => "92" 
            };
        }
    }
}