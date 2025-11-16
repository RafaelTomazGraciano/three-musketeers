# Visão Geral da Linguagem

Three Musketeers é uma linguagem de programação fortemente tipada, similar ao C, com conversão de tipo implícita. Serve como um subconjunto de C com recursos adicionais como tipos string e boolean nativos.

## Características da Linguagem

### Tipagem Forte

Todas as variáveis devem ser declaradas com um tipo explícito. O compilador impõe segurança de tipo em todo o seu programa.

### Conversão de Tipo Implícita

O compilador realiza automaticamente conversões de tipo quando necessário, eliminando a necessidade de conversão explícita na maioria dos casos.

### Sintaxe Similar ao C

Se você está familiarizado com C, se sentirá em casa com Three Musketeers. A sintaxe é muito similar, com algumas melhorias.

## Recursos Principais

- **Cinco Tipos Básicos**: `int`, `double`, `char`, `bool`, `string`
- **Ponteiros**: Suporte completo a ponteiros com desreferenciação
- **Arrays**: Arrays de uma e múltiplas dimensões
- **Estruturas e Uniões**: Tipos compostos definidos pelo usuário
- **Funções**: Funções com vários tipos de retorno e parâmetros
- **Fluxo de Controle**: `if/else`, `switch`, `for`, `while`, `do-while`
- **Gerenciamento de Memória**: Alocação dinâmica com `malloc` e `free`
- **Operações de E/S**: `printf`, `scanf`, `puts`, `gets` para entrada/saída
- **Preprocessador**: Diretivas `#include` e `#define`

## Estrutura do Programa

Um programa Three Musketeers normalmente segue esta estrutura:

```c
#include <stdio.tm>

int main() {
    // Seu código aqui
    return 0;
}
```

## Extensão de Arquivo

Arquivos-fonte Three Musketeers usam a extensão `.tm`.

## Próximos Passos

- [Tipos de Dados](tipos-dados.md) - Aprenda sobre os tipos disponíveis
- [Variáveis](variaveis.md) - Entenda declarações de variáveis
- [Funções](funcoes.md) - Crie blocos de código reutilizáveis

