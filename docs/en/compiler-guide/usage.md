# Compiler Usage

The Three Musketeers compiler translates `.tm` source files into executable programs.

## Basic Usage

```bash
Three_Musketeers <input-file> [options]
```

### Example

```bash
Three_Musketeers program.tm -o program
```

This compiles `program.tm` and creates an executable named `program` in the `bin` directory.

## Compilation Process

The compiler performs the following steps:

1. **Lexical Analysis**: Tokenizes the source code
2. **Syntax Analysis**: Parses tokens into an Abstract Syntax Tree (AST)
3. **Semantic Analysis**: Validates types, scopes, and semantics
4. **Code Generation**: Generates LLVM IR code
5. **Assembly Generation**: Converts LLVM IR to assembly using `llc`
6. **Linking**: Creates executable using `gcc`

## Output Location

By default, compiled files are placed in a `bin` directory relative to the source file:

```
source.tm          →  bin/source.ll  (LLVM IR, if --ll flag used)
                   →  bin/source.s   (Assembly, temporary)
                   →  bin/program    (Executable)
```

## File Requirements

- Source files must have the `.tm` extension
- The program must contain a `main` function
- All included files must exist and be accessible

## Error Messages

The compiler provides detailed error messages for:

- **Syntax Errors**: Invalid syntax or grammar violations
- **Semantic Errors**: Type mismatches, undefined variables, etc.
- **File Errors**: Missing files or invalid paths

## Next Steps

- [Command Line Options](command-line-options.md) - All available compiler options
- [Compilation Process](compilation-process.md) - Detailed compilation steps

