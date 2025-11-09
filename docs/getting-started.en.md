[English](#english) | [Português](getting-started.pt.md)

<a name="english"></a>
# Getting Started

This guide will help you get started with the Three Musketeers programming language.

## Installation

### Prerequisites

Before installing the Three Musketeers compiler, ensure you have the following tools installed:

- **.NET 9.0 SDK** - Required to build and run the compiler
- **LLVM tools** - Required for code generation:
  - `llvm-as` - LLVM assembler
  - `opt` - LLVM optimizer
  - `llc` - LLVM static compiler
- **GCC** - Required for final linking

### Building the Compiler

1. Clone the repository:
```bash
git clone https://github.com/yourusername/three-musketeers.git
cd three-musketeers
```

2. Navigate to the compiler directory:
```bash
cd Three_Musketeers
```

3. Build the compiler:
```bash
dotnet build
```

The compiler executable will be available in the `bin/` directory.

## Your First Program

Let's create a simple "Hello, World!" program to get started.

### Step 1: Create a Source File

Create a file named `hello.3m` with the following content:

```c
int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

### Step 2: Compile the Program

Compile your program using the compiler:

```bash
dotnet run -- hello.3m -o hello
```

Or if you've built the compiler:

```bash
./bin/Debug/net9.0/Three_Musketeers hello.3m -o hello
```

The `-o` option specifies the output executable name. If omitted, it defaults to `a.out`.

### Step 3: Run the Program

Execute the compiled program:

```bash
./hello
```

You should see the output:
```
Hello, Three Musketeers!
```

## Compilation Process

The Three Musketeers compiler follows these steps:

1. **Lexical Analysis** - Tokenizes the source code
2. **Syntax Analysis** - Parses tokens into an Abstract Syntax Tree (AST)
3. **Semantic Analysis** - Performs type checking and validation
4. **Code Generation** - Generates LLVM IR (`.ll` file)
5. **Optimization** - Optimizes the LLVM bytecode
6. **Assembly Generation** - Converts to assembly (`.s` file)
7. **Linking** - Links with GCC to produce the final executable

Intermediate files (`.ll`, `.bc`, `.s`) are automatically cleaned up after compilation.

## File Extensions

Three Musketeers source files use the `.3m` extension.

## Basic Concepts

### Program Structure

Every Three Musketeers program must have a `main` function:

```c
int main() {
    // Your code here
    return 0;
}
```

The `main` function can optionally accept command-line arguments:

```c
int main(int argc, char argv[]) {
    // argc: number of arguments
    // argv: array of argument strings
    return 0;
}
```

### Comments

Three Musketeers supports both single-line and multi-line comments:

```c
// This is a single-line comment

/* This is a
   multi-line comment */
```

### Includes

You can include header files using the `#include` directive:

```c
#include <stdio.tm>
#include <stdlib.tm>
```

### Defines

Use `#define` to create constants:

```c
#define MAX_SIZE 100
#define PI 3.14159
#define MESSAGE "Hello, World!"
```

## Next Steps

- Learn about [Types and Variables](language-reference/types.en.md)
- Explore [Operators](language-reference/operators.en.md)
- Understand [Control Flow](language-reference/control-flow.en.md)
- Check out [Examples](examples/examples.en.md)

