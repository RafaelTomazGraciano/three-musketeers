# Entrada e Saída

Three Musketeers fornece várias funções para ler entrada e exibir saída, similares às funções de E/S padrão do C.

## Funções de Saída

### printf

Imprime saída formatada no console.

**Sintaxe:**
```c
printf(string_formato, argumentos...);
```

**Especificadores de Formato:**

| Formato | Tipo   | Descrição              |
|--------|--------|------------------------|
| `%d`   | int    | Inteiro                |
| `%f`   | double | Número de ponto flutuante |
| `%s`   | string | String                 |
| `%c`   | char   | Caractere único        |

**Exemplos:**

```c
int idade = 25;
double preco = 19.99;
string nome = "Three Musketeers";
char inicial = 'T';

printf("Idade: %d\n", idade);
printf("Preço: %.2f\n", preco);
printf("Nome: %s\n", nome);
printf("Inicial: %c\n", inicial);
```

**Saída:**
```
Idade: 25
Preço: 19.99
Nome: Three Musketeers
Inicial: T
```

### puts

Imprime uma string no console seguida de uma nova linha.

**Sintaxe:**
```c
puts(variavel_string);
puts("literal de string");
```

**Exemplos:**

```c
string saudacao = "Hello, World!";
puts(saudacao);
puts("Three Musketeers");
```

**Saída:**
```
Hello, World!
Three Musketeers
```

## Funções de Entrada

### scanf

Lê entrada formatada do console.

**Sintaxe:**
```c
scanf(variavel1, variavel2, ...);
```

**Exemplos:**

```c
int idade;
double preco;
string nome;

printf("Digite sua idade: ");
scanf(idade);

printf("Digite o preço: ");
scanf(preco);

printf("Digite o nome: ");
scanf(nome);
```

**Nota:** `scanf` determina automaticamente o tipo com base no tipo da variável.

### gets

Lê uma única string do console.

**Sintaxe:**
```c
gets(variavel_string);
```

**Exemplo:**

```c
string nome;
printf("Digite seu nome: ");
gets(nome);
printf("Hello, %s!\n", nome);
```

## Exemplo Completo de E/S

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

## Formatação com printf

### Precisão para Ponto Flutuante

```c
double pi = 3.14159265359;
printf("%.2f\n", pi);   // 3.14
printf("%.4f\n", pi);   // 3.1416
```

### Largura de Campo

```c
int num = 42;
printf("%5d\n", num);   // "   42" (alinhado à direita, 5 caracteres de largura)
printf("%-5d\n", num);  // "42   " (alinhado à esquerda, 5 caracteres de largura)
```

### Múltiplos Especificadores de Formato

```c
int x = 10, y = 20;
double z = 3.14;
printf("x=%d, y=%d, z=%.2f\n", x, y, z);
```

## Lendo Múltiplos Valores

```c
int a, b, c;
printf("Digite três inteiros: ");
scanf(a, b, c);
printf("Você digitou: %d, %d, %d\n", a, b, c);
```

## Tratamento de Erros

Sempre certifique-se de que as variáveis estão inicializadas antes de ler nelas:

```c
int valor;
scanf(valor);  // Seguro - valor será definido por scanf
```

## Tópicos Relacionados

- [Variáveis](variaveis.md) - Declarações de variáveis
- [Conversão de Tipos](conversao-tipos.md) - Convertendo entre tipos para E/S
- [Exemplos](../exemplos/exemplos.md) - Mais exemplos de E/S

