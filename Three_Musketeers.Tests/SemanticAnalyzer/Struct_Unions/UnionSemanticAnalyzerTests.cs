using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Struct_Unions
{
    public class UnionSemanticAnalyzerTests
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
        public void VisitUnionStatement_ValidDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
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
        public void VisitUnionStatement_DuplicateDeclaration_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                union Data {
                    char c;
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
        public void VisitUnionStatement_DuplicateMember_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, int i
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
        public void VisitUnionGet_ValidMemberAccess_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_UndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    data.i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_NonExistentMember_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_SwitchBetweenMembers_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d, char c
                };
                
                int main() {
                    union Data data;
                    data.i = 10;
                    data.d = 3.14;
                    data.c = 'A';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_ReadMemberValue_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 10;
                    int val = data.i;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_PointerToUnion_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 0;
                    union Data* ptr;
                    ptr = &data;
                    ptr->i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_DereferencedPointer_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 0;
                    union Data* ptr;
                    ptr = &data;
                    (*ptr).i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_ArrowOnNonPointer_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    union Data data;
                    data->i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_ArrayMember_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int values[10]
                };
                
                int main() {
                    union Data data;
                    data.values[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_ArrayOfUnions_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    union Data dataArray[5];
                    dataArray[0].i = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionStatement_WithDifferentTypes_NoErrors()
        {
            //Arrange
            string input = @"
                union Mixed {
                    int i, double d, char c
                };
                
                int main() {
                    union Mixed m;
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
        public void VisitUnionGet_UseInExpression_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 10;
                    int result = data.i * 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_UseInConditional_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i, double d
                };
                
                int main() {
                    union Data data;
                    data.i = 10;
                    if(data.i > 5) {
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
        public void VisitUnionStatement_MultipleUnions_NoErrors()
        {
            //Arrange
            string input = @"
                union IntData {
                    int i, char c
                };
                
                union FloatData {
                    double d, char c
                };
                
                int main() {
                    union IntData idata;
                    union FloatData fdata;
                    idata.i = 10;
                    fdata.d = 3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_AssignUnionToUnion_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    union Data data1;
                    union Data data2;
                    data1 = data2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_PointerMember_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int *ptr
                };
                
                int main() {
                    union Data data;
                    int x = 5;
                    data.ptr = &x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_NestedStructInUnion_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                union Data {
                    struct Point p;
                    int i;
                };
                
                int main() {
                    union Data data;
                    data.p.x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_MultiDimensionalArrayMember_NoErrors()
        {
            //Arrange
            string input = @"
                union Matrix {
                    int data[3][3]
                };
                
                int main() {
                    union Matrix m;
                    m.data[0][0] = 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionStatement_EmptyUnion_NoErrors()
        {
            //Arrange
            string input = @"
                union Empty {
                    int dummy
                };
                
                int main() {
                    union Empty e;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_DoublePointerMember_NoErrors()
        {
            //Arrange
            string input = @"
                union Container {
                    int **matrix
                };
                
                int main() {
                    union Container c;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_CharArrayMember_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    char str[50], int i
                };
                
                int main() {
                    union Data data;
                    data.str[0] = 'H';
                    data.str[1] = 'i';
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionStatement_WithStructAndUnionMembers_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                union InnerData {
                    int value
                };
                
                union Data {
                    struct Point p,
                    union InnerData inner,
                    double d
                };
                
                int main() {
                    union Data data;
                    data.p.x = 10;
                    data.inner.value = 20;
                    data.d = 3.14;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_ComplexChainWithArrays_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                union Data {
                    struct Point points[3];
                };
                
                int main() {
                    union Data data;
                    data.points[0].x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionGet_TooManyIndices_ReportsError()
        {
            //Arrange
            string input = @"
                union Data {
                    int values[10]
                };
                
                int main() {
                    union Data data;
                    data.values[0][1] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitUnionStatement_LargeUnion_NoErrors()
        {
            //Arrange
            string input = @"
                union LargeData {
                    int i,
                    double d,
                    char c,
                    int arr[100]
                };
                
                int main() {
                    union LargeData data;
                    data.i = 10;
                    data.d = 3.14;
                    data.c = 'A';
                    data.arr[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}