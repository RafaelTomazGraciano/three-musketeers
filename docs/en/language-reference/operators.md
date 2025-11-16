# Operators

Three Musketeers supports a comprehensive set of operators for arithmetic, comparison, logical operations, and more.

## Arithmetic Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `+`      | Addition    | `a + b` |
| `-`      | Subtraction | `a - b` |
| `*`      | Multiplication | `a * b` |
| `/`      | Division    | `a / b` |
| `%`      | Modulus     | `a % b` |

```c
int a = 10, b = 3;
int sum = a + b;        // 13
int diff = a - b;        // 7
int prod = a * b;        // 30
int quot = a / b;        // 3
int mod = a % b;         // 1
```

## Comparison Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `==`     | Equal to    | `a == b` |
| `!=`     | Not equal to | `a != b` |
| `>`      | Greater than | `a > b` |
| `<`      | Less than   | `a < b` |
| `>=`     | Greater than or equal | `a >= b` |
| `<=`     | Less than or equal | `a <= b` |

```c
int x = 5, y = 10;
bool eq = x == y;        // false
bool ne = x != y;         // true
bool gt = x > y;          // false
bool lt = x < y;          // true
```

## Logical Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `&&`     | Logical AND | `a && b` |
| `||`     | Logical OR  | `a || b` |
| `!`      | Logical NOT | `!a` |

```c
bool a = true, b = false;
bool and = a && b;       // false
bool or = a || b;         // true
bool not = !a;            // false
```

## Increment and Decrement Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `++`     | Increment  | `++x` or `x++` |
| `--`     | Decrement  | `--x` or `x--` |

### Prefix vs Postfix

- **Prefix** (`++x`): Increments before using the value
- **Postfix** (`x++`): Uses the value, then increments

```c
int x = 5;
int y = ++x;  // x = 6, y = 6
int z = x++;  // x = 7, z = 6
```

## Assignment Operators

| Operator | Description | Example | Equivalent |
|----------|-------------|---------|------------|
| `=`     | Assignment  | `x = 5` | - |
| `+=`    | Add and assign | `x += 5` | `x = x + 5` |
| `-=`    | Subtract and assign | `x -= 5` | `x = x - 5` |
| `*=`    | Multiply and assign | `x *= 5` | `x = x * 5` |
| `/=`    | Divide and assign | `x /= 5` | `x = x / 5` |

```c
int x = 10;
x += 5;   // x = 15
x -= 3;   // x = 12
x *= 2;   // x = 24
x /= 4;   // x = 6
```

## Unary Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `-`      | Unary minus | `-x` |
| `!`      | Logical NOT | `!x` |
| `*`      | Dereference | `*ptr` |
| `&`      | Address of  | `&x` |

```c
int x = 10;
int *ptr = &x;   // Get address of x
int y = *ptr;    // Dereference ptr to get value
```

## Operator Precedence

Operators are evaluated in the following order (highest to lowest):

1. Parentheses `()`
2. Unary operators (`++`, `--`, `!`, `-`, `*`, `&`)
3. Multiplicative (`*`, `/`, `%`)
4. Additive (`+`, `-`)
5. Relational (`<`, `>`, `<=`, `>=`)
6. Equality (`==`, `!=`)
7. Logical AND (`&&`)
8. Logical OR (`||`)
9. Assignment (`=`, `+=`, `-=`, etc.)

Use parentheses to clarify or override precedence:

```c
int result = (a + b) * (c - d);
```

## Related Topics

- [Variables](variables.md) - Working with variables
- [Control Flow](control-flow.md) - Using operators in conditions
- [Pointers](pointers.md) - Pointer operators

