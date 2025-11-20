using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis
{
    public class SemanticAnalyzerBaseTests
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
        public void VisitStart_EmptyProgram_NoErrors()
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
        public void VisitStart_WithIncludes_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                #include <stdlib.tm>
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_WithDefines_NoErrors()
        {
            //Arrange
            string input = @"
                #define MAX 100
                #define PI 3.14
                #define NAME ""Test""
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_WithGlobalDeclarations_NoErrors()
        {
            //Arrange
            string input = @"
                int globalVar = 10;
                double globalDouble = 3.14;
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_WithStructDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_WithUnionDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_WithFunctionDeclaration_NoErrors()
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
        public void VisitStart_DuplicateFunctionDeclaration_ReportsError()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int add(int x, int y) {
                    return x + y;
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
        public void CollectFunctionSignatures_VoidReturnType_NoErrors()
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
        public void CollectFunctionSignatures_PointerReturnType_NoErrors()
        {
            //Arrange
            string input = @"
                int* getPointer() {
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
        public void CollectFunctionSignatures_MultipleParameters_NoErrors()
        {
            //Arrange
            string input = @"
                int compute(int a, double b, char c) {
                    return a;
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
        public void CollectFunctionSignatures_DuplicateParameterNames_ReportsError()
        {
            //Arrange
            string input = @"
                int add(int a, int a) {
                    return a;
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
        public void CollectFunctionSignatures_PointerParameters_NoErrors()
        {
            //Arrange
            string input = @"
                void process(int* ptr, double* dptr) {
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
        public void GetExpressionType_IntLiteral_ReturnsInt()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 42;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_DoubleLiteral_ReturnsDouble()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_StringLiteral_ReturnsString()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf(""Hello"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_CharLiteral_ReturnsChar()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_BoolLiteral_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = true;
                    int y = false;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_AtoiConversion_ReturnsInt()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int x = atoi(""123"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_AtodConversion_ReturnsDouble()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double x = atod(""3.14"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_ItoaConversion_ReturnsString()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int x = 123;
                    itoa(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_DtoaConversion_ReturnsString()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double d = 3.14;
                    dtoa(d);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_AdditionIntInt_ReturnsInt()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5 + 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_AdditionIntDouble_ReturnsDouble()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 5 + 3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_MultiplicationIntInt_ReturnsInt()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5 * 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_DivisionIntDouble_ReturnsDouble()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 10 / 3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_Modulo_ReturnsInt()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10 % 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_LogicalAnd_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1 && 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_LogicalOr_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1 || 0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_LogicalNot_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = !1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_Comparison_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5 > 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_Equality_ReturnsBool()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5 == 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_UnaryMinus_PreservesType()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = -5;
                    double y = -3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_ParenthesizedExpression_PreservesType()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = (5 + 3);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_FunctionCall_ReturnsCorrectType()
        {
            //Arrange
            string input = @"
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    int result = add(5, 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_FunctionCallPointerReturn_ReturnsPointerType()
        {
            //Arrange
            string input = @"
                int* getPointer() {
                    int x = 5;
                    return &x;
                }
                
                int main() {
                    int* ptr = getPointer();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_VariableReference_ReturnsCorrectType()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_StructMemberAccess_ReturnsCorrectType()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    int val = p.x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void GetExpressionType_PointerDereference_ReturnsPointeeType()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int* ptr = &x;
                    int val = (*ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_ComplexProgram_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                #include <stdlib.tm>
                
                #define MAX 100
                
                struct Point {
                    int x, int y
                };
                
                int globalVar = 0;
                
                int add(int a, int b) {
                    return a + b;
                }
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    p.y = 20;
                    int sum = add(p.x, p.y);
                    printf(""Sum: %d"", sum);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_OrderOfDeclarations_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                struct Point globalPoint;
                
                void usePoint(struct Point p) {
                    int x = p.x;
                }
                
                int main() {
                    usePoint(globalPoint);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CollectFunctionSignatures_NoParameters_NoErrors()
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
        public void GetExpressionType_ComplexExpression_CorrectTypePromotion()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 5;
                    double b = 3.14;
                    double result = (a + b) * 2.0;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStart_MultipleGlobalDeclarations_NoErrors()
        {
            //Arrange
            string input = @"
                int x = 1;
                double y = 2.0;
                char c = 'A';
                int arr[10];
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void CollectFunctionSignatures_StructParameterAndReturn_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                struct Point createPoint(int x, int y) {
                    struct Point p;
                    p.x = x;
                    p.y = y;
                    return p;
                }
                
                int main() {
                    struct Point p = createPoint(5, 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}