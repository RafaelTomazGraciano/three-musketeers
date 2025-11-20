# Hello World Tutorial

This tutorial will walk you through creating your first Three Musketeers program step by step.

## Step 1: Create the Source File

Create a new file called `hello.3m`. The `.3m` extension is required for Three Musketeers source files.

## Step 2: Write the Program

```c
#include <stdio.tm>

int main() {
    puts("Hello, World!");
    return 0;
}
```

### Program Explanation

- `#include <stdio.tm>`: Includes the standard input/output library.
- `int main()`: The entry point of every Three Musketeers program. It returns an integer.
- `puts("Hello, World!")`: Prints a string to the console and adds a newline.
- `return 0`: Indicates successful program execution.

## Step 3: Compile

Compile your program:

```bash
tm hello.3m --bin -o hello
```

By default, this creates an executable named `hello` in the `bin` directory.

## Step 4: Run

Execute the program:

```bash
./bin/hello
```

Output:
```
Hello, World!
```

## Using printf for Formatted Output

You can also use `printf` for more control over output formatting:

```c
#include <stdio.tm>

int main() {
    printf("Hello, %s!\n", "World");
    return 0;
}
```

### printf Format Specifiers

| Format | Type   | Description              |
|--------|--------|--------------------------|
| `%d`   | int    | Integer                  |
| `%f`   | double | Floating point number    |
| `%s`   | string | String                   |
| `%c`   | char   | Single character         |

## Reading Input

Here's an example that reads and displays user input:

```c
#include <stdio.tm>

int main() {
    string name;
    printf("Enter your name: ");
    gets(name);
    printf("Hello, %s!\n", name);
    return 0;
}
```

## Next Steps

- Learn about [Data Types](../language-reference/data-types.md)
- Explore [Variables](../language-reference/variables.md)
- Check out more [Examples](../examples/examples.md)

