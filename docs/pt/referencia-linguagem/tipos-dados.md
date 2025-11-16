# Tipos de Dados

Three Musketeers fornece cinco tipos de dados fundamentais, ordenados do mais genérico ao mais específico.

## Tipos Básicos

### 1. double

Usado para representar números de ponto flutuante (números decimais).

```c
double pi = 3.14159;
double temperatura = 98.6;
```

### 2. int

Usado para representar inteiros com sinal (números inteiros).

```c
int contagem = 42;
int idade = 25;
```

### 3. char

Usado para representar um único caractere.

```c
char letra = 'A';
char novaLinha = '\n';
```

### 4. bool

Usado para representar lógica booleana (`true` ou `false`).

```c
bool estaAtivo = true;
bool estaCompleto = false;
```

### 5. string

Representa uma sequência fixa de caracteres (literal de string).

```c
string saudacao = "Hello, World!";
string mensagem = "Three Musketeers";
```

## Hierarquia de Tipos

Os tipos são ordenados do mais genérico (double) ao mais específico (string). Essa ordenação é importante para conversão de tipo implícita:

```
double → int → char → bool → string
```

## Conversão de Tipo

O compilador converte automaticamente entre tipos compatíveis. Por exemplo:

```c
int x = 10;
double y = x;  // int automaticamente convertido para double
```

## Arrays

Arrays podem ser criados adicionando `[]` com tamanhos de dimensão:

```c
int numeros[10];           // Array unidimensional
double matriz[3][3];       // Array bidimensional
char buffer[100];          // Array de caracteres
```

Os índices de array começam em 0.

## Tipos Definidos pelo Usuário

Você pode criar tipos personalizados usando estruturas e uniões:

```c
struct Ponto {
    int x,
    int y
};

union Dados {
    int i,
    double d
};
```

Veja [Estruturas e Uniões](estruturas-unioes.md) para mais informações.

## Tópicos Relacionados

- [Variáveis](variaveis.md) - Como declarar e usar variáveis
- [Conversão de Tipos](conversao-tipos.md) - Funções de conversão de tipo explícita
- [Arrays](arrays.md) - Trabalhando com arrays

