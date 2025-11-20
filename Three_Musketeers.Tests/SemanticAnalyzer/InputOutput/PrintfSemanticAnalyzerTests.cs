using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.InputOutput
{
    public class PrintfSemanticAnalyzerTests
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
        public void VisitPrintfStatement_SimpleStringLiteral_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf(""Hello World"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_IntegerFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 42;
                    printf(""Value: %d"", x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_DoubleFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    double pi = 3.14;
                    printf(""PI: %f"", pi);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_CharFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    char letter = 'A';
                    printf(""Letter: %c"", letter);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_StringFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf(""Name: %s"", ""John"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_MultipleFormats_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int age = 25;
                    double height = 1.75;
                    printf(""Age: %d, Height: %f"", age, height);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_WrongArgumentCount_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 10;
                    printf(""Value: %d"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_TooManyArguments_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 10;
                    int y = 20;
                    printf(""Value: %d"", x, y);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_WrongTypeForFormat_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 10;
                    printf(""Value: %s"", x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_IntWithCharFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 65;
                    printf(""Char: %c"", x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_CharWithIntFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    char c = 'A';
                    printf(""ASCII: %d"", c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_NoFormatSpecifiers_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf(""No formats here"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_ThreeArguments_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int a = 1;
                    int b = 2;
                    int c = 3;
                    printf(""Values: %d %d %d"", a, b, c);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_MixedTypes_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int num = 42;
                    double dec = 3.14;
                    char ch = 'X';
                    printf(""Int: %d, Double: %f, Char: %c"", num, dec, ch);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_ExpressionAsArgument_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int a = 5;
                    int b = 3;
                    printf(""Sum: %d"", a + b);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_BoolWithIntFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int flag = 1;
                    printf(""Flag: %d"", flag);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_WithoutStdioInclude_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    printf(""Hello"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_NewlineCharacter_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf(""Hello\n"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_MultipleNewlines_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 10;
                    printf(""Value: %d\n"", x);
                    printf(""Done\n"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_EmptyString_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    printf("""");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrintfStatement_IntegerIFormat_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdio.tm>
                
                int main() {
                    int x = 42;
                    printf(""Value: %i"", x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}