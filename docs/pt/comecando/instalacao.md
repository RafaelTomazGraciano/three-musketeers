# Instalação

Este guia ajudará você a instalar o compilador Three Musketeers em seu sistema.

## Pré-requisitos

Antes de instalar o compilador Three Musketeers, certifique-se de ter as seguintes dependências instaladas:

### Dependências Necessárias

- **.NET SDK 9.0** ou posterior
- **LLVM** (com comando `llc` disponível)
- **GCC** (GNU Compiler Collection)

### Instalando Dependências

#### Ubuntu/Debian

```bash
# Instalar .NET SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0

# Instalar LLVM
sudo apt-get update
sudo apt-get install llvm

# Instalar GCC
sudo apt-get install gcc
```

#### macOS

```bash
# Instalar .NET SDK
brew install dotnet

# Instalar LLVM
brew install llvm

# Instalar GCC
brew install gcc
```

#### Windows

- Baixe e instale o .NET SDK 9.0 do [site da Microsoft](https://dotnet.microsoft.com/download)
- Instale o LLVM das [releases do LLVM](https://github.com/llvm/llvm-project/releases)
- Instale MinGW-w64 ou use WSL com as instruções do Linux

## Compilando o Compilador

1. Clone o repositório:

```bash
git clone <url-do-repositorio>
cd three-musketeers
```

2. Compile o compilador:

```bash
cd Three_Musketeers
dotnet build
```

3. (Opcional) Crie uma build de release:

```bash
./create_release.sh
```

O executável compilado estará disponível no diretório `release`.

## Verificando a Instalação

Para verificar se o compilador está instalado corretamente, execute:

```bash
./Three_Musketeers/bin/Debug/net9.0/tm --version
```

Você deve ver o número da versão (v1.0.0 Athos) exibido.

## Próximos Passos

- [Guia de Início Rápido](inicio-rapido.md)
- [Tutorial Olá Mundo](ola-mundo.md)

