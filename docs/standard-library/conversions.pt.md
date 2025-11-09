[English](conversions.en.md) | [Português](#português)

<a name="português"></a>
# Funções de Conversão de Tipo

Three Musketeers fornece funções integradas para converter entre strings e outros tipos. Essas funções são essenciais ao trabalhar com entrada/saída de strings e conversões de tipo.

## Conversões de String para Tipo

### atoi

Converte uma string para um inteiro.

**Sintaxe:**
```c
int atoi(literal_string);
int atoi(variavel_string);
```

**Exemplos:**

```c
string numStr = "123";
int num = atoi(numStr);        // num = 123

int valor = atoi("456");       // valor = 456

string entrada;
gets(entrada);
int numero = atoi(entrada);    // Converter entrada do usuário para int
```

**Uso com Entrada:**

```c
int main() {
    string entrada;
    puts("Digite um número:");
    gets(entrada);
    
    int num = atoi(entrada);
    printf("Você digitou: %d\n", num);
    return 0;
}
```

### atod

Converte uma string para um double (número de ponto flutuante).

**Sintaxe:**
```c
double atod(literal_string);
double atod(variavel_string);
```

**Exemplos:**

```c
string piStr = "3.14159";
double pi = atod(piStr);        // pi = 3.14159

double valor = atod("2.718");  // valor = 2.718

string entrada;
gets(entrada);
double numero = atod(entrada);  // Converter entrada do usuário para double
```

**Uso com Entrada:**

```c
int main() {
    string entrada;
    puts("Digite um número decimal:");
    gets(entrada);
    
    double num = atod(entrada);
    printf("Você digitou: %.2f\n", num);
    return 0;
}
```

## Conversões de Tipo para String

### itoa

Converte um inteiro para uma string.

**Sintaxe:**
```c
string itoa(valor_int);
string itoa(variavel_int);
```

**Exemplos:**

```c
int num = 123;
string str = itoa(num);         // str = "123"

string resultado = itoa(456);   // resultado = "456"

int idade = 25;
string idadeStr = itoa(idade);
printf("Idade como string: %s\n", idadeStr);
```

**Uso na Saída:**

```c
int main() {
    int contador = 42;
    string contadorStr = itoa(contador);
    puts("Contador: ");
    puts(contadorStr);
    return 0;
}
```

### dtoa

Converte um double (número de ponto flutuante) para uma string.

**Sintaxe:**
```c
string dtoa(valor_double);
string dtoa(variavel_double);
```

**Exemplos:**

```c
double pi = 3.14159;
string piStr = dtoa(pi);        // piStr = "3.14159"

string resultado = dtoa(2.718);  // resultado = "2.718"

double preco = 19.99;
string precoStr = dtoa(preco);
printf("Preço: %s\n", precoStr);
```

**Uso na Saída:**

```c
int main() {
    double total = 123.45;
    string totalStr = dtoa(total);
    puts("Total: ");
    puts(totalStr);
    return 0;
}
```

## Exemplos Completos

### Exemplo 1: Entrada de String para Cálculo

```c
int main() {
    string entrada1, entrada2;
    int num1, num2;
    
    puts("Digite o primeiro número:");
    gets(entrada1);
    num1 = atoi(entrada1);
    
    puts("Digite o segundo número:");
    gets(entrada2);
    num2 = atoi(entrada2);
    
    int soma = num1 + num2;
    string somaStr = itoa(soma);
    
    printf("Soma: %s\n", somaStr);
    return 0;
}
```

### Exemplo 2: Formatação de Números

```c
int main() {
    int contador = 100;
    double media = 87.5;
    
    string contadorStr = itoa(contador);
    string mediaStr = dtoa(media);
    
    puts("Contador: ");
    puts(contadorStr);
    puts("Média: ");
    puts(mediaStr);
    return 0;
}
```

### Exemplo 3: Calculadora com Conversão de String

```c
int main() {
    string num1Str, num2Str;
    double num1, num2;
    
    puts("Digite o primeiro número:");
    gets(num1Str);
    num1 = atod(num1Str);
    
    puts("Digite o segundo número:");
    gets(num2Str);
    num2 = atod(num2Str);
    
    double produto = num1 * num2;
    string produtoStr = dtoa(produto);
    
    printf("Produto: %s\n", produtoStr);
    return 0;
}
```

## Resumo de Conversões

| Função | Tipo Origem | Tipo Destino | Descrição                        |
|--------|-------------|--------------|----------------------------------|
| `atoi` | string      | int          | Converter string para inteiro    |
| `atod` | string      | double       | Converter string para double     |
| `itoa` | int         | string       | Converter inteiro para string    |
| `dtoa` | double      | string       | Converter double para string     |

## Tópicos Relacionados

- [Funções de Entrada/Saída](io.pt.md)
- [Tipos e Variáveis](../language-reference/types.pt.md)
- [Exemplos](../examples/examples.pt.md)

