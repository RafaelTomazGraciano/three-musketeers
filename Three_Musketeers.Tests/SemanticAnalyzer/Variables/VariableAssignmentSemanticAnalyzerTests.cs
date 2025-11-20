using Antlr4.Runtime;
using Three_Musketeers.Grammar;
using Three_Musketeers.Visitors;

namespace Three_Musketeers.Tests.SemanticAnalysis.Variables
{
    public class VariableAssignmentSemanticAnalyzerTests
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
        public void VisitAtt_ValidAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_AssignToUndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    x = 10;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_ReassignExistingVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x = 20;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_DuplicateDeclaration_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int x = 20;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_StructAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point p = p;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_UndefinedStructType_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    struct Point p;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_PointerAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int* ptr = &x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_UnionAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    union Data data;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_SimpleVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_MultipleTypes_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x;
                    double y;
                    char c;
                    bool b;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_ArrayDeclaration_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_MultiDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int matrix[3][3];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_ArrayWithZeroSize_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[0];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_ArrayWithValidSize_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[5];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_StringArray_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    string arr[10];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_PointerVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int* ptr;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_DoublePointer_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int** ptr;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_StructVariable_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point p;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_StructPointer_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point* ptr;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_StructArray_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point points[5];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_UnionVariable_NoErrors()
        {
            //Arrange
            string input = @"
                union Data {
                    int i,
                    double d
                };
                
                int main() {
                    union Data data;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitVar_DeclaredVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int y = x;
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
                    int y = x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitVar_UninitializedVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x;
                    int y = x;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_ValidIndexAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    arr[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_UndeclaredArray_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    arr[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_NonArrayVariable_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_TooManyIndices_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    arr[0][1] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_OutOfBoundsIndex_ReportsError()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    arr[15] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_ValidIndexInBounds_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    arr[5] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_MultiDimensionalAccess_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int matrix[3][3];
                    matrix[0][0] = 1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_PointerIndexing_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int arr[10];
                    int* ptr = arr;
                    ptr[0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_IntToIntAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int arr[10];
                    arr[0] = 42;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_ValidMalloc_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int* ptr = (int*)malloc(10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_MallocWithoutType_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int* ptr;
                    ptr = (int*)malloc(10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_MallocUndeclaredVariable_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int* ptr = malloc(10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_MallocDuplicateDeclaration_ReportsError()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int* ptr = (int*)malloc(10);
                    int* ptr = (int*)malloc(20);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.True(analyzer.hasErrors);
        }

        [Fact]
        public void VisitMallocAtt_MallocDoublePointer_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int** ptr = (int**)malloc(10);
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_ChainedAssignments_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    int y = 20;
                    int z = 30;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_CharArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    char str[50];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_PlusEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x += 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_MinusEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x -= 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_MultiplyEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x *= 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_DivideEquals_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int x = 10;
                    x /= 2;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_NestedStructMemberAssignment_NoErrors()
        {
            //Arrange
            string input = @"
                struct Inner {
                    int value
                };
                
                struct Outer {
                    struct Inner inner
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
        public void VisitDeclaration_PointerToStructArray_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point* arr[10];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitSingleArrayAtt_DoublePointerIndexing_NoErrors()
        {
            //Arrange
            string input = @"
                #include <stdlib.tm>
                
                int main() {
                    int** matrix = (int**)malloc(10);
                    matrix[0] = (int*)malloc(10);
                    matrix[0][0] = 5;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitVar_StructVariable_NoErrors()
        {
            //Arrange
            string input = @"
                struct Point {
                    int x,
                    int y
                };
                
                int main() {
                    struct Point p1;
                    struct Point p2 = p1;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitAtt_BooleanVariable_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    bool flag = true;
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }

        [Fact]
        public void VisitDeclaration_ThreeDimensionalArray_NoErrors()
        {
            //Arrange
            string input = @"
                int main() {
                    int cube[3][3][3];
                    return 0;
                }";

            //Act
            var analyzer = CreateAnalyzer(input);

            //Assert
            Assert.False(analyzer.hasErrors);
        }
    }
}