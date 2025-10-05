# Variables
Esta seção trata os tipos de variáveis e operações permitidas

## Tipagem e Tipos
A linguagem Three Musketeers é uma linguagem fortemente tipada e de conversão implícita. O que isso implica?

Isso implica que todas as variáveis devem ser atribuidas com um tipo e o próprio compilador realiza as conversos entre os tipos, sem a necessidade do programador escrever que deseja a conversão.

A linguagem Three Musketeers possui cinco tipos de variáveis sendo estas, (do tipo genérico até os inflexíveis):
1. double — Usado para representar números flutuantes
2. int — Usado para representar números inteiros com sinal 
3. char — Usado para representar um único caractere
4. bool — Usado para representar lógica booleana dentro da linguagem
5. string — Representa uma cadeia de caracteres de forma fixa

### Vetores e matrizes
Vetores e matrizes podem ser criados adicionando [] e o numero das dimensões, para atribuir valores deve-se sempre começar em 0

#### Exemplos de atribuição e criação de variáveis 
```C
    double d = 12.0;
    int i = 9;
    bool b = false;
    char c = 'c';
    string str = "Ola mundo!";
    double vetor[2];
    int matriz[2][2];
    vetor[0] = 1.0;
```

