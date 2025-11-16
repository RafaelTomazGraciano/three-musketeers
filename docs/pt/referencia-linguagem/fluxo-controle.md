# Fluxo de Controle

Declarações de fluxo de controle permitem controlar a ordem de execução do seu programa. Three Musketeers suporta declarações condicionais e loops similares ao C.

## Declaração If

Execute código condicionalmente com base em uma expressão booleana.

### If Básico

```c
if (condicao) {
    // Código para executar se condição for true
}
```

### If-Else

```c
if (condicao) {
    // Código se condição for true
} else {
    // Código se condição for false
}
```

### If-Else If-Else

```c
if (condicao1) {
    // Código se condicao1 for true
} else if (condicao2) {
    // Código se condicao2 for true
} else {
    // Código se todas as condições forem false
}
```

### Exemplo

```c
int valor = 10;

if (valor > 0) {
    printf("Valor é positivo\n");
} else if (valor < 0) {
    printf("Valor é negativo\n");
} else {
    printf("Valor é zero\n");
}
```

## Declaração Switch

Execute diferentes blocos de código com base no valor de uma expressão.

```c
switch (expressao) {
    case valor1:
        // Código para valor1
        break;
    case valor2:
        // Código para valor2
        break;
    default:
        // Código se nenhum case corresponder
        break;
}
```

### Exemplo

```c
int opcao = 2;

switch (opcao) {
    case 1:
        printf("Opção 1 selecionada\n");
        break;
    case 2:
        printf("Opção 2 selecionada\n");
        break;
    case 3:
        printf("Opção 3 selecionada\n");
        break;
    default:
        printf("Opção desconhecida\n");
        break;
}
```

**Nota**: A declaração `break` é necessária para sair do bloco switch. Sem ela, a execução "cairá" para o próximo case.

## Loop For

Execute código um número específico de vezes.

```c
for (inicializacao; condicao; incremento) {
    // Código para repetir
}
```

### Exemplo

```c
int i;
for (i = 0; i < 10; i++) {
    printf("Iteração %d\n", i);
}
```

### Variações

```c
// Inicializar variável no loop
for (int i = 0; i < 10; i++) {
    printf("%d\n", i);
}

// Múltiplas variáveis
int i, j;
for (i = 0, j = 10; i < 10; i++, j--) {
    printf("i=%d, j=%d\n", i, j);
}
```

## Loop While

Execute código enquanto uma condição for verdadeira.

```c
while (condicao) {
    // Código para repetir
}
```

### Exemplo

```c
int contagem = 0;
while (contagem < 5) {
    printf("Contagem: %d\n", contagem);
    contagem = contagem + 1;
}
```

## Loop Do-While

Execute código pelo menos uma vez, depois repita enquanto a condição for verdadeira.

```c
do {
    // Código para repetir
} while (condicao);
```

### Exemplo

```c
int contagem = 0;
do {
    printf("Contagem: %d\n", contagem);
    contagem = contagem + 1;
} while (contagem < 5);
```

## Declaração Break

Saia de um loop ou declaração switch imediatamente.

```c
for (int i = 0; i < 10; i++) {
    if (i == 5) {
        break;  // Sair do loop quando i for 5
    }
    printf("%d\n", i);
}
```

**Nota**: `break` só pode ser usado dentro de loops ou declarações switch.

## Declaração Continue

Pule o resto da iteração atual do loop e continue com a próxima iteração.

```c
for (int i = 0; i < 10; i++) {
    if (i == 5) {
        continue;  // Pular iteração quando i for 5
    }
    printf("%d\n", i);
}
```

**Nota**: `continue` só pode ser usado dentro de loops.

## Fluxo de Controle Aninhado

Você pode aninhar declarações de fluxo de controle:

```c
for (int i = 0; i < 3; i++) {
    for (int j = 0; j < 3; j++) {
        if (i == j) {
            printf("Diagonal: %d, %d\n", i, j);
        }
    }
}
```

## Tópicos Relacionados

- [Operadores](operadores.md) - Operadores usados em condições
- [Funções](funcoes.md) - Retornando de funções
- [Exemplos](../exemplos/exemplos.md) - Mais exemplos de fluxo de controle

