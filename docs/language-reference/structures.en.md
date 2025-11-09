[English](#english) | [Português](structures.pt.md)

<a name="english"></a>
# Structures and Unions

Three Musketeers supports user-defined composite types through structures (`struct`) and unions (`union`).

## Structures

Structures allow you to group related data together into a single type.

### Structure Declaration

```c
struct Point {
    int x,
    int y
};

struct Person {
    string name,
    int age,
    double height
};
```

### Structure Variable Declaration

```c
struct Point p1;
struct Point p2;
```

### Accessing Structure Members

Use the dot operator (`.`) to access structure members.

```c
struct Point p;
p.x = 10;
p.y = 20;

int xCoord = p.x;
int yCoord = p.y;
```

### Structure Initialization

```c
struct Point p;
p.x = 5;
p.y = 10;
```

### Structures with Arrays

Structures can contain arrays.

```c
struct myStruct {
    int a,
    int b[5]
};

struct myStruct s;
s.a = 10;
s.b[0] = 1;
s.b[1] = 2;
s.b[2] = 3;
```

### Nested Structures

Structures can contain other structures.

```c
struct Point {
    int x,
    int y
};

struct Rectangle {
    struct Point topLeft,
    struct Point bottomRight
};

struct Rectangle rect;
rect.topLeft.x = 0;
rect.topLeft.y = 0;
rect.bottomRight.x = 10;
rect.bottomRight.y = 10;
```

### Structures as Function Parameters

Structures can be passed to functions by value.

```c
struct Point {
    int x,
    int y
};

void printPoint(struct Point p) {
    printf("Point: (%d, %d)\n", p.x, p.y);
}

int main() {
    struct Point p;
    p.x = 5;
    p.y = 10;
    printPoint(p);
    return 0;
}
```

### Returning Structures

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

## Pointers to Structures

You can use pointers with structures and access members using the arrow operator (`->`).

### Structure Pointers

```c
struct Point {
    int x,
    int y
};

struct Point p;
struct Point *ptr = &p;

ptr->x = 10;        // Access through pointer
ptr->y = 20;       // Equivalent to (*ptr).y

int x = ptr->x;    // Read through pointer
```

### Arrow Operator (->)

The arrow operator (`->`) is a shorthand for dereferencing and accessing a member.

```c
struct Point *ptr = &p;
ptr->x = 10;       // Same as (*ptr).x = 10
```

### Dot vs Arrow

- Use `.` when you have a structure variable
- Use `->` when you have a pointer to a structure

```c
struct Point p;
struct Point *ptr = &p;

p.x = 10;          // Dot operator
ptr->x = 20;       // Arrow operator
```

## Unions

Unions are similar to structures, but all members share the same memory space. Only one member can be used at a time.

### Union Declaration

```c
union Data {
    int i,
    double d,
    char c
};
```

### Union Usage

```c
union Data data;

data.i = 10;       // Store as int
printf("%d\n", data.i);

data.d = 3.14;     // Now store as double (overwrites int)
printf("%f\n", data.d);

data.c = 'A';      // Now store as char (overwrites double)
printf("%c\n", data.c);
```

### Union with Structures

Unions can contain structures.

```c
struct Point {
    int x,
    int y
};

union myUnion {
    int g,
    struct Point point
};

union myUnion u;
u.g = 100;
// or
u.point.x = 10;
u.point.y = 20;
```

## Preprocessor Directives

### #include

Include header files using the `#include` directive.

```c
#include <stdio.tm>
#include <stdlib.tm>
#include "myheader.tm"
```

### #define

Define constants using the `#define` directive.

```c
#define MAX_SIZE 100
#define PI 3.14159
#define MESSAGE "Hello, World!"

int arr[MAX_SIZE];
double circumference = 2 * PI * radius;
puts(MESSAGE);
```

## Related Topics

- [Types and Variables](types.en.md)
- [Memory Management](memory.en.md)
- [Functions](functions.en.md)

