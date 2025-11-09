[English](compiler-reference.en.md) | [Português](#português)

<a name="português"></a>
# Referência do Compilador

Este documento descreve o compilador Three Musketeers, suas opções de linha de comando, formatos de arquivo e o processo de compilação.

## Uso da Linha de Comando

### Sintaxe Básica

```bash
dotnet run -- <arquivo_entrada> [opções]
```

Ou se o compilador já estiver compilado:

```bash
./bin/Debug/net9.0/Three_Musketeers <arquivo_entrada> [opções]
```

### Argumentos

- **`<arquivo_entrada>`** (obrigatório) - O arquivo fonte Three Musketeers para compilar (extensão `.3m`)

### Opções

- **`-o <saida>`** ou **`--out <saida>`** - Especifica o nome do executável de saída
  - Padrão: `a.out`
  - Exemplo: `-o meuprograma`

### Exemplos

```bash
# Compilar com nome de saída padrão
dotnet run -- programa.3m

# Compilar com nome de saída personalizado
dotnet run -- programa.3m -o meuprograma

# Usando o compilador compilado
./bin/Debug/net9.0/Three_Musketeers hello.3m -o hello
```

## Extensões de Arquivo

- **`.3m`** - Arquivos fonte Three Musketeers
- **`.ll`** - LLVM IR (representação intermediária) - gerado durante a compilação
- **`.bc`** - Bytecode LLVM - gerado durante a compilação
- **`.s`** - Código assembly - gerado durante a compilação

**Nota:** Arquivos intermediários (`.ll`, `.bc`, `.s`) são automaticamente limpos após compilação bem-sucedida.

## Processo de Compilação

O compilador Three Musketeers segue estas etapas:

### 1. Análise Léxica

O código fonte é tokenizado em um fluxo de tokens (palavras-chave, identificadores, operadores, literais, etc.).

### 2. Análise Sintática

Os tokens são analisados em uma Árvore Sintática Abstrata (AST) de acordo com a gramática da linguagem.

### 3. Análise Semântica

O compilador realiza:
- Verificação de tipos
- Validação de declaração de variáveis
- Validação de assinatura de funções
- Análise de escopo
- Construção de tabela de símbolos

### 4. Geração de Código

LLVM IR (Representação Intermediária) é gerado a partir da AST.

### 5. Otimização

O otimizador LLVM (`opt`) é aplicado com nível de otimização O2.

### 6. Geração de Assembly

LLVM IR é convertido para código assembly usando `llc`.

### 7. Ligação

GCC liga o código assembly para produzir o executável final.

## Mensagens de Erro

O compilador fornece mensagens de erro para erros de sintaxe e semântica.

### Erros de Sintaxe

Erros de sintaxe ocorrem durante a análise e são relatados imediatamente:

```
Compilation failed due to syntax errors
```

Erros de sintaxe comuns incluem:
- Ponto e vírgula ausentes
- Colchetes não correspondentes
- Sequências de tokens inválidas
- Parâmetros de função ausentes

### Erros Semânticos

Erros semânticos ocorrem durante a verificação de tipos e validação:

```
Compilation failed due to semantic errors
```

Erros semânticos comuns incluem:
- Variáveis não declaradas
- Incompatibilidades de tipo
- Chamadas de função inválidas
- Funções indefinidas

### Erros Internos do Compilador

Se o compilador encontrar um erro inesperado:

```
Internal compiler error
   <mensagem de erro>
   <rastreamento de pilha>
```

## Pré-requisitos

O compilador requer que as seguintes ferramentas estejam instaladas:

### Ferramentas Necessárias

1. **.NET 9.0 SDK** - Para compilar e executar o compilador
2. **Ferramentas LLVM**:
   - `llvm-as` - Montador LLVM
   - `opt` - Otimizador LLVM
   - `llc` - Compilador estático LLVM
3. **GCC** - Para ligação final

### Instalação

#### .NET 9.0 SDK

Instale a partir da [página de download do .NET da Microsoft](https://dotnet.microsoft.com/download).

#### LLVM

Instale as ferramentas LLVM baseado no seu sistema operacional:

- **Linux**: `sudo apt-get install llvm` (Ubuntu/Debian) ou `sudo yum install llvm` (RHEL/CentOS)
- **macOS**: `brew install llvm`
- **Windows**: Baixe de [LLVM releases](https://github.com/llvm/llvm-project/releases)

#### GCC

- **Linux**: Geralmente pré-instalado ou `sudo apt-get install gcc`
- **macOS**: `xcode-select --install` ou `brew install gcc`
- **Windows**: Instale [MinGW-w64](https://www.mingw-w64.org/) ou use WSL

## Compilando o Compilador

### A Partir do Código Fonte

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

O executável do compilador estará em `bin/Debug/net9.0/Three_Musketeers` (ou `bin/Release/net9.0/Three_Musketeers` para builds de release).

### Build de Release

```bash
dotnet build -c Release
```

## Arquitetura do Compilador

O compilador é construído usando:

- **ANTLR4** - Para análise léxica e sintática
- **LLVM** - Para geração de código e otimização
- **.NET 9.0** - Ambiente de execução

### Estrutura do Código Fonte

- `Generate_Grammar/` - Definição da gramática ANTLR
- `Grammar/` - Código do parser gerado
- `Visitors/` - Visitantes AST para análise semântica e geração de código
- `Listeners/` - Ouvintes de erro
- `Models/` - Modelos de tabela de símbolos e escopo
- `Program.cs` - Ponto de entrada principal do compilador

## Tópicos Relacionados

- [Começando](getting-started.pt.md)
- [Referência da Linguagem](language-reference/)
- [Exemplos](examples/examples.pt.md)

