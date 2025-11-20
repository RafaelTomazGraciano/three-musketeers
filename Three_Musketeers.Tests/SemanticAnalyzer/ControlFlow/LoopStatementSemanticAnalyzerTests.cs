using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.ControlFlow
{
    public class LoopStatementSemanticAnalyzerTests
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
        public void VisitForStatement_ValidForLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_EmptyInitialization_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    for (; i < 10; i++) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_EmptyCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; ; i++) {
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
        public void VisitForStatement_EmptyIncrement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; ) {
                        i++;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_AllPartsEmpty_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (; ; ) {
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
        public void VisitForStatement_InvalidStringCondition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; ""invalid""; i++) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_ValidBooleanCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i > 0 && i < 10; i++) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_NestedForLoops_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        for (int j = 0; j < 5; j++) {
                            int x = i + j;
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
        public void VisitForStatement_WithBreakStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        if (i == 5) {
                            break;
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
        public void VisitForStatement_WithContinueStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        if (i == 5) {
                            continue;
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
        public void VisitForStatement_VariableScopeIsolation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 5; i++) {
                        int x = 10;
                    }
                    for (int i = 0; i < 5; i++) {
                        int x = 20;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_ValidWhileLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    while (i < 10) {
                        i++;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_ValidBooleanCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    while (x) {
                        x = 0;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_InvalidStringCondition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    while (""invalid"") {
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
        public void VisitWhileStatement_WithBreakStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    while (i < 10) {
                        if (i == 5) {
                            break;
                        }
                        i++;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_WithContinueStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    while (i < 10) {
                        i++;
                        if (i == 5) {
                            continue;
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
        public void VisitWhileStatement_NestedWhileLoops_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    while (i < 5) {
                        int j = 0;
                        while (j < 3) {
                            j++;
                        }
                        i++;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_ComplexCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    int y = 10;
                    while (x < 5 && y > 0) {
                        x++;
                        y--;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_ValidDoWhileLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    do {
                        i++;
                    } while (i < 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_ValidBooleanCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    do {
                        x = 0;
                    } while (x);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_InvalidStringCondition_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    do {
                        break;
                    } while (""invalid"");
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_WithBreakStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    do {
                        if (i == 5) {
                            break;
                        }
                        i++;
                    } while (i < 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_WithContinueStatement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    do {
                        i++;
                        if (i == 5) {
                            continue;
                        }
                    } while (i < 10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_NestedDoWhileLoops_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int i = 0;
                    do {
                        int j = 0;
                        do {
                            j++;
                        } while (j < 3);
                        i++;
                    } while (i < 5);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_ComplexCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    int y = 10;
                    do {
                        x++;
                        y--;
                    } while (x < 5 && y > 0);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void MixedLoops_ForWhileDoWhile_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 5; i++) {
                        int j = 0;
                        while (j < 3) {
                            int k = 0;
                            do {
                                k++;
                            } while (k < 2);
                            j++;
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
        public void VisitForStatement_IncrementWithCompoundAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i += 2) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitForStatement_MultipleVariablesInInit_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for (int i = 0; i < 10; i++) {
                        int x = i;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitWhileStatement_TrueLiteralCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    while (true) {
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
        public void VisitWhileStatement_ComparisonCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    while (x < y) {
                        x++;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDoWhileStatement_FalseLiteralCondition_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    do {
                        int x = 10;
                    } while (false);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}