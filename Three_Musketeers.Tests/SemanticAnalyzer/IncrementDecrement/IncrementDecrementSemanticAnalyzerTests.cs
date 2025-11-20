using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.SemanticAnalysis.IncrementDecrement;

namespace Three_Musketeers.Tests.SemanticAnalysis.IncrementDecrement
{
    public class IncrementDecrementSemanticAnalyzerTests
    {
        private SymbolTable symbolTable;
        private List<string> errors;
        private List<string> warnings;
        private IncrementDecrementSemanticAnalyzer analyzer;

        public IncrementDecrementSemanticAnalyzerTests()
        {
            symbolTable = new SymbolTable();
            errors = new List<string>();
            warnings = new List<string>();
            analyzer = new IncrementDecrementSemanticAnalyzer(
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                (line, msg) => warnings.Add($"Line {line}: {msg}"),
                symbolTable
            );
        }

        [Fact]
        public void VisitPrefixIncrement_ValidIntVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("counter", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "++counter";
            var context = ParsePrefixIncrement(input);

            //Act
            var result = analyzer.VisitPrefixIncrement(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPrefixIncrement_UndeclaredVariable_ReportsError()
        {
            //Arrange
            var input = "++undeclared";
            var context = ParsePrefixIncrement(input);

            //Act
            var result = analyzer.VisitPrefixIncrement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("was not declared", errors[0]);
        }

        [Fact]
        public void VisitPrefixIncrement_UninitializedVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("counter", "int", 1);
            symbol.isInitializated = false;
            symbolTable.AddSymbol(symbol);
            var input = "++counter";
            var context = ParsePrefixIncrement(input);

            //Act
            var result = analyzer.VisitPrefixIncrement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("is not initialized", errors[0]);
        }

        [Fact]
        public void VisitPrefixIncrement_StringVariable_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("text", "string", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "++text";
            var context = ParsePrefixIncrement(input);

            //Act
            var result = analyzer.VisitPrefixIncrement(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with numeric types", errors[0]);
        }

        [Fact]
        public void VisitPrefixDecrement_ValidIntVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("counter", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "--counter";
            var context = ParsePrefixDecrement(input);

            //Act
            var result = analyzer.VisitPrefixDecrement(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPrefixDecrement_DoubleVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("value", "double", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "--value";
            var context = ParsePrefixDecrement(input);

            //Act
            var result = analyzer.VisitPrefixDecrement(context);

            //Assert
            Assert.Equal("double", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixIncrement_ValidIntVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("index", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "index++";
            var context = ParsePostfixIncrement(input);

            //Act
            var result = analyzer.VisitPostfixIncrement(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixIncrement_CharVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("letter", "char", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "letter++";
            var context = ParsePostfixIncrement(input);

            //Act
            var result = analyzer.VisitPostfixIncrement(context);

            //Assert
            Assert.Equal("char", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixDecrement_ValidIntVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("count", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "count--";
            var context = ParsePostfixDecrement(input);

            //Act
            var result = analyzer.VisitPostfixDecrement(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixDecrement_BoolVariable_ReturnsType()
        {
            //Arrange
            var symbol = new Symbol("flag", "bool", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "flag--";
            var context = ParsePostfixDecrement(input);

            //Act
            var result = analyzer.VisitPostfixDecrement(context);

            //Assert
            Assert.Equal("bool", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPrefixIncrementArray_ValidIntArray_ReturnsElementType()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("numbers", "int", 1, new List<int> { 5 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "++numbers[0]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPrefixIncrementArray_UndeclaredArray_ReportsError()
        {
            //Arrange
            var input = "++undeclared[0]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("was not declared", errors[0]);
        }

        [Fact]
        public void VisitPrefixIncrementArray_NotAnArray_ReportsError()
        {
            //Arrange
            var symbol = new Symbol("notArray", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);
            var input = "++notArray[0]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("is not an array", errors[0]);
        }

        [Fact]
        public void VisitPrefixIncrementArray_WrongDimensionCount_ReportsError()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("matrix", "int", 1, new List<int> { 3, 3 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "++matrix[0]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("expects 2 indices", errors[0]);
        }

        [Fact]
        public void VisitPrefixIncrementArray_StringArray_ReportsError()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("strings", "string", 1, new List<int> { 5 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "++strings[0]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Null(result);
            Assert.Single(errors);
            Assert.Contains("can only be used with numeric types", errors[0]);
        }

        [Fact]
        public void VisitPrefixDecrementArray_ValidDoubleArray_ReturnsElementType()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("values", "double", 1, new List<int> { 10 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "--values[5]";
            var context = ParsePrefixDecrementArray(input);

            //Act
            var result = analyzer.VisitPrefixDecrementArray(context);

            //Assert
            Assert.Equal("double", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixIncrementArray_ValidIntArray_ReturnsElementType()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("scores", "int", 1, new List<int> { 20 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "scores[10]++";
            var context = ParsePostfixIncrementArray(input);

            //Act
            var result = analyzer.VisitPostfixIncrementArray(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPostfixDecrementArray_ValidCharArray_ReturnsElementType()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("letters", "char", 1, new List<int> { 26 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "letters[0]--";
            var context = ParsePostfixDecrementArray(input);

            //Act
            var result = analyzer.VisitPostfixDecrementArray(context);

            //Assert
            Assert.Equal("char", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitPrefixIncrementArray_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            var arraySymbol = new ArraySymbol("grid", "int", 1, new List<int> { 5, 5 });
            arraySymbol.isInitializated = true;
            symbolTable.AddSymbol(arraySymbol);
            var input = "++grid[2][3]";
            var context = ParsePrefixIncrementArray(input);

            //Act
            var result = analyzer.VisitPrefixIncrementArray(context);

            //Assert
            Assert.Equal("int", result);
            Assert.Empty(errors);
        }

        [Fact]
        public void AllIncrementDecrementOperators_OnSameVariable_NoErrors()
        {
            //Arrange
            var symbol = new Symbol("num", "int", 1);
            symbol.isInitializated = true;
            symbolTable.AddSymbol(symbol);

            //Act
            var prefixInc = analyzer.VisitPrefixIncrement(ParsePrefixIncrement("++num"));
            var prefixDec = analyzer.VisitPrefixDecrement(ParsePrefixDecrement("--num"));
            var postfixInc = analyzer.VisitPostfixIncrement(ParsePostfixIncrement("num++"));
            var postfixDec = analyzer.VisitPostfixDecrement(ParsePostfixDecrement("num--"));

            //Assert
            Assert.Equal("int", prefixInc);
            Assert.Equal("int", prefixDec);
            Assert.Equal("int", postfixInc);
            Assert.Equal("int", postfixDec);
            Assert.Empty(errors);
        }

        private ExprParser.PrefixIncrementContext ParsePrefixIncrement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PrefixIncrementContext ?? throw new InvalidOperationException("Failed to parse prefix increment");
        }

        private ExprParser.PrefixDecrementContext ParsePrefixDecrement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PrefixDecrementContext ?? throw new InvalidOperationException("Failed to parse prefix decrement");
        }

        private ExprParser.PostfixIncrementContext ParsePostfixIncrement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PostfixIncrementContext ?? throw new InvalidOperationException("Failed to parse postfix increment");
        }

        private ExprParser.PostfixDecrementContext ParsePostfixDecrement(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PostfixDecrementContext ?? throw new InvalidOperationException("Failed to parse postfix decrement");
        }

        private ExprParser.PrefixIncrementArrayContext ParsePrefixIncrementArray(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PrefixIncrementArrayContext ?? throw new InvalidOperationException("Failed to parse prefix increment array");
        }

        private ExprParser.PrefixDecrementArrayContext ParsePrefixDecrementArray(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PrefixDecrementArrayContext ?? throw new InvalidOperationException("Failed to parse prefix decrement array");
        }

        private ExprParser.PostfixIncrementArrayContext ParsePostfixIncrementArray(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PostfixIncrementArrayContext ?? throw new InvalidOperationException("Failed to parse postfix increment array");
        }

        private ExprParser.PostfixDecrementArrayContext ParsePostfixDecrementArray(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.expr() as ExprParser.PostfixDecrementArrayContext ?? throw new InvalidOperationException("Failed to parse postfix decrement array");
        }
    }
}