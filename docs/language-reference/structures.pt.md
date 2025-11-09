[English](structures.en.md) | [Português](#português)

<a name="português"></a>
# Estruturas e Uniões

Three Musketeers suporta tipos compostos definidos pelo usuário através de estruturas (`struct`) e uniões (`union`).

## Estruturas

Estruturas permitem agrupar dados relacionados em um único tipo.

### Declaração de Estrutura

```c
struct Ponto {
    int x,
    int y
};

struct Pessoa {
    string nome,
    int idade,
    double altura
};
```

### Declaração de Variável de Estrutura

```c
struct Ponto p1;
struct Ponto p2;
```

### Acessando Membros de Estrutura

Use o operador ponto (`.`) para acessar membros de estrutura.

```c
struct Ponto p;
p.x = 10;
p.y = 20;

int xCoord = p.x;
int yCoord = p.y;
```

### Inicialização de Estrutura

```c
struct Ponto p;
p.x = 5;
p.y = 10;
```

### Estruturas com Arrays

Estruturas podem conter arrays.

```c
struct minhaStruct {
    int a,
    int b[5]
};

struct minhaStruct s;
s.a = 10;
s.b[0] = 1;
s.b[1] = 2;
s.b[2] = 3;
```

### Estruturas Aninhadas

Estruturas podem conter outras estruturas.

```c
struct Ponto {
    int x,
    int y
};

struct Retangulo {
    struct Ponto cantoSuperiorEsquerdo,
    struct Ponto cantoInferiorDireito
};

struct Retangulo rect;
rect.cantoSuperiorEsquerdo.x = 0;
rect.cantoSuperiorEsquerdo.y = 0;
rect.cantoInferiorDireito.x = 10;
rect.cantoInferiorDireito.y = 10;
```

### Estruturas como Parâmetros de Função

Estruturas podem ser passadas para funções por valor.

```c
struct Ponto {
    int x,
    int y
};

void imprimirPonto(struct Ponto p) {
    printf("Ponto: (%d, %d)\n", p.x, p.y);
}

int main() {
    struct Ponto p;
    p.x = 5;
    p.y = 10;
    imprimirPonto(p);
    return 0;
}
```

### Retornando Estruturas

Funções podem retornar estruturas.

```c
struct Ponto {
    int x,
    int y
};

struct Ponto criarPonto(int x, int y) {
    struct Ponto p;
    p.x = x;
    p.y = y;
    return p;
}
```

## Ponteiros para Estruturas

Você pode usar ponteiros com estruturas e acessar membros usando o operador seta (`->`).

### Ponteiros de Estrutura

```c
struct Ponto {
    int x,
    int y
};

struct Ponto p;
struct Ponto *ptr = &p;

ptr->x = 10;        // Acessar através do ponteiro
ptr->y = 20;       // Equivalente a (*ptr).y

int x = ptr->x;    // Ler através do ponteiro
```

### Operador Seta (->)

O operador seta (`->`) é uma abreviação para desreferenciar e acessar um membro.

```c
struct Ponto *ptr = &p;
ptr->x = 10;       // Mesmo que (*ptr).x = 10
```

### Ponto vs Seta

- Use `.` quando você tem uma variável de estrutura
- Use `->` quando você tem um ponteiro para uma estrutura

```c
struct Ponto p;
struct Ponto *ptr = &p;

p.x = 10;          // Operador ponto
ptr->x = 20;       // Operador seta
```

## Uniões

Uniões são similares a estruturas, mas todos os membros compartilham o mesmo espaço de memória. Apenas um membro pode ser usado por vez.

### Declaração de União

```c
union Dados {
    int i,
    double d,
    char c
};
```

### Uso de União

```c
union Dados dados;

dados.i = 10;      // Armazenar como int
printf("%d\n", dados.i);

dados.d = 3.14;    // Agora armazenar como double (sobrescreve int)
printf("%f\n", dados.d);

dados.c = 'A';     // Agora armazenar como char (sobrescreve double)
printf("%c\n", dados.c);
```

### União com Estruturas

Uniões podem conter estruturas.

```c
struct Ponto {
    int x,
    int y
};

union minhaUniao {
    int g,
    struct Ponto ponto
};

union minhaUniao u;
u.g = 100;
// ou
u.ponto.x = 10;
u.ponto.y = 20;
```

## Diretivas do Pré-processador

### #include

Inclua arquivos de cabeçalho usando a diretiva `#include`.

```c
#include <stdio.tm>
#include <stdlib.tm>
#include "meucabecalho.tm"
```

### #define

Defina constantes usando a diretiva `#define`.

```c
#define MAX_SIZE 100
#define PI 3.14159
#define MESSAGE "Olá, Mundo!"

int arr[MAX_SIZE];
double circunferencia = 2 * PI * raio;
puts(MESSAGE);
```

## Tópicos Relacionados

- [Tipos e Variáveis](types.pt.md)
- [Gerenciamento de Memória](memory.pt.md)
- [Funções](functions.pt.md)

