# Exemplos de Código

Uma coleção de exemplos práticos demonstrando recursos da linguagem Three Musketeers.

## Olá Mundo

O programa mais simples:

```c
int main() {
    puts("Hello, World!");
    return 0;
}
```

## Variáveis e Tipos

```c
int main() {
    int contagem = 42;
    double pi = 3.14159;
    char letra = 'A';
    bool ativo = true;
    string saudacao = "Hello";
    
    printf("Contagem: %d\n", contagem);
    printf("Pi: %.5f\n", pi);
    printf("Letra: %c\n", letra);
    printf("Ativo: %d\n", ativo);
    printf("Saudação: %s\n", saudacao);
    
    return 0;
}
```

## Arrays

```c
int main() {
    int numeros[5];
    int i;
    
    // Inicializar array
    for (i = 0; i < 5; i++) {
        numeros[i] = i * 10;
    }
    
    // Imprimir array
    for (i = 0; i < 5; i++) {
        printf("numeros[%d] = %d\n", i, numeros[i]);
    }
    
    return 0;
}
```

## Funções

```c
int adicionar(int x, int y) {
    return x + y;
}

double calcularArea(double largura, double altura) {
    return largura * altura;
}

int main() {
    int soma = adicionar(10, 20);
    double area = calcularArea(5.5, 3.2);
    
    printf("Soma: %d\n", soma);
    printf("Área: %.2f\n", area);
    
    return 0;
}
```

## Fluxo de Controle

### If-Else

```c
void verificarValor(int valor) {
    if (valor > 0) {
        printf("Valor %d é positivo\n", valor);
    } else if (valor < 0) {
        printf("Valor %d é negativo\n", valor);
    } else {
        printf("Valor %d é zero\n", valor);
    }
}

int main() {
    verificarValor(5);
    verificarValor(-3);
    verificarValor(0);
    return 0;
}
```

### Switch

```c
void processarOpcao(int opcao) {
    switch (opcao) {
        case 1:
            printf("Opção 1 selecionada\n");
            break;
        case 2:
            printf("Opção 2 selecionada\n");
            break;
        case 3:
            printf("Opção 3 selecionada\n");
            break;
        default:
            printf("Opção desconhecida: %d\n", opcao);
            break;
    }
}

int main() {
    processarOpcao(1);
    processarOpcao(2);
    processarOpcao(4);
    return 0;
}
```

### Loops

```c
int main() {
    int i;
    
    // Loop for
    printf("Loop for:\n");
    for (i = 1; i <= 5; i++) {
        printf("Iteração %d\n", i);
    }
    
    // Loop while
    printf("\nLoop while:\n");
    int contagem = 1;
    while (contagem <= 5) {
        printf("Contagem: %d\n", contagem);
        contagem = contagem + 1;
    }
    
    // Loop do-while
    printf("\nLoop do-while:\n");
    int num = 1;
    do {
        printf("Número: %d\n", num);
        num = num + 1;
    } while (num <= 5);
    
    return 0;
}
```

## Ponteiros

```c
void trocar(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10, y = 20;
    
    printf("Antes da troca: x=%d, y=%d\n", x, y);
    trocar(&x, &y);
    printf("Depois da troca: x=%d, y=%d\n", x, y);
    
    return 0;
}
```

## Memória Dinâmica

```c
int main() {
    int *arr = malloc(5);
    int i;
    
    // Inicializar array
    for (i = 0; i < 5; i++) {
        arr[i] = i * 2;
    }
    
    // Imprimir array
    for (i = 0; i < 5; i++) {
        printf("arr[%d] = %d\n", i, arr[i]);
    }
    
    free(arr);
    return 0;
}
```

## Estruturas

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
    p.x = 10;
    p.y = 20;
    imprimirPonto(p);
    
    return 0;
}
```

## Entrada/Saída

```c
int main() {
    string nome;
    int idade;
    double altura;
    
    printf("Digite seu nome: ");
    gets(nome);
    
    printf("Digite sua idade: ");
    scanf(idade);
    
    printf("Digite sua altura (metros): ");
    scanf(altura);
    
    printf("\n--- Suas Informações ---\n");
    printf("Nome: %s\n", nome);
    printf("Idade: %d\n", idade);
    printf("Altura: %.2f metros\n", altura);
    
    return 0;
}
```

## Conversão de Tipos

```c
int main() {
    // String para número
    string numStr = "42";
    int num = atoi(numStr);
    double valor = atod("3.14159");
    
    printf("Inteiro: %d\n", num);
    printf("Double: %.5f\n", valor);
    
    // Número para string
    int idade = 25;
    double preco = 19.99;
    string idadeStr = itoa(idade);
    string precoStr = dtoa(preco);
    
    printf("Idade como string: %s\n", idadeStr);
    printf("Preço como string: %s\n", precoStr);
    
    return 0;
}
```

## Recursão

```c
int fatorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * fatorial(n - 1);
}

int main() {
    int resultado = fatorial(5);
    printf("5! = %d\n", resultado);
    return 0;
}
```

## Exemplo Completo

Um exemplo mais abrangente combinando múltiplos recursos:

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
    struct Estudante estudantes[3];
    int i;
    
    for (i = 0; i < 3; i++) {
        printf("Digite o nome do estudante %d: ", i + 1);
        gets(estudantes[i].nome);
        
        printf("Digite a idade do estudante %d: ", i + 1);
        scanf(estudantes[i].idade);
        
        printf("Digite a nota do estudante %d: ", i + 1);
        scanf(estudantes[i].nota);
    }
    
    printf("\n--- Lista de Estudantes ---\n");
    for (i = 0; i < 3; i++) {
        imprimirEstudante(estudantes[i]);
        printf("\n");
    }
    
    return 0;
}
```

## Tópicos Relacionados

- [Referência da Linguagem](../referencia-linguagem/README.md) - Documentação detalhada da linguagem
- [Começando](../comecando/ola-mundo.md) - Tutoriais para iniciantes

