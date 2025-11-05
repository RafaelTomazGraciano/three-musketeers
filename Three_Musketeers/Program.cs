using Antlr4.Runtime;
using System.Diagnostics;
using Three_Musketeers.Visitors;
using Three_Musketeers.Listeners;
using Three_Musketeers.Grammar;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Three_Musketeers {
    public class Program
    {

        private static readonly Argument<string> input = new("input")
            {
                Description = "File to be compiled",
            };

        private static readonly Option<string> output = new("-o", ["--out"])
        {
            Description = "Path of executable",
            DefaultValueFactory = value => "a.out"
        };

        public static int Main(string[] args)
        {
            RootCommand rootCommand = new("Three Musketeers Language Compiler");
            rootCommand.Add(input);
            rootCommand.Add(output);

            var result = rootCommand.Parse(args);
            if (result.Errors.Count > 0)
            {
                foreach (ParseError parseError in result.Errors)
                {
                    Console.Error.WriteLine(parseError.Message);
                }
                return 1;
            }
            string filePath = result.GetValue(input)!;
            string resultPath = result.GetValue(output)!;

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

                // Intermediate Code generation
                var codeGenerator = new CodeGenerator();
                var llvmCode = codeGenerator.Visit(tree);

                // Create bin directory
                string fileDir = Path.GetDirectoryName(Path.GetFullPath(filePath))!;
                string binDir = Path.Combine(fileDir, "bin");
                Directory.CreateDirectory(binDir);

                // Generate output paths inside bin directory
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string outputPath = Path.Combine(binDir, $"{fileName}.ll");
                string bytecodePath = Path.Combine(binDir, $"{fileName}.bc");
                string optBytecodePath = Path.Combine(binDir, $"{fileName}-opt.bc");
                string assemblyPath = Path.Combine(binDir, $"{fileName}.s");
                string executablePath = Path.Combine(binDir, fileName);

                File.WriteAllText(outputPath, llvmCode);
                Process.Start("llvm-as", $"{outputPath} -o {bytecodePath}").WaitForExit();
                Process.Start("opt", $"-O2 {bytecodePath} -o {optBytecodePath}").WaitForExit();
                Process.Start("llc", $"{optBytecodePath} -o {assemblyPath}").WaitForExit();
                Process.Start("gcc", $"{assemblyPath} -o {executablePath} -no-pie").WaitForExit();
                Process.Start("rm", $"{bytecodePath} {optBytecodePath} {assemblyPath}");
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
