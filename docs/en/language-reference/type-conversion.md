# Type Conversion

Three Musketeers provides built-in functions for converting between strings and other data types. The compiler also performs implicit type conversion automatically.

## Implicit Type Conversion

The compiler automatically converts between compatible types:

```c
int x = 10;
double y = x;        // int automatically converted to double
int z = y;           // double converted to int (truncated)
```

## String to Type Conversion

### atoi

Convert a string to an integer.

**Syntax:**
```c
int atoi(string_variable);
int atoi("string literal");
```

**Examples:**

```c
string numStr = "42";
int num = atoi(numStr);
printf("%d\n", num);  // 42

int value = atoi("123");
printf("%d\n", value);  // 123
```

### atod

Convert a string to a double.

**Syntax:**
```c
double atod(string_variable);
double atod("string literal");
```

**Examples:**

```c
string piStr = "3.14159";
double pi = atod(piStr);
printf("%.5f\n", pi);  // 3.14159

double value = atod("2.718");
printf("%.3f\n", value);  // 2.718
```

## Type to String Conversion

### itoa

Convert an integer to a string.

**Syntax:**
```c
string itoa(int_value);
string itoa(integer_variable);
```

**Examples:**

```c
int age = 25;
string ageStr = itoa(age);
printf("Age: %s\n", ageStr);  // Age: 25

string numStr = itoa(42);
puts(numStr);  // 42
```

### dtoa

Convert a double to a string.

**Syntax:**
```c
string dtoa(double_value);
string dtoa(double_variable);
```

**Examples:**

```c
double pi = 3.14159;
string piStr = dtoa(pi);
printf("Pi: %s\n", piStr);  // Pi: 3.14159

string valueStr = dtoa(2.718);
puts(valueStr);  // 2.718
```

## Complete Conversion Example

```c
#include <stdio.tm>

int main() {
    // String to number
    string input = "42";
    int num = atoi(input);
    double value = atod("3.14");
    
    printf("Integer: %d\n", num);
    printf("Double: %.2f\n", value);
    
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

## Use Cases

### Reading Numbers from Input

```c
string input;
int number;

printf("Enter a number: ");
gets(input);
number = atoi(input);
printf("You entered: %d\n", number);
```

### Formatting Output

```c
int count = 42;
string message = "Count: " + itoa(count);
puts(message);  // Count: 42
```

### Building Strings

```c
int x = 10, y = 20;
string result = "x=" + itoa(x) + ", y=" + itoa(y);
puts(result);  // x=10, y=20
```

## Type Conversion Table

| Function | From Type | To Type | Example |
|----------|-----------|---------|---------|
| `atoi`   | string    | int     | `atoi("42")` → `42` |
| `atod`   | string    | double  | `atod("3.14")` → `3.14` |
| `itoa`   | int       | string  | `itoa(42)` → `"42"` |
| `dtoa`   | double    | string  | `dtoa(3.14)` → `"3.14"` |

## Implicit Conversion Rules

The compiler follows these rules for implicit conversion:

1. **Numeric types**: `int` can be converted to `double` automatically
2. **Truncation**: `double` to `int` truncates the decimal part
3. **Type hierarchy**: Conversions follow the type hierarchy (double → int → char → bool → string)

## Related Topics

- [Data Types](data-types.md) - Available types
- [Variables](variables.md) - Variable declarations
- [Input and Output](input-output.md) - Using conversions with I/O

