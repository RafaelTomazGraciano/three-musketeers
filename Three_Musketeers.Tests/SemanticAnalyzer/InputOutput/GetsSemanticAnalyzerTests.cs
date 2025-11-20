using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.InputOutput;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct_Unions;

namespace Three_Musketeers.Tests.SemanticAnalysis.InputOutput
{
    public class GetsSemanticAnalyzerTests
    {
        private SymbolTable symbolTable;
        private List<string> errors;
        private List<string> warnings;
        private LibraryDependencyTracker libraryTracker;
        private StructSemanticAnalyzer structSemanticAnalyzer;
        private GetsSemanticAnalyzer analyzer;

        public GetsSemanticAnalyzerTests()
        {
            symbolTable = new SymbolTable();
            errors = new List<string>();
            warnings = new List<string>();
            libraryTracker = new LibraryDependencyTracker(
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            
            var heterogenousInfo = new Dictionary<string, HeterogenousInfo>();
            
            structSemanticAnalyzer = new StructSemanticAnalyzer(
                symbolTable,
                heterogenousInfo,
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            analyzer = new GetsSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                symbolTable,
                libraryTracker,
                structSemanticAnalyzer
            );

            libraryTracker.RegisterInclude("stdio.tm");
        }

        [Fact]
        public void VisitGetsStatement_ValidStringVariable_NoErrors()
        {
            //Arrange
            var symbol = new Symbol("name", "string", 1);
            symbol.isInitializated = false;
            symbolTable.AddSymbol(symbol);
            var input = "gets(name);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitGetsStatement_UndeclaredVariable_ReportsError()
        {
            //Arrange
            var input = "gets(undeclared);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("not declared before use", errors[0]);
        }

        [Fact]
        public void VisitGetsStatement_NonStringVariable_ReportsError()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("age", "int", 1));
            var input = "gets(age);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
            Assert.Contains("'age' is 'int'", errors[0]);
        }

        [Fact]
        public void VisitGetsStatement_ConstantVariable_ReportsError()
        {
            //Arrange
            var constantSymbol = new Symbol("MESSAGE", "string", 1, "\"Hello\"");
            constantSymbol.isConstant = true;
            symbolTable.AddSymbol(constantSymbol);
            var input = "gets(MESSAGE);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("Cannot use #define constant", errors[0]);
            Assert.Contains("Constants are read-only", errors[0]);
        }

        [Fact]
        public void VisitGetsStatement_WithoutStdioInclude_ReportsError()
        {
            //Arrange
            var newLibraryTracker = new LibraryDependencyTracker(
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            var newAnalyzer = new GetsSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                symbolTable,
                newLibraryTracker,
                structSemanticAnalyzer
            );
            symbolTable.AddSymbol(new Symbol("name", "string", 1));
            var input = "gets(name);";
            var context = ParseGetsStatement(input);

            //Act
            var result = newAnalyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
        }

        [Fact]
        public void VisitGetsStatement_DoubleVariable_ReportsError()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("value", "double", 1));
            var input = "gets(value);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        [Fact]
        public void VisitGetsStatement_CharVariable_ReportsError()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("letter", "char", 1));
            var input = "gets(letter);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        [Fact]
        public void VisitGetsStatement_MultipleValidCalls_NoErrors()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("firstName", "string", 1));
            symbolTable.AddSymbol(new Symbol("lastName", "string", 2));
            var input1 = "gets(firstName);";
            var input2 = "gets(lastName);";
            var context1 = ParseGetsStatement(input1);
            var context2 = ParseGetsStatement(input2);

            //Act
            analyzer.VisitGetsStatement(context1);
            analyzer.VisitGetsStatement(context2);

            //Assert
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitGetsStatement_MarksVariableAsInitialized()
        {
            //Arrange
            var symbol = new Symbol("input", "string", 1);
            symbol.isInitializated = false;
            symbolTable.AddSymbol(symbol);
            var input = "gets(input);";
            var context = ParseGetsStatement(input);

            //Act
            analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitGetsStatement_BoolVariable_ReportsError()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("flag", "bool", 1));
            var input = "gets(flag);";
            var context = ParseGetsStatement(input);

            //Act
            var result = analyzer.VisitGetsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        private ExprParser.GetsStatementContext ParseGetsStatement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.getsStatement();
        }
    }
}