# Variáveis

Variáveis em Three Musketeers devem ser declaradas com um tipo explícito antes do uso. A linguagem é fortemente tipada com conversão de tipo implícita.

## Declaração de Variáveis

### Declaração Básica

```c
int contagem;
double preco;
char inicial;
bool estaPronto;
string nome;
```

### Declaração com Inicialização

```c
int contagem = 10;
double pi = 3.14159;
char letra = 'A';
bool ativo = true;
string saudacao = "Hello";
```

### Múltiplas Declarações

Você pode declarar múltiplas variáveis do mesmo tipo:

```c
int x, y, z;
double a = 1.0, b = 2.0, c = 3.0;
```

## Variáveis de Array

### Arrays Unidimensionais

```c
int numeros[5];
double precos[10];
char buffer[100];
```

### Arrays Multi-dimensionais

```c
int matriz[3][3];
double grade[5][5][5];
```

### Inicialização de Arrays

Arrays são acessados usando indexação baseada em zero:

```c
int arr[3];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
```

## Variáveis de Ponteiro

Ponteiros são declarados usando o operador `*`:

```c
int *ptr;
double *dptr;
int **ponteiroDuplo;  // Ponteiro para ponteiro
```

Veja [Ponteiros](ponteiros.md) para informações detalhadas.

## Variáveis Globais

Variáveis declaradas fora de funções são globais:

```c
int contadorGlobal;
double valorGlobal = 3.14;
string mensagemGlobal = "Global";

int main() {
    contadorGlobal = 42;
    return 0;
}
```

## Escopo de Variáveis

- **Escopo Global**: Variáveis declaradas fora de funções são acessíveis em todo o programa
- **Escopo Local**: Variáveis declaradas dentro de funções são acessíveis apenas dentro dessa função

## Conversão de Tipo

O compilador realiza automaticamente conversão de tipo implícita:

```c
int x = 10;
double y = x;        // int convertido para double
int z = y;           // double convertido para int (truncado)
```

## Tópicos Relacionados

- [Tipos de Dados](tipos-dados.md) - Tipos disponíveis
- [Operadores](operadores.md) - Operações em variáveis
- [Ponteiros](ponteiros.md) - Trabalhando com ponteiros

