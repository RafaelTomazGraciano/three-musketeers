using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Equality
{
    public class EqualitySemanticAnalyzerTests
    {
        private SemanticAnalyzer CreateAnalyzer(string input)
        {
            var inputStream = new AntlrInputStream(input);
            var lexer = new ExprLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            var tree = parser.start();
            
            var analyzer = new SemanticAnalyzer();
            analyzer.Visit(tree);
            
            return analyzer;
        }

        [Fact]
        public void VisitEquality_IntegerEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 5;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_IntegerNotEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int result = a != b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_DoubleEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 5.5;
                    double b = 5.5;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_MixedIntDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    double b = 5.0;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_CharEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    char b = 'A';
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_CharWithInt_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    int b = 65;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_BoolWithInt_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_StringWithChar_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    int result = c == 'A';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_PointerWithPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    int *ptr1 = &x;
                    int *ptr2 = &y;
                    int result = ptr1 == ptr2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_PointerWithInt_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int result = ptr == 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_PointerNotEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int result = ptr != 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_InvalidStringWithInt_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int result = a == ""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_InvalidPointerWithDouble_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    double d = 5.5;
                    int result = ptr == d;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_ComplexExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int c = 15;
                    int result = (a + b) == c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_ChainedEquality_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 5;
                    int c = 5;
                    int result = (a == b) && (b == c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_ZeroComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    int result = x == 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_NegativeNumbers_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = -5;
                    int b = -5;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_BothOperators_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int r1 = a == b;
                    int r2 = a != b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_WithArithmeticOperations_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int result = (a * 2) == (b + 15);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_SameType_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 5;
                    int result = a == b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}