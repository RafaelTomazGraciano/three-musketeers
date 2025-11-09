[English](functions.en.md) | [Português](#português)

<a name="português"></a>
# Funções

Funções permitem organizar código em blocos reutilizáveis. Three Musketeers suporta declaração de funções, parâmetros, valores de retorno e recursão.

## Declaração de Função

A sintaxe geral para uma função é:

```c
tipo_retorno nome_funcao(parametros) {
    // corpo da função
    return valor;  // se tipo_retorno não for void
}
```

## Tipos de Retorno de Função

Funções podem retornar um valor de qualquer tipo, ou `void` se não retornarem nada.

### Funções com Valores de Retorno

```c
int somar(int x, int y) {
    return x + y;
}

double calcularArea(double largura, double altura) {
    return largura * altura;
}

bool ehPositivo(int num) {
    return num > 0;
}

char obterPrimeiraLetra(int codigo) {
    char letra = 'A';
    return letra;
}
```

### Funções void

Funções que não retornam um valor usam `void` como tipo de retorno.

```c
void imprimirBemVindo() {
    puts("=== Three Musketeers ===");
}

void imprimirMensagem(string msg) {
    puts(msg);
}
```

## Parâmetros de Função

Funções podem aceitar zero ou mais parâmetros. Os parâmetros são passados por valor.

### Sem Parâmetros

```c
void cumprimentar() {
    puts("Olá!");
}
```

### Parâmetro Único

```c
void imprimirNumero(int n) {
    printf("Número: %d\n", n);
}
```

### Múltiplos Parâmetros

```c
int multiplicar(int a, int b, int c) {
    return a * b * c;
}

double calcularDistancia(double x1, double y1, double x2, double y2) {
    double dx = x2 - x1;
    double dy = y2 - y1;
    return dx * dx + dy * dy;
}
```

### Parâmetros de Ponteiro

Funções podem aceitar ponteiros como parâmetros, permitindo que modifiquem os valores originais.

```c
void incrementar(int *ptr) {
    (*ptr)++;
}

int main() {
    int x = 10;
    incrementar(&x);
    printf("x = %d\n", x);  // x = 11
    return 0;
}
```

## Chamadas de Função

Chame uma função usando seu nome seguido de parênteses contendo os argumentos.

```c
int resultado = somar(5, 3);           // resultado = 8
double area = calcularArea(5.0, 3.0);   // area = 15.0
bool verifica = ehPositivo(10);        // verifica = true
imprimirBemVindo();                    // Chama função void
```

### Chamadas de Função Aninhadas

Funções podem ser chamadas dentro de outras chamadas de função.

```c
int quadrado(int n) {
    return multiplicar(n, n, 1);
}

int resultado = quadrado(5);           // resultado = 25
int complexo = somar(5, 3) * multiplicar(2, 2, 2);  // 8 * 8 = 64
```

## Recursão

Funções podem chamar a si mesmas, permitindo algoritmos recursivos.

```c
int fatorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * fatorial(n - 1);
}

int fibonacci(int n) {
    if (n <= 1) {
        return n;
    }
    return fibonacci(n - 1) + fibonacci(n - 2);
}

void imprimirRecursivo(string mensagem, int contador) {
    if (contador <= 0) {
        return;
    }
    printf("%s - contador: %d\n", mensagem, contador);
    imprimirRecursivo(mensagem, contador - 1);
}
```

## Escopo de Função

Variáveis declaradas dentro de uma função são locais a essa função e não podem ser acessadas de fora.

```c
int variavelGlobal = 100;

void minhaFuncao() {
    int variavelLocal = 50;
    printf("Global: %d, Local: %d\n", variavelGlobal, variavelLocal);
    // variavelLocal é acessível apenas dentro de minhaFuncao
}

int main() {
    minhaFuncao();
    // variavelLocal não é acessível aqui
    return 0;
}
```

## Retornando Estruturas

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

## Retornando Ponteiros

Funções podem retornar ponteiros.

```c
int *criarArray(int tamanho) {
    int *arr = malloc(tamanho);
    return arr;
}
```

## Ordem de Declaração de Função

Funções devem ser declaradas antes de serem chamadas, ou você pode defini-las em qualquer ordem se estiver chamando de `main` ou outras funções definidas depois.

```c
// Declaração antecipada não necessária - funções podem ser definidas em qualquer ordem
int main() {
    int resultado = somar(5, 3);
    return 0;
}

int somar(int x, int y) {
    return x + y;
}
```

## Tópicos Relacionados

- [Tipos e Variáveis](types.pt.md)
- [Fluxo de Controle](control-flow.pt.md)
- [Gerenciamento de Memória](memory.pt.md)
- [Estruturas e Uniões](structures.pt.md)

