# Quick Start

Get up and running with Three Musketeers in minutes.

## Your First Program

Create a file named `hello.3m` with the following content:

```c
#include <stdio.tm>

int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

## Compiling Your Program

Compile the program using the Three Musketeers compiler:

```bash
tm hello.3m --bin -o hello
```

This will:
1. Parse and analyze your source code
2. Generate LLVM IR code
3. Compile to assembly
4. Link into an executable named `hello` in the `bin` directory

## Running Your Program

Execute the compiled program:

```bash
./bin/hello
```

You should see the output:

```
Hello, Three Musketeers!
```

## Compiler Options

### Basic Usage

```bash
tm <input-file> [options]
```

### Common Options

- `-o <name>` or `--out <name>`: Specify output executable name (default: `a.out`)
- `-O <level>` or `--opt <level>`: Set optimization level (0-3, default: 2)
- `-g`: Add debug information
- `--ll`: Keep generated LLVM IR code
- `-I <path>` or `--Include <path>`: Include library path

### Example with Options

```bash
tm hello.3m --bin -o myprogram -O 3 -g
```

## File Structure

After compilation, you'll find:
- `bin/hello`: The compiled executable
- `bin/hello.ll`: LLVM IR code (if `--ll` flag was used)

## Next Steps

- [Hello World Tutorial](hello-world.md) - Learn more about the basics
- [Language Reference](../language-reference/overview.md) - Explore language features
- [Examples](../examples/examples.md) - See more code examples

