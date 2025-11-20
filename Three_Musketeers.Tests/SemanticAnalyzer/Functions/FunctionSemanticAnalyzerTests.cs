using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Functions
{
    public class FunctionSemanticAnalyzerTests
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
        public void AnalyzeFunction_ValidFunctionWithReturn_NoErrors()
        {
            //Arrange
            string input = @"
                int getValue() {
                    return 42;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_ValidFunctionWithParameters_NoErrors()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_VoidFunctionWithoutReturn_NoErrors()
        {
            //Arrange
            string input = @"
                void doSomething() {
                    int x = 5;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_VoidFunctionWithReturn_NoErrors()
        {
            //Arrange
            string input = @"
                void doSomething() {
                    return;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_VoidFunctionReturningValue_ReportsError()
        {
            //Arrange
            string input = @"
                void doSomething() {
                    return 5;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionWithPointerParameter_NoErrors()
        {
            //Arrange
            string input = @"
                void processPointer(int *ptr) {
                    return;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionReturningPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int *getPointer() {
                    int x = 5;
                    return &x;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_MultipleParameters_NoErrors()
        {
            //Arrange
            string input = @"
                int calculate(int a, int b, int c) {
                    return a + b + c;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_MixedParameterTypes_NoErrors()
        {
            //Arrange
            string input = @"
                int process(int a, double b, char c) {
                    return 1;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_UsingParameterInBody_NoErrors()
        {
            //Arrange
            string input = @"
                int square(int x) {
                    return x * x;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_NestedScopes_NoErrors()
        {
            //Arrange
            string input = @"
                int processValue(int x) {
                    if (x > 0) {
                        int y = x * 2;
                        return y;
                    }
                    return 0;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_RecursiveFunction_NoErrors()
        {
            //Arrange
            string input = @"
                int factorial(int n) {
                    if (n <= 1) {
                        return 1;
                    }
                    return n * factorial(n - 1);
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionWithLocalVariables_NoErrors()
        {
            //Arrange
            string input = @"
                int calculate() {
                    int a = 5;
                    int b = 10;
                    int sum = a + b;
                    return sum;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_DoubleReturnType_NoErrors()
        {
            //Arrange
            string input = @"
                double getPI() {
                    return 3.14;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_CharReturnType_NoErrors()
        {
            //Arrange
            string input = @"
                char getChar() {
                    return 'A';
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_EmptyFunction_NoErrors()
        {
            //Arrange
            string input = @"
                void doNothing() {
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionWithLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int sum(int n) {
                    int total = 0;
                    for (int i = 0; i < n; i++) {
                        total = total + i;
                    }
                    return total;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionWithMultipleReturns_NoErrors()
        {
            //Arrange
            string input = @"
                int abs(int x) {
                    if (x < 0) {
                        return -x;
                    }
                    return x;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionCallingOtherFunction_NoErrors()
        {
            //Arrange
            string input = @"
                int getValue() {
                    return 10;
                }
                
                int process() {
                    return getValue() * 2;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void AnalyzeFunction_FunctionWithArrayParameter_NoErrors()
        {
            //Arrange
            string input = @"
                int sumArray(int *arr, int size) {
                    int total = 0;
                    for (int i = 0; i < size; i++) {
                        total = total + arr[i];
                    }
                    return total;
                }
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}