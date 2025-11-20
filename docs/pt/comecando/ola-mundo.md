# Tutorial Olá Mundo

Este tutorial guiará você passo a passo na criação do seu primeiro programa Three Musketeers.

## Passo 1: Criar o Arquivo Fonte

Crie um novo arquivo chamado `hello.3m`. A extensão `.3m` é obrigatória para arquivos-fonte Three Musketeers.

## Passo 2: Escrever o Programa

```c
#include <stdio.tm>

int main() {
    puts("Hello, World!");
    return 0;
}
```

### Explicação do Programa

- `#include <stdio.tm>`: Inclui a biblioteca padrão de entrada/saída.
- `int main()`: O ponto de entrada de todo programa Three Musketeers. Retorna um inteiro.
- `puts("Hello, World!")`: Imprime uma string no console e adiciona uma nova linha.
- `return 0`: Indica execução bem-sucedida do programa.

## Passo 3: Compilar

Compile seu programa:

```bash
tm hello.3m --bin -o hello
```

Por padrão, isso cria um executável chamado `hello` no diretório `bin`.

## Passo 4: Executar

Execute o programa:

```bash
./bin/hello
```

Saída:
```
Hello, World!
```

## Usando printf para Saída Formatada

Você também pode usar `printf` para mais controle sobre a formatação da saída:

```c
#include <stdio.tm>

int main() {
    printf("Hello, %s!\n", "World");
    return 0;
}
```

### Especificadores de Formato printf

| Formato | Tipo   | Descrição                 |
|--------|---------|---------------------------|
| `%d`   | int     | Inteiro                   |
| `%f`   | double  | Número de ponto flutuante |
| `%s`   | string  | String                    |
| `%c`   | char    | Caractere único           |

## Lendo Entrada

Aqui está um exemplo que lê e exibe a entrada do usuário:

```c
#include <stdio.tm>

int main() {
    string name;
    printf("Digite seu nome: ");
    gets(name);
    printf("Hello, %s!\n", name);
    return 0;
}
```

## Próximos Passos

- Aprenda sobre [Tipos de Dados](../referencia-linguagem/tipos-dados.md)
- Explore [Variáveis](../referencia-linguagem/variaveis.md)
- Veja mais [Exemplos](../exemplos/exemplos.md)

