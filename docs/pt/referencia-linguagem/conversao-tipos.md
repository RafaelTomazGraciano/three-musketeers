# Conversão de Tipos

Three Musketeers fornece funções integradas para converter entre strings e outros tipos de dados. O compilador também realiza conversão de tipo implícita automaticamente.

## Conversão de Tipo Implícita

O compilador converte automaticamente entre tipos compatíveis:

```c
int x = 10;
double y = x;        // int automaticamente convertido para double
int z = y;           // double convertido para int (truncado)
```

## Conversão de String para Tipo

### atoi

Converte uma string para um inteiro.

**Sintaxe:**
```c
int atoi(variavel_string);
int atoi("literal de string");
```

**Exemplos:**

```c
string numStr = "42";
int num = atoi(numStr);
printf("%d\n", num);  // 42

int valor = atoi("123");
printf("%d\n", valor);  // 123
```

### atod

Converte uma string para um double.

**Sintaxe:**
```c
double atod(variavel_string);
double atod("literal de string");
```

**Exemplos:**

```c
string piStr = "3.14159";
double pi = atod(piStr);
printf("%.5f\n", pi);  // 3.14159

double valor = atod("2.718");
printf("%.3f\n", valor);  // 2.718
```

## Conversão de Tipo para String

### itoa

Converte um inteiro para uma string.

**Sintaxe:**
```c
string itoa(valor_int);
string itoa(variavel_inteira);
```

**Exemplos:**

```c
int idade = 25;
string idadeStr = itoa(idade);
printf("Idade: %s\n", idadeStr);  // Idade: 25

string numStr = itoa(42);
puts(numStr);  // 42
```

### dtoa

Converte um double para uma string.

**Sintaxe:**
```c
string dtoa(valor_double);
string dtoa(variavel_double);
```

**Exemplos:**

```c
double pi = 3.14159;
string piStr = dtoa(pi);
printf("Pi: %s\n", piStr);  // Pi: 3.14159

string valorStr = dtoa(2.718);
puts(valorStr);  // 2.718
```

## Exemplo Completo de Conversão

```c
#include <stdio.tm>

int main() {
    // String para número
    string entrada = "42";
    int num = atoi(entrada);
    double valor = atod("3.14");
    
    printf("Inteiro: %d\n", num);
    printf("Double: %.2f\n", valor);
    
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

## Casos de Uso

### Lendo Números da Entrada

```c
string entrada;
int numero;

printf("Digite um número: ");
gets(entrada);
numero = atoi(entrada);
printf("Você digitou: %d\n", numero);
```

### Formatando Saída

```c
int contagem = 42;
string mensagem = "Contagem: " + itoa(contagem);
puts(mensagem);  // Contagem: 42
```

### Construindo Strings

```c
int x = 10, y = 20;
string resultado = "x=" + itoa(x) + ", y=" + itoa(y);
puts(resultado);  // x=10, y=20
```

## Tabela de Conversão de Tipos

| Função | De Tipo | Para Tipo | Exemplo |
|--------|---------|-----------|---------|
| `atoi`   | string    | int     | `atoi("42")` → `42` |
| `atod`   | string    | double  | `atod("3.14")` → `3.14` |
| `itoa`   | int       | string  | `itoa(42)` → `"42"` |
| `dtoa`   | double    | string  | `dtoa(3.14)` → `"3.14"` |

## Regras de Conversão Implícita

O compilador segue estas regras para conversão implícita:

1. **Tipos numéricos**: `int` pode ser convertido para `double` automaticamente
2. **Truncamento**: `double` para `int` trunca a parte decimal
3. **Hierarquia de tipos**: Conversões seguem a hierarquia de tipos (double → int → char → bool → string)

## Tópicos Relacionados

- [Tipos de Dados](tipos-dados.md) - Tipos disponíveis
- [Variáveis](variaveis.md) - Declarações de variáveis
- [Entrada e Saída](entrada-saida.md) - Usando conversões com E/S

