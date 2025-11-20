using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Logical
{
    public class LogicalSemanticAnalyzerTests
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
        public void VisitLogicalAndOr_IntegerAnd_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int result = a && b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_IntegerOr_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int result = a || b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_BooleanAnd_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    int y = 0;
                    int result = x && y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_BooleanOr_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    int y = 0;
                    int result = x || y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_DoubleOperands_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double a = 1.5;
                    double b = 0.0;
                    int result = a && b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_CharOperands_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char a = 'A';
                    char b = 'B';
                    int result = a && b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_MixedIntDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    double b = 2.5;
                    int result = a && b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_InvalidStringOperand_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int result = a && ""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_BothStringOperands_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int result = ""hello"" && ""world"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_IntegerOperand_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    int result = !x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_BooleanOperand_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int flag = 1;
                    int result = !flag;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_DoubleOperand_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 5.5;
                    int result = !x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_CharOperand_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    int result = !c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_InvalidStringOperand_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int result = !""text"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_ComplexExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int c = 1;
                    int result = (a && b) || c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_ChainedAnd_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 1;
                    int c = 1;
                    int result = a && b && c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_ChainedOr_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 0;
                    int b = 0;
                    int c = 1;
                    int result = a || b || c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_DoubleNegation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    int result = !!x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_WithComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int result = (a > 0) && (b < 20);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_WithEquality_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int result = (a == 5) && (b == 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_WithComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int result = !(x > 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_ZeroOperands_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int result = 0 && 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalNot_ZeroOperand_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int result = !0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_MixedAndOr_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 1;
                    int b = 0;
                    int c = 1;
                    int d = 0;
                    int result = (a && b) || (c && d);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}