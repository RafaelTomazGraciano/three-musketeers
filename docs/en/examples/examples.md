# Code Examples

A collection of practical examples demonstrating Three Musketeers language features.

## Hello World

The simplest program:

```c
int main() {
    puts("Hello, World!");
    return 0;
}
```

## Variables and Types

```c
int main() {
    int count = 42;
    double pi = 3.14159;
    char letter = 'A';
    bool active = true;
    string greeting = "Hello";
    
    printf("Count: %d\n", count);
    printf("Pi: %.5f\n", pi);
    printf("Letter: %c\n", letter);
    printf("Active: %d\n", active);
    printf("Greeting: %s\n", greeting);
    
    return 0;
}
```

## Arrays

```c
int main() {
    int numbers[5];
    int i;
    
    // Initialize array
    for (i = 0; i < 5; i++) {
        numbers[i] = i * 10;
    }
    
    // Print array
    for (i = 0; i < 5; i++) {
        printf("numbers[%d] = %d\n", i, numbers[i]);
    }
    
    return 0;
}
```

## Functions

```c
int add(int x, int y) {
    return x + y;
}

double calculateArea(double width, double height) {
    return width * height;
}

int main() {
    int sum = add(10, 20);
    double area = calculateArea(5.5, 3.2);
    
    printf("Sum: %d\n", sum);
    printf("Area: %.2f\n", area);
    
    return 0;
}
```

## Control Flow

### If-Else

```c
void checkValue(int value) {
    if (value > 0) {
        printf("Value %d is positive\n", value);
    } else if (value < 0) {
        printf("Value %d is negative\n", value);
    } else {
        printf("Value %d is zero\n", value);
    }
}

int main() {
    checkValue(5);
    checkValue(-3);
    checkValue(0);
    return 0;
}
```

### Switch

```c
void processOption(int option) {
    switch (option) {
        case 1:
            printf("Option 1 selected\n");
            break;
        case 2:
            printf("Option 2 selected\n");
            break;
        case 3:
            printf("Option 3 selected\n");
            break;
        default:
            printf("Unknown option: %d\n", option);
            break;
    }
}

int main() {
    processOption(1);
    processOption(2);
    processOption(4);
    return 0;
}
```

### Loops

```c
int main() {
    int i;
    
    // For loop
    printf("For loop:\n");
    for (i = 1; i <= 5; i++) {
        printf("Iteration %d\n", i);
    }
    
    // While loop
    printf("\nWhile loop:\n");
    int count = 1;
    while (count <= 5) {
        printf("Count: %d\n", count);
        count = count + 1;
    }
    
    // Do-while loop
    printf("\nDo-while loop:\n");
    int num = 1;
    do {
        printf("Number: %d\n", num);
        num = num + 1;
    } while (num <= 5);
    
    return 0;
}
```

## Pointers

```c
void swap(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10, y = 20;
    
    printf("Before swap: x=%d, y=%d\n", x, y);
    swap(&x, &y);
    printf("After swap: x=%d, y=%d\n", x, y);
    
    return 0;
}
```

## Dynamic Memory

```c
int main() {
    int *arr = malloc(5);
    int i;
    
    // Initialize array
    for (i = 0; i < 5; i++) {
        arr[i] = i * 2;
    }
    
    // Print array
    for (i = 0; i < 5; i++) {
        printf("arr[%d] = %d\n", i, arr[i]);
    }
    
    free(arr);
    return 0;
}
```

## Structures

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
    p.x = 10;
    p.y = 20;
    printPoint(p);
    
    return 0;
}
```

## Input/Output

```c
int main() {
    string name;
    int age;
    double height;
    
    printf("Enter your name: ");
    gets(name);
    
    printf("Enter your age: ");
    scanf(age);
    
    printf("Enter your height (meters): ");
    scanf(height);
    
    printf("\n--- Your Information ---\n");
    printf("Name: %s\n", name);
    printf("Age: %d\n", age);
    printf("Height: %.2f meters\n", height);
    
    return 0;
}
```

## Type Conversion

```c
int main() {
    // String to number
    string numStr = "42";
    int num = atoi(numStr);
    double value = atod("3.14159");
    
    printf("Integer: %d\n", num);
    printf("Double: %.5f\n", value);
    
    // Number to string
    int age = 25;
    double price = 19.99;
    string ageStr = itoa(age);
    string priceStr = dtoa(price);
    
    printf("Age as string: %s\n", ageStr);
    printf("Price as string: %s\n", priceStr);
    
    return 0;
}
```

## Recursion

```c
int factorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}

int main() {
    int result = factorial(5);
    printf("5! = %d\n", result);
    return 0;
}
```

## Complete Example

A more comprehensive example combining multiple features:

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
    struct Student students[3];
    int i;
    
    for (i = 0; i < 3; i++) {
        printf("Enter student %d name: ", i + 1);
        gets(students[i].name);
        
        printf("Enter student %d age: ", i + 1);
        scanf(students[i].age);
        
        printf("Enter student %d grade: ", i + 1);
        scanf(students[i].grade);
    }
    
    printf("\n--- Student List ---\n");
    for (i = 0; i < 3; i++) {
        printStudent(students[i]);
        printf("\n");
    }
    
    return 0;
}
```

## Related Topics

- [Language Reference](../language-reference/README.md) - Detailed language documentation
- [Getting Started](../getting-started/hello-world.md) - Tutorials for beginners

