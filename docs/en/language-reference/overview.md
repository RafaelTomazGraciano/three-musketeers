# Language Overview

Three Musketeers is a strongly typed, C-like programming language with implicit type conversion. It serves as a subset of C with additional features like native string and boolean types.

## Language Characteristics

### Strong Typing

All variables must be declared with an explicit type. The compiler enforces type safety throughout your program.

### Implicit Type Conversion

The compiler automatically performs type conversions when needed, eliminating the need for explicit casting in most cases.

### C-like Syntax

If you're familiar with C, you'll feel right at home with Three Musketeers. The syntax is very similar, with some enhancements.

## Core Features

- **Five Basic Types**: `int`, `double`, `char`, `bool`, `string`
- **Pointers**: Full pointer support with dereferencing
- **Arrays**: Single and multi-dimensional arrays
- **Structures & Unions**: User-defined composite types
- **Functions**: Functions with various return types and parameters
- **Control Flow**: `if/else`, `switch`, `for`, `while`, `do-while`
- **Memory Management**: Dynamic allocation with `malloc` and `free`
- **I/O Operations**: `printf`, `scanf`, `puts`, `gets`
- **Preprocessor**: `#include` and `#define` directives

## Program Structure

A Three Musketeers program typically follows this structure:

```c
#include <stdio.tm>

int main() {
    // Your code here
    return 0;
}
```

## File Extension

Three Musketeers source files use the `.tm` extension.

## Next Steps

- [Data Types](data-types.md) - Learn about the available types
- [Variables](variables.md) - Understand variable declarations
- [Functions](functions.md) - Create reusable code blocks

