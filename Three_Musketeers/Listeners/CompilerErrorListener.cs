using Antlr4.Runtime;
using System;
using System.IO;
using System.Collections.Generic;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Listeners
{
    public class CompilerErrorListener : BaseErrorListener
    {
        public bool hasErrors { get; private set; } = false;
        private readonly HashSet<string> reportedErrors = new HashSet<string>();

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

            // Create unique error identifier
            string errorKey = $"{line}:{charPositionInLine}:{offendingSymbol?.Text ?? ""}:{GetErrorType(msg)}";
            
            // Skip duplicate errors
            if (reportedErrors.Contains(errorKey))
            {
                return;
            }
            
            reportedErrors.Add(errorKey);
            Console.ForegroundColor = ConsoleColor.Red;

            // Check for missing main function
            if (offendingSymbol != null &&
                offendingSymbol.Type == TokenConstants.EOF &&
                msg.Contains("expecting") &&
                (msg.Contains("'int'") || msg.Contains("mainFunction")))
            {
                PrintMissingMainError(line);
                Console.ResetColor();
                return;
            }

            // Check for #define in wrong position
            if (offendingSymbol != null && offendingSymbol.Text == "#define")
            {
                if (msg.Contains("extraneous input") || msg.Contains("mismatched input"))
                {
                    PrintDefinePositionError(line);
                    Console.ResetColor();
                    return;
                }
            }

            // Check for #include in wrong position
            if (offendingSymbol != null && offendingSymbol.Text == "#include")
            {
                if (msg.Contains("extraneous input") || msg.Contains("mismatched input"))
                {
                    PrintIncludePositionError(line);
                    Console.ResetColor();
                    return;
                }
            }

            // Handle all other errors with simple messages
            PrintSimpleError(line, msg, offendingSymbol);
            Console.ResetColor();
        }

        private string GetErrorType(string msg)
        {
            if (msg.Contains("missing ';'")) return "missing_semicolon";
            if (msg.Contains("missing '{'")) return "missing_open_brace";
            if (msg.Contains("missing '}'")) return "missing_close_brace";
            if (msg.Contains("missing '('")) return "missing_open_paren";
            if (msg.Contains("missing ')'")) return "missing_close_paren";
            if (msg.Contains("extraneous input")) return "extraneous";
            if (msg.Contains("no viable alternative")) return "no_viable";
            if (msg.Contains("mismatched input")) return "mismatched";
            return "generic";
        }

        private void PrintSimpleError(int line, string msg, IToken? offendingSymbol)
        {
            Console.WriteLine($"\n[SYNTAX ERROR] Line {line}");

            string errorMsg = GetSimpleErrorMessage(msg, offendingSymbol);
            Console.WriteLine($"  {errorMsg}");
            Console.WriteLine();
        }

        private string GetSimpleErrorMessage(string msg, IToken? offendingSymbol)
        {
            string token = offendingSymbol?.Text ?? "";

            // Priority order: more specific errors first
            if (msg.Contains("missing ';'"))
            {
                return string.IsNullOrEmpty(token) ? "Missing ';'" : $"Missing ';' before '{token}'";
            }

            if (msg.Contains("missing '{'"))
            {
                return "Missing '{'";
            }

            if (msg.Contains("missing '}'"))
            {
                return "Missing '}'";
            }

            if (msg.Contains("missing '('"))
            {
                return "Missing '('";
            }

            if (msg.Contains("missing ')'"))
            {
                return "Missing ')'";
            }

            if (msg.Contains("extraneous input"))
            {
                return string.IsNullOrEmpty(token) ? "Unexpected token" : $"Unexpected token '{token}'";
            }

            if (msg.Contains("mismatched input"))
            {
                return string.IsNullOrEmpty(token) ? "Unexpected token" : $"Unexpected '{token}'";
            }

            if (msg.Contains("no viable alternative"))
            {
                return string.IsNullOrEmpty(token) ? "Syntax error" : $"Syntax error at '{token}'";
            }

            // Default
            return string.IsNullOrEmpty(token) ? "Syntax error" : $"Syntax error at '{token}'";
        }

        private void PrintIncludePositionError(int line)
        {
            Console.WriteLine($"\n[SYNTAX ERROR] Line {line}");
            Console.WriteLine("  #include must be at the beginning of the file");
            Console.WriteLine();
            Console.WriteLine("  Correct order:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    1. #include directives");
            Console.WriteLine("    2. #define directives");
            Console.WriteLine("    3. Global variables and functions");
            Console.WriteLine("    4. main function");
            Console.ResetColor();
            Console.WriteLine();
        }
        
        private void PrintDefinePositionError(int line)
        {
            Console.WriteLine($"\n[SYNTAX ERROR] Line {line}");
            Console.WriteLine("  #define must appear before any code or declarations");
            Console.WriteLine();
            Console.WriteLine("  Correct order:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    1. #include directives");
            Console.WriteLine("    2. #define directives");
            Console.WriteLine("    3. Global variables and functions");
            Console.WriteLine("    4. main function");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void PrintMissingMainError(int line)
        {
            Console.WriteLine($"\n[SYNTAX ERROR] Line {line}");
            Console.WriteLine("  Missing 'main' function");
            Console.WriteLine();
            Console.WriteLine("  Every program must have a main function:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    int main() {");
            Console.WriteLine("        return 0;");
            Console.WriteLine("    }");
            Console.WriteLine();
            Console.WriteLine("  or");
            Console.WriteLine();
            Console.WriteLine("    int main(int argc, char argv[]) {");
            Console.WriteLine("        return 0;");
            Console.WriteLine("    }");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}