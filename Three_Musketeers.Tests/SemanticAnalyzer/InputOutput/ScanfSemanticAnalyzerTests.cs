using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.InputOutput
{
    public class ScanfSemanticAnalyzerTests
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
        public void VisitScanfStatement_ValidIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 0;
                    scanf(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    double pi = 0.0;
                    scanf(pi);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_ValidCharVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    char letter = 'a';
                    scanf(letter);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_MultipleVariables_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 0;
                    int y = 0;
                    scanf(x, y);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    scanf(undeclared);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_StringVariable_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    scanf(""text"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_ConstantVariable_ReportsError()
        {
            //Arrange
            string input = @"
                #define MAX 100
                #include <stdio.tm>
                
                int main() {
                    scanf(MAX);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_ArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int arr[5];
                    scanf(arr[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int matrix[3][3];
                    scanf(matrix[0][0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_IncompleteArrayIndex_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int matrix[3][3];
                    scanf(matrix[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_TooManyIndices_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int arr[5];
                    scanf(arr[0][1]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_NonArrayWithIndex_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 0;
                    scanf(x[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_WithoutStdioInclude_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    scanf(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_MixedTypes_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int num = 0;
                    double dec = 0.0;
                    char ch = 'a';
                    scanf(num, dec, ch);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_BoolVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int flag = 0;
                    scanf(flag);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_PointerVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                #include <stdlib.tm>
                
                int main() {
                    int *ptr = (int*)malloc(10);
                    scanf(ptr[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_CharArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    char str[50];
                    scanf(str[0]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_DoubleArrayElement_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    double values[10];
                    scanf(values[5]);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitScanfStatement_ThreeVariables_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    scanf(a, b, c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}