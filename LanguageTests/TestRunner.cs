using System;
using System.Diagnostics;
using System.IO;

class TestRunner
{
    static void Main()
    {
        string testsDir = "language_tests";
        var testFiles = Directory.GetFiles(testsDir, "*.3m", SearchOption.AllDirectories);
        int passed = 0, failed = 0;
        string testingFile = "";

        try
        {
            foreach (var testFile in testFiles)
            {
                testingFile = testFile;
                string expectedFile = Path.ChangeExtension(testFile, ".expected");
                string inputFile = Path.ChangeExtension(testFile, ".input");
                string expectedOutput = File.Exists(expectedFile) ? File.ReadAllText(expectedFile).Trim() : "";
                string inputData = File.Exists(inputFile) ? File.ReadAllText(inputFile) : "";

                string actualOutput = RunCompiler(testFile, inputData).Trim();

                if (actualOutput == expectedOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[PASS] {testFile}");
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[FAIL] {testFile}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Expected:\n" + expectedOutput);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Got:\n" + actualOutput);
                    failed++;
                }


            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FAIL] {testingFile}");
            Console.WriteLine(e.Message);
            failed++;
        }
        finally
        {
            Console.WriteLine($"\nTotal: {passed + failed}, Passed: {passed}, Failed: {failed}");
            Console.ResetColor();
        }
    }

    static string RunCompiler(string file, string input)
    {
        // get absolute path
        string absolutePath = Path.GetFullPath(file);
        
        // execute compiler
        var compile = new Process();
        compile.StartInfo.FileName = "../Three_Musketeers/bin/Debug/net9.0/tm";
        compile.StartInfo.Arguments = $"{absolutePath} --bin --ll";
        compile.StartInfo.RedirectStandardOutput = true;
        compile.StartInfo.RedirectStandardError = true;
        compile.StartInfo.UseShellExecute = false;
        compile.Start();
        
        string compilerOutput = compile.StandardOutput.ReadToEnd();
        string compilerError = compile.StandardError.ReadToEnd();
        compile.WaitForExit();
        
        // check if compilation succeeded
        if (compile.ExitCode != 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            throw new Exception($"Compiler output: {compilerOutput}");
        }
        
        // path to binary
        string testDir = Path.GetDirectoryName(absolutePath)!;
        string binDir = Path.Combine(testDir, "bin");
        string baseName = Path.GetFileNameWithoutExtension(file);
        string exePath = Path.Combine(binDir, baseName);
        
        if (!File.Exists(exePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            throw new FileNotFoundException($"Executable not found: {exePath}");
        }
        
        // run binary
        var run = new Process();
        run.StartInfo.FileName = exePath;
        run.StartInfo.RedirectStandardOutput = true;
        run.StartInfo.RedirectStandardInput = true;
        run.StartInfo.UseShellExecute = false;
        
        run.Start();
        
        if (!string.IsNullOrEmpty(input))
        {
            run.StandardInput.Write(input);
            run.StandardInput.Close();
        }
        
        string output = run.StandardOutput.ReadToEnd();
        run.WaitForExit();
        return output.TrimEnd();
    }
}
