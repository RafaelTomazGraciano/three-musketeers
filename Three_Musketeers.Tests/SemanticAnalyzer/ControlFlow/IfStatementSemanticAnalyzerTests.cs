using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.ControlFlow
{
    public class IfStatementSemanticAnalyzerTests
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
        public void VisitIfStatement_ValidBooleanCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_ValidIntegerCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    if (x) {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_ValidDoubleCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    double x = 3.14;
                    if (x) {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_ValidCharCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    if (c) {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_InvalidStringCondition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    if (""hello"") {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_WithElseBlock_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 10) {
                        int y = 1;
                    } else {
                        int z = 2;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_WithElseIfBlock_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 10) {
                        int y = 1;
                    } else if (x > 5) {
                        int z = 2;
                    } else {
                        int w = 3;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_MultipleElseIfBlocks_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 20) {
                        int a = 1;
                    } else if (x > 15) {
                        int b = 2;
                    } else if (x > 10) {
                        int c = 3;
                    } else if (x > 5) {
                        int d = 4;
                    } else {
                        int e = 5;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_InvalidElseIfCondition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 10) {
                        int y = 1;
                    } else if (""invalid"") {
                        int z = 2;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_NestedIfStatements_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    if (x > 0) {
                        if (y > 5) {
                            int z = 20;
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
        public void VisitIfStatement_VariableScopeIsolation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        int y = 10;
                    }
                    if (x < 10) {
                        int y = 20;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_AccessOuterScopeVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        x = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_ComplexConditionExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    if ((x > 0) && (y < 20)) {
                        int z = 15;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_ComparisonCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    if (x == y) {
                        int z = 1;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_LogicalOrCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    if (x > 10 || y > 5) {
                        int z = 1;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_NegatedCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (!x) {
                        int y = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_EmptyBlocks_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                    } else {
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_MultipleStatementsInBlock_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    if (x > 0) {
                        int y = 10;
                        int z = 20;
                        x = y + z;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_WithReturnStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int func() {
                    int x = 5;
                    if (x > 0) {
                        return 1;
                    }
                    return 0;
                }
                
                int main() {
                    int result = func();
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_TrueLiteralCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    if (true) {
                        int x = 10;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitIfStatement_FalseLiteralCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    if (false) {
                        int x = 10;
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