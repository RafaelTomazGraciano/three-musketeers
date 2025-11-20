# Input and Output

Three Musketeers provides several functions for reading input and displaying output, similar to C's standard I/O functions.

## Output Functions

### printf

Print formatted output to the console.

**Syntax:**
```c
printf(format_string, arguments...);
```

**Format Specifiers:**

| Format | Type   | Description              |
|--------|--------|--------------------------|
| `%d`   | int    | Integer                  |
| `%f`   | double | Floating point number    |
| `%s`   | string | String                   |
| `%c`   | char   | Single character         |

**Examples:**

```c
int age = 25;
double price = 19.99;
string name = "Three Musketeers";
char initial = 'T';

printf("Age: %d\n", age);
printf("Price: %.2f\n", price);
printf("Name: %s\n", name);
printf("Initial: %c\n", initial);
```

**Output:**
```
Age: 25
Price: 19.99
Name: Three Musketeers
Initial: T
```

### puts

Print a string to the console followed by a newline.

**Syntax:**
```c
puts(string_variable);
puts("string literal");
```

**Examples:**

```c
string greeting = "Hello, World!";
puts(greeting);
puts("Three Musketeers");
```

**Output:**
```
Hello, World!
Three Musketeers
```

## Input Functions

### scanf

Read formatted input from the console.

**Syntax:**
```c
scanf(variable1, variable2, ...);
```

**Examples:**

```c
int age;
double price;
string name;

printf("Enter your age: ");
scanf(age);

printf("Enter price: ");
scanf(price);

printf("Enter name: ");
scanf(name);
```

**Note:** `scanf` automatically determines the type based on the variable type.

### gets

Read a single string from the console.

**Syntax:**
```c
gets(string_variable);
```

**Example:**

```c
string name;
printf("Enter your name: ");
gets(name);
printf("Hello, %s!\n", name);
```

## Complete I/O Example

```c
#include <stdio.tm>

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

## Formatting with printf

### Precision for Floating Point

```c
double pi = 3.14159265359;
printf("%.2f\n", pi);   // 3.14
printf("%.4f\n", pi);   // 3.1416
```

### Field Width

```c
int num = 42;
printf("%5d\n", num);   // "   42" (right-aligned, 5 characters wide)
printf("%-5d\n", num);  // "42   " (left-aligned, 5 characters wide)
```

### Multiple Format Specifiers

```c
int x = 10, y = 20;
double z = 3.14;
printf("x=%d, y=%d, z=%.2f\n", x, y, z);
```

## Reading Multiple Values

```c
int a, b, c;
printf("Enter three integers: ");
scanf(a, b, c);
printf("You entered: %d, %d, %d\n", a, b, c);
```

## Error Handling

Always ensure variables are initialized before reading into them:

```c
int value;
scanf(value);  // Safe - value will be set by scanf
```

## Related Topics

- [Variables](variables.md) - Variable declarations
- [Type Conversion](type-conversion.md) - Converting between types for I/O
- [Examples](../examples/examples.md) - More I/O examples

