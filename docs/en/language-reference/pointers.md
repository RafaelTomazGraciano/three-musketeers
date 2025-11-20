# Pointers

Pointers are variables that store memory addresses. They provide powerful capabilities for memory management and efficient data manipulation.

## Pointer Declaration

Declare a pointer using the `*` operator:

```c
int *ptr;        // Pointer to int
double *dptr;    // Pointer to double
char *cptr;      // Pointer to char
```

## Address-of Operator (`&`)

Get the memory address of a variable:

```c
int x = 10;
int *ptr = &x;   // ptr now contains the address of x
```

## Dereference Operator (`*`)

Access the value at a pointer's address:

```c
int x = 10;
int *ptr = &x;
int y = *ptr;    // y = 10 (value at address stored in ptr)
```

## Pointer Assignment

```c
int x = 10;
int *ptr = &x;   // ptr points to x
*ptr = 20;       // x is now 20
```

## Pointer to Pointer

You can create pointers to pointers:

```c
int x = 10;
int *ptr = &x;
int **pptr = &ptr;  // Pointer to pointer

**pptr = 30;        // x is now 30
```

## Dynamic Memory Allocation

### malloc

Allocate memory dynamically:

```c
int *arr = malloc(10);  // Allocate memory for 10 integers
arr[0] = 1;
arr[1] = 2;
```

### free

Release dynamically allocated memory:

```c
int *ptr = malloc(5);
// Use ptr...
free(ptr);  // Release memory
```

**Important**: Always free memory allocated with `malloc` to prevent memory leaks.

## Pointers and Arrays

Pointers and arrays are closely related:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;  // ptr points to first element

// These are equivalent:
arr[0] = 10;
*ptr = 10;
```

## Pointer Arithmetic

You can perform arithmetic on pointers:

```c
int arr[5] = {1, 2, 3, 4, 5};
int *ptr = arr;

ptr++;        // Move to next element
int value = *ptr;  // value = 2
```

## Function Parameters

Pointers allow functions to modify variables:

```c
void increment(int *x) {
    (*x)++;
}

int main() {
    int value = 10;
    increment(&value);  // value is now 11
    return 0;
}
```

## Returning Pointers

Functions can return pointers:

```c
int* createArray(int size) {
    int *arr = malloc(size);
    return arr;
}
```

## Example: Swapping Values

```c
void swap(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10, y = 20;
    swap(&x, &y);  // x = 20, y = 10
    return 0;
}
```

## Common Pitfalls

1. **Dereferencing NULL or uninitialized pointers**: Always initialize pointers
2. **Memory leaks**: Always free memory allocated with `malloc`
3. **Dangling pointers**: Don't use pointers after freeing memory

## Related Topics

- [Variables](variables.md) - Variable declarations
- [Arrays](arrays.md) - Arrays and pointers
- [Functions](functions.md) - Functions with pointer parameters

