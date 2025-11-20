using Antlr4.Runtime.Misc;
using Three_Musketeers.Grammar;
using Three_Musketeers.Utils;

namespace Three_Musketeers.Visitors.SemanticAnalysis.CompilerDirectives
{
    public class IncludeSemanticAnalyzer
    {
        private readonly Action<int, string> reportError;
        private readonly Action<int, string> reportWarning;
        private readonly HashSet<string> includedFiles;
        private readonly HashSet<string> systemLibraries;
        private readonly string currentFilePath;
        private readonly LibraryDependencyTracker libraryTracker;

        public IncludeSemanticAnalyzer(
            Action<int, string> reportError,
            Action<int, string> reportWarning,
            string currentFilePath,
            LibraryDependencyTracker libraryTracker)
        {
            this.reportError = reportError;
            this.reportWarning = reportWarning;
            this.includedFiles = new HashSet<string>();
            this.currentFilePath = currentFilePath;
            this.libraryTracker = libraryTracker;

            // Define valid system libraries
            this.systemLibraries = new HashSet<string>
            {
                "stdio.tm",
                "stdlib.tm"
            };
        }

        public string? VisitIncludeSystem([NotNull] ExprParser.IncludeSystemContext context)
        {
            int line = context.Start.Line;

            // Build library name from ID tokens
            var ids = context.ID();
            string libraryName = string.Join(".", ids.Select(id => id.GetText()));

            // Validate system library
            if (!systemLibraries.Contains(libraryName))
            {
                reportWarning(line, $"Unknown system library '{libraryName}'. Available libraries: stdio.tm, stdlib.tm");
            }

            // Check duplicate include
            if (includedFiles.Contains(libraryName))
            {
                reportWarning(line, $"Library '{libraryName}' has already been included");
                return null;
            }

            includedFiles.Add(libraryName);
            libraryTracker.RegisterInclude(libraryName);
            return libraryName;
        }

        public string? VisitIncludeUser([NotNull] ExprParser.IncludeUserContext context)
        {
            int line = context.Start.Line;
            string stringLiteral = context.STRING_LITERAL().GetText();
            // Remove quotes
            string fileName = stringLiteral.Substring(1, stringLiteral.Length - 2);

            // Validate file extension
            if (!fileName.EndsWith(".tm"))
            {
                reportError(line, $"Include file must have .tm extension, got '{fileName}'");
                return null;
            }

            // Check for duplicate include
            if (includedFiles.Contains(fileName))
            {
                reportWarning(line, $"File '{fileName}' has already been included");
                return null;
            }

            // Build full path 
            string? directory = Path.GetDirectoryName(currentFilePath);
            string fullPath = directory != null
                ? Path.Combine(directory, fileName)
                : fileName;

            // Check if file exists
            if (!File.Exists(fullPath))
            {
                reportError(line, $"Include file '{fileName}' not found at path: {fullPath}");
                return null;
            }

            includedFiles.Add(fileName);

            return fileName;
        }
    }
}