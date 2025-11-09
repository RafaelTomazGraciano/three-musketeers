[English](memory.en.md) | [Português](#português)

<a name="português"></a>
# Gerenciamento de Memória

Three Musketeers fornece gerenciamento manual de memória através de ponteiros, alocação dinâmica com `malloc` e desalocação com `free`.

## Ponteiros

Ponteiros são variáveis que armazenam endereços de memória. Eles permitem trabalhar com memória diretamente e passar dados por referência.

### Declaração de Ponteiro

```c
int *ptr;           // Ponteiro para int
double *dptr;       // Ponteiro para double
char *cptr;         // Ponteiro para char
int **pptr;         // Ponteiro para ponteiro para int
```

### Operador de Endereço (&)

O operador de endereço `&` retorna o endereço de memória de uma variável.

```c
int x = 10;
int *ptr = &x;      // ptr agora contém o endereço de x
```

### Operador de Desreferência (*)

O operador de desreferência `*` acessa o valor em um endereço de memória.

```c
int x = 10;
int *ptr = &x;

int valor = *ptr;   // valor = 10 (desreferência)
*ptr = 20;          // x = 20 (modificar através do ponteiro)
```

### Aritmética de Ponteiros

Ponteiros podem ser usados com arrays e suportam operações aritméticas.

```c
int arr[5] = {10, 20, 30, 40, 50};
int *ptr = arr;     // Aponta para o primeiro elemento

int primeiro = *ptr;           // 10
int segundo = *(ptr + 1);       // 20
int terceiro = ptr[2];          // 30 (notação de array)
```

## Alocação Dinâmica de Memória

### malloc

A função `malloc` aloca um bloco de memória do tamanho especificado e retorna um ponteiro para ele.

```c
int *arr = malloc(5);       // Alocar memória para 5 inteiros
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
```

### malloc com Atribuição

Você pode declarar e alocar em uma única declaração:

```c
int *ptr;
ptr = malloc(10);           // Alocar memória para 10 inteiros

// Ou combinar declaração e alocação
int *newPtr = malloc(5);   // Alocar memória para 5 inteiros
```

### Inicialização de Ponteiro com malloc

```c
int *pointer;
pointer = malloc(size);     // Alocar memória
pointer[0] = 100;
pointer[1] = 200;
```

## Desalocação de Memória

### free

A função `free` desaloca memória que foi previamente alocada com `malloc`. Sempre libere a memória quando terminar de usá-la para evitar vazamentos de memória.

```c
int *ptr = malloc(5);
// ... usar ptr ...
free(ptr);          // Desalocar memória
```

### Exemplo: Gerenciamento Completo de Memória

```c
int *criarArray(int tamanho) {
    int *arr = malloc(tamanho);
    int i;
    for (i = 0; i < tamanho; i++) {
        arr[i] = i * 10;
    }
    return arr;
}

int main() {
    int *numeros = criarArray(5);
    
    int i;
    for (i = 0; i < 5; i++) {
        printf("%d ", numeros[i]);
    }
    printf("\n");
    
    free(numeros);  // Importante: liberar memória alocada
    return 0;
}
```

## Ponteiro para Ponteiro

Você pode ter ponteiros que apontam para outros ponteiros.

```c
int x = 10;
int *ptr = &x;          // ptr aponta para x
int **pptr = &ptr;      // pptr aponta para ptr

int valor = **pptr;     // valor = 10 (dupla desreferência)
**pptr = 20;            // x = 20

// Modificar através de ponteiro duplo
int *otherPtr = malloc(5);
int **otherPptr = &otherPtr;
(**otherPptr)[0] = 77;  // Modificar array através de ponteiro duplo
```

## Passando Ponteiros para Funções

Passar ponteiros para funções permite que elas modifiquem os valores originais.

```c
void modificarValor(int *ptr) {
    *ptr = 100;
}

void trocar(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10;
    modificarValor(&x);
    printf("x = %d\n", x);  // x = 100
    
    int a = 5, b = 10;
    trocar(&a, &b);
    printf("a = %d, b = %d\n", a, b);  // a = 10, b = 5
    return 0;
}
```

## Retornando Ponteiros de Funções

Funções podem retornar ponteiros para memória alocada dinamicamente.

```c
int *alocarArray(int tamanho) {
    int *arr = malloc(tamanho);
    return arr;
}

int *criarArrayInicializado(int tamanho, int valor) {
    int *arr = malloc(tamanho);
    int i;
    for (i = 0; i < tamanho; i++) {
        arr[i] = valor;
    }
    return arr;
}
```

## Armadilhas Comuns

### Vazamentos de Memória

Sempre libere a memória alocada com `malloc`:

```c
int *ptr = malloc(10);
// ... usar ptr ...
free(ptr);  // Não esqueça de liberar!
```

### Ponteiros Pendentes

Não use ponteiros após liberá-los:

```c
int *ptr = malloc(10);
free(ptr);
// ptr agora é um ponteiro pendente - não use!
```

### Desreferência de Ponteiro Nulo

Verifique ponteiros nulos antes de desreferenciar:

```c
int *ptr = malloc(10);
if (ptr != 0) {  // Verificar se a alocação teve sucesso
    *ptr = 100;
}
```

## Tópicos Relacionados

- [Tipos e Variáveis](types.pt.md)
- [Funções](functions.pt.md)
- [Estruturas e Uniões](structures.pt.md)

