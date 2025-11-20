using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Struct_Unions
{
    public class StructSemanticAnalyzerTests
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
        public void VisitStructStatement_ValidDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructStatement_DuplicateDeclaration_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                struct Point {
                    int z;
                };
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructStatement_DuplicateMember_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int x
                };
                
                int main() {
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ValidMemberAccess_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
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
        public void VisitStructGet_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    p.x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_NonExistentMember_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.z = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_NestedStruct_NoErrors()
        {
            //Arrange
            string input = @"
                struct Inner {
                    int value;
                };
                
                struct Outer {
                    struct Inner inner;
                };
                
                int main() {
                    struct Outer o;
                    o.inner.value = 42;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_PointerMemberWithArrow_NoErrors()
        {
            //Arrange
            string input = @"
                struct Node {
                    int data, struct Node *next
                };
                
                int main() {
                    struct Node n;
                    struct Node n2;
                    n.next = &n2;
                    n.next->data = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ArrowOnNonPointer_ReportsError()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point p;
                    p->x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ArrayMember_NoErrors()
        {
            //Arrange
            string input = @"
                struct Data {
                    int values[10];
                };
                
                int main() {
                    struct Data d;
                    d.values[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ArrayOfStructs_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x;
                    int y;
                };
                
                int main() {
                    struct Point points[5];
                    points[0].x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_TooManyIndices_ReportsError()
        {
            //Arrange
            string input = @"
                struct Data {
                    int values[10];
                };
                
                int main() {
                    struct Data d;
                    d.values[0][1] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                struct Matrix {
                    int data[3][3];
                };
                
                int main() {
                    struct Matrix m;
                    m.data[0][0] = 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructStatement_WithDifferentTypes_NoErrors()
        {
            //Arrange
            string input = @"
                struct Mixed {
                    int i, double d, char c
                };
                
                int main() {
                    struct Mixed m;
                    m.i = 10;
                    m.d = 3.14;
                    m.c = 'A';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ReadMemberValue_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    int val = p.x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_PointerToStruct_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 0;
                    p.y = 0;
                    struct Point* ptr;
                    ptr = &p;
                    ptr->x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_DereferencedPointer_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 0;
                    p.y = 0;
                    struct Point* ptr;
                    ptr = &p;
                    (*ptr).x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ThreeLevelNesting_NoErrors()
        {
            //Arrange
            string input = @"
                struct Level3 {
                    int value;
                };
                
                struct Level2 {
                    struct Level3 l3;
                };
                
                struct Level1 {
                    struct Level2 l2;
                };
                
                int main() {
                    struct Level1 l1;
                    l1.l2.l3.value = 100;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructStatement_EmptyStruct_NoErrors()
        {
            //Arrange
            string input = @"
                struct Empty {
                    int dummy;
                };
                
                int main() {
                    struct Empty e;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_AssignStructToStruct_NoErrors()
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
        public void VisitStructGet_PointerMemberInNestedStruct_NoErrors()
        {
            //Arrange
            string input = @"
                struct Inner {
                    int *ptr;
                };
                
                struct Outer {
                    struct Inner inner;
                };
                
                int main() {
                    struct Outer o;
                    int x = 5;
                    o.inner.ptr = &x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_ComplexChainWithArrays_NoErrors()
        {
            //Arrange
            string input = @"
                struct Data {
                    int values[5];
                };
                
                struct Container {
                    struct Data data[3];
                };
                
                int main() {
                    struct Container c;
                    c.data[0].values[2] = 42;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_UseInExpression_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    p.y = 20;
                    int sum = p.x + p.y;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_UseInConditional_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                int main() {
                    struct Point p;
                    p.x = 10;
                    if(p.x > 5) {
                        int a = 1;
                    }
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructStatement_MultipleStructs_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x, int y
                };
                
                struct Circle {
                    struct Point center, int radius
                };
                
                int main() {
                    struct Circle c;
                    c.center.x = 0;
                    c.center.y = 0;
                    c.radius = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitStructGet_DoublePointerMember_NoErrors()
        {
            //Arrange
            string input = @"
                struct Container {
                    int **matrix
                };
                
                int main() {
                    struct Container c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}