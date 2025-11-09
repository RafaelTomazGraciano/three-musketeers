[English](#english) | [Português](compiler-reference.pt.md)

<a name="english"></a>
# Compiler Reference

This document describes the Three Musketeers compiler, its command-line options, file formats, and the compilation process.

## Command-Line Usage

### Basic Syntax

```bash
dotnet run -- <input_file> [options]
```

Or if the compiler is already built:

```bash
./bin/Debug/net9.0/Three_Musketeers <input_file> [options]
```

### Arguments

- **`<input_file>`** (required) - The Three Musketeers source file to compile (`.3m` extension)

### Options

- **`-o <output>`** or **`--out <output>`** - Specify the output executable name
  - Default: `a.out`
  - Example: `-o myprogram`

### Examples

```bash
# Compile with default output name
dotnet run -- program.3m

# Compile with custom output name
dotnet run -- program.3m -o myprogram

# Using the built compiler
./bin/Debug/net9.0/Three_Musketeers hello.3m -o hello
```

## File Extensions

- **`.3m`** - Three Musketeers source files
- **`.ll`** - LLVM IR (intermediate representation) - generated during compilation
- **`.bc`** - LLVM bytecode - generated during compilation
- **`.s`** - Assembly code - generated during compilation

**Note:** Intermediate files (`.ll`, `.bc`, `.s`) are automatically cleaned up after successful compilation.

## Compilation Process

The Three Musketeers compiler follows these stages:

### 1. Lexical Analysis

The source code is tokenized into a stream of tokens (keywords, identifiers, operators, literals, etc.).

### 2. Syntax Analysis

Tokens are parsed into an Abstract Syntax Tree (AST) according to the language grammar.

### 3. Semantic Analysis

The compiler performs:
- Type checking
- Variable declaration validation
- Function signature validation
- Scope analysis
- Symbol table construction

### 4. Code Generation

LLVM IR (Intermediate Representation) is generated from the AST.

### 5. Optimization

The LLVM optimizer (`opt`) is applied with optimization level O2.

### 6. Assembly Generation

LLVM IR is converted to assembly code using `llc`.

### 7. Linking

GCC links the assembly code to produce the final executable.

## Error Messages

The compiler provides error messages for syntax and semantic errors.

### Syntax Errors

Syntax errors occur during parsing and are reported immediately:

```
Compilation failed due to syntax errors
```

Common syntax errors include:
- Missing semicolons
- Mismatched brackets
- Invalid token sequences
- Missing function parameters

### Semantic Errors

Semantic errors occur during type checking and validation:

```
Compilation failed due to semantic errors
```

Common semantic errors include:
- Undeclared variables
- Type mismatches
- Invalid function calls
- Undefined functions

### Internal Compiler Errors

If the compiler encounters an unexpected error:

```
Internal compiler error
   <error message>
   <stack trace>
```

## Prerequisites

The compiler requires the following tools to be installed:

### Required Tools

1. **.NET 9.0 SDK** - For building and running the compiler
2. **LLVM Tools**:
   - `llvm-as` - LLVM assembler
   - `opt` - LLVM optimizer
   - `llc` - LLVM static compiler
3. **GCC** - For final linking

### Installation

#### .NET 9.0 SDK

Install from [Microsoft's .NET download page](https://dotnet.microsoft.com/download).

#### LLVM

Install LLVM tools based on your operating system:

- **Linux**: `sudo apt-get install llvm` (Ubuntu/Debian) or `sudo yum install llvm` (RHEL/CentOS)
- **macOS**: `brew install llvm`
- **Windows**: Download from [LLVM releases](https://github.com/llvm/llvm-project/releases)

#### GCC

- **Linux**: Usually pre-installed or `sudo apt-get install gcc`
- **macOS**: `xcode-select --install` or `brew install gcc`
- **Windows**: Install [MinGW-w64](https://www.mingw-w64.org/) or use WSL

## Building the Compiler

### From Source

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

The compiler executable will be in `bin/Debug/net9.0/Three_Musketeers` (or `bin/Release/net9.0/Three_Musketeers` for release builds).

### Release Build

```bash
dotnet build -c Release
```

## Compiler Architecture

The compiler is built using:

- **ANTLR4** - For lexical and syntax analysis
- **LLVM** - For code generation and optimization
- **.NET 9.0** - Runtime environment

### Source Code Structure

- `Generate_Grammar/` - ANTLR grammar definition
- `Grammar/` - Generated parser code
- `Visitors/` - AST visitors for semantic analysis and code generation
- `Listeners/` - Error listeners
- `Models/` - Symbol table and scope models
- `Program.cs` - Main compiler entry point

## Related Topics

- [Getting Started](getting-started.en.md)
- [Language Reference](language-reference/)
- [Examples](examples/examples.en.md)

