using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.StringConversion
{
    public class AtoiSemanticAnalyzerTests
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
        public void VisitAtoiConversion_ValidStringLiteral_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int num = atoi(""123"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_ValidStringVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""456"";
                    int num = atoi(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_IntArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int x = 123;
                    int num = atoi(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_DoubleArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    double x = 3.14;
                    int num = atoi(x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_WithoutStdlibInclude_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int num = atoi(""123"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_AssignToIntVariable_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""789"";
                    int result = atoi(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_InExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""10"";
                    int result = atoi(str) + 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_InConditional_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""42"";
                    int num = atoi(str);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }


        [Fact]
        public void VisitAtoiConversion_MultipleConversions_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str1 = ""10"";
                    string str2 = ""20"";
                    int num1 = atoi(str1);
                    int num2 = atoi(str2);
                    int sum = num1 + num2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_EmptyString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int num = atoi("""");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_NegativeNumberString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int num = atoi(""-123"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_InLoop_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    for(int i = 0; i < 3; i++) {
                        int num = atoi(""123"");
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_CharArgument_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    char c = 'A';
                    int num = atoi(c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_InReturnStatement_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""42"";
                    return atoi(str);
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtoiConversion_NestedInExpression_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    string str = ""5"";
                    int result = atoi(str) * 2 + 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}