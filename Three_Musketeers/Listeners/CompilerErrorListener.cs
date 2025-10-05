using Antlr4.Runtime;
using System;
using System.IO;

namespace Three_Musketeers.Listeners{
    public class CompilerErrorListener : BaseErrorListener{

        public bool hasErrors {get; private set;} = false;

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol,
                int line, int charPositionInLine, string msg, RecognitionException e){
            
            hasErrors = true;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Syntax error Line {line}: {charPositionInLine}");
            Console.WriteLine($"  {msg}");

            if(offendingSymbol != null){
                Console.WriteLine($"Offending symbol: '{offendingSymbol.Text}'");
            }
            Console.ResetColor();
        }
    }
}