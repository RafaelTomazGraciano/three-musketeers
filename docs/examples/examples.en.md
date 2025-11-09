[English](#english) | [Português](examples.pt.md)

<a name="english"></a>
# Examples

This section provides various code examples demonstrating different features of the Three Musketeers language.

## Hello World

The simplest program:

```c
int main() {
    puts("Hello, World!");
    return 0;
}
```

## Basic Calculations

### Simple Arithmetic

```c
int main() {
    int a = 10;
    int b = 5;
    
    int sum = a + b;
    int diff = a - b;
    int prod = a * b;
    int quot = a / b;
    
    printf("Sum: %d\n", sum);      // 15
    printf("Difference: %d\n", diff);  // 5
    printf("Product: %d\n", prod);    // 50
    printf("Quotient: %d\n", quot);   // 2
    
    return 0;
}
```

### Floating-Point Operations

```c
int main() {
    double pi = 3.14159;
    double radius = 5.0;
    
    double area = pi * radius * radius;
    double circumference = 2 * pi * radius;
    
    printf("Area: %.2f\n", area);           // 78.54
    printf("Circumference: %.2f\n", circumference);  // 31.42
    
    return 0;
}
```

## Control Flow Examples

### If-Else Statement

```c
int main() {
    int score = 85;
    
    if (score >= 90) {
        puts("Grade: A");
    } else if (score >= 80) {
        puts("Grade: B");
    } else if (score >= 70) {
        puts("Grade: C");
    } else {
        puts("Grade: F");
    }
    
    return 0;
}
```

### Switch Statement

```c
int main() {
    int option = 2;
    
    switch (option) {
        case 1:
            puts("Option 1 selected");
            break;
        case 2:
            puts("Option 2 selected");
            break;
        case 3:
            puts("Option 3 selected");
            break;
        default:
            puts("Unknown option");
            break;
    }
    
    return 0;
}
```

### For Loop

```c
int main() {
    int i;
    int sum = 0;
    
    for (i = 1; i <= 10; i++) {
        sum = sum + i;
    }
    
    printf("Sum of 1 to 10: %d\n", sum);  // 55
    return 0;
}
```

### While Loop

```c
int main() {
    int count = 0;
    
    while (count < 5) {
        printf("Count: %d\n", count);
        count++;
    }
    
    return 0;
}
```

### Do-While Loop

```c
int main() {
    int x = 0;
    
    do {
        printf("x = %d\n", x);
        x++;
    } while (x < 5);
    
    return 0;
}
```

## Function Examples

### Simple Function

```c
int add(int x, int y) {
    return x + y;
}

int main() {
    int result = add(5, 3);
    printf("5 + 3 = %d\n", result);  // 8
    return 0;
}
```

### Function with Multiple Parameters

```c
double calculateArea(double width, double height) {
    return width * height;
}

int main() {
    double area = calculateArea(5.5, 3.2);
    printf("Area: %.2f\n", area);  // 17.60
    return 0;
}
```

### Recursive Function

```c
int factorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}

int main() {
    int result = factorial(5);
    printf("5! = %d\n", result);  // 120
    return 0;
}
```

## Array Examples

### One-Dimensional Array

```c
int main() {
    int arr[5];
    int i;
    
    // Initialize array
    for (i = 0; i < 5; i++) {
        arr[i] = i * 10;
    }
    
    // Print array
    for (i = 0; i < 5; i++) {
        printf("arr[%d] = %d\n", i, arr[i]);
    }
    
    return 0;
}
```

### Two-Dimensional Array (Matrix)

```c
int main() {
    int matrix[3][3];
    int i, j;
    
    // Initialize matrix
    for (i = 0; i < 3; i++) {
        for (j = 0; j < 3; j++) {
            matrix[i][j] = i * 3 + j;
        }
    }
    
    // Print matrix
    for (i = 0; i < 3; i++) {
        for (j = 0; j < 3; j++) {
            printf("%d ", matrix[i][j]);
        }
        printf("\n");
    }
    
    return 0;
}
```

## Pointer Examples

### Basic Pointers

```c
int main() {
    int x = 10;
    int *ptr = &x;
    
    printf("x = %d\n", x);           // 10
    printf("*ptr = %d\n", *ptr);      // 10
    
    *ptr = 20;
    printf("x = %d\n", x);           // 20
    
    return 0;
}
```

### Dynamic Memory Allocation

```c
int main() {
    int *arr = malloc(5);
    int i;
    
    // Initialize array
    for (i = 0; i < 5; i++) {
        arr[i] = i * 10;
    }
    
    // Print array
    for (i = 0; i < 5; i++) {
        printf("arr[%d] = %d\n", i, arr[i]);
    }
    
    free(arr);  // Don't forget to free!
    return 0;
}
```

## Structure Examples

### Basic Structure

```c
struct Point {
    int x,
    int y
};

int main() {
    struct Point p;
    p.x = 10;
    p.y = 20;
    
    printf("Point: (%d, %d)\n", p.x, p.y);
    return 0;
}
```

### Structure with Functions

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

int main() {
    struct Point p = createPoint(5, 10);
    printf("Point: (%d, %d)\n", p.x, p.y);
    return 0;
}
```

### Structure with Pointer

```c
struct Point {
    int x,
    int y
};

int main() {
    struct Point p;
    struct Point *ptr = &p;
    
    ptr->x = 10;
    ptr->y = 20;
    
    printf("Point: (%d, %d)\n", ptr->x, ptr->y);
    return 0;
}
```

## Input/Output Examples

### Reading Input

```c
int main() {
    string name;
    int age;
    
    puts("Enter your name:");
    gets(name);
    
    puts("Enter your age:");
    scanf(age);
    
    printf("Hello, %s! You are %d years old.\n", name, age);
    return 0;
}
```

### Type Conversion with Input

```c
int main() {
    string input;
    int number;
    
    puts("Enter a number:");
    gets(input);
    
    number = atoi(input);
    printf("You entered: %d\n", number);
    
    return 0;
}
```

## Complete Program Example

Here's a complete program demonstrating multiple features:

```c
#include <stdio.tm>

struct Student {
    string name,
    int age,
    double grade
};

void printStudent(struct Student s) {
    printf("Name: %s\n", s.name);
    printf("Age: %d\n", s.age);
    printf("Grade: %.2f\n", s.grade);
}

int main() {
    struct Student student;
    
    puts("Enter student name:");
    gets(student.name);
    
    puts("Enter student age:");
    scanf(student.age);
    
    puts("Enter student grade:");
    scanf(student.grade);
    
    printStudent(student);
    
    return 0;
}
```

## Related Topics

- [Getting Started](../getting-started.en.md)
- [Language Reference](../language-reference/)
- [Standard Library](../standard-library/)

