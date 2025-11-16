# Funções

Funções permitem organizar código em blocos reutilizáveis. Three Musketeers suporta funções com vários tipos de retorno e parâmetros.

## Declaração de Função

### Sintaxe Básica

```c
tipo_retorno nome_funcao(parametros) {
    // Corpo da função
    return valor;  // Se tipo_retorno não for void
}
```

### Funções Void

Funções que não retornam um valor:

```c
void imprimirBemVindo() {
    puts("Bem-vindo ao Three Musketeers!");
}
```

### Funções com Valores de Retorno

```c
int adicionar(int x, int y) {
    return x + y;
}
```

## Parâmetros de Função

Funções podem aceitar zero ou mais parâmetros:

```c
// Sem parâmetros
int obterValor() {
    return 42;
}

// Parâmetro único
int quadrado(int n) {
    return n * n;
}

// Múltiplos parâmetros
int multiplicar(int a, int b, int c) {
    return a * b * c;
}
```

## Tipos de Retorno

Funções podem retornar qualquer um dos tipos básicos:

```c
int obterInt() { return 10; }
double obterDouble() { return 3.14; }
bool ehPositivo(int n) { return n > 0; }
char obterChar() { return 'A'; }
string obterString() { return "Hello"; }
```

## Chamadas de Função

Chame uma função usando seu nome seguido de parênteses:

```c
int resultado = adicionar(5, 3);
imprimirBemVindo();
double area = calcularArea(10.5, 20.0);
```

## Função Main

Todo programa Three Musketeers deve ter uma função `main` como ponto de entrada:

```c
int main() {
    // Código do programa
    return 0;
}
```

### Main com Argumentos

A função main pode aceitar argumentos da linha de comando:

```c
int main(int argc, char argv[]) {
    printf("Número de argumentos: %d\n", argc);
    if (argc > 1) {
        puts(argv[1]);
    }
    return 0;
}
```

- `argc`: Número de argumentos da linha de comando
- `argv`: Array de strings contendo os argumentos

## Exemplos de Funções

### Função Simples

```c
void cumprimentar(string nome) {
    printf("Hello, %s!\n", nome);
}

int main() {
    cumprimentar("Three Musketeers");
    return 0;
}
```

### Função com Múltiplos Tipos de Retorno

```c
int adicionar(int x, int y) {
    return x + y;
}

double calcularArea(double largura, double altura) {
    return largura * altura;
}

bool ehPar(int n) {
    return n % 2 == 0;
}
```

### Chamadas de Função Aninhadas

```c
int quadrado(int n) {
    return n * n;
}

int adicionar(int x, int y) {
    return x + y;
}

int main() {
    int resultado = quadrado(adicionar(3, 4));  // quadrado(7) = 49
    printf("Resultado: %d\n", resultado);
    return 0;
}
```

### Funções Recursivas

Funções podem chamar a si mesmas:

```c
int fatorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * fatorial(n - 1);
}
```

## Escopo de Função

Variáveis declaradas dentro de uma função são locais a essa função:

```c
int funcao1() {
    int x = 10;  // Local a funcao1
    return x;
}

int funcao2() {
    int x = 20;  // x diferente, local a funcao2
    return x;
}
```

## Tópicos Relacionados

- [Ponteiros](ponteiros.md) - Funções com parâmetros de ponteiro
- [Fluxo de Controle](fluxo-controle.md) - Usando fluxo de controle em funções
- [Exemplos](../exemplos/exemplos.md) - Mais exemplos de funções

