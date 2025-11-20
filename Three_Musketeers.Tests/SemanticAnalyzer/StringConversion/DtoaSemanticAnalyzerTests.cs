using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.StringConversion
{
    public class DtoaSemanticAnalyzerTests
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
        public void VisitDtoaConversion_ValidDoubleLiteral_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = dtoa(3.14);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_ValidDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = 2.718;
                    string str = dtoa(num);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_IntArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int x = 123;
                    string str = dtoa(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_StringArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string s = ""hello"";
                    string result = dtoa(s);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_WithoutStdlibInclude_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    string str = dtoa(3.14);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_AssignToStringVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = 9.81;
                    string result = dtoa(num);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_NegativeNumber_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = -42.5;
                    string str = dtoa(num);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_ZeroValue_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = dtoa(0.0);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_WithExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double a = 10.5;
                    double b = 20.3;
                    string str = dtoa(a + b);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_MultipleConversions_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num1 = 10.5;
                    double num2 = 20.3;
                    string str1 = dtoa(num1);
                    string str2 = dtoa(num2);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
       public void VisitDtoaConversion_InLoop_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    for(int i = 0; i < 5; i++) {
                        double value = 3.14;
                        string str = dtoa(value);
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_CharArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    char c = 'A';
                    string str = dtoa(c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_LargeNumber_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = 999999.999;
                    string str = dtoa(num);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_WithArithmeticExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double x = 5.5;
                    string str = dtoa(x * 2.0 + 10.5);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDtoaConversion_SmallDecimal_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = 0.001;
                    string str = dtoa(num);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}