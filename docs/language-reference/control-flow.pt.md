[English](control-flow.en.md) | [Português](#português)

<a name="português"></a>
# Fluxo de Controle

Three Musketeers fornece várias declarações de fluxo de controle para controlar a execução do seu programa: condicionais, loops e declarações de ramificação.

## Declarações Condicionais

### Declaração if

A declaração `if` executa um bloco de código se uma condição for verdadeira.

```c
int x = 10;

if (x > 5) {
    puts("x é maior que 5");
}
```

### Declaração if-else

A cláusula `else` executa quando a condição é falsa.

```c
int x = 3;

if (x > 5) {
    puts("x é maior que 5");
} else {
    puts("x não é maior que 5");
}
```

### Declaração if-else if-else

Você pode encadear múltiplas condições usando `else if`.

```c
int pontuacao = 85;

if (pontuacao >= 90) {
    puts("Nota: A");
} else if (pontuacao >= 80) {
    puts("Nota: B");
} else if (pontuacao >= 70) {
    puts("Nota: C");
} else {
    puts("Nota: F");
}
```

## Declaração switch

A declaração `switch` permite selecionar um de muitos blocos de código para executar com base em um valor.

```c
int opcao = 2;

switch (opcao) {
    case 1:
        puts("Opção 1 selecionada");
        break;
    case 2:
        puts("Opção 2 selecionada");
        break;
    case 3:
        puts("Opção 3 selecionada");
        break;
    default:
        puts("Opção desconhecida");
        break;
}
```

### Switch com Caracteres

Você também pode fazer switch em valores de caractere:

```c
char nota = 'B';

switch (nota) {
    case 'A':
        puts("Excelente");
        break;
    case 'B':
        puts("Bom");
        break;
    case 'C':
        puts("Médio");
        break;
    default:
        puts("Abaixo da média");
        break;
}
```

### Importante: Declaração break

A declaração `break` é crucial em declarações switch. Sem ela, a execução "cairá" para o próximo case.

## Loops

### Loop for

O loop `for` repete um bloco de código um número específico de vezes.

```c
int i;
for (i = 0; i < 10; i++) {
    printf("Iteração %d\n", i);
}
```

O loop `for` tem três partes:
1. **Inicialização** - Executada uma vez no início
2. **Condição** - Verificada antes de cada iteração
3. **Incremento** - Executado após cada iteração

### Variações do Loop for

```c
// Declaração na inicialização
for (int i = 0; i < 5; i++) {
    printf("%d\n", i);
}

// Atribuição na inicialização
int i;
for (i = 0; i < 5; i++) {
    printf("%d\n", i);
}

// Incremento complexo
for (int i = 0; i < 10; i += 2) {
    printf("%d\n", i);  // Imprime 0, 2, 4, 6, 8
}
```

### Loop while

O loop `while` repete um bloco de código enquanto uma condição for verdadeira.

```c
int contador = 0;

while (contador < 5) {
    printf("Contador: %d\n", contador);
    contador++;
}
```

### Loop do-while

O loop `do-while` é similar ao `while`, mas sempre executa pelo menos uma vez porque a condição é verificada no final.

```c
int x = 0;

do {
    printf("x = %d\n", x);
    x++;
} while (x < 5);
```

## Declarações de Controle de Loop

### Declaração break

A declaração `break` sai de um loop imediatamente.

```c
int i;
for (i = 0; i < 10; i++) {
    if (i == 5) {
        break;  // Sair do loop quando i igual a 5
    }
    printf("%d\n", i);
}
// Imprime: 0, 1, 2, 3, 4
```

### Declaração continue

A declaração `continue` pula o resto da iteração atual e continua com a próxima iteração.

```c
int i;
for (i = 0; i < 10; i++) {
    if (i == 5) {
        continue;  // Pular iteração quando i igual a 5
    }
    printf("%d\n", i);
}
// Imprime: 0, 1, 2, 3, 4, 6, 7, 8, 9
```

## Estruturas de Controle Aninhadas

Você pode aninhar estruturas de controle dentro umas das outras.

### Declarações if Aninhadas

```c
int x = 10;
int y = 5;

if (x > 0) {
    if (y > 0) {
        puts("Ambos x e y são positivos");
    } else {
        puts("x é positivo, mas y não é");
    }
}
```

### Loops Aninhados

```c
int i, j;
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        printf("(%d, %d) ", i, j);
    }
    printf("\n");
}
// Saída:
// (0, 0) (0, 1) (0, 2)
// (1, 0) (1, 1) (1, 2)
// (2, 0) (2, 1) (2, 2)
```

### Loops com Condicionais

```c
int i;
for (i = 1; i <= 10; i++) {
    if (i % 2 == 0) {
        printf("%d é par\n", i);
    } else {
        printf("%d é ímpar\n", i);
    }
}
```

## Tópicos Relacionados

- [Operadores](operators.pt.md)
- [Funções](functions.pt.md)
- [Exemplos](../examples/examples.pt.md)

