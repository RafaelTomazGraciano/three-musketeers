# Control Flow

Control flow statements allow you to control the execution order of your program. Three Musketeers supports conditional statements and loops similar to C.

## If Statement

Execute code conditionally based on a boolean expression.

### Basic If

```c
if (condition) {
    // Code to execute if condition is true
}
```

### If-Else

```c
if (condition) {
    // Code if condition is true
} else {
    // Code if condition is false
}
```

### If-Else If-Else

```c
if (condition1) {
    // Code if condition1 is true
} else if (condition2) {
    // Code if condition2 is true
} else {
    // Code if all conditions are false
}
```

### Example

```c
int value = 10;

if (value > 0) {
    printf("Value is positive\n");
} else if (value < 0) {
    printf("Value is negative\n");
} else {
    printf("Value is zero\n");
}
```

## Switch Statement

Execute different code blocks based on the value of an expression.

```c
switch (expression) {
    case value1:
        // Code for value1
        break;
    case value2:
        // Code for value2
        break;
    default:
        // Code if no case matches
        break;
}
```

### Example

```c
int option = 2;

switch (option) {
    case 1:
        printf("Option 1 selected\n");
        break;
    case 2:
        printf("Option 2 selected\n");
        break;
    case 3:
        printf("Option 3 selected\n");
        break;
    default:
        printf("Unknown option\n");
        break;
}
```

**Note**: The `break` statement is required to exit the switch block. Without it, execution will "fall through" to the next case.

## For Loop

Execute code a specific number of times.

```c
for (initialization; condition; increment) {
    // Code to repeat
}
```

### Example

```c
int i;
for (i = 0; i < 10; i++) {
    printf("Iteration %d\n", i);
}
```

### Variations

```c
// Initialize variable in loop
for (int i = 0; i < 10; i++) {
    printf("%d\n", i);
}

// Multiple variables
int i, j;
for (i = 0, j = 10; i < 10; i++, j--) {
    printf("i=%d, j=%d\n", i, j);
}
```

## While Loop

Execute code while a condition is true.

```c
while (condition) {
    // Code to repeat
}
```

### Example

```c
int count = 0;
while (count < 5) {
    printf("Count: %d\n", count);
    count = count + 1;
}
```

## Do-While Loop

Execute code at least once, then repeat while condition is true.

```c
do {
    // Code to repeat
} while (condition);
```

### Example

```c
int count = 0;
do {
    printf("Count: %d\n", count);
    count = count + 1;
} while (count < 5);
```

## Break Statement

Exit a loop or switch statement immediately.

```c
for (int i = 0; i < 10; i++) {
    if (i == 5) {
        break;  // Exit loop when i is 5
    }
    printf("%d\n", i);
}
```

**Note**: `break` can only be used inside loops or switch statements.

## Continue Statement

Skip the rest of the current loop iteration and continue with the next iteration.

```c
for (int i = 0; i < 10; i++) {
    if (i == 5) {
        continue;  // Skip iteration when i is 5
    }
    printf("%d\n", i);
}
```

**Note**: `continue` can only be used inside loops.

## Nested Control Flow

You can nest control flow statements:

```c
for (int i = 0; i < 3; i++) {
    for (int j = 0; j < 3; j++) {
        if (i == j) {
            printf("Diagonal: %d, %d\n", i, j);
        }
    }
}
```

## Related Topics

- [Operators](operators.md) - Operators used in conditions
- [Functions](functions.md) - Returning from functions
- [Examples](../examples/examples.md) - More control flow examples

