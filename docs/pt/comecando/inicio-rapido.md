# Início Rápido

Comece a usar o Three Musketeers em minutos.

## Seu Primeiro Programa

Crie um arquivo chamado `hello.tm` com o seguinte conteúdo:

```c
int main() {
    puts("Hello, Three Musketeers!");
    return 0;
}
```

## Compilando Seu Programa

Compile o programa usando o compilador Three Musketeers:

```bash
Three_Musketeers hello.tm -o hello
```

Isso irá:
1. Analisar e processar seu código-fonte
2. Gerar código LLVM IR
3. Compilar para assembly
4. Linkar em um executável chamado `hello` no diretório `bin`

## Executando Seu Programa

Execute o programa compilado:

```bash
./bin/hello
```

Você deve ver a saída:

```
Hello, Three Musketeers!
```

## Opções do Compilador

### Uso Básico

```bash
Three_Musketeers <arquivo-entrada> [opções]
```

### Opções Comuns

- `-o <nome>` ou `--out <nome>`: Especifica o nome do executável de saída (padrão: `a.out`)
- `-O <nível>` ou `--opt <nível>`: Define o nível de otimização (0-3, padrão: 2)
- `-g`: Adiciona informações de debug
- `--ll`: Mantém o código LLVM IR gerado
- `-I <caminho>` ou `--Include <caminho>`: Caminho de biblioteca de inclusão

### Exemplo com Opções

```bash
Three_Musketeers hello.tm -o meuprograma -O 3 -g
```

## Estrutura de Arquivos

Após a compilação, você encontrará:
- `bin/hello`: O executável compilado
- `bin/hello.ll`: Código LLVM IR (se a flag `--ll` foi usada)

## Próximos Passos

- [Tutorial Olá Mundo](ola-mundo.md) - Aprenda mais sobre o básico
- [Referência da Linguagem](../referencia-linguagem/visao-geral.md) - Explore os recursos da linguagem
- [Exemplos](../exemplos/exemplos.md) - Veja mais exemplos de código

