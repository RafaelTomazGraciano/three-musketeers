# Installation

This guide will help you install the Three Musketeers compiler on your system.

## Prerequisites

Before installing the Three Musketeers compiler, ensure you have the following dependencies installed:

### Required Dependencies

- **.NET SDK 9.0** or later
- **LLVM** (with `llc` command available)
- **GCC** (GNU Compiler Collection)

### Installing Dependencies

#### Ubuntu/Debian

```bash
# Install .NET SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0

# Install LLVM
sudo apt-get update
sudo apt-get install llvm

# Install GCC
sudo apt-get install gcc
```

#### macOS

```bash
# Install .NET SDK
brew install dotnet

# Install LLVM
brew install llvm

# Install GCC
brew install gcc
```

#### Windows

- Download and install .NET SDK 9.0 from [Microsoft's website](https://dotnet.microsoft.com/download)
- Install LLVM from [LLVM releases](https://github.com/llvm/llvm-project/releases)
- Install MinGW-w64 or use WSL with the Linux instructions

## Building the Compiler

1. Clone the repository:

```bash
git clone <repository-url>
cd three-musketeers
```

2. Build the compiler:

```bash
cd Three_Musketeers
dotnet build
```

3. (Optional) Create a release build:

```bash
./create_release.sh
```

The compiled executable will be available in the `release` directory.

## Verifying Installation

To verify that the compiler is installed correctly, run:

```bash
./Three_Musketeers/bin/Debug/net9.0/Three_Musketeers --version
```

You should see the version number (v0.9.5) displayed.

## Next Steps

- [Quick Start Guide](quick-start.md)
- [Hello World Tutorial](hello-world.md)

