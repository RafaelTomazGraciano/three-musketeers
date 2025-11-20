# Ponteiros

Ponteiros são variáveis que armazenam endereços de memória. Eles fornecem capacidades poderosas para gerenciamento de memória e manipulação eficiente de dados.

## Declaração de Ponteiro

Declare um ponteiro usando o operador `*`:

```c
int *ptr;        // Ponteiro para int
double *dptr;    // Ponteiro para double
char *cptr;      // Ponteiro para char
```

## Operador Endereço-de (`&`)

Obtenha o endereço de memória de uma variável:

```c
int x = 10;
int *ptr = &x;   // ptr agora contém o endereço de x
```

## Operador Desreferenciação (`*`)

Acesse o valor no endereço de um ponteiro:

```c
int x = 10;
int *ptr = &x;
int y = *ptr;    // y = 10 (valor no endereço armazenado em ptr)
```

## Atribuição de Ponteiro

```c
int x = 10;
int *ptr = &x;   // ptr aponta para x
*ptr = 20;       // x agora é 20
```

## Ponteiro para Ponteiro

Você pode criar ponteiros para ponteiros:

```c
int x = 10;
int *ptr = &x;
int **pptr = &ptr;  // Ponteiro para ponteiro

**pptr = 30;        // x agora é 30
```

## Alocação Dinâmica de Memória

### malloc

Aloque memória dinamicamente:

```c
int *arr = malloc(10);  // Alocar memória para 10 inteiros
arr[0] = 1;
arr[1] = 2;
```

### free

Libere memória alocada dinamicamente:

```c
int *ptr = malloc(5);
// Use ptr...
free(ptr);  // Liberar memória
```

**Importante**: Sempre libere memória alocada com `malloc` para evitar vazamentos de memória.

## Ponteiros e Arrays

Ponteiros e arrays estão intimamente relacionados:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;  // ptr aponta para o primeiro elemento

// Estes são equivalentes:
arr[0] = 10;
*ptr = 10;
```

## Aritmética de Ponteiros

Você pode realizar aritmética em ponteiros:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;

ptr++;        // Mover para o próximo elemento
int valor = *ptr;  // valor = 2
```

## Parâmetros de Função

Ponteiros permitem que funções modifiquem variáveis:

```c
void incrementar(int *x) {
    (*x)++;
}

int main() {
    int valor = 10;
    incrementar(&valor);  // valor agora é 11
    return 0;
}
```

## Retornando Ponteiros

Funções podem retornar ponteiros:

```c
int* criarArray(int tamanho) {
    int *arr = malloc(tamanho);
    return arr;
}
```

## Exemplo: Trocando Valores

```c
void trocar(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10, y = 20;
    trocar(&x, &y);  // x = 20, y = 10
    return 0;
}
```

## Armadilhas Comuns

1. **Desreferenciar ponteiros NULL ou não inicializados**: Sempre inicialize ponteiros
2. **Vazamentos de memória**: Sempre libere memória alocada com `malloc`
3. **Ponteiros pendentes**: Não use ponteiros após liberar memória

## Tópicos Relacionados

- [Variáveis](variaveis.md) - Declarações de variáveis
- [Arrays](arrays.md) - Arrays e ponteiros
- [Funções](funcoes.md) - Funções com parâmetros de ponteiro

