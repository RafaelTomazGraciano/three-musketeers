[English](#english) | [Português](types.pt.md)

<a name="english"></a>
# Types and Variables

Three Musketeers is a strongly-typed language with implicit type conversion. This means all variables must be declared with a type, and the compiler automatically performs type conversions when needed.

## Primitive Types

Three Musketeers supports five primitive types:

### `int`

Used to represent signed integers.

```c
int age = 25;
int count = -10;
int zero = 0;
```

### `double`

Used to represent floating-point numbers.

```c
double pi = 3.14159;
double temperature = -5.5;
double ratio = 0.75;
```

### `char`

Used to represent a single character.

```c
char letter = 'A';
char digit = '5';
char symbol = '@';
```

### `bool`

Used to represent boolean logic. Values are `true` and `false`.

```c
bool isActive = true;
bool isComplete = false;
bool result = (5 > 3);  // true
```

### `string`

Represents a fixed sequence of characters (string literal).

```c
string greeting = "Hello, World!";
string name = "Three Musketeers";
string empty = "";
```

## Variable Declaration

Variables must be declared before use. The syntax is:

```c
type variable_name;
```

Or with initialization:

```c
type variable_name = value;
```

### Examples

```c
int x;
int y = 10;
double pi = 3.14159;
bool flag = true;
char ch = 'A';
string msg = "Hello";
```

## Arrays

Arrays can be created by adding `[]` with the number of dimensions. Array indices start at 0.

### One-Dimensional Arrays

```c
int numbers[5];           // Array of 5 integers
double values[10];        // Array of 10 doubles
char buffer[100];         // Array of 100 characters
```

### Multi-Dimensional Arrays (Matrices)

```c
int matrix[3][3];        // 3x3 matrix
double grid[5][10];      // 5x10 matrix
int cube[2][2][2];       // 2x2x2 three-dimensional array
```

### Array Initialization and Access

```c
int arr[5];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;

int value = arr[0];      // value = 10

int matrix[2][2];
matrix[0][0] = 1;
matrix[0][1] = 2;
matrix[1][0] = 3;
matrix[1][1] = 4;
```

## Pointers

Pointers store memory addresses. Use `*` to declare a pointer type.

```c
int *ptr;                // Pointer to int
double *dptr;            // Pointer to double
char *cptr;              // Pointer to char
int **pptr;              // Pointer to pointer to int
```

### Pointer Operations

```c
int x = 10;
int *ptr = &x;           // ptr points to x
int value = *ptr;        // Dereference: value = 10
*ptr = 20;               // Modify x through pointer
```

## Type Conversion

Three Musketeers performs implicit type conversion automatically. The compiler converts between compatible types as needed.

### Conversion Rules

- `int` can be converted to `double`
- `double` can be converted to `int` (truncation)
- `char` can be converted to `int` (ASCII value)
- `int` can be converted to `char` (if within valid range)

### Examples

```c
int i = 10;
double d = i;            // int to double: d = 10.0

double pi = 3.14159;
int n = pi;              // double to int: n = 3 (truncated)

char c = 'A';
int code = c;            // char to int: code = 65 (ASCII)

int value = 66;
char ch = value;         // int to char: ch = 'B'
```

For explicit string conversions, see [Type Conversion Functions](../standard-library/conversions.en.md).

## Global Variables

Variables can be declared outside functions as global variables:

```c
int globalCounter;
double globalValue = 3.14;
string globalMessage = "Global variable";

int main() {
    globalCounter = 42;
    return 0;
}
```

## Related Topics

- [Operators](operators.en.md)
- [Memory Management](memory.en.md)
- [Structures and Unions](structures.en.md)
- [Type Conversion Functions](../standard-library/conversions.en.md)

