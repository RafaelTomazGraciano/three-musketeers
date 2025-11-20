# Processo de Compilação

Entendendo como o compilador Three Musketeers transforma código-fonte em programas executáveis.

## Visão Geral

O processo de compilação consiste em seis fases principais:

1. Análise Léxica
2. Análise Sintática
3. Análise Semântica
4. Geração de Código
5. Geração de Assembly
6. Linking

## Fase 1: Análise Léxica

O compilador lê o arquivo-fonte e o divide em tokens (palavras-chave, identificadores, operadores, literais, etc.).

**Entrada:** Código-fonte (arquivo `.3m`)
**Saída:** Fluxo de tokens

**Exemplo:**
```c
int x = 10;
```

Tokens: `int`, `x`, `=`, `10`, `;`

## Fase 2: Análise Sintática

O parser usa ANTLR4 para construir uma Árvore de Sintaxe Abstrata (AST) a partir do fluxo de tokens, verificando se o código segue a gramática da linguagem.

**Entrada:** Fluxo de tokens
**Saída:** Árvore de Sintaxe Abstrata (AST)

**Erros:** Erros de sintaxe (gramática inválida, ponto e vírgula ausentes, etc.)

## Fase 3: Análise Semântica

O analisador semântico valida o significado do programa:

- Verificação de tipos
- Validação de escopo de variáveis
- Validação de chamadas de função
- Gerenciamento de tabela de símbolos
- Validação de fluxo de controle (break/continue no contexto correto)

**Entrada:** AST
**Saída:** AST validado com informações semânticas

**Erros:** Erros semânticos (incompatibilidades de tipo, variáveis indefinidas, etc.)

## Fase 4: Geração de Código

O gerador de código percorre a AST validada e gera código LLVM Intermediate Representation (IR).

**Entrada:** AST validado
**Saída:** Código LLVM IR (arquivo `.ll`)

**Exemplo:**
```c
int add(int x, int y) {
    return x + y;
}
```

Gera LLVM IR com definições de função, instruções e informações de tipo.

## Fase 5: Geração de Assembly

A toolchain LLVM (`llc`) converte LLVM IR em código assembly específico do alvo.

**Entrada:** LLVM IR (arquivo `.ll`)
**Saída:** Código assembly (arquivo `.s`)

**Comando:**
```bash
llc programa.ll -O2 -o programa.s
```

O nível de otimização (`-O`) é determinado pela opção `-O` do compilador.

## Fase 6: Linking

GCC linka o código assembly e bibliotecas do sistema para criar o executável final.

**Entrada:** Código assembly (arquivo `.s`)
**Saída:** Binário executável

**Comando:**
```bash
gcc programa.s -o programa -no-pie
```

Se a flag `-g` for usada, GCC é chamado com a opção `-g` para informações de debug.

## Ciclo de Vida do Arquivo

```
programa.tm
    ↓ (Análise Léxica e Sintática)
AST
    ↓ (Análise Semântica)
AST Validado
    ↓ (Geração de Código)
programa.ll  (LLVM IR)
    ↓ (Geração de Assembly)
programa.s   (Assembly, temporário)
    ↓ (Linking)
programa     (Executável)
```

**Nota:** O arquivo `.s` é deletado após o linking, a menos que ocorra um erro. O arquivo `.ll` é mantido apenas se a flag `--ll` for usada.

## Tratamento de Erros

### Erros de Sintaxe

Detectados durante a análise sintática:
```
Erro: Esperado ';' na linha 5
```

### Erros Semânticos

Detectados durante a análise semântica:
```
Erro: Incompatibilidade de tipo na linha 10
Erro: Variável 'x' indefinida na linha 15
```

### Erros de Compilação

Se qualquer fase falhar, a compilação para e uma mensagem de erro é exibida.

## Otimização

A otimização ocorre no nível LLVM IR:

- **Nível 0**: Sem otimização
- **Nível 1**: Otimizações básicas
- **Nível 2**: Otimizações padrão (padrão)
- **Nível 3**: Otimizações agressivas

Otimizações incluem:
- Eliminação de código morto
- Dobramento de constantes
- Otimizações de loop
- Inlining

## Informações de Debug

Ao usar a flag `-g`, informações de debug são adicionadas durante a fase de linking, permitindo que debuggers mapeiem código executável de volta para código-fonte.

## Tópicos Relacionados

- [Uso](uso.md) - Como usar o compilador
- [Opções de Linha de Comando](opcoes-linha-comando.md) - Opções do compilador

