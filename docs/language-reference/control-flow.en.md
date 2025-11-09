[English](#english) | [Português](control-flow.pt.md)

<a name="english"></a>
# Control Flow

Three Musketeers provides several control flow statements to control the execution of your program: conditionals, loops, and branching statements.

## Conditional Statements

### if Statement

The `if` statement executes a block of code if a condition is true.

```c
int x = 10;

if (x > 5) {
    puts("x is greater than 5");
}
```

### if-else Statement

The `else` clause executes when the condition is false.

```c
int x = 3;

if (x > 5) {
    puts("x is greater than 5");
} else {
    puts("x is not greater than 5");
}
```

### if-else if-else Statement

You can chain multiple conditions using `else if`.

```c
int score = 85;

if (score >= 90) {
    puts("Grade: A");
} else if (score >= 80) {
    puts("Grade: B");
} else if (score >= 70) {
    puts("Grade: C");
} else {
    puts("Grade: F");
}
```

## Switch Statement

The `switch` statement allows you to select one of many code blocks to execute based on a value.

```c
int option = 2;

switch (option) {
    case 1:
        puts("Option 1 selected");
        break;
    case 2:
        puts("Option 2 selected");
        break;
    case 3:
        puts("Option 3 selected");
        break;
    default:
        puts("Unknown option");
        break;
}
```

### Switch with Characters

You can also switch on character values:

```c
char grade = 'B';

switch (grade) {
    case 'A':
        puts("Excellent");
        break;
    case 'B':
        puts("Good");
        break;
    case 'C':
        puts("Average");
        break;
    default:
        puts("Below average");
        break;
}
```

### Important: break Statement

The `break` statement is crucial in switch statements. Without it, execution will "fall through" to the next case.

## Loops

### for Loop

The `for` loop repeats a block of code a specific number of times.

```c
int i;
for (i = 0; i < 10; i++) {
    printf("Iteration %d\n", i);
}
```

The `for` loop has three parts:
1. **Initialization** - Executed once at the start
2. **Condition** - Checked before each iteration
3. **Increment** - Executed after each iteration

### for Loop Variations

```c
// Declaration in initialization
for (int i = 0; i < 5; i++) {
    printf("%d\n", i);
}

// Assignment in initialization
int i;
for (i = 0; i < 5; i++) {
    printf("%d\n", i);
}

// Complex increment
for (int i = 0; i < 10; i += 2) {
    printf("%d\n", i);  // Prints 0, 2, 4, 6, 8
}
```

### while Loop

The `while` loop repeats a block of code while a condition is true.

```c
int count = 0;

while (count < 5) {
    printf("Count: %d\n", count);
    count++;
}
```

### do-while Loop

The `do-while` loop is similar to `while`, but it always executes at least once because the condition is checked at the end.

```c
int x = 0;

do {
    printf("x = %d\n", x);
    x++;
} while (x < 5);
```

## Loop Control Statements

### break Statement

The `break` statement exits a loop immediately.

```c
int i;
for (i = 0; i < 10; i++) {
    if (i == 5) {
        break;  // Exit loop when i equals 5
    }
    printf("%d\n", i);
}
// Prints: 0, 1, 2, 3, 4
```

### continue Statement

The `continue` statement skips the rest of the current iteration and continues with the next iteration.

```c
int i;
for (i = 0; i < 10; i++) {
    if (i == 5) {
        continue;  // Skip iteration when i equals 5
    }
    printf("%d\n", i);
}
// Prints: 0, 1, 2, 3, 4, 6, 7, 8, 9
```

## Nested Control Structures

You can nest control structures inside each other.

### Nested if Statements

```c
int x = 10;
int y = 5;

if (x > 0) {
    if (y > 0) {
        puts("Both x and y are positive");
    } else {
        puts("x is positive, but y is not");
    }
}
```

### Nested Loops

```c
int i, j;
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        printf("(%d, %d) ", i, j);
    }
    printf("\n");
}
// Output:
// (0, 0) (0, 1) (0, 2)
// (1, 0) (1, 1) (1, 2)
// (2, 0) (2, 1) (2, 2)
```

### Loops with Conditionals

```c
int i;
for (i = 1; i <= 10; i++) {
    if (i % 2 == 0) {
        printf("%d is even\n", i);
    } else {
        printf("%d is odd\n", i);
    }
}
```

## Related Topics

- [Operators](operators.en.md)
- [Functions](functions.en.md)
- [Examples](../examples/examples.en.md)

