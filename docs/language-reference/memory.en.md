[English](#english) | [Português](memory.pt.md)

<a name="english"></a>
# Memory Management

Three Musketeers provides manual memory management through pointers, dynamic allocation with `malloc`, and deallocation with `free`.

## Pointers

Pointers are variables that store memory addresses. They allow you to work with memory directly and pass data by reference.

### Pointer Declaration

```c
int *ptr;           // Pointer to int
double *dptr;       // Pointer to double
char *cptr;         // Pointer to char
int **pptr;         // Pointer to pointer to int
```

### Address Operator (&)

The address operator `&` returns the memory address of a variable.

```c
int x = 10;
int *ptr = &x;      // ptr now contains the address of x
```

### Dereference Operator (*)

The dereference operator `*` accesses the value at a memory address.

```c
int x = 10;
int *ptr = &x;

int value = *ptr;   // value = 10 (dereference)
*ptr = 20;          // x = 20 (modify through pointer)
```

### Pointer Arithmetic

Pointers can be used with arrays and support arithmetic operations.

```c
int arr[5] = {10, 20, 30, 40, 50};
int *ptr = arr;     // Points to first element

int first = *ptr;           // 10
int second = *(ptr + 1);    // 20
int third = ptr[2];         // 30 (array notation)
```

## Dynamic Memory Allocation

### malloc

The `malloc` function allocates a block of memory of the specified size and returns a pointer to it.

```c
int *arr = malloc(5);       // Allocate memory for 5 integers
arr[0] = 10;
arr[1] = 20;
arr[2] = 30;
```

### malloc with Assignment

You can declare and allocate in one statement:

```c
int *ptr;
ptr = malloc(10);           // Allocate memory for 10 integers

// Or combine declaration and allocation
int *newPtr = malloc(5);   // Allocate memory for 5 integers
```

### Pointer Initialization with malloc

```c
int *pointer;
pointer = malloc(size);     // Allocate memory
pointer[0] = 100;
pointer[1] = 200;
```

## Memory Deallocation

### free

The `free` function deallocates memory that was previously allocated with `malloc`. Always free memory when you're done with it to prevent memory leaks.

```c
int *ptr = malloc(5);
// ... use ptr ...
free(ptr);          // Deallocate memory
```

### Example: Complete Memory Management

```c
int *createArray(int size) {
    int *arr = malloc(size);
    int i;
    for (i = 0; i < size; i++) {
        arr[i] = i * 10;
    }
    return arr;
}

int main() {
    int *numbers = createArray(5);
    
    int i;
    for (i = 0; i < 5; i++) {
        printf("%d ", numbers[i]);
    }
    printf("\n");
    
    free(numbers);  // Important: free allocated memory
    return 0;
}
```

## Pointer to Pointer

You can have pointers that point to other pointers.

```c
int x = 10;
int *ptr = &x;          // ptr points to x
int **pptr = &ptr;      // pptr points to ptr

int value = **pptr;     // value = 10 (double dereference)
**pptr = 20;            // x = 20

// Modify through double pointer
int *otherPtr = malloc(5);
int **otherPptr = &otherPtr;
(**otherPptr)[0] = 77;  // Modify array through double pointer
```

## Passing Pointers to Functions

Passing pointers to functions allows them to modify the original values.

```c
void modifyValue(int *ptr) {
    *ptr = 100;
}

void swap(int *a, int *b) {
    int temp = *a;
    *a = *b;
    *b = temp;
}

int main() {
    int x = 10;
    modifyValue(&x);
    printf("x = %d\n", x);  // x = 100
    
    int a = 5, b = 10;
    swap(&a, &b);
    printf("a = %d, b = %d\n", a, b);  // a = 10, b = 5
    return 0;
}
```

## Returning Pointers from Functions

Functions can return pointers to dynamically allocated memory.

```c
int *allocateArray(int size) {
    int *arr = malloc(size);
    return arr;
}

int *createInitializedArray(int size, int value) {
    int *arr = malloc(size);
    int i;
    for (i = 0; i < size; i++) {
        arr[i] = value;
    }
    return arr;
}
```

## Common Pitfalls

### Memory Leaks

Always free memory allocated with `malloc`:

```c
int *ptr = malloc(10);
// ... use ptr ...
free(ptr);  // Don't forget to free!
```

### Dangling Pointers

Don't use pointers after freeing them:

```c
int *ptr = malloc(10);
free(ptr);
// ptr is now a dangling pointer - don't use it!
```

### Null Pointer Dereference

Check for null pointers before dereferencing:

```c
int *ptr = malloc(10);
if (ptr != 0) {  // Check if allocation succeeded
    *ptr = 100;
}
```

## Related Topics

- [Types and Variables](types.en.md)
- [Functions](functions.en.md)
- [Structures and Unions](structures.en.md)

