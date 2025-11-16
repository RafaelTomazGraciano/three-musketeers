# Three Musketeers

![Version](https://img.shields.io/badge/version-v0.9.5-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Language](https://img.shields.io/badge/language-C%23-purple.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)

**English** | [Português](#português)

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

### Compile a Program

```bash
Three_Musketeers program.tm -o program
```

### Example Program

```c
int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
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
./create_release.sh
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

# Português

Three Musketeers é um compilador de linguagem de programação similar ao C com suporte nativo para strings e tipos booleanos. Serve como um subconjunto de C com recursos adicionais, projetado para ser fortemente tipado com conversão de tipo implícita.

## Características

- **Fortemente Tipado**: Todas as variáveis devem ser declaradas com um tipo explícito
- **Conversão de Tipo Implícita**: Conversões de tipo automáticas tratadas pelo compilador
- **Tipos Nativos**: `int`, `double`, `char`, `bool`, `string`
- **Ponteiros**: Suporte completo a ponteiros com operadores de desreferenciação e endereço
- **Estruturas e Uniões**: Suporte para tipos compostos definidos pelo usuário
- **Arrays**: Arrays de uma e múltiplas dimensões
- **Funções**: Funções com vários tipos de retorno e parâmetros
- **Fluxo de Controle**: Loops `if/else`, `switch`, `for`, `while`, `do-while`
- **Gerenciamento de Memória**: Alocação dinâmica com `malloc` e `free`
- **Operações de E/S**: `printf`, `scanf`, `puts`, `gets` para entrada/saída
- **Conversão de Tipos**: Funções integradas para conversão de tipos (`atoi`, `atod`, `itoa`, `dtoa`)

## Filosofia

Three Musketeers é projetado para servir como um subconjunto da linguagem de programação C com alguns atributos adicionais, como tipos string e boolean nativos. A linguagem é fortemente tipada com conversão de tipo implícita, facilitando o trabalho mantendo o poder e a flexibilidade do C.

## Compilador

O compilador Three Musketeers é escrito em C# (.NET 9.0) e usa:
- **ANTLR4** para análise léxica e sintática
- **LLVM** para geração de código intermediário
- **GCC** para compilação final em executáveis nativos

## Início Rápido

### Compilar um Programa

```bash
Three_Musketeers programa.tm -o programa
```

### Programa de Exemplo

```c
int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

## Documentação

Documentação abrangente está disponível em inglês e português:

- **[English Documentation](docs/en/README.md)** - Documentação completa em inglês
- **[Documentação em Português](docs/pt/README.md)** - Documentação completa em português

### Links Rápidos

- [Getting Started](docs/en/getting-started/README.md) / [Começando](docs/pt/comecando/README.md)
- [Language Reference](docs/en/language-reference/README.md) / [Referência da Linguagem](docs/pt/referencia-linguagem/README.md)
- [Compiler Guide](docs/en/compiler-guide/README.md) / [Guia do Compilador](docs/pt/guia-compilador/README.md)
- [Examples](docs/en/examples/examples.md) / [Exemplos](docs/pt/exemplos/exemplos.md)

## Compilando o Compilador

### Pré-requisitos

- .NET SDK 9.0 ou posterior
- LLVM (com comando `llc` disponível)
- GCC (GNU Compiler Collection)

### Instruções de Compilação

```bash
cd Three_Musketeers
dotnet build
```

Para uma build de release:

```bash
./create_release.sh
```

## Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para enviar um Pull Request.
