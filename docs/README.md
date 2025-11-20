# Three Musketeers Language Documentation

![Version](https://img.shields.io/badge/version-v1.0.0Athos-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Language](https://img.shields.io/badge/language-C%23-purple.svg)

Welcome to the Three Musketeers programming language documentation. Three Musketeers is a C-like programming language with native support for strings and boolean types, designed as a subset of C with additional features.

## Philosophy

Three Musketeers is designed to serve as a subset of the C programming language with some additional attributes such as native string and boolean types. The language is strongly typed with implicit type conversion, making it easier to work with while maintaining the power and flexibility of C.

## Documentation

Choose your preferred language:

- **[English Documentation](en/README.md)** - Complete documentation in English
- **[Documentação em Português](pt/README.md)** - Documentação completa em português

## Quick Navigation

### English
- [Getting Started](en/getting-started/README.md)
- [Language Reference](en/language-reference/README.md)
- [Compiler Guide](en/compiler-guide/README.md)
- [Examples](en/examples/examples.md)

### Português
- [Começando](pt/comecando/README.md)
- [Referência da Linguagem](pt/referencia-linguagem/README.md)
- [Guia do Compilador](pt/guia-compilador/README.md)
- [Exemplos](pt/exemplos/exemplos.md)

## Features

- **Strongly Typed**: All variables must be declared with a type
- **Implicit Type Conversion**: Automatic type conversions handled by the compiler
- **Native Types**: `int`, `double`, `char`, `bool`, `string`
- **Pointers**: Full pointer support with dereferencing and address-of operators
- **Structures & Unions**: Support for user-defined composite types
- **Arrays**: Multi-dimensional arrays support
- **Functions**: Function definitions with various return types
- **Control Flow**: `if/else`, `switch`, `for`, `while`, `do-while` loops
- **Memory Management**: Dynamic memory allocation with `malloc` and `free`
- **I/O Operations**: `printf`, `scanf`, `puts`, `gets` for input/output
- **Type Conversion**: Built-in functions for type conversion (`atoi`, `atod`, `itoa`, `dtoa`)

## Compiler

The Three Musketeers compiler is written in C# and uses:
- **ANTLR4** for lexical and syntax analysis
- **LLVM** for intermediate code generation
- **GCC** for final compilation to native executables

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

