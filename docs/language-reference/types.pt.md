[English](types.en.md) | [Português](#português)

<a name="português"></a>
# Tipos e Variáveis

Three Musketeers é uma linguagem fortemente tipada com conversão implícita de tipos. Isso significa que todas as variáveis devem ser declaradas com um tipo, e o compilador realiza automaticamente conversões de tipo quando necessário.

## Tipos Primitivos

Three Musketeers suporta cinco tipos primitivos:

### `int`

Usado para representar números inteiros com sinal.

```c
int idade = 25;
int contador = -10;
int zero = 0;
```

### `double`

Usado para representar números de ponto flutuante.

```c
double pi = 3.14159;
double temperatura = -5.5;
double razao = 0.75;
```

### `char`

Usado para representar um único caractere.

```c
char letra = 'A';
char digito = '5';
char simbolo = '@';
```

### `bool`

Usado para representar lógica booleana. Os valores são `true` e `false`.

```c
bool estaAtivo = true;
bool estaCompleto = false;
bool resultado = (5 > 3);  // true
```

### `string`

Representa uma sequência fixa de caracteres (literal de string).

```c
string saudacao = "Olá, Mundo!";
string nome = "Three Musketeers";
string vazio = "";
```

## Declaração de Variáveis

Variáveis devem ser declaradas antes do uso. A sintaxe é:

```c
tipo nome_da_variavel;
```

Ou com inicialização:

```c
tipo nome_da_variavel = valor;
```

### Exemplos

```c
int x;
int y = 10;
double pi = 3.14159;
bool flag = true;
char ch = 'A';
string msg = "Olá";
```

## Arrays

Arrays podem ser criados adicionando `[]` com o número de dimensões. Os índices dos arrays começam em 0.

### Arrays Unidimensionais

```c
int numeros[5];           // Array de 5 inteiros
double valores[10];      // Array de 10 doubles
char buffer[100];         // Array de 100 caracteres
```

### Arrays Multidimensionais (Matrizes)

```c
int matriz[3][3];        // Matriz 3x3
double grade[5][10];     // Matriz 5x10
int cubo[2][2][2];       // Array tridimensional 2x2x2
```

### Inicialização e Acesso a Arrays

```c
int arr[5];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;

int valor = arr[0];      // valor = 10

int matriz[2][2];
matriz[0][0] = 1;
matriz[0][1] = 2;
matriz[1][0] = 3;
matriz[1][1] = 4;
```

## Ponteiros

Ponteiros armazenam endereços de memória. Use `*` para declarar um tipo ponteiro.

```c
int *ptr;                // Ponteiro para int
double *dptr;            // Ponteiro para double
char *cptr;              // Ponteiro para char
int **pptr;              // Ponteiro para ponteiro para int
```

### Operações com Ponteiros

```c
int x = 10;
int *ptr = &x;           // ptr aponta para x
int valor = *ptr;        // Desreferência: valor = 10
*ptr = 20;               // Modifica x através do ponteiro
```

## Conversão de Tipos

Three Musketeers realiza conversão implícita de tipos automaticamente. O compilador converte entre tipos compatíveis conforme necessário.

### Regras de Conversão

- `int` pode ser convertido para `double`
- `double` pode ser convertido para `int` (truncamento)
- `char` pode ser convertido para `int` (valor ASCII)
- `int` pode ser convertido para `char` (se estiver na faixa válida)

### Exemplos

```c
int i = 10;
double d = i;            // int para double: d = 10.0

double pi = 3.14159;
int n = pi;              // double para int: n = 3 (truncado)

char c = 'A';
int codigo = c;          // char para int: codigo = 65 (ASCII)

int valor = 66;
char ch = valor;         // int para char: ch = 'B'
```

Para conversões explícitas de string, veja [Funções de Conversão de Tipo](../standard-library/conversions.pt.md).

## Variáveis Globais

Variáveis podem ser declaradas fora de funções como variáveis globais:

```c
int contadorGlobal;
double valorGlobal = 3.14;
string mensagemGlobal = "Variável global";

int main() {
    contadorGlobal = 42;
    return 0;
}
```

## Tópicos Relacionados

- [Operadores](operators.pt.md)
- [Gerenciamento de Memória](memory.pt.md)
- [Estruturas e Uniões](structures.pt.md)
- [Funções de Conversão de Tipo](../standard-library/conversions.pt.md)

