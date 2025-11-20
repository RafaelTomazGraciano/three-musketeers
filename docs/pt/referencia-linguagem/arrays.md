# Arrays

Arrays permitem armazenar múltiplos valores do mesmo tipo em um bloco contíguo de memória. Three Musketeers suporta arrays de uma e múltiplas dimensões.

## Declaração de Array

### Arrays Unidimensionais

```c
int numeros[10];        // Array de 10 inteiros
double precos[5];       // Array de 5 doubles
char buffer[100];       // Array de 100 caracteres
```

### Arrays Multi-dimensionais

```c
int matriz[3][3];       // Array bidimensional 3x3
double grade[5][5][5];   // Array tridimensional 5x5x5
```

## Inicialização de Array

Arrays são indexados a partir de zero (índices começam em 0):

```c
int arr[5];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
arr[3] = 40;
arr[4] = 50;
```

### Acesso a Array Multi-dimensional

```c
int matriz[3][3];
matriz[0][0] = 1;
matriz[0][1] = 2;
matriz[1][0] = 3;
matriz[2][2] = 9;
```

## Limites de Array

Índices de array devem estar dentro dos limites declarados. Acessar índices fora dos limites resulta em comportamento indefinido:

```c
int arr[5];
arr[0] = 10;  // Válido
arr[4] = 50;  // Válido
arr[5] = 60;  // Inválido! Fora dos limites
```

## Arrays e Loops

Padrão comum para iterar através de arrays:

```c
int arr[5];
int i;

// Inicializar array
for (i = 0; i < 5; i++) {
    arr[i] = i * 10;
}

// Imprimir array
for (i = 0; i < 5; i++) {
    printf("arr[%d] = %d\n", i, arr[i]);
}
```

## Arrays Multi-dimensionais e Loops Aninhados

```c
int matriz[3][3];
int i, j;

// Inicializar matriz
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        matriz[i][j] = i * 3 + j;
    }
}

// Imprimir matriz
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        printf("%d ", matriz[i][j]);
    }
    printf("\n");
}
```

## Arrays como Parâmetros de Função

Arrays podem ser passados para funções. Eles são passados por referência:

```c
#include <stdio.tm>

void imprimirArray(int arr[], int tamanho) {
    int i;
    for (i = 0; i < tamanho; i++) {
        printf("%d ", arr[i]);
    }
    printf("\n");
}

int main() {
    int numeros[5] = {10, 20, 30, 40, 50};
    imprimirArray(numeros, 5);
    return 0;
}
```

## Arrays e Ponteiros

Arrays e ponteiros estão intimamente relacionados:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;  // ptr aponta para o primeiro elemento

// Estes são equivalentes:
arr[0] = 10;
*ptr = 10;

// Aritmética de ponteiros
ptr++;
*ptr = 20;  // arr[1] = 20
```

## Arrays de Caracteres (Strings)

Arrays de caracteres podem ser usados para armazenar strings:

```c
char nome[50];
nome[0] = 'J';
nome[1] = 'o';
nome[2] = 'h';
nome[3] = 'n';
nome[4] = '\0';  // Terminador nulo
```

No entanto, Three Musketeers também fornece o tipo `string` para facilitar o trabalho com strings.

## Array de Estruturas

Você pode criar arrays de estruturas:

```c
struct Ponto {
    int x,
    int y
};

struct Ponto pontos[10];
pontos[0].x = 1;
pontos[0].y = 2;
pontos[1].x = 3;
pontos[1].y = 4;
```

## Arrays Dinâmicos

Use ponteiros e `malloc` para arrays dinâmicos:

```c
int *arr = malloc(10);  // Alocar array de 10 inteiros
arr[0] = 1;
arr[1] = 2;
// ... usar array ...
free(arr);  // Não esqueça de liberar!
```

## Padrões Comuns

### Encontrar Valor Máximo

```c
int encontrarMax(int arr[], int tamanho) {
    int max = arr[0];
    int i;
    for (i = 1; i < tamanho; i++) {
        if (arr[i] > max) {
            max = arr[i];
        }
    }
    return max;
}
```

### Soma de Elementos do Array

```c
int somarArray(int arr[], int tamanho) {
    int soma = 0;
    int i;
    for (i = 0; i < tamanho; i++) {
        soma = soma + arr[i];
    }
    return soma;
}
```

## Tópicos Relacionados

- [Variáveis](variaveis.md) - Declarações de variáveis
- [Ponteiros](ponteiros.md) - Arrays e ponteiros
- [Fluxo de Controle](fluxo-controle.md) - Iterando através de arrays

