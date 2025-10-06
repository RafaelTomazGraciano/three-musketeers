using Antlr4.Runtime;
using Three_Musketeers.Visitors;
using Three_Musketeers.Listeners;
using Three_Musketeers.Grammar;
using System.Diagnostics;

namespace Three_Musketeers{
    public class Program
    {
        public static int Main()
        {
            string filePath = "Examples/code.3m";

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
                    return 1;
                }

                //Semantic Analysis
                var semanticAnalyzer = new SemanticAnalyzer();
                semanticAnalyzer.Visit(tree);

                if (semanticAnalyzer.hasErrors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Compilation failed due to semantic errors");
                    Console.ResetColor();
                    return 1;
                }

                //Intermediate Code generation 
                var codeGenerator = new CodeGenerator();
                var llvmCode = codeGenerator.Visit(tree);

                //genereate directory bin
                string outputDir = Path.Combine(Path.GetDirectoryName(filePath) ?? "", "bin");
                string baseFileName = Path.GetFileNameWithoutExtension(filePath);
                Directory.CreateDirectory(outputDir);

                //LLVM
                string outputPath = Path.Combine(outputDir, baseFileName + ".ll");
                string bytecodePath = Path.Combine(outputDir, baseFileName + ".bc");
                string optBytecodePath = Path.Combine(outputDir, baseFileName + "-opt.bc");
                string assemblyPath = Path.Combine(outputDir, baseFileName + ".s");
                string resultPath = Path.Combine(outputDir, baseFileName);
                
                File.WriteAllText(outputPath, llvmCode);

                Process.Start("llvm-as", $"{outputPath} -o {bytecodePath}").WaitForExit();
                Process.Start("opt", $"-O2 {bytecodePath} -o {optBytecodePath}").WaitForExit();
                Process.Start("llc", $"{optBytecodePath} -o {assemblyPath}").WaitForExit();
                Process.Start("gcc", $"{assemblyPath} -o {resultPath} -no-pie").WaitForExit();

                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInternal compiler error");
                Console.WriteLine($"   {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                return 1;
            }
        }
    }
}
