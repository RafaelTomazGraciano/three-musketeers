using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.CompilerDirectives;
using Xunit;

namespace Three_Musketeers.Tests.SemanticAnalysis.CompilerDirectives
{
    public class IncludeSemanticAnalyzerTests : IDisposable
    {
        private List<string> errors;
        private List<string> warnings;
        private IncludeSemanticAnalyzer analyzer;
        private LibraryDependencyTracker libraryTracker;
        private string testFilePath;
        private string testDirectory;

        public IncludeSemanticAnalyzerTests()
        {
            errors = new List<string>();
            warnings = new List<string>();
            libraryTracker = new LibraryDependencyTracker(
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);
            testFilePath = Path.Combine(testDirectory, "test.tm");
            
            analyzer = new IncludeSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                (line, msg) => warnings.Add($"Line {line}: {msg}"),
                testFilePath,
                libraryTracker
            );
        }

        public void Dispose()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Fact]
        public void VisitIncludeSystem_ValidStdioLibrary_ReturnsLibraryName()
        {
            //Arrange
            var input = "#include <stdio.tm>";
            var context = ParseIncludeSystem(input);

            //Act
            var result = analyzer.VisitIncludeSystem(context);

            //Assert
            Assert.Equal("stdio.tm", result);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeSystem_ValidStdlibLibrary_ReturnsLibraryName()
        {
            //Arrange
            var input = "#include <stdlib.tm>";
            var context = ParseIncludeSystem(input);

            //Act
            var result = analyzer.VisitIncludeSystem(context);

            //Assert
            Assert.Equal("stdlib.tm", result);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeSystem_UnknownLibrary_ReportsWarning()
        {
            //Arrange
            var input = "#include <math.tm>";
            var context = ParseIncludeSystem(input);

            //Act
            var result = analyzer.VisitIncludeSystem(context);

            //Assert
            Assert.Equal("math.tm", result);
            Assert.Empty(errors);
            Assert.Single(warnings);
            Assert.Contains("Unknown system library", warnings[0]);
            Assert.Contains("Available libraries: stdio.tm, stdlib.tm", warnings[0]);
        }

        [Fact]
        public void VisitIncludeSystem_DuplicateInclude_ReportsWarningAndReturnsNull()
        {
            //Arrange
            var input = "#include <stdio.tm>";
            var context1 = ParseIncludeSystem(input);
            var context2 = ParseIncludeSystem(input);

            //Act
            var result1 = analyzer.VisitIncludeSystem(context1);
            var result2 = analyzer.VisitIncludeSystem(context2);

            //Assert
            Assert.Equal("stdio.tm", result1);
            Assert.Null(result2);
            Assert.Single(warnings);
            Assert.Contains("has already been included", warnings[0]);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitIncludeSystem_RegistersIncludeInTracker()
        {
            //Arrange
            var input = "#include <stdio.tm>";
            var context = ParseIncludeSystem(input);

            //Act
            analyzer.VisitIncludeSystem(context);
            bool canUsePrintf = libraryTracker.CheckFunctionDependency("printf", 1);

            //Assert
            Assert.True(canUsePrintf);
        }

        [Fact]
        public void VisitIncludeUser_ValidFileExists_ReturnsFileName()
        {
            //Arrange
            var fileName = "utils.tm";
            var filePath = Path.Combine(testDirectory, fileName);
            File.WriteAllText(filePath, "// test content");
            var input = $"#include \"{fileName}\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Equal(fileName, result);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeUser_FileDoesNotExist_ReportsError()
        {
            //Arrange
            var input = "#include \"nonexistent.tm\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("Include file 'nonexistent.tm' not found", errors[0]);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeUser_InvalidExtension_ReportsError()
        {
            //Arrange
            var input = "#include \"file.txt\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("must have .tm extension", errors[0]);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeUser_DuplicateInclude_ReportsWarningAndReturnsNull()
        {
            //Arrange
            var fileName = "helper.tm";
            var filePath = Path.Combine(testDirectory, fileName);
            File.WriteAllText(filePath, "// helper content");
            var input = $"#include \"{fileName}\"";
            var context1 = ParseIncludeUser(input);
            var context2 = ParseIncludeUser(input);

            //Act
            var result1 = analyzer.VisitIncludeUser(context1);
            var result2 = analyzer.VisitIncludeUser(context2);

            //Assert
            Assert.Equal(fileName, result1);
            Assert.Null(result2);
            Assert.Single(warnings);
            Assert.Contains("has already been included", warnings[0]);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitIncludeUser_FileWithPath_ValidatesCorrectly()
        {
            //Arrange
            var fileName = "module.tm";
            var filePath = Path.Combine(testDirectory, fileName);
            File.WriteAllText(filePath, "// module content");
            var input = $"#include \"{fileName}\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Equal(fileName, result);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeUser_InvalidExtensionTakesPriorityOverMissingFile()
        {
            //Arrange
            var input = "#include \"missing.cpp\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("must have .tm extension", errors[0]);
            Assert.DoesNotContain("not found", errors[0]);
        }

        [Fact]
        public void MixedIncludes_SystemAndUser_BothProcessedCorrectly()
        {
            //Arrange
            var fileName = "custom.tm";
            var filePath = Path.Combine(testDirectory, fileName);
            File.WriteAllText(filePath, "// custom content");
            
            var systemInput = "#include <stdio.tm>";
            var userInput = $"#include \"{fileName}\"";
            var systemContext = ParseIncludeSystem(systemInput);
            var userContext = ParseIncludeUser(userInput);

            //Act
            var systemResult = analyzer.VisitIncludeSystem(systemContext);
            var userResult = analyzer.VisitIncludeUser(userContext);

            //Assert
            Assert.Equal("stdio.tm", systemResult);
            Assert.Equal(fileName, userResult);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeSystem_MultipleValidLibraries_AllProcessedSuccessfully()
        {
            //Arrange
            var stdioInput = "#include <stdio.tm>";
            var stdlibInput = "#include <stdlib.tm>";
            var stdioContext = ParseIncludeSystem(stdioInput);
            var stdlibContext = ParseIncludeSystem(stdlibInput);

            //Act
            var stdioResult = analyzer.VisitIncludeSystem(stdioContext);
            var stdlibResult = analyzer.VisitIncludeSystem(stdlibContext);
            bool canUsePrintf = libraryTracker.CheckFunctionDependency("printf", 1);
            bool canUseMalloc = libraryTracker.CheckFunctionDependency("malloc", 1);

            //Assert
            Assert.Equal("stdio.tm", stdioResult);
            Assert.Equal("stdlib.tm", stdlibResult);
            Assert.True(canUsePrintf);
            Assert.True(canUseMalloc);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitIncludeUser_EmptyFileName_ReportsError()
        {
            //Arrange
            var input = "#include \"\"";
            var context = ParseIncludeUser(input);

            //Act
            var result = analyzer.VisitIncludeUser(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("must have .tm extension", errors[0]);
        }

        private ExprParser.IncludeSystemContext ParseIncludeSystem(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.include() as ExprParser.IncludeSystemContext ?? throw new InvalidOperationException("Failed to parse include system");
        }

        private ExprParser.IncludeUserContext ParseIncludeUser(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.include() as ExprParser.IncludeUserContext ?? throw new InvalidOperationException("Failed to parse include user");
        }
    }
}