# Data Types

Three Musketeers provides five fundamental data types, ordered from most generic to most specific.

## Basic Types

### 1. double

Used to represent floating-point numbers (decimal numbers).

```c
double pi = 3.14159;
double temperature = 98.6;
```

### 2. int

Used to represent signed integers (whole numbers).

```c
int count = 42;
int age = 25;
```

### 3. char

Used to represent a single character.

```c
char letter = 'A';
char newline = '\n';
```

### 4. bool

Used to represent boolean logic (`true` or `false`).

```c
bool isActive = true;
bool isComplete = false;
```

### 5. string

Represents a fixed sequence of characters (string literal).

```c
string greeting = "Hello, World!";
string message = "Three Musketeers";
```

## Type Hierarchy

The types are ordered from most generic (double) to most specific (string). This ordering is important for implicit type conversion:

```
double → int → char → bool → string
```

## Type Conversion

The compiler automatically converts between compatible types. For example:

```c
int x = 10;
double y = x;  // int automatically converted to double
```

## Arrays

Arrays can be created by adding `[]` with dimension sizes:

```c
int numbers[10];           // One-dimensional array
double matrix[3][3];       // Two-dimensional array
char buffer[100];          // Character array
```

Array indices start at 0.

## User-Defined Types

You can create custom types using structures and unions:

```c
struct Point {
    int x,
    int y
};

union Data {
    int i,
    double d
};
```

See [Structures and Unions](structs-unions.md) for more information.

## Related Topics

- [Variables](variables.md) - How to declare and use variables
- [Type Conversion](type-conversion.md) - Explicit type conversion functions
- [Arrays](arrays.md) - Working with arrays

