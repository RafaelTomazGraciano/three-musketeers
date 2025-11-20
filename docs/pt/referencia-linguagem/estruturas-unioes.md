# Estruturas e Uniões

Estruturas e uniões permitem criar tipos compostos personalizados que agrupam dados relacionados.

## Estruturas

Uma estrutura (struct) agrupa múltiplas variáveis de tipos potencialmente diferentes sob um único nome.

### Declaração de Struct

```c
struct Ponto {
    int x,
    int y
};
```

### Declaração de Variável Struct

```c
struct Ponto p;
p.x = 10;
p.y = 20;
```

### Inicialização de Struct

```c
struct Ponto p;
p.x = 10;
p.y = 20;
```

### Acessando Membros de Struct

Use o operador `.` para acessar membros de struct:

```c
struct Ponto p;
p.x = 10;
p.y = 20;
printf("Ponto: (%d, %d)\n", p.x, p.y);
```

### Struct com Arrays

```c
struct Dados {
    int valores[5],
    int contagem
};

struct Dados d;
d.valores[0] = 10;
d.contagem = 1;
```

### Estruturas Aninhadas

Estruturas podem conter outras estruturas:

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

### Ponteiros para Estruturas

Use o operador `->` com ponteiros para estruturas:

```c
struct Ponto {
    int x,
    int y
};

struct Ponto p;
struct Ponto *ptr = &p;
ptr->x = 10;
ptr->y = 20;
```

### Estruturas como Parâmetros de Função

```c
#include <stdio.tm>

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

Funções podem retornar estruturas:

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

## Uniões

Uma união permite armazenar diferentes tipos de dados no mesmo local de memória. Apenas um membro pode ser usado por vez.

### Declaração de Union

```c
union Dados {
    int i,
    double d,
    char c
};
```

### Usando Uniões

```c
union Dados dados;
dados.i = 10;      // Armazenar como int
printf("%d\n", dados.i);

dados.d = 3.14;    // Agora armazenar como double (sobrescreve int)
printf("%f\n", dados.d);
```

**Importante**: Acessar um membro de união que não foi o último escrito resulta em comportamento indefinido.

### Casos de Uso de Union

Uniões são úteis quando você precisa armazenar diferentes tipos no mesmo local de memória:

```c
union Valor {
    int valorInt,
    double valorDouble,
    bool valorBool
};

void imprimirValor(union Valor v, string tipo) {
    if (tipo == "int") {
        printf("%d\n", v.valorInt);
    } else if (tipo == "double") {
        printf("%f\n", v.valorDouble);
    }
}
```

## Diferenças: Struct vs Union

| Característica | Struct | Union |
|----------------|--------|-------|
| Memória | Cada membro tem sua própria memória | Todos os membros compartilham a mesma memória |
| Tamanho | Soma de todos os tamanhos dos membros | Tamanho do maior membro |
| Uso | Todos os membros podem ser usados simultaneamente | Apenas um membro por vez |

## Exemplo: Programa Completo

```c
#include <stdio.tm>

struct Ponto {
    int x,
    int y
};

union Numero {
    int i,
    double d
};

int main() {
    struct Ponto p;
    p.x = 10;
    p.y = 20;
    
    union Numero num;
    num.i = 42;
    printf("Inteiro: %d\n", num.i);
    
    num.d = 3.14;
    printf("Double: %f\n", num.d);
    
    return 0;
}
```

## Tópicos Relacionados

- [Tipos de Dados](tipos-dados.md) - Tipos básicos
- [Ponteiros](ponteiros.md) - Ponteiros para estruturas
- [Funções](funcoes.md) - Funções com estruturas

