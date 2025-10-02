using Antlr4.Runtime;
using Three_Musketeers.Visitors;
using Three_Musketeers.Listeners;
using Three_Musketeers.Grammar;

namespace Three_Musketeers{
    public class Program
    {
        public static void Main()
        {
            string filePath = "Examples/code.m3";

            try
            {
                // Lexical Analysis
                var inputStream = new AntlrFileStream(filePath);
                var lexer = new ExprLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                // Syntax Analysis
                var parser = new ExprParser(tokenStream);
                //Listeners
                parser.RemoveErrorListeners();
                var errorListener = new CompilerErrorListener();
                parser.AddErrorListener(errorListener);
                //AST
                var tree = parser.start();

                if (errorListener.hasErrors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Compilation failed due to syntax errors");
                    Console.ResetColor();
                    return;
                }

                //Semantic Analysis
                var semanticAnalyzer = new SemanticAnalyzer();
                semanticAnalyzer.Visit(tree);

                if (semanticAnalyzer.hasErrors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Compilation failed due to semantic errors");
                    Console.ResetColor();
                    return;
                }

                //Intermediate Code generation
                var codeGenerator = new CodeGenerator();
                var llvmCode = codeGenerator.Visit(tree);

                string outputPath = Path.ChangeExtension(filePath, ".ll");
                File.WriteAllText(outputPath, llvmCode);

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInternal compiler error");
                Console.WriteLine($"   {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
    }
}
