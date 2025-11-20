# Structures and Unions

Structures and unions allow you to create custom composite types that group related data together.

## Structures

A structure (struct) groups multiple variables of potentially different types under a single name.

### Struct Declaration

```c
struct Point {
    int x,
    int y
};
```

### Struct Variable Declaration

```c
struct Point p;
p.x = 10;
p.y = 20;
```

### Struct Initialization

```c
struct Point p;
p.x = 10;
p.y = 20;
```

### Accessing Struct Members

Use the `.` operator to access struct members:

```c
struct Point p;
p.x = 10;
p.y = 20;
printf("Point: (%d, %d)\n", p.x, p.y);
```

### Struct with Arrays

```c
struct Data {
    int values[5],
    int count
};

struct Data d;
d.values[0] = 10;
d.count = 1;
```

### Nested Structures

Structures can contain other structures:

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

### Pointers to Structures

Use the `->` operator with pointers to structures:

```c
struct Point {
    int x,
    int y
};

struct Point p;
struct Point *ptr = &p;
ptr->x = 10;
ptr->y = 20;
```

### Structures as Function Parameters

```c
#include <stdio.tm>

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

Functions can return structures:

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

## Unions

A union allows you to store different types of data in the same memory location. Only one member can be used at a time.

### Union Declaration

```c
union Data {
    int i,
    double d,
    char c
};
```

### Using Unions

```c
union Data data;
data.i = 10;      // Store as int
printf("%d\n", data.i);

data.d = 3.14;    // Now store as double (overwrites int)
printf("%f\n", data.d);
```

**Important**: Accessing a union member that wasn't the last one written results in undefined behavior.

### Union Use Cases

Unions are useful when you need to store different types in the same memory location:

```c
union Value {
    int intValue,
    double doubleValue,
    bool boolValue
};

void printValue(union Value v, string type) {
    if (type == "int") {
        printf("%d\n", v.intValue);
    } else if (type == "double") {
        printf("%f\n", v.doubleValue);
    }
}
```

## Differences: Struct vs Union

| Feature | Struct | Union |
|---------|--------|-------|
| Memory | Each member has its own memory | All members share the same memory |
| Size | Sum of all member sizes | Size of largest member |
| Usage | All members can be used simultaneously | Only one member at a time |

## Example: Complete Program

```c
#include <stdio.tm>

struct Point {
    int x,
    int y
};

union Number {
    int i,
    double d
};

int main() {
    struct Point p;
    p.x = 10;
    p.y = 20;
    
    union Number num;
    num.i = 42;
    printf("Integer: %d\n", num.i);
    
    num.d = 3.14;
    printf("Double: %f\n", num.d);
    
    return 0;
}
```

## Related Topics

- [Data Types](data-types.md) - Basic types
- [Pointers](pointers.md) - Pointers to structures
- [Functions](functions.md) - Functions with structures

