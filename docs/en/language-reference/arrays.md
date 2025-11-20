# Arrays

Arrays allow you to store multiple values of the same type in a contiguous block of memory. Three Musketeers supports single and multi-dimensional arrays.

## Array Declaration

### One-Dimensional Arrays

```c
int numbers[10];        // Array of 10 integers
double prices[5];       // Array of 5 doubles
char buffer[100];       // Array of 100 characters
```

### Multi-Dimensional Arrays

```c
int matrix[3][3];       // 3x3 two-dimensional array
double grid[5][5][5];   // 5x5x5 three-dimensional array
```

## Array Initialization

Arrays are zero-indexed (indices start at 0):

```c
int arr[5];
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
arr[3] = 40;
arr[4] = 50;
```

### Multi-Dimensional Array Access

```c
int matrix[3][3];
matrix[0][0] = 1;
matrix[0][1] = 2;
matrix[1][0] = 3;
matrix[2][2] = 9;
```

## Array Bounds

Array indices must be within the declared bounds. Accessing out-of-bounds indices results in undefined behavior:

```c
int arr[5];
arr[0] = 10;  // Valid
arr[4] = 50;  // Valid
arr[5] = 60;  // Invalid! Out of bounds
```

## Arrays and Loops

Common pattern for iterating through arrays:

```c
int arr[5];
int i;

// Initialize array
for (i = 0; i < 5; i++) {
    arr[i] = i * 10;
}

// Print array
for (i = 0; i < 5; i++) {
    printf("arr[%d] = %d\n", i, arr[i]);
}
```

## Multi-Dimensional Arrays and Nested Loops

```c
int matrix[3][3];
int i, j;

// Initialize matrix
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        matrix[i][j] = i * 3 + j;
    }
}

// Print matrix
for (i = 0; i < 3; i++) {
    for (j = 0; j < 3; j++) {
        printf("%d ", matrix[i][j]);
    }
    printf("\n");
}
```

## Arrays as Function Parameters

Arrays can be passed to functions. They are passed by reference:

```c
#include <stdio.tm>

void printArray(int arr[], int size) {
    int i;
    for (i = 0; i < size; i++) {
        printf("%d ", arr[i]);
    }
    printf("\n");
}

int main() {
    int numbers[5] = {10, 20, 30, 40, 50};
    printArray(numbers, 5);
    return 0;
}
```

## Arrays and Pointers

Arrays and pointers are closely related:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;  // ptr points to first element

// These are equivalent:
arr[0] = 10;
*ptr = 10;

// Pointer arithmetic
ptr++;
*ptr = 20;  // arr[1] = 20
```

## Character Arrays (Strings)

Character arrays can be used to store strings:

```c
char name[50];
name[0] = 'J';
name[1] = 'o';
name[2] = 'h';
name[3] = 'n';
name[4] = '\0';  // Null terminator
```

However, Three Musketeers also provides the `string` type for easier string handling.

## Array of Structures

You can create arrays of structures:

```c
struct Point {
    int x,
    int y
};

struct Point points[10];
points[0].x = 1;
points[0].y = 2;
points[1].x = 3;
points[1].y = 4;
```

## Dynamic Arrays

Use pointers and `malloc` for dynamic arrays:

```c
int *arr = malloc(10);  // Allocate array of 10 integers
arr[0] = 1;
arr[1] = 2;
// ... use array ...
free(arr);  // Don't forget to free!
```

## Common Patterns

### Finding Maximum Value

```c
int findMax(int arr[], int size) {
    int max = arr[0];
    int i;
    for (i = 1; i < size; i++) {
        if (arr[i] > max) {
            max = arr[i];
        }
    }
    return max;
}
```

### Sum of Array Elements

```c
int sumArray(int arr[], int size) {
    int sum = 0;
    int i;
    for (i = 0; i < size; i++) {
        sum = sum + arr[i];
    }
    return sum;
}
```

## Related Topics

- [Variables](variables.md) - Variable declarations
- [Pointers](pointers.md) - Arrays and pointers
- [Control Flow](control-flow.md) - Looping through arrays

