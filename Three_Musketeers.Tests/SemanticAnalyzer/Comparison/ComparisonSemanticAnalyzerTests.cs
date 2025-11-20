using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Comparison
{
    public class ComparisonSemanticAnalyzerTests
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
        public void VisitComparison_IntegerLessThan_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_IntegerGreaterThan_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int result = a > b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_IntegerLessThanOrEqual_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int result = a <= b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_IntegerGreaterThanOrEqual_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int result = a >= b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_DoubleComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 5.5;
                    double b = 10.3;
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_MixedIntDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    double b = 10.5;
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_CharComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    char b = 'Z';
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_CharWithInt_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    int b = 100;
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_BoolComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int result = a > b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_InvalidStringComparison_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int result = a < ""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_StringWithString_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int result = ""hello"" < ""world"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_PointerWithPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    int *ptr1 = &x;
                    int *ptr2 = &y;
                    int result = ptr1 < ptr2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_PointerWithInt_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int result = ptr > 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_ComplexExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int c = 3;
                    int result = (a + b) > (c * 2);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_ChainedComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int c = 15;
                    int result = (a < b) && (b < c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_NegativeNumbers_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = -5;
                    int b = 10;
                    int result = a < b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_AllOperators_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int r1 = a < b;
                    int r2 = a > b;
                    int r3 = a <= b;
                    int r4 = a >= b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_DoubleWithInt_AllOperators_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 5.5;
                    int b = 10;
                    int r1 = a < b;
                    int r2 = a > b;
                    int r3 = a <= b;
                    int r4 = a >= b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_ZeroComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    int result = x > 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_WithArithmeticOperations_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int result = (a * 2) > (b + 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}