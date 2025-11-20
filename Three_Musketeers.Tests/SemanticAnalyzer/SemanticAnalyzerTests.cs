using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis
{
    public class SemanticAnalyzerTests
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
        public void VisitGenericAtt_ValidIntAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 0;
                    x = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitGenericAtt_InvalidTypeAssignment_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x;
                    x = ""hello"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitGenericAtt_StructToStructAssignment_SameType_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p1;
                    struct Point p2;
                    p1 = p2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitGenericAtt_StructToStructAssignment_DifferentType_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                struct Vector {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p;
                    struct Vector v;
                    p = v;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_ValidAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    arr[0] = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_InvalidTypeAssignment_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    arr[0] = ""test"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_ValidDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    double y = 3.14;
                    char c = 'a';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitVar_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    x = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDerrefExpr_ValidPointerDereference_NoErrors()
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
        public void VisitMulDivMod_ValidArithmetic_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int a = 10;
                    int b = 5;
                    int c = a * b;
                    int d = a / b;
                    int e = a % b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitLogicalAndOr_ValidLogicalOperations_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 1;
                    int y = 0;
                    int z = x && y;
                    int w = x || y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitEquality_ValidComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    int z = x == y;
                    int w = x != y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitComparison_ValidComparison_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    int y = 10;
                    int a = x < y;
                    int b = x > y;
                    int c = x <= y;
                    int d = x >= y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPrefixIncrement_ValidIncrement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    ++x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitPostfixIncrement_ValidIncrement_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    x++;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStm_BreakOutsideLoop_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    break;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStm_BreakInsideLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    while(1) {
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
        public void VisitStm_ContinueOutsideLoop_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    continue;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStm_ContinueInsideLoop_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    for(int i = 0; i < 10; i++) {
                        continue;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructAtt_ValidStructFieldAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructAtt_InvalidTypeAssignment_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p;
                    p.x = ""invalid"";
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitCompoundAssignment_PlusEquals_ValidOperation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    x += 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitCompoundAssignment_MinusEquals_ValidOperation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 15;
                    x -= 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitCompoundAssignment_MultiplyEquals_ValidOperation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 5;
                    x *= 3;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitCompoundAssignment_DivideEquals_ValidOperation_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 20;
                    x /= 4;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_ValidMallocAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int *ptr = (int*)malloc(10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitFreeStatement_ValidFree_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int *ptr = (int*)malloc(10);
                    free(ptr);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}