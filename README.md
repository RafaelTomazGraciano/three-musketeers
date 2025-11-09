[English](#english) | [Português](#português)

# Three Musketeers Language

<a name="english"></a>
## English

**Three Musketeers** is a strongly-typed programming language that serves as a subset of C with additional features, including native `string` and `bool` types. The language compiles to LLVM IR and produces native executables.

### Features

- **Strong typing** with implicit type conversion
- **Native types**: `int`, `double`, `char`, `bool`, `string`
- **Memory management**: Pointers, `malloc`, `free`
- **Control flow**: `if/else`, `switch`, `for`, `while`, `do-while`
- **Functions**: Full function support with recursion
- **Structures and unions**: User-defined composite types
- **Arrays and matrices**: Multi-dimensional arrays
- **Preprocessor**: `#include` and `#define` directives

### Quick Start

#### Prerequisites

- .NET 9.0 SDK
- LLVM tools (`llvm-as`, `opt`, `llc`)
- GCC

#### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/three-musketeers.git
cd three-musketeers
```

2. Build the compiler:
```bash
cd Three_Musketeers
dotnet build
```

#### Your First Program

Create a file `hello.3m`:

```c
int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

Compile and run:

```bash
dotnet run -- hello.3m -o hello
./hello
```

### Documentation

Comprehensive documentation is available in the [`docs/`](docs/README.md) directory:

- [Getting Started](docs/getting-started.en.md)
- [Language Reference](docs/language-reference/)
- [Standard Library](docs/standard-library/)
- [Examples](docs/examples/)
- [Compiler Reference](docs/compiler-reference.en.md)

### License

See [LICENSE](LICENSE) file for details.

---

<a name="português"></a>
## Português

**Three Musketeers** é uma linguagem de programação fortemente tipada que serve como um subconjunto de C com recursos adicionais, incluindo tipos nativos `string` e `bool`. A linguagem compila para LLVM IR e produz executáveis nativos.

### Características

- **Tipagem forte** com conversão implícita de tipos
- **Tipos nativos**: `int`, `double`, `char`, `bool`, `string`
- **Gerenciamento de memória**: Ponteiros, `malloc`, `free`
- **Fluxo de controle**: `if/else`, `switch`, `for`, `while`, `do-while`
- **Funções**: Suporte completo a funções com recursão
- **Estruturas e uniões**: Tipos compostos definidos pelo usuário
- **Vetores e matrizes**: Arrays multidimensionais
- **Pré-processador**: Diretivas `#include` e `#define`

### Início Rápido

#### Pré-requisitos

- .NET 9.0 SDK
- Ferramentas LLVM (`llvm-as`, `opt`, `llc`)
- GCC

#### Instalação

1. Clone o repositório:
```bash
git clone https://github.com/yourusername/three-musketeers.git
cd three-musketeers
```

2. Compile o compilador:
```bash
cd Three_Musketeers
dotnet build
```

#### Seu Primeiro Programa

Crie um arquivo `hello.3m`:

```c
int main() {
    puts("Olá, Three Musketeers!");
    return 0;
}
```

Compile e execute:

```bash
dotnet run -- hello.3m -o hello
./hello
```

### Documentação

Documentação completa está disponível no diretório [`docs/`](docs/README.md):

- [Começando](docs/getting-started.pt.md)
- [Referência da Linguagem](docs/language-reference/)
- [Biblioteca Padrão](docs/standard-library/)
- [Exemplos](docs/examples/)
- [Referência do Compilador](docs/compiler-reference.pt.md)

### Licença

Veja o arquivo [LICENSE](LICENSE) para detalhes.
