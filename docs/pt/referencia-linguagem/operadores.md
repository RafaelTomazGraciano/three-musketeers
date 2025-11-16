# Operadores

Three Musketeers suporta um conjunto abrangente de operadores para aritmética, comparação, operações lógicas e mais.

## Operadores Aritméticos

| Operador | Descrição | Exemplo |
|----------|-----------|---------|
| `+`      | Adição    | `a + b` |
| `-`      | Subtração | `a - b` |
| `*`      | Multiplicação | `a * b` |
| `/`      | Divisão    | `a / b` |
| `%`      | Módulo     | `a % b` |

```c
int a = 10, b = 3;
int soma = a + b;        // 13
int diff = a - b;        // 7
int prod = a * b;        // 30
int quot = a / b;        // 3
int mod = a % b;         // 1
```

## Operadores de Comparação

| Operador | Descrição | Exemplo |
|----------|-----------|---------|
| `==`     | Igual a    | `a == b` |
| `!=`     | Diferente de | `a != b` |
| `>`      | Maior que | `a > b` |
| `<`      | Menor que   | `a < b` |
| `>=`     | Maior ou igual | `a >= b` |
| `<=`     | Menor ou igual | `a <= b` |

```c
int x = 5, y = 10;
bool eq = x == y;        // false
bool ne = x != y;         // true
bool gt = x > y;          // false
bool lt = x < y;          // true
```

## Operadores Lógicos

| Operador | Descrição | Exemplo |
|----------|-----------|---------|
| `&&`     | E lógico | `a && b` |
| `||`     | OU lógico  | `a || b` |
| `!`      | NÃO lógico | `!a` |

```c
bool a = true, b = false;
bool e = a && b;       // false
bool ou = a || b;         // true
bool nao = !a;            // false
```

## Operadores de Incremento e Decremento

| Operador | Descrição | Exemplo |
|----------|-----------|---------|
| `++`     | Incremento  | `++x` ou `x++` |
| `--`     | Decremento  | `--x` ou `x--` |

### Prefixo vs Pós-fixo

- **Prefixo** (`++x`): Incrementa antes de usar o valor
- **Pós-fixo** (`x++`): Usa o valor, depois incrementa

```c
int x = 5;
int y = ++x;  // x = 6, y = 6
int z = x++;  // x = 7, z = 6
```

## Operadores de Atribuição

| Operador | Descrição | Exemplo | Equivalente |
|----------|-----------|---------|-------------|
| `=`     | Atribuição  | `x = 5` | - |
| `+=`    | Adicionar e atribuir | `x += 5` | `x = x + 5` |
| `-=`    | Subtrair e atribuir | `x -= 5` | `x = x - 5` |
| `*=`    | Multiplicar e atribuir | `x *= 5` | `x = x * 5` |
| `/=`    | Dividir e atribuir | `x /= 5` | `x = x / 5` |

```c
int x = 10;
x += 5;   // x = 15
x -= 3;   // x = 12
x *= 2;   // x = 24
x /= 4;   // x = 6
```

## Operadores Unários

| Operador | Descrição | Exemplo |
|----------|-----------|---------|
| `-`      | Menos unário | `-x` |
| `!`      | NÃO lógico | `!x` |
| `*`      | Desreferenciação | `*ptr` |
| `&`      | Endereço de  | `&x` |

```c
int x = 10;
int *ptr = &x;   // Obter endereço de x
int y = *ptr;    // Desreferenciar ptr para obter valor
```

## Precedência de Operadores

Os operadores são avaliados na seguinte ordem (maior para menor):

1. Parênteses `()`
2. Operadores unários (`++`, `--`, `!`, `-`, `*`, `&`)
3. Multiplicativos (`*`, `/`, `%`)
4. Aditivos (`+`, `-`)
5. Relacionais (`<`, `>`, `<=`, `>=`)
6. Igualdade (`==`, `!=`)
7. E lógico (`&&`)
8. OU lógico (`||`)
9. Atribuição (`=`, `+=`, `-=`, etc.)

Use parênteses para esclarecer ou sobrescrever precedência:

```c
int resultado = (a + b) * (c - d);
```

## Tópicos Relacionados

- [Variáveis](variaveis.md) - Trabalhando com variáveis
- [Fluxo de Controle](fluxo-controle.md) - Usando operadores em condições
- [Ponteiros](ponteiros.md) - Operadores de ponteiro

