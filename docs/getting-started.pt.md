[English](getting-started.en.md) | [Português](#português)

<a name="português"></a>
# Começando

Este guia ajudará você a começar com a linguagem de programação Three Musketeers.

## Instalação

### Pré-requisitos

Antes de instalar o compilador Three Musketeers, certifique-se de ter as seguintes ferramentas instaladas:

- **.NET 9.0 SDK** - Necessário para compilar e executar o compilador
- **Ferramentas LLVM** - Necessárias para geração de código:
  - `llvm-as` - Montador LLVM
  - `opt` - Otimizador LLVM
  - `llc` - Compilador estático LLVM
- **GCC** - Necessário para a ligação final

### Compilando o Compilador

1. Clone o repositório:
```bash
git clone https://github.com/yourusername/three-musketeers.git
cd three-musketeers
```

2. Navegue até o diretório do compilador:
```bash
cd Three_Musketeers
```

3. Compile o compilador:
```bash
dotnet build
```

O executável do compilador estará disponível no diretório `bin/`.

## Seu Primeiro Programa

Vamos criar um programa simples "Hello, World!" para começar.

### Passo 1: Criar um Arquivo Fonte

Crie um arquivo chamado `hello.3m` com o seguinte conteúdo:

```c
int main() {
    puts("Olá, Three Musketeers!");
    return 0;
}
```

### Passo 2: Compilar o Programa

Compile seu programa usando o compilador:

```bash
dotnet run -- hello.3m -o hello
```

Ou se você já compilou o compilador:

```bash
./bin/Debug/net9.0/Three_Musketeers hello.3m -o hello
```

A opção `-o` especifica o nome do executável de saída. Se omitida, o padrão é `a.out`.

### Passo 3: Executar o Programa

Execute o programa compilado:

```bash
./hello
```

Você deve ver a saída:
```
Olá, Three Musketeers!
```

## Processo de Compilação

O compilador Three Musketeers segue estes passos:

1. **Análise Léxica** - Tokeniza o código fonte
2. **Análise Sintática** - Analisa os tokens em uma Árvore Sintática Abstrata (AST)
3. **Análise Semântica** - Realiza verificação de tipos e validação
4. **Geração de Código** - Gera LLVM IR (arquivo `.ll`)
5. **Otimização** - Otimiza o bytecode LLVM
6. **Geração de Assembly** - Converte para assembly (arquivo `.s`)
7. **Ligação** - Liga com GCC para produzir o executável final

Arquivos intermediários (`.ll`, `.bc`, `.s`) são automaticamente limpos após a compilação.

## Extensões de Arquivo

Arquivos fonte Three Musketeers usam a extensão `.3m`.

## Conceitos Básicos

### Estrutura do Programa

Todo programa Three Musketeers deve ter uma função `main`:

```c
int main() {
    // Seu código aqui
    return 0;
}
```

A função `main` pode opcionalmente aceitar argumentos da linha de comando:

```c
int main(int argc, char argv[]) {
    // argc: número de argumentos
    // argv: array de strings de argumentos
    return 0;
}
```

### Comentários

Three Musketeers suporta comentários de linha única e múltiplas linhas:

```c
// Este é um comentário de linha única

/* Este é um
   comentário de múltiplas linhas */
```

### Includes

Você pode incluir arquivos de cabeçalho usando a diretiva `#include`:

```c
#include <stdio.tm>
#include <stdlib.tm>
```

### Defines

Use `#define` para criar constantes:

```c
#define MAX_SIZE 100
#define PI 3.14159
#define MESSAGE "Olá, Mundo!"
```

## Próximos Passos

- Aprenda sobre [Tipos e Variáveis](language-reference/types.pt.md)
- Explore [Operadores](language-reference/operators.pt.md)
- Entenda [Fluxo de Controle](language-reference/control-flow.pt.md)
- Veja [Exemplos](examples/examples.pt.md)

