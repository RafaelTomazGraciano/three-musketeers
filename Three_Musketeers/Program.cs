using Antlr4.Runtime;
using System.Diagnostics;
using Three_Musketeers.Visitors;
using Three_Musketeers.Listeners;
using Three_Musketeers.Grammar;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Three_Musketeers 
{
    public class Program
    {
        public static readonly string VERSION = "v0.9.5";
        private static readonly CliArgument<string> input = new("input") {
            Description = "Path of the source file to compile",
            Arity = ArgumentArity.ZeroOrOne
        };

        private static readonly CliOption<string> output = new("-o", ["--out"])
        {
            DefaultValueFactory = (res) => "a.out",
        };

        private static readonly CliOption<uint> compilerOptimazionLevel = new("-O", ["--opt"])
        {
            DefaultValueFactory = (res) => 2,
            Description = "Optimization level (0-3)",
        };

        private static readonly CliOption<string> dotLLCodePath = new("--ll")
        {
            DefaultValueFactory = (res) => "",
            Description = "Path of generated LLVM IR code"
        };

        private static readonly CliOption<bool> addDebugFlagToGcc = new("-g")
        {
            DefaultValueFactory = (res) => false,
            Description = "Add debug information to the generated code"
        };

        private static readonly CliOption<string> includeLibrary = new("-I", [ "--Include"])
        {
            DefaultValueFactory = (res) => "",
            Description = "Include path libraries"
        };

        public static int Main(string[] args)
        {
            var rootCommand = new CliRootCommand("Three Musketeers Language Compiler")
            {
                input,
                output,
                compilerOptimazionLevel,
                dotLLCodePath,
                addDebugFlagToGcc,
                includeLibrary
            }!;

            foreach (var option in rootCommand.Options)
            {
                if (option.Name.Contains("version"))
                {

                    option.Action = new VersionAction();
                    option.Aliases.Add("-v");
                }
            }

            rootCommand.SetAction((result) =>
            {
                string? inputPath = result.GetValue(input);
                string outputPath = result.GetValue(output)!;
                uint optLevel = result.GetValue(compilerOptimazionLevel);
                string llPath = result.GetValue(dotLLCodePath)!;
                bool debugFlag = result.GetValue(addDebugFlagToGcc)!;
                string includeLib = result.GetValue(includeLibrary)!;

                return CompileFile(inputPath, outputPath, optLevel, llPath, debugFlag, includeLib);
            });

            CliConfiguration config = new(rootCommand);

            var parserResult = config.Parse(args);
            if (parserResult.Errors.Count > 0)
            {
                foreach (var error in parserResult.Errors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error.Message);
                    Console.ResetColor();
                }
                return 1;
            }

            return parserResult.Invoke();
        }

        private static int CompileFile(string? filePath, string resultPath,
                                       uint optimizationLevel, string llCodePath,
                                       bool addDebugFlag, string includeLib)
        {
            if (filePath == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No input file specified.");
                Console.ResetColor();
                return 1;
            }
            
            bool dontSaveDotLL = string.IsNullOrEmpty(llCodePath);
            if (dontSaveDotLL)
            {

                llCodePath = Path.GetTempFileName();
            }

            try
            {
                // Lexical Analysis
                var inputStream = new AntlrFileStream(filePath);
                var lexer = new ExprLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);

                // Syntax Analysis
                var parser = new ExprParser(tokenStream);

                // Listeners
                parser.RemoveErrorListeners();
                var errorListener = new CompilerErrorListener();
                parser.AddErrorListener(errorListener);

                // AST
                var tree = parser.start();

                if (errorListener.hasErrors)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Compilation failed due to syntax errors");
                    Console.ResetColor();
                    return 1;
                }

                // Semantic Analysis
                var semanticAnalyzer = new SemanticAnalyzer(includeLib);
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
                string outputPath = Path.ChangeExtension(llCodePath, ".ll");
                string assemblyPath = Path.ChangeExtension(outputPath, ".s");

                File.WriteAllText(outputPath, llvmCode);

                Process.Start("llc", $"{outputPath} -O{optimizationLevel} -o {assemblyPath}")?.WaitForExit();
                Process.Start("gcc", $"{(addDebugFlag ? "-g" : "")} {assemblyPath} -o {resultPath} -no-pie")?.WaitForExit();
                File.Delete(assemblyPath);

                if (dontSaveDotLL)
                {
                    File.Delete(outputPath);
                }

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
    
    public sealed class VersionAction : SynchronousCliAction
    {
        public override int Invoke(ParseResult parseResult)
        {
            parseResult.Configuration.Output.WriteLine($"Three Musketeers Compiler {Program.VERSION}");
            return 0;
        }
    }
}