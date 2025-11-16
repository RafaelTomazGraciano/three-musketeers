# Opções de Linha de Comando

Referência completa para todas as opções de linha de comando do compilador Three Musketeers.

## Visão Geral das Opções

| Opção | Curta | Descrição | Padrão |
|--------|-------|-----------|---------|
| `--out` | `-o` | Nome do executável de saída | `a.out` |
| `--opt` | `-O` | Nível de otimização (0-3) | `2` |
| `--ll` | - | Manter código LLVM IR | `false` |
| `-g` | - | Adicionar informações de debug | `false` |
| `--Include` | `-I` | Caminho de biblioteca de inclusão | - |
| `--version` | `-v` | Mostrar versão | - |

## Opções de Saída

### `-o, --out`

Especifique o nome do executável de saída.

**Sintaxe:**
```bash
Three_Musketeers entrada.tm -o nome_saida
```

**Exemplo:**
```bash
Three_Musketeers programa.tm -o meuprograma
# Cria: bin/meuprograma
```

**Padrão:** `a.out`

## Opções de Otimização

### `-O, --opt`

Defina o nível de otimização para geração de código LLVM.

**Sintaxe:**
```bash
Three_Musketeers entrada.tm -O <nível>
```

**Níveis:**
- `0` - Sem otimização
- `1` - Otimizações básicas
- `2` - Otimizações padrão (padrão)
- `3` - Otimizações agressivas

**Exemplos:**
```bash
Three_Musketeers programa.tm -O 0    # Sem otimização
Three_Musketeers programa.tm -O 3    # Máxima otimização
```

**Padrão:** `2`

## Opções de Debug

### `-g`

Adicione informações de debug ao executável gerado para ferramentas de debug.

**Sintaxe:**
```bash
Three_Musketeers entrada.tm -g
```

**Exemplo:**
```bash
Three_Musketeers programa.tm -g -o programa_debug
```

**Nota:** Esta flag é passada para GCC durante o linking.

## Opções de LLVM IR

### `--ll`

Mantenha o arquivo de código LLVM IR gerado (`.ll`) após a compilação.

**Sintaxe:**
```bash
Three_Musketeers entrada.tm --ll
```

**Exemplo:**
```bash
Three_Musketeers programa.tm --ll
# Cria: bin/programa.ll (mantido)
```

**Padrão:** Arquivos LLVM IR são deletados após a compilação a menos que esta flag seja usada.

## Opções de Caminho de Inclusão

### `-I, --Include`

Especifique um caminho para bibliotecas incluídas.

**Sintaxe:**
```bash
Three_Musketeers entrada.tm -I <caminho>
Three_Musketeers entrada.tm --Include <caminho>
```

**Exemplo:**
```bash
Three_Musketeers programa.tm -I /usr/local/include
```

## Informações de Versão

### `-v, --version`

Exiba a versão do compilador.

**Sintaxe:**
```bash
Three_Musketeers --version
Three_Musketeers -v
```

**Saída:**
```
Three Musketeers Compiler v0.9.5
```

## Combinando Opções

Você pode combinar múltiplas opções:

```bash
Three_Musketeers programa.tm -o minhaapp -O 3 -g --ll
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
Three_Musketeers hello.tm
# Cria: bin/a.out
```

### Build de Release Otimizado
```bash
Three_Musketeers app.tm -o app -O 3
# Cria: bin/app (otimizado)
```

### Build de Debug
```bash
Three_Musketeers app.tm -o app_debug -g
# Cria: bin/app_debug (com informações de debug)
```

### Build de Desenvolvimento (Manter IR)
```bash
Three_Musketeers app.tm -o app --ll
# Cria: bin/app e bin/app.ll
```

## Tópicos Relacionados

- [Uso](uso.md) - Uso básico do compilador
- [Processo de Compilação](processo-compilacao.md) - Como a compilação funciona

