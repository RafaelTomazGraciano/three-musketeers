[English](io.en.md) | [Português](#português)

<a name="português"></a>
# Funções de Entrada/Saída

Three Musketeers fornece várias funções integradas para ler e escrever no console.

## Funções de Saída

### printf

A função `printf` imprime saída formatada no console. Ela recebe uma string de formato e argumentos opcionais.

**Sintaxe:**
```c
printf(string_formato, arg1, arg2, ...);
```

**Especificadores de Formato:**

| Especificador | Tipo    | Descrição                        |
|---------------|---------|----------------------------------|
| `%d`          | int     | Inteiro decimal com sinal        |
| `%f`          | double  | Número de ponto flutuante        |
| `%s`          | string  | String                           |
| `%c`          | char    | Caractere único                  |

**Exemplos:**

```c
int idade = 25;
double altura = 5.9;
string nome = "João";
char nota = 'A';

printf("Nome: %s\n", nome);              // Nome: João
printf("Idade: %d\n", idade);            // Idade: 25
printf("Altura: %f\n", altura);          // Altura: 5.900000
printf("Nota: %c\n", nota);              // Nota: A

// Múltiplos especificadores de formato
printf("Nome: %s, Idade: %d, Altura: %.2f\n", nome, idade, altura);
```

**Opções de Formatação:**

Você pode especificar precisão para números de ponto flutuante:

```c
double pi = 3.14159;
printf("Pi: %.2f\n", pi);     // Pi: 3.14 (2 casas decimais)
printf("Pi: %.4f\n", pi);    // Pi: 3.1416 (4 casas decimais)
```

### puts

A função `puts` imprime uma string seguida de um caractere de nova linha.

**Sintaxe:**
```c
puts(literal_string);
puts(variavel_string);
```

**Exemplos:**

```c
puts("Olá, Mundo!");        // Imprime: Olá, Mundo!

string mensagem = "Bem-vindo ao Three Musketeers";
puts(mensagem);             // Imprime: Bem-vindo ao Three Musketeers

string saudacao = "Olá";
puts(saudacao);            // Imprime: Olá
```

**Nota:** `puts` adiciona automaticamente uma nova linha no final, diferentemente de `printf` que requer `\n`.

## Funções de Entrada

### scanf

A função `scanf` lê entrada formatada do console. Ela lê múltiplos valores de tipos diferentes.

**Sintaxe:**
```c
scanf(variavel1, variavel2, ...);
```

**Exemplos:**

```c
int idade;
double altura;
char inicial;

scanf(idade, altura, inicial);
// Usuário digita: 25 5.9 A
// idade = 25, altura = 5.9, inicial = 'A'
```

**Lendo Múltiplos Valores:**

```c
int x, y, z;
scanf(x, y, z);
// Usuário digita: 10 20 30
// x = 10, y = 20, z = 30
```

**Nota:** `scanf` lê valores separados por espaços em branco. As variáveis devem ser declaradas antes do uso.

### gets

A função `gets` lê uma única string do console até encontrar uma nova linha.

**Sintaxe:**
```c
gets(variavel_string);
```

**Exemplos:**

```c
string nome;
gets(nome);
// Usuário digita: Three Musketeers
// nome = "Three Musketeers"

string entrada;
gets(entrada);
puts(entrada);  // Imprime o que foi lido
```

**Importante:** A variável deve ser do tipo `string` e deve ser declarada antes de chamar `gets`.

## Exemplos Completos

### Exemplo 1: E/S Básica

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

### Exemplo 2: Entrada de Calculadora

```c
int main() {
    double num1, num2;
    
    puts("Digite dois números:");
    scanf(num1, num2);
    
    double soma = num1 + num2;
    double produto = num1 * num2;
    
    printf("Soma: %.2f\n", soma);
    printf("Produto: %.2f\n", produto);
    return 0;
}
```

### Exemplo 3: Entrada de Caractere

```c
int main() {
    char ch;
    string palavra;
    
    puts("Digite um caractere:");
    scanf(ch);
    
    puts("Digite uma palavra:");
    gets(palavra);
    
    printf("Caractere: %c\n", ch);
    printf("Palavra: %s\n", palavra);
    return 0;
}
```

## Tópicos Relacionados

- [Funções de Conversão de Tipo](conversions.pt.md)
- [Tipos e Variáveis](../language-reference/types.pt.md)
- [Exemplos](../examples/examples.pt.md)

