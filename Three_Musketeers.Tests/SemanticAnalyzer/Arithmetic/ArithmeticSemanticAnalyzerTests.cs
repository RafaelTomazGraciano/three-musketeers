using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Arithmetic
{
    public class ArithmeticSemanticAnalyzerTests
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
        public void VisitAddSub_IntegerAddition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 3;
                    int c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_IntegerSubtraction_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 4;
                    int c = a - b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_DoubleAddition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 5.5;
                    double b = 3.2;
                    double c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_MixedIntDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    double b = 3.5;
                    double c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_CharAddition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    int b = 2;
                    int c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_InvalidStringAddition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = a + ""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_InvalidStringSubtraction_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = a - ""invalid"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_IntegerMultiplication_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 3;
                    int c = a * b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_IntegerDivision_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 2;
                    int c = a / b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_IntegerModulo_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 3;
                    int c = a % b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_DoubleMultiplication_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 5.5;
                    double b = 2.0;
                    double c = a * b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_DoubleDivision_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 10.0;
                    double b = 2.5;
                    double c = a / b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_MixedIntDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    double b = 2.5;
                    double c = a * b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_ModuloWithDouble_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 10.5;
                    int b = 3;
                    int c = a % b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_ModuloWithBothDoubles_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 10.5;
                    double b = 3.2;
                    double c = a % b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_CharMultiplication_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    int b = 2;
                    int c = a * b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_InvalidStringMultiplication_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = a * ""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_ComplexExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int c = 2;
                    int result = a * b + c / 2 - 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_NestedArithmetic_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int c = (a + b) * (a - b) / 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_BooleanAddition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_CharModulo_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'Z';
                    int b = 3;
                    int c = a % b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAddSub_NegativeNumbers_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = -5;
                    int b = 3;
                    int c = a + b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMulDivMod_ChainedOperations_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 20;
                    int b = 4;
                    int c = 2;
                    int result = a / b / c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}