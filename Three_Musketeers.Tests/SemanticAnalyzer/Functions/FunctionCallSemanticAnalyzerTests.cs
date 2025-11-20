using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Functions
{
    public class FunctionCallSemanticAnalyzerTests
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
        public void VisitFunctionCall_ValidCallNoArgs_NoErrors()
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
        public void VisitFunctionCall_ValidCallWithArgs_NoErrors()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = add(5, 3);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_UndeclaredFunction_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = undeclaredFunc();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_WrongNumberOfArgs_ReportsError()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = add(5);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_TooManyArgs_ReportsError()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = add(5, 3, 2);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_WrongArgumentType_ReportsError()
        {
            //Arrange
            string input = @"
                int processInt(int x) {
                    return x * 2;
                }
                
                int main() {
                    int result = processInt(""text"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_CompatibleArgumentTypes_NoErrors()
        {
            //Arrange
            string input = @"
                int processDouble(double x) {
                    return 1;
                }
                
                int main() {
                    int a = 5;
                    int result = processDouble(a);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_MultipleArguments_NoErrors()
        {
            //Arrange
            string input = @"
                int calculate(int a, int b, int c) {
                    return a + b + c;
                }
                
                int main() {
                    int result = calculate(1, 2, 3);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_NestedFunctionCalls_NoErrors()
        {
            //Arrange
            string input = @"
                int getValue() {
                    return 5;
                }
                
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = add(getValue(), getValue());
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_FunctionReturningPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int *getPointer() {
                    int x = 5;
                    return &x;
                }
                
                int main() {
                    int *ptr = getPointer();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_PassingPointerArgument_NoErrors()
        {
            //Arrange
            string input = @"
                void processPointer(int *ptr) {
                    return;
                }
                
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    processPointer(ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_PassingArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int processInt(int x) {
                    return x * 2;
                }
                
                int main() {
                    int arr[5];
                    arr[0] = 10;
                    int result = processInt(arr[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_PassingAddressOf_NoErrors()
        {
            //Arrange
            string input = @"
                void processPointer(int *ptr) {
                    return;
                }
                
                int main() {
                    int x = 5;
                    processPointer(&x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_VoidFunction_NoErrors()
        {
            //Arrange
            string input = @"
                void doSomething() {
                    return;
                }
                
                int main() {
                    doSomething();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_CharArgument_NoErrors()
        {
            //Arrange
            string input = @"
                int processChar(char c) {
                    return 1;
                }
                
                int main() {
                    int result = processChar('A');
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_DoubleArgument_NoErrors()
        {
            //Arrange
            string input = @"
                int processDouble(double x) {
                    return 1;
                }
                
                int main() {
                    int result = processDouble(3.14);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_ExpressionAsArgument_NoErrors()
        {
            //Arrange
            string input = @"
                int processInt(int x) {
                    return x * 2;
                }
                
                int main() {
                    int a = 5;
                    int b = 3;
                    int result = processInt(a + b);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_FunctionAsArgument_NoErrors()
        {
            //Arrange
            string input = @"
                int getValue() {
                    return 10;
                }
                
                int processInt(int x) {
                    return x * 2;
                }
                
                int main() {
                    int result = processInt(getValue());
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_MixedArgumentTypes_NoErrors()
        {
            //Arrange
            string input = @"
                int calculate(int a, double b, char c) {
                    return 1;
                }
                
                int main() {
                    int result = calculate(5, 3.14, 'A');
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_RecursiveCall_NoErrors()
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
                    int result = factorial(5);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFunctionCall_MultipleWrongArgTypes_ReportsError()
        {
            //Arrange
            string input = @"
                int process(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = process(""text"", ""more"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }
    }
}