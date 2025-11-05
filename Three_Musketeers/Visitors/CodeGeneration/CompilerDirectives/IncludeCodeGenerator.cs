using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;

namespace Three_Musketeers.Visitors.CodeGeneration.CompilerDirectives
{
    public class IncludeCodeGenerator
    {
        private readonly StringBuilder declarations;
        private readonly HashSet<string> includedLibraries;

        public IncludeCodeGenerator(StringBuilder declarations)
        {
            this.declarations = declarations;
            this.includedLibraries = new HashSet<string>();
        }

        public string? VisitIncludeSystem([NotNull] ExprParser.IncludeSystemContext context)
        {
            string angleString = context.ANGLE_STRING().GetText();
            string libraryName = angleString.Substring(1, angleString.Length - 2);

            if (includedLibraries.Contains(libraryName))
            {
                return null;
            }

            includedLibraries.Add(libraryName);
            GenerateLibraryDeclarations(libraryName);

            return libraryName;
        }

        public string? VisitIncludeUser([NotNull] ExprParser.IncludeUserContext context)
        {
            string stringLiteral = context.STRING_LITERAL().GetText();
            string fileName = stringLiteral.Substring(1, stringLiteral.Length - 2);

            return fileName;
        }
        
        private void GenerateLibraryDeclarations(string libraryName)
        {
            switch (libraryName)
            {
                case "stdio.tm":
                    GenerateStdioDeclarations();
                    break;
                case "stdlib.tm":
                    GenerateStdlibDeclarations();
                    break;
                default:
                    Console.WriteLine($"[WARNING] Unknown library: {libraryName}");
                    break;
            }
        }

        private void GenerateStdioDeclarations()
        {
            // printf declaration
            if (!declarations.ToString().Contains("declare i32 @printf"))
            {
                declarations.AppendLine("declare i32 @printf(i8*, ...)");
            }

            // scanf declaration
            if (!declarations.ToString().Contains("declare i32 @__isoc99_scanf"))
            {
                declarations.AppendLine("declare i32 @__isoc99_scanf(i8*, ...)");
            }

            // gets declaration
            if (!declarations.ToString().Contains("declare i8* @gets"))
            {
                declarations.AppendLine("declare i8* @gets(i8*)");
            }

            // puts declaration
            if (!declarations.ToString().Contains("declare i32 @puts"))
            {
                declarations.AppendLine("declare i32 @puts(i8*)");
            }
        }

        private void GenerateStdlibDeclarations()
        {
            // malloc declaration
            if (!declarations.ToString().Contains("declare i8* @malloc"))
            {
                declarations.AppendLine("declare i8* @malloc(i64)");
            }

            // free declaration
            if (!declarations.ToString().Contains("declare void @free"))
            {
                declarations.AppendLine("declare void @free(i8*)");
            }

            // atoi declaration
            if (!declarations.ToString().Contains("declare i32 @atoi"))
            {
                declarations.AppendLine("declare i32 @atoi(i8*)");
            }

            // atof declaration (for atod)
            if (!declarations.ToString().Contains("declare double @atof"))
            {
                declarations.AppendLine("declare double @atof(i8*)");
            }

            // sprintf declaration (for itoa/dtoa)
            if (!declarations.ToString().Contains("declare i32 @sprintf"))
            {
                declarations.AppendLine("declare i32 @sprintf(i8*, i8*, ...)");
            }
        }
    }
}