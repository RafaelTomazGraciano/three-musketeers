[English](#english) | [Português](functions.pt.md)

<a name="english"></a>
# Functions

Functions allow you to organize code into reusable blocks. Three Musketeers supports function declarations, parameters, return values, and recursion.

## Function Declaration

The general syntax for a function is:

```c
return_type function_name(parameters) {
    // function body
    return value;  // if return_type is not void
}
```

## Function Return Types

Functions can return a value of any type, or `void` if they don't return anything.

### Functions with Return Values

```c
int add(int x, int y) {
    return x + y;
}

double calculateArea(double width, double height) {
    return width * height;
}

bool isPositive(int num) {
    return num > 0;
}

char getFirstLetter(int code) {
    char letter = 'A';
    return letter;
}
```

### void Functions

Functions that don't return a value use `void` as the return type.

```c
void printWelcome() {
    puts("=== Three Musketeers ===");
}

void printMessage(string msg) {
    puts(msg);
}
```

## Function Parameters

Functions can accept zero or more parameters. Parameters are passed by value.

### No Parameters

```c
void greet() {
    puts("Hello!");
}
```

### Single Parameter

```c
void printNumber(int n) {
    printf("Number: %d\n", n);
}
```

### Multiple Parameters

```c
int multiply(int a, int b, int c) {
    return a * b * c;
}

double calculateDistance(double x1, double y1, double x2, double y2) {
    double dx = x2 - x1;
    double dy = y2 - y1;
    return dx * dx + dy * dy;
}
```

### Pointer Parameters

Functions can accept pointers as parameters, allowing them to modify the original values.

```c
void increment(int *ptr) {
    (*ptr)++;
}

int main() {
    int x = 10;
    increment(&x);
    printf("x = %d\n", x);  // x = 11
    return 0;
}
```

## Function Calls

Call a function by using its name followed by parentheses containing the arguments.

```c
int result = add(5, 3);           // result = 8
double area = calculateArea(5.0, 3.0);  // area = 15.0
bool check = isPositive(10);      // check = true
printWelcome();                   // Calls void function
```

### Nested Function Calls

Functions can be called within other function calls.

```c
int square(int n) {
    return multiply(n, n, 1);
}

int result = square(5);           // result = 25
int complex = add(5, 3) * multiply(2, 2, 2);  // 8 * 8 = 64
```

## Recursion

Functions can call themselves, enabling recursive algorithms.

```c
int factorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}

int fibonacci(int n) {
    if (n <= 1) {
        return n;
    }
    return fibonacci(n - 1) + fibonacci(n - 2);
}

void recursionPrint(string message, int count) {
    if (count <= 0) {
        return;
    }
    printf("%s - count: %d\n", message, count);
    recursionPrint(message, count - 1);
}
```

## Function Scope

Variables declared inside a function are local to that function and cannot be accessed from outside.

```c
int globalVar = 100;

void myFunction() {
    int localVar = 50;
    printf("Global: %d, Local: %d\n", globalVar, localVar);
    // localVar is only accessible within myFunction
}

int main() {
    myFunction();
    // localVar is not accessible here
    return 0;
}
```

## Returning Structures

Functions can return structures.

```c
struct Point {
    int x,
    int y
};

struct Point createPoint(int x, int y) {
    struct Point p;
    p.x = x;
    p.y = y;
    return p;
}
```

## Returning Pointers

Functions can return pointers.

```c
int *createArray(int size) {
    int *arr = malloc(size);
    return arr;
}
```

## Function Declaration Order

Functions must be declared before they are called, or you can define them in any order if you're calling from `main` or other functions defined later.

```c
// Forward declaration not required - functions can be defined in any order
int main() {
    int result = add(5, 3);
    return 0;
}

int add(int x, int y) {
    return x + y;
}
```

## Related Topics

- [Types and Variables](types.en.md)
- [Control Flow](control-flow.en.md)
- [Memory Management](memory.en.md)
- [Structures and Unions](structures.en.md)

