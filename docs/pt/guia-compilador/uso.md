# Uso do Compilador

O compilador Three Musketeers traduz arquivos-fonte `.3m` em programas executáveis.

## Uso Básico

```bash
tm <arquivo-entrada> [opções]
```

### Exemplo

```bash
tm programa.3m --bin -o programa
```

Isso compila `programa.3m` e cria um executável chamado `programa` no diretório `bin`.

## Processo de Compilação

O compilador executa os seguintes passos:

1. **Análise Léxica**: Tokeniza o código-fonte
2. **Análise Sintática**: Analisa tokens em uma Árvore de Sintaxe Abstrata (AST)
3. **Análise Semântica**: Valida tipos, escopos e semântica
4. **Geração de Código**: Gera código LLVM IR
5. **Geração de Assembly**: Converte LLVM IR para assembly usando `llc`
6. **Linking**: Cria executável usando `gcc`

## Localização da Saída

Por padrão, arquivos compilados são colocados em um diretório `bin` relativo ao arquivo-fonte:

```
fonte.tm          →  bin/fonte.ll  (LLVM IR, se flag --ll usada)
                   →  bin/fonte.s   (Assembly, temporário)
                   →  bin/programa  (Executável)
```

## Requisitos de Arquivo

- Arquivos-fonte devem ter a extensão `.3m`
- O programa deve conter uma função `main`
- Todos os arquivos incluídos devem existir e ser acessíveis

## Mensagens de Erro

O compilador fornece mensagens de erro detalhadas para:

- **Erros de Sintaxe**: Sintaxe inválida ou violações de gramática
- **Erros Semânticos**: Incompatibilidades de tipo, variáveis indefinidas, etc.
- **Erros de Arquivo**: Arquivos ausentes ou caminhos inválidos

## Próximos Passos

- [Opções de Linha de Comando](opcoes-linha-comando.md) - Todas as opções disponíveis do compilador
- [Processo de Compilação](processo-compilacao.md) - Passos detalhados de compilação

