using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Functions
{
    public class MainFunctionSemanticAnalyzerTests
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
        public void AnalyzeMainFunction_ValidMainWithReturn_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithoutReturn_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithArguments_NoErrors()
        {
            //Arrange
            string input = @"
                int main(int argc, char argv[]) {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainUsingArgc_NoErrors()
        {
            //Arrange
            string input = @"
                int main(int argc, char argv[]) {
                    int count = argc;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainUsingArgv_NoErrors()
        {
            //Arrange
            string input = @"
                int main(int argc, char argv[]) {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithLocalVariables_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int y = 20;
                    int sum = x + y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithFunctionCall_NoErrors()
        {
            //Arrange
            string input = @"
                int getValue() {
                    return 42;
                }
                
                int main() {
                    int x = getValue();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithIfStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        x = x + 1;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        int x = i * 2;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithWhileLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    while (i < 10) {
                        i = i + 1;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    arr[0] = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainReturningNonZero_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    return 1;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithMultipleStatements_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    int b = 10;
                    int c = a + b;
                    return c;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithSwitch_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 2;
                    switch (x) {
                        case 1:
                            break;
                        case 2:
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithDoWhile_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    do {
                        i = i + 1;
                    } while (i < 5);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithNestedScopes_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        int y = 10;
                        if (y > 5) {
                            int z = 15;
                        }
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithArithmetic_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int sum = a + b;
                    int diff = a - b;
                    int prod = a * b;
                    int quot = a / b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainReturningExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    return x + y;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithCompoundAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += 5;
                    x -= 2;
                    x *= 3;
                    x /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeMainFunction_MainWithIncrementDecrement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    x++;
                    ++x;
                    x--;
                    --x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}