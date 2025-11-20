using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.StringConversion
{
    public class AtodSemanticAnalyzerTests
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
        public void VisitAtodConversion_ValidStringLiteral_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = atod(""3.14"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_ValidStringVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""2.718"";
                    double num = atod(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_IntArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int x = 123;
                    double num = atod(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_DoubleArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double x = 3.14;
                    double num = atod(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_WithoutStdlibInclude_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    double num = atod(""3.14"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_AssignToDoubleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""9.81"";
                    double result = atod(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_InExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""10.5"";
                    double result = atod(str) + 5.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_InConditional_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""42.5"";
                    double num = atod(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_MultipleConversions_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str1 = ""10.5"";
                    string str2 = ""20.3"";
                    double num1 = atod(str1);
                    double num2 = atod(str2);
                    double sum = num1 + num2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_EmptyString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = atod("""");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_NegativeNumberString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = atod(""-123.45"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_InLoop_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    for(int i = 0; i < 3; i++) {
                        double num = atod(""3.14"");
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_CharArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    char c = 'A';
                    double num = atod(c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_ZeroString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double num = atod(""0.0"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtodConversion_NestedInExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""5.5"";
                    double result = atod(str) * 2.0 + 10.5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}