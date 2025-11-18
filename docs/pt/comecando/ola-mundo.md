# Tutorial Olá Mundo

Este tutorial guiará você passo a passo na criação do seu primeiro programa Three Musketeers.

## Passo 1: Criar o Arquivo Fonte

Crie um novo arquivo chamado `hello.tm`. A extensão `.tm` é obrigatória para arquivos-fonte Three Musketeers.

## Passo 2: Escrever o Programa

```c
int main() {
    puts("Hello, World!");
    return 0;
}
```

### Explicação do Programa

- `int main()`: O ponto de entrada de todo programa Three Musketeers. Retorna um inteiro.
- `puts("Hello, World!")`: Imprime uma string no console e adiciona uma nova linha.
- `return 0`: Indica execução bem-sucedida do programa.

## Passo 3: Compilar

Compile seu programa:

```bash
Three_Musketeers hello.tm
```

Por padrão, isso cria um executável chamado `a.out` no diretório `bin`.

## Passo 4: Executar

Execute o programa:

```bash
./bin/a.out
```

Saída:
```
Hello, World!
```

## Usando printf para Saída Formatada

Você também pode usar `printf` para mais controle sobre a formatação da saída:

```c
int main() {
    printf("Hello, %s!\n", "World");
    return 0;
}
```

### Especificadores de Formato printf

| Formato | Tipo   | Descrição              |
|--------|--------|------------------------|
| `%d`   | int    | Inteiro                |
| `%f`   | double | Número de ponto flutuante |
| `%s`   | string | String                 |
| `%c`   | char   | Caractere único        |

## Lendo Entrada

Aqui está um exemplo que lê e exibe a entrada do usuário:

```c
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

