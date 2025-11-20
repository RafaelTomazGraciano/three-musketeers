using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Models;
using Three_Musketeers.Visitors.SemanticAnalysis.CompilerDirectives;
using Xunit;

namespace Three_Musketeers.Tests.SemanticAnalysis.CompilerDirectives
{
    public class DefineSemanticAnalyzerTests
    {
        private SymbolTable symbolTable;
        private List<string> errors;
        private List<string> warnings;
        private DefineSemanticAnalyzer analyzer;

        public DefineSemanticAnalyzerTests()
        {
            symbolTable = new SymbolTable();
            errors = new List<string>();
            warnings = new List<string>();
            analyzer = new DefineSemanticAnalyzer(
                symbolTable,
                (line, msg) => errors.Add($"Line {line}: {msg}"),
                (line, msg) => warnings.Add($"Line {line}: {msg}")
            );
        }

        [Fact]
        public void VisitDefineInt_ValidDefinition_AddsSymbolToTable()
        {
            //Arrange
            var input = "#define MAX 100";
            var context = ParseDefineInt(input);

            //Act
            var result = analyzer.VisitDefineInt(context);

            //Assert
            Assert.Equal("MAX", result);
            Assert.True(symbolTable.Contains("MAX"));
            var symbol = symbolTable.GetSymbol("MAX");
            Assert.NotNull(symbol);
            Assert.Equal("int", symbol.type);
            Assert.True(symbol.isConstant);
            Assert.Equal("100", symbol.constantValue);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitDefineInt_ShadowsExistingVariable_ReportsWarning()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("COUNT", "int", 1));
            var input = "#define COUNT 50";
            var context = ParseDefineInt(input);

            //Act
            var result = analyzer.VisitDefineInt(context);

            //Assert
            Assert.Equal("COUNT", result);
            Assert.Single(warnings);
            Assert.Contains("shadows an existing variable", warnings[0]);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitDefineInt_LargeValue_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define LARGE 999999";
            var context = ParseDefineInt(input);

            //Act
            var result = analyzer.VisitDefineInt(context);

