# Three Musketeers

![Version](https://img.shields.io/badge/version-v1.0.0Athos-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Language](https://img.shields.io/badge/language-C%23-purple.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)
![ANTLR4](https://img.shields.io/badge/ANTLR-4-orange.svg)
![LLVM](https://img.shields.io/badge/LLVM-IR-blue.svg)

Three Musketeers is a C-like programming language compiler with native support for strings and boolean types. It serves as a subset of C with additional features, designed to be strongly typed with implicit type conversion.

## Features

- **Strongly Typed**: All variables must be declared with an explicit type
- **Implicit Type Conversion**: Automatic type conversions handled by the compiler
- **Native Types**: `int`, `double`, `char`, `bool`, `string`
- **Pointers**: Full pointer support with dereferencing and address-of operators
- **Structures & Unions**: Support for user-defined composite types
- **Arrays**: Single and multi-dimensional arrays
- **Functions**: Functions with various return types and parameters
- **Control Flow**: `if/else`, `switch`, `for`, `while`, `do-while` loops
- **Memory Management**: Dynamic allocation with `malloc` and `free`
- **I/O Operations**: `printf`, `scanf`, `puts`, `gets` for input/output
- **Type Conversion**: Built-in functions for type conversion (`atoi`, `atod`, `itoa`, `dtoa`)

## Philosophy

Three Musketeers is designed to serve as a subset of the C programming language with some additional attributes such as native string and boolean types. The language is strongly typed with implicit type conversion, making it easier to work with while maintaining the power and flexibility of C.

## Compiler

The Three Musketeers compiler is written in C# (.NET 9.0) and uses:
- **ANTLR4** for lexical and syntax analysis
- **LLVM** for intermediate code generation
- **GCC** for final compilation to native executables

## Quick Start

### Example Program

```c
#include <stdio.tm>

int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

### Compile a Program

```bash
tm program.3m -o program
```

## Documentation

Comprehensive documentation is available in both English and Portuguese:

- **[English Documentation](docs/en/README.md)** - Complete documentation in English
- **[Documentação em Português](docs/pt/README.md)** - Documentação completa em português

### Quick Links

- [Getting Started](docs/en/getting-started/README.md) / [Começando](docs/pt/comecando/README.md)
- [Language Reference](docs/en/language-reference/README.md) / [Referência da Linguagem](docs/pt/referencia-linguagem/README.md)
- [Compiler Guide](docs/en/compiler-guide/README.md) / [Guia do Compilador](docs/pt/guia-compilador/README.md)
- [Examples](docs/en/examples/examples.md) / [Exemplos](docs/pt/exemplos/exemplos.md)

## Building the Compiler

### Prerequisites

- .NET SDK 9.0 or later
- LLVM (with `llc` command available)
- GCC (GNU Compiler Collection)

### Build Instructions

```bash
cd Three_Musketeers
dotnet build
```

For a release build:

```bash
cd ..
./create_release.sh
```

## Running Tests

To run the Code Generation tests:

```bash
cd Three_Musketeers/LanguageTests
dotnet run
```

To run the Semantic Analysis tests:

```bash
cd Three_Musketeers/Three_Musketeers.Tests
dotnet test
```

## Use Our Syntax Highlighting

You can use our syntax highlighting for Visual Studio Code by our repository: [Three Musketeers Syntax Highlighting](https://github.com/RafaelTomazGraciano/three-musketeers-vscode)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## The Three Musketeers

<table>
  <tr>
  <td align="center">
      <a href="https://github.com/RafaelTomazGraciano">
        <img src="https://github.com/RafaelTomazGraciano.png" width="100px;" alt="Foto do Rafael Tomaz"/><br>
        <sub>
          <b>Rafael Tomaz</b>
        </sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/gabrielwitor">
        <img src="https://github.com/gabrielwitor.png" width="100px;" alt="Foto do Gabriel Witor"/><br>
        <sub>
          <b>Gabriel Witor</b>
        </sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/HedroPedro">
        <img src="https://github.com/HedroPedro.png" width="100px;" alt="Foto do Hedro Pedro"/><br>
        <sub>
          <b>Pedro Henrique de Oliveira</b>
        </sub>
      </a>
    </td>
  </tr>
</table>