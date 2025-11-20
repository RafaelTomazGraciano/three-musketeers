# Compilation Process

Understanding how the Three Musketeers compiler transforms source code into executable programs.

## Overview

The compilation process consists of six main phases:

1. Lexical Analysis
2. Syntax Analysis
3. Semantic Analysis
4. Code Generation
5. Assembly Generation
6. Linking

## Phase 1: Lexical Analysis

The compiler reads the source file and breaks it down into tokens (keywords, identifiers, operators, literals, etc.).

**Input:** Source code (`.3m` file)
**Output:** Stream of tokens

**Example:**
```c
int x = 10;
```

Tokens: `int`, `x`, `=`, `10`, `;`

## Phase 2: Syntax Analysis

The parser uses ANTLR4 to build an Abstract Syntax Tree (AST) from the token stream, checking that the code follows the language grammar.

**Input:** Token stream
**Output:** Abstract Syntax Tree (AST)

**Errors:** Syntax errors (invalid grammar, missing semicolons, etc.)

## Phase 3: Semantic Analysis

The semantic analyzer validates the program's meaning:

- Type checking
- Variable scope validation
- Function call validation
- Symbol table management
- Control flow validation (break/continue in correct context)

**Input:** AST
**Output:** Validated AST with semantic information

**Errors:** Semantic errors (type mismatches, undefined variables, etc.)

## Phase 4: Code Generation

The code generator traverses the validated AST and generates LLVM Intermediate Representation (IR) code.

**Input:** Validated AST
**Output:** LLVM IR code (`.ll` file)

**Example:**
```c
int add(int x, int y) {
    return x + y;
}
```

Generates LLVM IR with function definitions, instructions, and type information.

## Phase 5: Assembly Generation

The LLVM toolchain (`llc`) converts LLVM IR into target-specific assembly code.

**Input:** LLVM IR (`.ll` file)
**Output:** Assembly code (`.s` file)

**Command:**
```bash
llc program.ll -O2 -o program.s
```

The optimization level (`-O`) is determined by the `-O` compiler option.

## Phase 6: Linking

GCC links the assembly code and system libraries to create the final executable.

**Input:** Assembly code (`.s` file)
**Output:** Executable binary

**Command:**
```bash
gcc program.s -o program -no-pie
```

If the `-g` flag is used, GCC is called with the `-g` option for debug information.

## File Lifecycle

```
program.tm
    ↓ (Lexical & Syntax Analysis)
AST
    ↓ (Semantic Analysis)
Validated AST
    ↓ (Code Generation)
program.ll  (LLVM IR)
    ↓ (Assembly Generation)
program.s   (Assembly, temporary)
    ↓ (Linking)
program     (Executable)
```

**Note:** The `.s` file is deleted after linking unless an error occurs. The `.ll` file is kept only if `--ll` flag is used.

## Error Handling

### Syntax Errors

Detected during syntax analysis:
```
Error: Expected ';' at line 5
```

### Semantic Errors

Detected during semantic analysis:
```
Error: Type mismatch at line 10
Error: Undefined variable 'x' at line 15
```

### Compilation Errors

If any phase fails, compilation stops and an error message is displayed.

## Optimization

Optimization occurs at the LLVM IR level:

- **Level 0**: No optimization
- **Level 1**: Basic optimizations
- **Level 2**: Standard optimizations (default)
- **Level 3**: Aggressive optimizations

Optimizations include:
- Dead code elimination
- Constant folding
- Loop optimizations
- Inlining

## Debug Information

When using the `-g` flag, debug information is added during the linking phase, allowing debuggers to map executable code back to source code.

## Related Topics

- [Usage](usage.md) - How to use the compiler
- [Command Line Options](command-line-options.md) - Compiler options

