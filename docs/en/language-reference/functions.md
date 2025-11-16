# Functions

Functions allow you to organize code into reusable blocks. Three Musketeers supports functions with various return types and parameters.

## Function Declaration

### Basic Syntax

```c
return_type function_name(parameters) {
    // Function body
    return value;  // If return_type is not void
}
```

### Void Functions

Functions that don't return a value:

```c
void printWelcome() {
    puts("Welcome to Three Musketeers!");
}
```

### Functions with Return Values

```c
int add(int x, int y) {
    return x + y;
}
```

## Function Parameters

Functions can accept zero or more parameters:

```c
// No parameters
int getValue() {
    return 42;
}

// Single parameter
int square(int n) {
    return n * n;
}

// Multiple parameters
int multiply(int a, int b, int c) {
    return a * b * c;
}
```

## Return Types

Functions can return any of the basic types:

```c
int getInt() { return 10; }
double getDouble() { return 3.14; }
bool isPositive(int n) { return n > 0; }
char getChar() { return 'A'; }
string getString() { return "Hello"; }
```

## Function Calls

Call a function by using its name followed by parentheses:

```c
int result = add(5, 3);
printWelcome();
double area = calculateArea(10.5, 20.0);
```

## Main Function

Every Three Musketeers program must have a `main` function as the entry point:

```c
int main() {
    // Program code
    return 0;
}
```

### Main with Arguments

The main function can accept command-line arguments:

```c
int main(int argc, char argv[]) {
    printf("Argument count: %d\n", argc);
    if (argc > 1) {
        puts(argv[1]);
    }
    return 0;
}
```

- `argc`: Number of command-line arguments
- `argv`: Array of strings containing the arguments

## Function Examples

### Simple Function

```c
void greet(string name) {
    printf("Hello, %s!\n", name);
}

int main() {
    greet("Three Musketeers");
    return 0;
}
```

### Function with Multiple Return Types

```c
int add(int x, int y) {
    return x + y;
}

double calculateArea(double width, double height) {
    return width * height;
}

bool isEven(int n) {
    return n % 2 == 0;
}
```

### Nested Function Calls

```c
int square(int n) {
    return n * n;
}

int add(int x, int y) {
    return x + y;
}

int main() {
    int result = square(add(3, 4));  // square(7) = 49
    printf("Result: %d\n", result);
    return 0;
}
```

### Recursive Functions

Functions can call themselves:

```c
int factorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}
```

## Function Scope

Variables declared inside a function are local to that function:

```c
int function1() {
    int x = 10;  // Local to function1
    return x;
}

int function2() {
    int x = 20;  // Different x, local to function2
    return x;
}
```

## Related Topics

- [Pointers](pointers.md) - Functions with pointer parameters
- [Control Flow](control-flow.md) - Using control flow in functions
- [Examples](../examples/examples.md) - More function examples

