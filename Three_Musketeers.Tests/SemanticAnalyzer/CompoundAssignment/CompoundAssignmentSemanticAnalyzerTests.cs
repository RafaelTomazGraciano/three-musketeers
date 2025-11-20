using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.CompoundAssignment
{
    public class CompoundAssignmentSemanticAnalyzerTests
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
        public void VisitSingleAttPlusEquals_ValidIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttPlusEquals_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 10.5;
                    x += 5.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttPlusEquals_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    x += 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttPlusEquals_UninitializedVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x;
                    x += 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttPlusEquals_StringVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    string s = ""hello"";
                    s += ""world"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttPlusEquals_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    arr[0] = 10;
                    arr[0] += 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMinusEquals_ValidIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 20;
                    x -= 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMinusEquals_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 20.5;
                    x -= 5.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMinusEquals_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    y -= 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMinusEquals_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[3];
                    arr[1] = 15;
                    arr[1] -= 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMultiplyEquals_ValidIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    x *= 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMultiplyEquals_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 5.5;
                    x *= 2.0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMultiplyEquals_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    z *= 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttMultiplyEquals_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[4];
                    arr[2] = 5;
                    arr[2] *= 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttDivideEquals_ValidIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 20;
                    x /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttDivideEquals_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 20.5;
                    x /= 2.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttDivideEquals_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    w /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleAttDivideEquals_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    arr[3] = 20;
                    arr[3] /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_AllOperators_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 20;
                    int c = 30;
                    int d = 40;
                    a += 5;
                    b -= 5;
                    c *= 2;
                    d /= 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_MixedTypes_IntAndDouble_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    double y = 5.5;
                    x += 5;
                    y -= 2.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_CharVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    c += 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int matrix[3][3];
                    matrix[0][0] = 5;
                    matrix[0][0] += 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_ChainedOperations_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += 5;
                    x -= 3;
                    x *= 2;
                    x /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_WithExpressions_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int y = 5;
                    x += y + 3;
                    y *= x - 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_InsideLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int sum = 0;
                    for(int i = 0; i < 10; i++) {
                        sum += i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_InsideIfStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    if(x > 5) {
                        x += 5;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_BoolVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int b = 1;
                    b += 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_ZeroValue_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += 0;
                    x -= 0;
                    x *= 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CompoundAssignment_NegativeValue_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += -5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}