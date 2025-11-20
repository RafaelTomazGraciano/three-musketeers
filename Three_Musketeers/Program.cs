using Antlr4.Runtime;
using System.Diagnostics;
using Three_Musketeers.Visitors;
using Three_Musketeers.Listeners;
using Three_Musketeers.Grammar;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Net;

namespace Three_Musketeers 
{
    public class Program
    {
        public static readonly string VERSION = "v1.0.0 Athos";
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

        private static readonly CliOption<bool> dotLLCodePath = new("--ll")
        {
            DefaultValueFactory = (res) => false,
            Description = "Save generated LLVM IR code"
        };

        private static readonly CliOption<bool> addDebugFlagToGcc = new("-g")
        {
            DefaultValueFactory = (res) => false,
            Description = "Add debug information to the generated code"
        };

        private static readonly CliOption<string> includeLibrary = new("-I", ["--Include"])
        {
            DefaultValueFactory = (res) => "",
            Description = "Include path libraries"
        };

        private static readonly CliOption<bool> generateBin = new("--bin")
        {
            DefaultValueFactory = (res) => false,
            Description = "Create files on a bin directory"
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
                includeLibrary,
                generateBin
            }!;

            foreach (var option in rootCommand.Options)
            {
                if (option.Name.Contains("version"))
                {

                    option.Action = new VersionAction();
                    option.Aliases.Add("-v");
                    break;
                }
            }

            rootCommand.SetAction((result) =>
            {
                string? inputPath = result.GetValue(input);
                string outputPath = result.GetValue(output)!;
                uint optLevel = result.GetValue(compilerOptimazionLevel);
                bool saveLLVM = result.GetValue(dotLLCodePath)!;
                bool debugFlag = result.GetValue(addDebugFlagToGcc)!;
                string includeLib = result.GetValue(includeLibrary)!;
                bool shouldCreateInBin = result.GetValue(generateBin)!;

                return CompileFile(inputPath, outputPath, optLevel, saveLLVM, debugFlag, includeLib, shouldCreateInBin);
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
                                       uint optimizationLevel, bool saveLLVM,
                                       bool addDebugFlag, string includeLib,
                                       bool shouldCreateInBin)
        {
            if (filePath == null)
            {
                WriteError("No input file especified");
                return 1;
            }

            if (Path.GetExtension(filePath) != ".3m")
            {
                WriteError("No valid extension from input file");
                return 1;
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

                string fileDir = Path.GetDirectoryName(Path.GetFullPath(filePath))!;
                string result = fileDir;

                if (shouldCreateInBin)
                {
                    result = Path.Combine(fileDir, "bin");
                    Directory.CreateDirectory(result);
                }

                // Generate output paths inside bin directory
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string outputPath = Path.Combine(result, $"{fileName}.ll");
                string assemblyFilePath = Path.ChangeExtension(outputPath, ".s");
                string executablePath = Path.Combine(result, resultPath);
                File.WriteAllText(outputPath, llvmCode);

                var llcProcess = Process.Start("llc", $"{outputPath} -O{optimizationLevel} -o {assemblyFilePath}");
                llcProcess?.WaitForExit();

                if (llcProcess?.ExitCode != 0)
                {
                    WriteError($"LLVM Compilation failed (llc exited with code {llcProcess?.ExitCode})");
                    return 1;
                }

                var gccProcess = Process.Start("gcc", $"{(addDebugFlag ? "-g" : "")} {assemblyFilePath} -o {executablePath} -no-pie");
                gccProcess?.WaitForExit();

                if (gccProcess?.ExitCode != 0)
                {
                    WriteError($"GCC Linking failed (gcc exited with code {gccProcess?.ExitCode})");
                    return 1;
                }

                File.Delete(assemblyFilePath);

                if (!saveLLVM)
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
        
        private static void WriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
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
