using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.ControlFlow
{
    public class SwitchStatementSemanticAnalyzerTests
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
        public void VisitSwitchStatement_ValidIntExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            break;
                        case 2:
                            int z = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_ValidCharExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    switch (c) {
                        case 'A':
                            int x = 1;
                            break;
                        case 'B':
                            int y = 2;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_ValidBoolExpression_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int b = 1;
                    switch (b) {
                        case 0:
                            int x = 1;
                            break;
                        case 1:
                            int y = 2;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_InvalidStringExpression_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    switch (""hello"") {
                        case 1:
                            int x = 1;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_InvalidDoubleExpression_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    double d = 3.14;
                    switch (d) {
                        case 1:
                            int x = 1;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_WithDefaultCase_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            break;
                        case 2:
                            int z = 20;
                            break;
                        default:
                            int w = 30;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_OnlyDefaultCase_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        default:
                            int y = 10;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_DuplicateCaseValues_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            break;
                        case 1:
                            int z = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_DuplicateCharCaseValues_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    char c = 'A';
                    switch (c) {
                        case 'A':
                            int x = 1;
                            break;
                        case 'A':
                            int y = 2;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_WithBreakStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            break;
                        case 2:
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_EmptyCaseBlock_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                        case 2:
                            int y = 10;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_MultipleStatementsInCase_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            int z = 20;
                            int w = y + z;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_NestedSwitch_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    switch (x) {
                        case 1:
                            switch (y) {
                                case 10:
                                    int z = 20;
                                    break;
                            }
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_VariableScopeIsolation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            break;
                        case 2:
                            int y = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_AccessOuterScopeVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int result = 0;
                    switch (x) {
                        case 1:
                            result = 10;
                            break;
                        case 2:
                            result = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_NegativeIntCaseValue_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = -5;
                    switch (x) {
                        case -1:
                            int y = 10;
                            break;
                        case -5:
                            int z = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_MixedIntAndCharCases_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 65;
                    switch (x) {
                        case 65:
                            int y = 1;
                            break;
                        case 'B':
                            int z = 2;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_WithIfStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            if (x > 0) {
                                int y = 10;
                            }
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_WithLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            for (int i = 0; i < 10; i++) {
                                int y = i;
                            }
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_ExpressionCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    switch (x + y) {
                        case 15:
                            int z = 1;
                            break;
                        case 20:
                            int w = 2;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_LargeCaseValues_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1000;
                    switch (x) {
                        case 100:
                            int a = 1;
                            break;
                        case 200:
                            int b = 2;
                            break;
                        case 1000:
                            int c = 3;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_EmptySwitch_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_CaseWithoutBreak_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                        case 2:
                            int z = 20;
                            break;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSwitchStatement_DefaultWithoutBreak_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    switch (x) {
                        case 1:
                            int y = 10;
                            break;
                        default:
                            int z = 20;
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