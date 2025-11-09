[English](examples.en.md) | [Português](#português)

<a name="português"></a>
# Exemplos

Esta seção fornece vários exemplos de código demonstrando diferentes recursos da linguagem Three Musketeers.

## Hello World

O programa mais simples:

```c
int main() {
    puts("Olá, Mundo!");
    return 0;
}
```

## Cálculos Básicos

### Aritmética Simples

```c
int main() {
    int a = 10;
    int b = 5;
    
    int soma = a + b;
    int diff = a - b;
    int prod = a * b;
    int quoc = a / b;
    
    printf("Soma: %d\n", soma);      // 15
    printf("Diferença: %d\n", diff);  // 5
    printf("Produto: %d\n", prod);    // 50
    printf("Quociente: %d\n", quoc);  // 2
    
    return 0;
}
```

### Operações de Ponto Flutuante

```c
int main() {
    double pi = 3.14159;
    double raio = 5.0;
    
    double area = pi * raio * raio;
    double circunferencia = 2 * pi * raio;
    
    printf("Área: %.2f\n", area);           // 78.54
    printf("Circunferência: %.2f\n", circunferencia);  // 31.42
    
    return 0;
}
```

## Exemplos de Fluxo de Controle

### Declaração If-Else

```c
int main() {
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
    
    return 0;
}
```

### Declaração Switch

```c
int main() {
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
    
    return 0;
}
```

### Loop For

```c
int main() {
    int i;
    int soma = 0;
    
    for (i = 1; i <= 10; i++) {
        soma = soma + i;
    }
    
    printf("Soma de 1 a 10: %d\n", soma);  // 55
    return 0;
}
```

### Loop While

```c
int main() {
    int contador = 0;
    
    while (contador < 5) {
        printf("Contador: %d\n", contador);
        contador++;
    }
    
    return 0;
}
```

### Loop Do-While

```c
int main() {
    int x = 0;
    
    do {
        printf("x = %d\n", x);
        x++;
    } while (x < 5);
    
    return 0;
}
```

## Exemplos de Funções

### Função Simples

```c
int somar(int x, int y) {
    return x + y;
}

int main() {
    int resultado = somar(5, 3);
    printf("5 + 3 = %d\n", resultado);  // 8
    return 0;
}
```

### Função com Múltiplos Parâmetros

```c
double calcularArea(double largura, double altura) {
    return largura * altura;
}

int main() {
    double area = calcularArea(5.5, 3.2);
    printf("Área: %.2f\n", area);  // 17.60
    return 0;
}
```

### Função Recursiva

```c
int fatorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * fatorial(n - 1);
}

int main() {
    int resultado = fatorial(5);
    printf("5! = %d\n", resultado);  // 120
    return 0;
}
```

## Exemplos de Arrays

### Array Unidimensional

```c
int main() {
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
    
    return 0;
}
```

### Array Bidimensional (Matriz)

```c
int main() {
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
    
    return 0;
}
```

## Exemplos de Ponteiros

### Ponteiros Básicos

```c
int main() {
    int x = 10;
    int *ptr = &x;
    
    printf("x = %d\n", x);           // 10
    printf("*ptr = %d\n", *ptr);      // 10
    
    *ptr = 20;
    printf("x = %d\n", x);           // 20
    
    return 0;
}
```

### Alocação Dinâmica de Memória

```c
int main() {
    int *arr = malloc(5);
    int i;
    
    // Inicializar array
    for (i = 0; i < 5; i++) {
        arr[i] = i * 10;
    }
    
    // Imprimir array
    for (i = 0; i < 5; i++) {
        printf("arr[%d] = %d\n", i, arr[i]);
    }
    
    free(arr);  // Não esqueça de liberar!
    return 0;
}
```

## Exemplos de Estruturas

### Estrutura Básica

```c
struct Ponto {
    int x,
    int y
};

int main() {
    struct Ponto p;
    p.x = 10;
    p.y = 20;
    
    printf("Ponto: (%d, %d)\n", p.x, p.y);
    return 0;
}
```

### Estrutura com Funções

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

int main() {
    struct Ponto p = criarPonto(5, 10);
    printf("Ponto: (%d, %d)\n", p.x, p.y);
    return 0;
}
```

### Estrutura com Ponteiro

```c
struct Ponto {
    int x,
    int y
};

int main() {
    struct Ponto p;
    struct Ponto *ptr = &p;
    
    ptr->x = 10;
    ptr->y = 20;
    
    printf("Ponto: (%d, %d)\n", ptr->x, ptr->y);
    return 0;
}
```

## Exemplos de Entrada/Saída

### Lendo Entrada

```c
int main() {
    string nome;
    int idade;
    
    puts("Digite seu nome:");
    gets(nome);
    
    puts("Digite sua idade:");
    scanf(idade);
    
    printf("Olá, %s! Você tem %d anos.\n", nome, idade);
    return 0;
}
```

### Conversão de Tipo com Entrada

```c
int main() {
    string entrada;
    int numero;
    
    puts("Digite um número:");
    gets(entrada);
    
    numero = atoi(entrada);
    printf("Você digitou: %d\n", numero);
    
    return 0;
}
```

## Exemplo de Programa Completo

Aqui está um programa completo demonstrando múltiplos recursos:

```c
#include <stdio.tm>

struct Estudante {
    string nome,
    int idade,
    double nota
};

void imprimirEstudante(struct Estudante s) {
    printf("Nome: %s\n", s.nome);
    printf("Idade: %d\n", s.idade);
    printf("Nota: %.2f\n", s.nota);
}

int main() {
    struct Estudante estudante;
    
    puts("Digite o nome do estudante:");
    gets(estudante.nome);
    
    puts("Digite a idade do estudante:");
    scanf(estudante.idade);
    
    puts("Digite a nota do estudante:");
    scanf(estudante.nota);
    
    imprimirEstudante(estudante);
    
    return 0;
}
```

## Tópicos Relacionados

- [Começando](../getting-started.pt.md)
- [Referência da Linguagem](../language-reference/)
- [Biblioteca Padrão](../standard-library/)

