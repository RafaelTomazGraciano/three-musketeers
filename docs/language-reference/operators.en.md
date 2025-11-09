[English](#english) | [Português](operators.pt.md)

<a name="english"></a>
# Operators

Three Musketeers provides a comprehensive set of operators for arithmetic, comparison, logical operations, and more.

## Arithmetic Operators

### Binary Operators

- `+` - Addition
- `-` - Subtraction
- `*` - Multiplication
- `/` - Division
- `%` - Modulo (remainder)

```c
int a = 10;
int b = 3;

int sum = a + b;        // 13
int diff = a - b;       // 7
int prod = a * b;       // 30
int quot = a / b;       // 3
int rem = a % b;        // 1

double x = 10.5;
double y = 2.0;
double result = x / y;  // 5.25
```

### Unary Operators

- `-` - Unary minus (negation)

```c
int a = 10;
int b = -a;             // b = -10
double x = -3.14;       // x = -3.14
```

## Comparison Operators

- `==` - Equal to
- `!=` - Not equal to
- `>` - Greater than
- `<` - Less than
- `>=` - Greater than or equal to
- `<=` - Less than or equal to

All comparison operators return a `bool` value.

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

## Logical Operators

- `&&` - Logical AND
- `||` - Logical OR
- `!` - Logical NOT

```c
bool a = true;
bool b = false;

bool andResult = a && b;    // false
bool orResult = a || b;      // true
bool notA = !a;              // false
bool notB = !b;              // true

// Combined expressions
bool complex = (a && b) || (!a && !b);  // false
```

## Increment and Decrement Operators

- `++` - Increment
- `--` - Decrement

These can be used as prefix or postfix operators.

### Prefix Operators

```c
int x = 5;
int y = ++x;            // x = 6, y = 6 (increment then use)
int z = --x;            // x = 5, z = 5 (decrement then use)
```

### Postfix Operators

```c
int x = 5;
int y = x++;            // x = 6, y = 5 (use then increment)
int z = x--;            // x = 5, z = 6 (use then decrement)
```

### Array Increment/Decrement

```c
int arr[5] = {1, 2, 3, 4, 5};
++arr[0];               // arr[0] = 2
arr[1]++;               // arr[1] = 3
--arr[2];               // arr[2] = 2
arr[3]--;               // arr[3] = 3
```

## Assignment Operators

- `=` - Assignment
- `+=` - Add and assign
- `-=` - Subtract and assign
- `*=` - Multiply and assign
- `/=` - Divide and assign

```c
int x = 10;

x += 5;                 // x = 15 (equivalent to x = x + 5)
x -= 3;                 // x = 12 (equivalent to x = x - 3)
x *= 2;                 // x = 24 (equivalent to x = x * 2)
x /= 4;                 // x = 6 (equivalent to x = x / 4)
```

### Array Assignment Operators

```c
int arr[5];
arr[0] = 10;
arr[1] += 5;            // arr[1] = arr[1] + 5
arr[2] -= 3;            // arr[2] = arr[2] - 3
arr[3] *= 2;            // arr[3] = arr[3] * 2
arr[4] /= 4;            // arr[4] = arr[4] / 4
```

## Address and Dereference Operators

- `&` - Address of (returns pointer)
- `*` - Dereference (accesses value through pointer)

```c
int x = 10;
int *ptr = &x;          // ptr points to x
int value = *ptr;       // value = 10 (dereference)
*ptr = 20;              // x = 20 (modify through pointer)
```

## Operator Precedence

Operators are evaluated in the following order (highest to lowest):

1. Parentheses `()`
2. Unary operators (`!`, `-`, `++`, `--`, `*` dereference, `&` address)
3. Multiplicative (`*`, `/`, `%`)
4. Additive (`+`, `-`)
5. Comparison (`<`, `>`, `<=`, `>=`)
6. Equality (`==`, `!=`)
7. Logical AND (`&&`)
8. Logical OR (`||`)
9. Assignment (`=`, `+=`, `-=`, `*=`, `/=`)

### Examples

```c
int result = 2 + 3 * 4;        // 14 (multiplication first)
bool check = 5 > 3 && 2 < 4;  // true (comparisons, then AND)
int x = (2 + 3) * 4;          // 20 (parentheses override)
```

## Related Topics

- [Types and Variables](types.en.md)
- [Control Flow](control-flow.en.md)
- [Expressions](../examples/examples.en.md#expressions)

