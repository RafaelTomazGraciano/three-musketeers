[English](operators.en.md) | [Português](#português)

<a name="português"></a>
# Operadores

Three Musketeers fornece um conjunto abrangente de operadores para operações aritméticas, comparação, lógicas e mais.

## Operadores Aritméticos

### Operadores Binários

- `+` - Adição
- `-` - Subtração
- `*` - Multiplicação
- `/` - Divisão
- `%` - Módulo (resto)

```c
int a = 10;
int b = 3;

int soma = a + b;        // 13
int diff = a - b;        // 7
int prod = a * b;        // 30
int quoc = a / b;        // 3
int resto = a % b;       // 1

double x = 10.5;
double y = 2.0;
double resultado = x / y;  // 5.25
```

### Operadores Unários

- `-` - Menos unário (negação)

```c
int a = 10;
int b = -a;             // b = -10
double x = -3.14;       // x = -3.14
```

## Operadores de Comparação

- `==` - Igual a
- `!=` - Diferente de
- `>` - Maior que
- `<` - Menor que
- `>=` - Maior ou igual a
- `<=` - Menor ou igual a

Todos os operadores de comparação retornam um valor `bool`.

```c
int a = 10;
int b = 5;

bool eq = (a == b);     // false
bool ne = (a != b);     // true
bool gt = (a > b);      // true
bool lt = (a < b);      // false
bool ge = (a >= b);     // true
bool le = (a <= b);     // false
```

## Operadores Lógicos

- `&&` - E lógico
- `||` - OU lógico
- `!` - NÃO lógico

```c
bool a = true;
bool b = false;

bool resultadoE = a && b;    // false
bool resultadoOU = a || b;    // true
bool naoA = !a;               // false
bool naoB = !b;               // true

// Expressões combinadas
bool complexo = (a && b) || (!a && !b);  // false
```

## Operadores de Incremento e Decremento

- `++` - Incremento
- `--` - Decremento

Estes podem ser usados como operadores de prefixo ou sufixo.

### Operadores de Prefixo

```c
int x = 5;
int y = ++x;            // x = 6, y = 6 (incrementa depois usa)
int z = --x;            // x = 5, z = 5 (decrementa depois usa)
```

### Operadores de Sufixo

```c
int x = 5;
int y = x++;            // x = 6, y = 5 (usa depois incrementa)
int z = x--;            // x = 5, z = 6 (usa depois decrementa)
```

### Incremento/Decremento de Array

```c
int arr[5] = {1, 2, 3, 4, 5};
++arr[0];               // arr[0] = 2
arr[1]++;               // arr[1] = 3
--arr[2];               // arr[2] = 2
arr[3]--;               // arr[3] = 3
```

## Operadores de Atribuição

- `=` - Atribuição
- `+=` - Adiciona e atribui
- `-=` - Subtrai e atribui
- `*=` - Multiplica e atribui
- `/=` - Divide e atribui

```c
int x = 10;

x += 5;                 // x = 15 (equivalente a x = x + 5)
x -= 3;                 // x = 12 (equivalente a x = x - 3)
x *= 2;                 // x = 24 (equivalente a x = x * 2)
x /= 4;                 // x = 6 (equivalente a x = x / 4)
```

### Operadores de Atribuição de Array

```c
int arr[5];
arr[0] = 10;
arr[1] += 5;            // arr[1] = arr[1] + 5
arr[2] -= 3;            // arr[2] = arr[2] - 3
arr[3] *= 2;            // arr[3] = arr[3] * 2
arr[4] /= 4;            // arr[4] = arr[4] / 4
```

## Operadores de Endereço e Desreferência

- `&` - Endereço de (retorna ponteiro)
- `*` - Desreferência (acessa valor através do ponteiro)

```c
int x = 10;
int *ptr = &x;          // ptr aponta para x
int valor = *ptr;       // valor = 10 (desreferência)
*ptr = 20;              // x = 20 (modifica através do ponteiro)
```

## Precedência de Operadores

Os operadores são avaliados na seguinte ordem (maior para menor):

1. Parênteses `()`
2. Operadores unários (`!`, `-`, `++`, `--`, `*` desreferência, `&` endereço)
3. Multiplicativos (`*`, `/`, `%`)
4. Aditivos (`+`, `-`)
5. Comparação (`<`, `>`, `<=`, `>=`)
6. Igualdade (`==`, `!=`)
7. E lógico (`&&`)
8. OU lógico (`||`)
9. Atribuição (`=`, `+=`, `-=`, `*=`, `/=`)

### Exemplos

```c
int resultado = 2 + 3 * 4;        // 14 (multiplicação primeiro)
bool verifica = 5 > 3 && 2 < 4;  // true (comparações, depois E)
int x = (2 + 3) * 4;              // 20 (parênteses sobrescrevem)
```

## Tópicos Relacionados

- [Tipos e Variáveis](types.pt.md)
- [Fluxo de Controle](control-flow.pt.md)
- [Expressões](../examples/examples.pt.md#expressões)

