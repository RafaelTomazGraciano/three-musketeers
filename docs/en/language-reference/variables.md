# Variables

Variables in Three Musketeers must be declared with an explicit type before use. The language is strongly typed with implicit type conversion.

## Variable Declaration

### Basic Declaration

```c
int count;
double price;
char initial;
bool isReady;
string name;
```

### Declaration with Initialization

```c
int count = 10;
double pi = 3.14159;
char letter = 'A';
bool active = true;
string greeting = "Hello";
```

### Multiple Declarations

You can declare multiple variables of the same type:

```c
int x, y, z;
double a = 1.0, b = 2.0, c = 3.0;
```

## Array Variables

### One-Dimensional Arrays

```c
int numbers[5];
double prices[10];
char buffer[100];
```

### Multi-Dimensional Arrays

```c
int matrix[3][3];
double grid[5][5][5];
```

### Array Initialization

Arrays are accessed using zero-based indexing:

```c
int arr[3];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
```

## Pointer Variables

Pointers are declared using the `*` operator:

```c
int *ptr;
double *dptr;
int **doublePtr;  // Pointer to pointer
```

See [Pointers](pointers.md) for detailed information.

## Global Variables

Variables declared outside of functions are global:

```c
int globalCounter;
double globalValue = 3.14;
string globalMessage = "Global";

int main() {
    globalCounter = 42;
    return 0;
}
```

## Variable Scope

- **Global Scope**: Variables declared outside functions are accessible throughout the program
- **Local Scope**: Variables declared inside functions are only accessible within that function

## Type Conversion

The compiler automatically performs implicit type conversion:

```c
int x = 10;
double y = x;        // int converted to double
int z = y;           // double converted to int (truncated)
```

## Related Topics

- [Data Types](data-types.md) - Available types
- [Operators](operators.md) - Operations on variables
- [Pointers](pointers.md) - Working with pointers

