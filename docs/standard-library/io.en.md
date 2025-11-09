[English](#english) | [Português](io.pt.md)

<a name="english"></a>
# Input/Output Functions

Three Musketeers provides several built-in functions for reading from and writing to the console.

## Output Functions

### printf

The `printf` function prints formatted output to the console. It takes a format string and optional arguments.

**Syntax:**
```c
printf(format_string, arg1, arg2, ...);
```

**Format Specifiers:**

| Specifier | Type    | Description                    |
|-----------|---------|--------------------------------|
| `%d`      | int     | Signed decimal integer         |
| `%f`      | double  | Floating-point number          |
| `%s`      | string  | String                         |
| `%c`      | char    | Single character               |

**Examples:**

```c
int age = 25;
double height = 5.9;
string name = "John";
char grade = 'A';

printf("Name: %s\n", name);              // Name: John
printf("Age: %d\n", age);                // Age: 25
printf("Height: %f\n", height);          // Height: 5.900000
printf("Grade: %c\n", grade);            // Grade: A

// Multiple format specifiers
printf("Name: %s, Age: %d, Height: %.2f\n", name, age, height);
```

**Formatting Options:**

You can specify precision for floating-point numbers:

```c
double pi = 3.14159;
printf("Pi: %.2f\n", pi);     // Pi: 3.14 (2 decimal places)
printf("Pi: %.4f\n", pi);    // Pi: 3.1416 (4 decimal places)
```

### puts

The `puts` function prints a string followed by a newline character.

**Syntax:**
```c
puts(string_literal);
puts(string_variable);
```

**Examples:**

```c
puts("Hello, World!");        // Prints: Hello, World!

string message = "Welcome to Three Musketeers";
puts(message);                 // Prints: Welcome to Three Musketeers

string greeting = "Hello";
puts(greeting);                // Prints: Hello
```

**Note:** `puts` automatically adds a newline at the end, unlike `printf` which requires `\n`.

## Input Functions

### scanf

The `scanf` function reads formatted input from the console. It reads multiple values of different types.

**Syntax:**
```c
scanf(variable1, variable2, ...);
```

**Examples:**

```c
int age;
double height;
char initial;

scanf(age, height, initial);
// User enters: 25 5.9 A
// age = 25, height = 5.9, initial = 'A'
```

**Reading Multiple Values:**

```c
int x, y, z;
scanf(x, y, z);
// User enters: 10 20 30
// x = 10, y = 20, z = 30
```

**Note:** `scanf` reads values separated by whitespace. The variables must be declared before use.

### gets

The `gets` function reads a single string from the console until a newline is encountered.

**Syntax:**
```c
gets(string_variable);
```

**Examples:**

```c
string name;
gets(name);
// User enters: Three Musketeers
// name = "Three Musketeers"

string input;
gets(input);
puts(input);  // Prints what was read
```

**Important:** The variable must be of type `string` and must be declared before calling `gets`.

## Complete Examples

### Example 1: Basic I/O

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

### Example 2: Calculator Input

```c
int main() {
    double num1, num2;
    
    puts("Enter two numbers:");
    scanf(num1, num2);
    
    double sum = num1 + num2;
    double product = num1 * num2;
    
    printf("Sum: %.2f\n", sum);
    printf("Product: %.2f\n", product);
    return 0;
}
```

### Example 3: Character Input

```c
int main() {
    char ch;
    string word;
    
    puts("Enter a character:");
    scanf(ch);
    
    puts("Enter a word:");
    gets(word);
    
    printf("Character: %c\n", ch);
    printf("Word: %s\n", word);
    return 0;
}
```

## Related Topics

- [Type Conversion Functions](conversions.en.md)
- [Types and Variables](../language-reference/types.en.md)
- [Examples](../examples/examples.en.md)