            //Assert
            Assert.Equal("LARGE", result);
            Assert.True(symbolTable.Contains("LARGE"));
            var symbol = symbolTable.GetSymbol("LARGE");
            Assert.NotNull(symbol);
            Assert.Equal("int", symbol.type);
            Assert.Equal("999999", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineDouble_ValidDefinition_AddsSymbolToTable()
        {
            //Arrange
            var input = "#define PI 3.14";
            var context = ParseDefineDouble(input);

            //Act
            var result = analyzer.VisitDefineDouble(context);

            //Assert
            Assert.Equal("PI", result);
            Assert.True(symbolTable.Contains("PI"));
            var symbol = symbolTable.GetSymbol("PI");
            Assert.NotNull(symbol);
            Assert.Equal("double", symbol.type);
            Assert.True(symbol.isConstant);
            Assert.Equal("3.14", symbol.constantValue);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitDefineDouble_ShadowsExistingVariable_ReportsWarning()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("RATE", "double", 1));
            var input = "#define RATE 2.5";
            var context = ParseDefineDouble(input);

            //Act
            var result = analyzer.VisitDefineDouble(context);

            //Assert
            Assert.Equal("RATE", result);
            Assert.Single(warnings);
            Assert.Contains("shadows an existing variable", warnings[0]);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitDefineDouble_SmallDecimal_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define EPSILON 0.001";
            var context = ParseDefineDouble(input);

            //Act
            var result = analyzer.VisitDefineDouble(context);

            //Assert
            Assert.Equal("EPSILON", result);
            Assert.True(symbolTable.Contains("EPSILON"));
            var symbol = symbolTable.GetSymbol("EPSILON");
            Assert.NotNull(symbol);
            Assert.Equal("double", symbol.type);
            Assert.Equal("0.001", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineString_ValidDefinition_AddsSymbolToTable()
        {
            //Arrange
            var input = "#define MESSAGE \"Hello World\"";
            var context = ParseDefineString(input);

            //Act
            var result = analyzer.VisitDefineString(context);

            //Assert
            Assert.Equal("MESSAGE", result);
            Assert.True(symbolTable.Contains("MESSAGE"));
            var symbol = symbolTable.GetSymbol("MESSAGE");
            Assert.NotNull(symbol);
            Assert.Equal("string", symbol.type);
            Assert.True(symbol.isConstant);
            Assert.Equal("\"Hello World\"", symbol.constantValue);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitDefineString_ShadowsExistingVariable_ReportsWarning()
        {
            //Arrange
            symbolTable.AddSymbol(new Symbol("NAME", "string", 1));
            var input = "#define NAME \"New Name\"";
            var context = ParseDefineString(input);

            //Act
            var result = analyzer.VisitDefineString(context);

            //Assert
            Assert.Equal("NAME", result);
            Assert.Single(warnings);
            Assert.Contains("shadows an existing variable", warnings[0]);
            Assert.Empty(errors);
        }

        [Fact]
        public void VisitDefineString_EmptyString_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define EMPTY \"\"";
            var context = ParseDefineString(input);

            //Act
            var result = analyzer.VisitDefineString(context);

            //Assert
            Assert.Equal("EMPTY", result);
            Assert.True(symbolTable.Contains("EMPTY"));
            var symbol = symbolTable.GetSymbol("EMPTY");
            Assert.NotNull(symbol);
            Assert.Equal("string", symbol.type);
            Assert.Equal("\"\"", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineString_StringWithEscapeSequences_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define PATH \"C:\\\\Users\\\\file.txt\"";
            var context = ParseDefineString(input);

            //Act
            var result = analyzer.VisitDefineString(context);

            //Assert
            Assert.Equal("PATH", result);
            Assert.True(symbolTable.Contains("PATH"));
            var symbol = symbolTable.GetSymbol("PATH");
            Assert.NotNull(symbol);
            Assert.Equal("string", symbol.type);
            Assert.Equal("\"C:\\\\Users\\\\file.txt\"", symbol.constantValue);
        }

        [Fact]
        public void MultipleDefines_DifferentTypes_AllAddedSuccessfully()
        {
            //Arrange
            var intContext = ParseDefineInt("#define MAX 100");
            var doubleContext = ParseDefineDouble("#define PI 3.14");
            var stringContext = ParseDefineString("#define NAME \"Test\"");

            //Act
            analyzer.VisitDefineInt(intContext);
            analyzer.VisitDefineDouble(doubleContext);
            analyzer.VisitDefineString(stringContext);

            //Assert
            Assert.True(symbolTable.Contains("MAX"));
            Assert.True(symbolTable.Contains("PI"));
            Assert.True(symbolTable.Contains("NAME"));
            var maxSymbol = symbolTable.GetSymbol("MAX");
            var piSymbol = symbolTable.GetSymbol("PI");
            var nameSymbol = symbolTable.GetSymbol("NAME");
            Assert.NotNull(maxSymbol);
            Assert.NotNull(piSymbol);
            Assert.NotNull(nameSymbol);
            Assert.Equal("int", maxSymbol.type);
            Assert.Equal("double", piSymbol.type);
            Assert.Equal("string", nameSymbol.type);
            Assert.True(maxSymbol.isConstant);
            Assert.True(piSymbol.isConstant);
            Assert.True(nameSymbol.isConstant);
            Assert.Empty(errors);
            Assert.Empty(warnings);
        }

        [Fact]
        public void VisitDefineInt_ZeroValue_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define ZERO 0";
            var context = ParseDefineInt(input);

            //Act
            var result = analyzer.VisitDefineInt(context);

            //Assert
            Assert.Equal("ZERO", result);
            Assert.True(symbolTable.Contains("ZERO"));
            var symbol = symbolTable.GetSymbol("ZERO");
            Assert.NotNull(symbol);
            Assert.Equal("int", symbol.type);
            Assert.Equal("0", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineDouble_ZeroPointValue_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define ZERO_DOUBLE 0.0";
            var context = ParseDefineDouble(input);

            //Act
            var result = analyzer.VisitDefineDouble(context);

            //Assert
            Assert.Equal("ZERO_DOUBLE", result);
            Assert.True(symbolTable.Contains("ZERO_DOUBLE"));
            var symbol = symbolTable.GetSymbol("ZERO_DOUBLE");
            Assert.NotNull(symbol);
            Assert.Equal("double", symbol.type);
            Assert.Equal("0.0", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineString_StringWithSpaces_AddsSymbolCorrectly()
        {
            //Arrange
            var input = "#define GREETING \"Hello from Three Musketeers\"";
            var context = ParseDefineString(input);

            //Act
            var result = analyzer.VisitDefineString(context);

            //Assert
            Assert.Equal("GREETING", result);
            Assert.True(symbolTable.Contains("GREETING"));
            var symbol = symbolTable.GetSymbol("GREETING");
            Assert.NotNull(symbol);
            Assert.Equal("string", symbol.type);
            Assert.Equal("\"Hello from Three Musketeers\"", symbol.constantValue);
        }

        [Fact]
        public void VisitDefineInt_SymbolMarkedAsInitialized()
        {
            //Arrange
            var input = "#define COUNT 42";
            var context = ParseDefineInt(input);

            //Act
            analyzer.VisitDefineInt(context);

            //Assert
            var symbol = symbolTable.GetSymbol("COUNT");
            Assert.NotNull(symbol);
            Assert.True(symbol.isInitializated);
        }

        [Fact]
        public void VisitDefineDouble_SymbolMarkedAsInitialized()
        {
            //Arrange
            var input = "#define RATIO 1.618";
            var context = ParseDefineDouble(input);

            //Act
            analyzer.VisitDefineDouble(context);

            //Assert
            var symbol = symbolTable.GetSymbol("RATIO");
            Assert.NotNull(symbol);
            Assert.True(symbol.isInitializated);
        }

        [Fact]
        public void VisitDefineString_SymbolMarkedAsInitialized()
        {
            //Arrange
            var input = "#define TITLE \"Compiler\"";
            var context = ParseDefineString(input);

            //Act
            analyzer.VisitDefineString(context);

            //Assert
            var symbol = symbolTable.GetSymbol("TITLE");
            Assert.NotNull(symbol);
            Assert.True(symbol.isInitializated);
        }

        private ExprParser.DefineIntContext ParseDefineInt(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.define() as ExprParser.DefineIntContext ?? throw new InvalidOperationException("Failed to parse define int");
        }

        private ExprParser.DefineDoubleContext ParseDefineDouble(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.define() as ExprParser.DefineDoubleContext ?? throw new InvalidOperationException("Failed to parse define double");
        }

        private ExprParser.DefineStringContext ParseDefineString(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            return parser.define() as ExprParser.DefineStringContext ?? throw new InvalidOperationException("Failed to parse define string");
        }
    }
}