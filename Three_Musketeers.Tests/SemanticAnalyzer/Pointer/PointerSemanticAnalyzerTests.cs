using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Pointer
{
    public class PointerSemanticAnalyzerTests
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
        public void VisitExprAddress_ValidIntVariable_NoErrors()
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
        public void VisitExprAddress_ValidCharVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    char *ptr = &c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int *ptr = &undeclared;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    int *ptr = &arr[0];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_MultipleVariables_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    int *ptr1 = &x;
                    int *ptr2 = &y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_PointerToPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int **pptr = &ptr;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerref_ValidPointerDereference_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int y = (*ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerref_AssignThroughPointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    (*ptr) = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerref_DoublePointerDereference_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    int **pptr = &ptr;
                    int y = (*(*pptr));
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_BoolVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int flag = 1;
                    int *ptr = &flag;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_StructMember_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p;
                    p.x = 5;
                    int *ptr = &p.x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_InFunctionCall_NoErrors()
        {
            //Arrange
            string input = @"
                void modify(int *ptr) {
                    (*ptr) = 10;
                    return;
                }
                
                int main() {
                    int x = 5;
                    modify(&x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int matrix[3][3];
                    int *ptr = &matrix[0][0];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_CharArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char str[50];
                    char *ptr = &str[0];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_InArithmeticExpression_NoErrors()
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
        public void VisitExprAddress_MultiplePointersToSameVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr1 = &x;
                    int *ptr2 = &x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_PointerComparison_NoErrors()
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
        public void VisitExprAddress_NullComparison_NoErrors()
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
        public void VisitDerref_ReadValue_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 42;
                    int *ptr = &x;
                    int value = (*ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerref_ModifyValue_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int *ptr = &x;
                    (*ptr) = (*ptr) + 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_WithMalloc_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int *ptr;
                    ptr = malloc(10);
                    int **pptr = &ptr;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerref_WithMalloc_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int *ptr;
                    ptr = malloc(10);
                    (*ptr) = 42;
                    int value = (*ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitExprAddress_InConditional_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if(x > 0) {
                        int *ptr = &x;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}