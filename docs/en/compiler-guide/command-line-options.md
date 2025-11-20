# Command Line Options

Complete reference for all Three Musketeers compiler command-line options.

## Options Overview

| Option | Short | Description | Default |
|--------|-------|-------------|---------|
| `--bin` | - | Generate executable in bin directory | `false` |
| `--out` | `-o` | Output executable name | `a.out` |
| `--opt` | `-O` | Optimization level (0-3) | `2` |
| `--ll` | - | Keep LLVM IR code | `false` |
| `-g` | - | Add debug information | `false` |
| `--Include` | `-I` | Include library path | - |
| `--version` | `-v` | Show version | - |

## Output Options

### `-o, --out`

Specify the name of the output executable.

**Syntax:**
```bash
Three_Musketeers input.tm -o output_name
```

**Example:**
```bash
Three_Musketeers program.tm -o myprogram
# Creates: myprogram
```

**Default:** `a.out`

## Optimization Options

### `-O, --opt`

Set the optimization level for LLVM code generation.

**Syntax:**
```bash
Three_Musketeers input.tm -O <level>
```

**Levels:**
- `0` - No optimization
- `1` - Basic optimizations
- `2` - Standard optimizations (default)
- `3` - Aggressive optimizations

**Examples:**
```bash
Three_Musketeers program.tm -O 0    # No optimization
Three_Musketeers program.tm -O 3    # Maximum optimization
```

**Default:** `2`

### `--bin`

Generate the executable in the `bin` directory.

**Syntax:**
```bash
tm input.tm --bin
```

**Example:**
```bash
tm program.tm --bin
# Creates: bin/program
```

## Debug Options

### `-g`

Add debug information to the generated executable for debugging tools.

**Syntax:**
```bash
Three_Musketeers input.tm -g
```

**Example:**
```bash
Three_Musketeers program.tm -g -o debug_program
```

**Note:** This flag is passed to GCC during linking.

## LLVM IR Options

### `--ll`

Keep the generated LLVM IR code file (`.ll`) after compilation.

**Syntax:**
```bash
Three_Musketeers input.tm --ll
```

**Example:**
```bash
Three_Musketeers program.tm --ll
# Creates: program.ll (kept)
```

**Default:** LLVM IR files are deleted after compilation unless this flag is used.

## Include Path Options

### `-I, --Include`

Specify a path for included libraries.

**Syntax:**
```bash
Three_Musketeers input.tm -I <path>
Three_Musketeers input.tm --Include <path>
```

**Example:**
```bash
Three_Musketeers program.tm -I /usr/local/include
```

## Version Information

### `-v, --version`

Display the compiler version.

**Syntax:**
```bash
Three_Musketeers --version
Three_Musketeers -v
```

**Output:**
```
Three Musketeers Compiler v1.0.0Athos
```

## Combining Options

You can combine multiple options:

```bash
Three_Musketeers program.tm -o myapp -O 3 -g --ll
```

This command:
- Compiles `program.tm`
- Creates executable `myapp`
- Uses optimization level 3
- Adds debug information
- Keeps the LLVM IR file

## Examples

### Basic Compilation
```bash
Three_Musketeers hello.tm
# Creates: a.out
```

### Optimized Release Build
```bash
Three_Musketeers app.tm -o app -O 3
# Creates: app (optimized)
```

### Debug Build
```bash
Three_Musketeers app.tm -o app_debug -g
# Creates: app_debug (with debug info)
```

### Development Build (Keep IR)
```bash
Three_Musketeers app.tm -o app --ll
# Creates: app and app.ll
```

## Related Topics

- [Usage](usage.md) - Basic compiler usage
- [Compilation Process](compilation-process.md) - How compilation works

