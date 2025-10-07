using Antlr4.Runtime;
using System;
using System.IO;

namespace Three_Musketeers.Listeners
{
    public class CompilerErrorListener : BaseErrorListener
    {
        public bool hasErrors { get; private set; } = false;

        public override void SyntaxError(
            TextWriter output, 
            IRecognizer recognizer, 
            IToken offendingSymbol,
            int line, 
            int charPositionInLine, 
            string msg, 
            RecognitionException e)
        {
            hasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;

            if (offendingSymbol != null && 
                offendingSymbol.Type == TokenConstants.EOF && 
                msg.Contains("expecting") && 
                (msg.Contains("'int'") || msg.Contains("mainFunction")))
            {
                Console.WriteLine($"\n[SYNTAX ERROR] Line {line}");
                Console.WriteLine("  Missing 'main' function");
                Console.WriteLine();
                Console.WriteLine("  Every program must have a main function with one of these signatures:");
                Console.WriteLine("    int main() {");
                Console.WriteLine("        // your code here");
                Console.WriteLine("    }");
                Console.WriteLine();
                Console.WriteLine("  or");
                Console.WriteLine();
                Console.WriteLine("    int main(int argc, char argv[]) {");
                Console.WriteLine("        // your code here");
                Console.WriteLine("    }\n");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nSyntax error Line {line}: {charPositionInLine}");
            Console.WriteLine($"  {msg}");
            
            if (offendingSymbol != null)
            {
                Console.WriteLine($"Offending symbol: '{offendingSymbol.Text}'");
            }
            
            Console.ResetColor();
        }
    }
}