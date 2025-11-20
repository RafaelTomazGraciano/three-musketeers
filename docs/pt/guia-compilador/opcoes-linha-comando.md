# Opções de Linha de Comando

Referência completa para todas as opções de linha de comando do compilador Three Musketeers.

## Visão Geral das Opções

|    Opção    | Curta |              Descrição            | Padrão  |
|-------------|-------|-----------------------------------|---------|
| `--bin`     | -     | Gerar executável no diretório bin | `false` |
| `--out`     | `-o`  | Nome do executável de saída       | `a.out` |
| `--opt`     | `-O`  | Nível de otimização (0-3)         | `2`     |
| `--ll`      | -     | Manter código LLVM IR             | `false` |
| `-g`        | -     | Adicionar informações de debug    | `false` |
| `--Include` | `-I`  | Caminho de biblioteca de inclusão | -       |
| `--version` | `-v`  | Mostrar versão                    | -       |

## Opções de Saída

### `-o, --out`

Especifique o nome do executável de saída.

**Sintaxe:**
```bash
tm entrada.tm -o nome_saida
```

**Exemplo:**
```bash
tm programa.tm -o meuprograma
# Cria: meuprograma
```

**Padrão:** `a.out`

## Opções de Otimização

### `-O, --opt`

Defina o nível de otimização para geração de código LLVM.

**Sintaxe:**
```bash
tm entrada.tm -O <nível>
```

**Níveis:**
- `0` - Sem otimização
- `1` - Otimizações básicas
- `2` - Otimizações padrão (padrão)
- `3` - Otimizações agressivas

**Exemplos:**
```bash
tm programa.tm -O 0    # Sem otimização
tm programa.tm -O 3    # Máxima otimização
```

**Padrão:** `2`

### `--bin`

Gera o executável no diretório `bin`.

**Sintaxe:**
```bash
tm input.tm --bin
```

**Exemplo:**
```bash
tm program.tm --bin -o program
# Cria: bin/program
```

## Opções de Debug

### `-g`

Adicione informações de debug ao executável gerado para ferramentas de debug.

**Sintaxe:**
```bash
tm entrada.tm -g
```

**Exemplo:**
```bash
tm programa.tm -g -o programa_debug
```

**Nota:** Esta flag é passada para GCC durante o linking.

## Opções de LLVM IR

### `--ll`

Mantenha o arquivo de código LLVM IR gerado (`.ll`) após a compilação.

**Sintaxe:**
```bash
tm entrada.tm --ll
```

**Exemplo:**
```bash
tm programa.tm --ll
# Cria: programa.ll
```

**Padrão:** Arquivos LLVM IR são deletados após a compilação a menos que esta flag seja usada.

## Opções de Caminho de Inclusão

### `-I, --Include`

Especifique um caminho para bibliotecas incluídas.

**Sintaxe:**
```bash
tm entrada.tm -I <caminho>
tm entrada.tm --Include <caminho>
```

**Exemplo:**
```bash
tm programa.tm -I /usr/local/include
```

## Informações de Versão

### `-v, --version`

Exiba a versão do compilador.

**Sintaxe:**
```bash
tm --version
tm -v
```

**Saída:**
```
Three Musketeers Compiler v1.0.0Athos
```

## Combinando Opções

Você pode combinar múltiplas opções:

```bash
tm programa.tm -o minhaapp -O 3 -g --ll
```

Este comando:
- Compila `programa.tm`
- Cria executável `minhaapp`
- Usa nível de otimização 3
- Adiciona informações de debug
- Mantém o arquivo LLVM IR

## Exemplos

### Compilação Básica
```bash
tm hello.tm
# Cria: a.out
```

### Build de Release Otimizado
```bash
tm app.tm -o app -O 3
# Cria: app (otimizado)
```

### Build de Debug
```bash
tm app.tm -o app_debug -g
# Cria: app_debug (com informações de debug)
```

### Build de Desenvolvimento (Manter IR)
```bash
tm app.tm -o app --ll
# Cria: app e app.ll
```

## Tópicos Relacionados

- [Uso](uso.md) - Uso básico do compilador
- [Processo de Compilação](processo-compilacao.md) - Como a compilação funciona

