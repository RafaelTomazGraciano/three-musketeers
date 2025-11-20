using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Utils;
using Three_Musketeers.Visitors.SemanticAnalysis.InputOutput;
using Three_Musketeers.Visitors.SemanticAnalysis.Struct_Unions;

namespace Three_Musketeers.Tests.SemanticAnalysis.InputOutput
{
    public class PutsSemanticAnalyzerTests
    {
        private SymbolTable symbolTable;
        private List<string> errors;
        private LibraryDependencyTracker libraryTracker;
        private StructSemanticAnalyzer structSemanticAnalyzer;
        private PutsSemanticAnalyzer analyzer;

        public PutsSemanticAnalyzerTests()
        {
            symbolTable = new SymbolTable();
            errors = new List<string>();
            libraryTracker = new LibraryDependencyTracker(
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            
            var heterogenousInfo = new Dictionary<string, HeterogenousInfo>();
            
            structSemanticAnalyzer = new StructSemanticAnalyzer(
                symbolTable,
                heterogenousInfo,
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            
            analyzer = new PutsSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                symbolTable,
                libraryTracker,
                structSemanticAnalyzer
            );

            libraryTracker.RegisterInclude("stdio.tm");
        }

        [Fact]
        public void VisitPutsStatement_StringLiteral_NoErrors()
        {
            //Arrange
            var input = "puts(\"Hello World\");";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_ValidStringVariable_NoErrors()
        {
            //Arrange
            var symbol = new Symbol("message", "string", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(message);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_UndeclaredVariable_ReportsError()
        {
            //Arrange
            var input = "puts(undeclared);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("not declared before use", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_UninitializedVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("text", "string", 1);
            symbol.isInitializated = false;
            symbolTable.AddSymbol(symbol);
            var input = "puts(text);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("may not have been initialized", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_NonStringVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("number", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(number);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_CharArray_NoErrors()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("str", "char", 1, new List<int> { 50 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "puts(str);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_IntArray_ReportsError()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("numbers", "int", 1, new List<int> { 10 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "puts(numbers);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("cannot print array of type", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_StringArrayElement_NoErrors()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("messages", "string", 1, new List<int> { 5 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "puts(messages[0]);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_CharArrayElement_NoErrors()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("chars", "char", 1, new List<int> { 10 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "puts(chars[0]);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_IntArrayElement_ReportsError()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("numbers", "int", 1, new List<int> { 10 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "puts(numbers[0]);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only print string or char array elements", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_NonArrayWithIndex_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("text", "string", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(text[0]);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("is not an array", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_WithoutStdioInclude_ReportsError()
        {
            //Arrange
            var newLibraryTracker = new LibraryDependencyTracker(
                (line, msg) => errors.Add($"Line {line}: {msg}")
            );
            var newAnalyzer = new PutsSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                symbolTable,
                newLibraryTracker,
                structSemanticAnalyzer
            );
            var input = "puts(\"Hello\");";
            var context = ParsePutsStatement(input);

            //Act
            var result = newAnalyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
        }

        [Fact]
        public void VisitPutsStatement_DoubleVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("pi", "double", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(pi);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_CharVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("letter", "char", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(letter);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        [Fact]
        public void VisitPutsStatement_EmptyStringLiteral_NoErrors()
        {
            //Arrange
            var input = "puts(\"\");";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_MultipleValidCalls_NoErrors()
        {
            //Arrange
            var symbol1 = new Symbol("msg1", "string", 1);
            symbol1.isInitializated = true;
            var symbol2 = new Symbol("msg2", "string", 2);
            symbol2.isInitializated = true;
            symbolTable.AddSymbol(symbol1);
            symbolTable.AddSymbol(symbol2);
            
            var input1 = "puts(msg1);";
            var input2 = "puts(msg2);";
            var context1 = ParsePutsStatement(input1);
            var context2 = ParsePutsStatement(input2);

            //Act
            analyzer.VisitPutsStatement(context1);
            analyzer.VisitPutsStatement(context2);

            //Assert
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPutsStatement_BoolVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("flag", "bool", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "puts(flag);";
            var context = ParsePutsStatement(input);

            //Act
            var result = analyzer.VisitPutsStatement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with string variables", errors[0]);
        }

        private ExprParser.PutsStatementContext ParsePutsStatement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.putsStatement();
        }
    }
}