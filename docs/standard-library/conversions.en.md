[English](#english) | [Português](conversions.pt.md)

<a name="english"></a>
# Type Conversion Functions

Three Musketeers provides built-in functions to convert between strings and other types. These functions are essential when working with string input/output and type conversions.

## String to Type Conversions

### atoi

Converts a string to an integer.

**Syntax:**
```c
int atoi(string_literal);
int atoi(string_variable);
```

**Examples:**

```c
string numStr = "123";
int num = atoi(numStr);        // num = 123

int value = atoi("456");       // value = 456

string input;
gets(input);
int number = atoi(input);      // Convert user input to int
```

**Usage with Input:**

```c
int main() {
    string input;
    puts("Enter a number:");
    gets(input);
    
    int num = atoi(input);
    printf("You entered: %d\n", num);
    return 0;
}
```

### atod

Converts a string to a double (floating-point number).

**Syntax:**
```c
double atod(string_literal);
double atod(string_variable);
```

**Examples:**

```c
string piStr = "3.14159";
double pi = atod(piStr);        // pi = 3.14159

double value = atod("2.718");   // value = 2.718

string input;
gets(input);
double number = atod(input);    // Convert user input to double
```

**Usage with Input:**

```c
int main() {
    string input;
    puts("Enter a decimal number:");
    gets(input);
    
    double num = atod(input);
    printf("You entered: %.2f\n", num);
    return 0;
}
```

## Type to String Conversions

### itoa

Converts an integer to a string.

**Syntax:**
```c
string itoa(int_value);
string itoa(int_variable);
```

**Examples:**

```c
int num = 123;
string str = itoa(num);         // str = "123"

string result = itoa(456);      // result = "456"

int age = 25;
string ageStr = itoa(age);
printf("Age as string: %s\n", ageStr);
```

**Usage in Output:**

```c
int main() {
    int count = 42;
    string countStr = itoa(count);
    puts("Count: ");
    puts(countStr);
    return 0;
}
```

### dtoa

Converts a double (floating-point number) to a string.

**Syntax:**
```c
string dtoa(double_value);
string dtoa(double_variable);
```

**Examples:**

```c
double pi = 3.14159;
string piStr = dtoa(pi);        // piStr = "3.14159"

string result = dtoa(2.718);   // result = "2.718"

double price = 19.99;
string priceStr = dtoa(price);
printf("Price: %s\n", priceStr);
```

**Usage in Output:**

```c
int main() {
    double total = 123.45;
    string totalStr = dtoa(total);
    puts("Total: ");
    puts(totalStr);
    return 0;
}
```

## Complete Examples

### Example 1: String Input to Calculation

```c
int main() {
    string input1, input2;
    int num1, num2;
    
    puts("Enter first number:");
    gets(input1);
    num1 = atoi(input1);
    
    puts("Enter second number:");
    gets(input2);
    num2 = atoi(input2);
    
    int sum = num1 + num2;
    string sumStr = itoa(sum);
    
    printf("Sum: %s\n", sumStr);
    return 0;
}
```

### Example 2: Number Formatting

```c
int main() {
    int count = 100;
    double average = 87.5;
    
    string countStr = itoa(count);
    string avgStr = dtoa(average);
    
    puts("Count: ");
    puts(countStr);
    puts("Average: ");
    puts(avgStr);
    return 0;
}
```

### Example 3: Calculator with String Conversion

```c
int main() {
    string num1Str, num2Str;
    double num1, num2;
    
    puts("Enter first number:");
    gets(num1Str);
    num1 = atod(num1Str);
    
    puts("Enter second number:");
    gets(num2Str);
    num2 = atod(num2Str);
    
    double product = num1 * num2;
    string productStr = dtoa(product);
    
    printf("Product: %s\n", productStr);
    return 0;
}
```

## Conversion Summary

| Function | From Type | To Type | Description                    |
|----------|-----------|---------|--------------------------------|
| `atoi`   | string    | int     | Convert string to integer      |
| `atod`   | string    | double  | Convert string to double       |
| `itoa`   | int       | string  | Convert integer to string      |
| `dtoa`   | double    | string  | Convert double to string        |

## Related Topics

- [Input/Output Functions](io.en.md)
- [Types and Variables](../language-reference/types.en.md)
- [Examples](../examples/examples.en.md)

