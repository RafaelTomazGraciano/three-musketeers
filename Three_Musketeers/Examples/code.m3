string msg = "Welcome to the Three Musketeers Language!";
puts(msg);

printf("=== Testing Basic Types ===\n");

// Teste de bool
bool isRegistered;
isRegistered = false;
printf("Bool value: %d\n", isRegistered);
isRegistered = true;
printf("Bool value changed: %d\n", isRegistered);

// Teste de char
char letter = 'A';
printf("Character: %c\n", letter);

// Teste de int
int count = 42;
printf("Integer: %d\n", count);

// Teste de double
double price = 19.99;
printf("Double: %.2f\n", price);

// Teste de array
printf("\n=== Testing Arrays ===\n");
int numbers[3];
numbers[0] = 10;
numbers[1] = 20;
numbers[2] = 30;
printf("Array[0]: %d\n", numbers[0]);
printf("Array[1]: %d\n", numbers[1]);
printf("Array[2]: %d\n", numbers[2]);

// Teste de string input
printf("\n=== Testing String Input ===\n");
string name;
printf("Enter your name: ");
gets(name);
printf("Hello, %s!\n", name);

// Teste de scanf
printf("\n=== Testing Scanf ===\n");
int age;
double weight;
printf("Enter your age: ");
scanf(age);
printf("Enter your weight: ");
scanf(weight);
printf("You are %d years old and weigh %.1f kg\n", age, weight);

// Teste de conversões string para número
printf("\n=== Testing String to Number Conversions ===\n");
string strNum = "123";
int convertedInt = atoi(strNum);
printf("String '123' converted to int: %d\n", convertedInt);

string strDouble = "45.67";
double convertedDouble = atod(strDouble);
printf("String '45.67' converted to double: %.2f\n", convertedDouble);

// Teste de conversões número para string
printf("\n=== Testing Number to String Conversions ===\n");
int myInt = 999;
string strFromInt = itoa(myInt);
printf("Int 999 converted to string: %s\n", strFromInt);

double myDouble = 3.14159;
string strFromDouble = dtoa(myDouble);
printf("Double 3.14159 converted to string: %s\n", strFromDouble);

// Teste de operações aritméticas
// printf("\n=== Testing Arithmetic ===\n");
// int a = 10;
// int b = 3;
// int sum = a + b;
// int diff = a - b;
// int prod = a * b;
// int quot = a / b;
// printf("%d + %d = %d\n", a, b, sum);
// printf("%d - %d = %d\n", a, b, diff);
// printf("%d * %d = %d\n", a, b, prod);
// printf("%d / %d = %d\n", a, b, quot);

// Teste final
printf("\n=== All Tests Completed ===\n");
puts("Three Musketeers compiler is working!");