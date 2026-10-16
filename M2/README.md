# Trabalho M2 — Grafos em C#

Implementação das duas partes da M2: coloração por força bruta, gulosa sem ordenação,
Welsh–Powell e DSATUR; árvore geradora mínima por Prim e Kruskal.

## Executar

Requer SDK do .NET 8 ou superior e runtime do .NET 8. Dentro de `M2`:

```sh
dotnet run
dotnet run -- --test
```

A partir da pasta pai:

```sh
dotnet run --project M2/TrabalhoM2.Grafos.csproj
dotnet run --project M2/TrabalhoM2.Grafos.csproj -- --test
```

O menu lista arquivos de M2, da pasta pai e das respectivas subpastas `grafos`.
Também permite informar um caminho relativo ao diretório atual ou um caminho absoluto.
Após escolher o arquivo, selecione lista (padrão, pressionando Enter) ou matriz.
A representação selecionada é a única carregada; não há impressão automática do grafo.
A comparação de coloração executa somente as três heurísticas. A força bruta tem opção própria.
EOF não deixa o menu preso: encerra a execução como a opção Sair.

## Reutilização da M1

O projeto inclui por referência os fontes `../Grafos.cs`, `../GrafoLista.cs` e
`../GrafoMatriz.cs`. É necessário manter M2 dentro da pasta do trabalho anterior;
a pasta M2 sozinha não é uma entrega independente.

Os novos fontes estão em **`.src/`**:

- [Algoritmos.cs](.src/Algoritmos.cs): algoritmos e tipos de resultado;
- [Program.cs](.src/Program.cs): seleção de arquivo, representação, algoritmo e impressão;
- [Testes.cs](.src/Testes.cs): testes executáveis sem bibliotecas externas.

Os diretórios `.src` e `.build` são intencionalmente ocultos: o projeto da M1 inclui
recursivamente arquivos `.cs`, mas ignora diretórios ocultos. Isso evita que a M1
compile o segundo `Main` ou os atributos gerados da M2. `Directory.Build.props`
coloca os artefatos em `.build/`; nenhum arquivo da M1 precisa ser alterado.
No Finder, `Command+Shift+.` mostra os diretórios ocultos.

## Formato de entrada

É o mesmo da M1, com vértices numerados de zero até V−1:

```text
V A D P
origem destino [peso]
```

`D=0` indica não direcionado; `D=1`, direcionado. `P=0` indica não ponderado;
`P=1`, ponderado. Pesos usam ponto decimal. Exemplo de ciclo ímpar:

```text
5 5 0 0
0 1
1 2
2 3
3 4
4 0
```

A leitura é a implementação original da M1: aceita espaços, tabulações e linhas em
branco entre arestas. Ela pode ignorar linhas de arestas inválidas, arestas duplicadas
e índices fora do intervalo, e não valida rigorosamente a quantidade declarada.
Peso zero é rejeitado pela base; pesos negativos são aceitos. O menu informa falha
quando o leitor retorna `false` (por exemplo, arquivo inexistente ou cabeçalho inválido).

## Algoritmos e resultados

Os métodos públicos de `TrabalhoM2.Algoritmos` recebem `Grafos`, sem alterar sua
estrutura. São compatíveis com lista e matriz. Não imprimem no console.
Os pseudocódigos dos slides não foram fornecidos; foram adotadas as versões clássicas.

### Coloração

- `Gulosa`: percorre os índices em ordem natural, sem classificação por grau ou saturação,
  e escolhe a menor cor ausente nos vizinhos.
- `WelshPowell`: ordena por grau decrescente; para cada cor percorre a ordem inteira,
  colorindo os vértices ainda livres que não tenham vizinho naquela cor.
- `Dsatur`: seleciona a maior quantidade de cores distintas entre vizinhos já coloridos;
  desempata pelo maior grau original e depois pelo menor índice. Usa a menor cor disponível.
- `ForcaBruta`: enumera atribuições completas começando por duas cores, depois três,
  até encontrar a primeira atribuição válida. Usa contador iterativo, sem poda,
  ordenação heurística ou limite de tempo. Casos triviais: vazio usa zero cores e
  grafo não vazio sem arestas usa uma.

`ResultadoColoracao` contém `Cores` (posição = índice do vértice; cores a partir de 1),
`QuantidadeCores` e `Duracao`. A força bruta encontra o mínimo; as heurísticas não
garantem otimalidade. Grafos direcionados ou ponderados são rejeitados com
`ArgumentException`, assim como grafos com laços, que não admitem coloração própria.

A saída sempre mostra tempo em milissegundos e número de cores. Para menos de dez
vértices, mostra também cada vértice, seu rótulo e sua cor. Exemplo com o arquivo
`grafos/ciclo_impar.txt`, usando DSATUR (o tempo depende da máquina):

```text
DSATUR:
Cores utilizadas: 3
Tempo de execução: ... ms
Vértice 0 [0]: cor 1
Vértice 1 [1]: cor 2
Vértice 2 [2]: cor 1
Vértice 3 [3]: cor 2
Vértice 4 [4]: cor 3
```

A força bruta tem custo exponencial, chegando a enumerar k^V atribuições por
quantidade de cores. Mesmo um grafo pequeno pode levar muito tempo. A execução
ocorre somente na opção 4; Ctrl+C encerra o programa, sem apresentar resultado
parcial como se fosse ótimo. Não use essa opção para comparar grafos grandes.

### Árvore geradora mínima

- `Prim`: começa no menor índice, usa fila de prioridade por peso e acrescenta
  a aresta mais leve que alcança um vértice ainda não visitado. Reinicia em cada componente.
- `Kruskal`: considera cada aresta uma vez, ordena por peso e usa união por tamanho
  e compressão de caminhos para evitar ciclos.

Empates de arestas usam o menor e o maior índice das extremidades, nessa ordem.
Ambos aceitam grafos não direcionados com pesos negativos; em não ponderados o
peso é 1. Grafos direcionados são rejeitados com `ArgumentException`. Laços são ignorados.

`ResultadoAgm` contém `Arestas` (`Origem`, `Destino`, `Peso`), `PesoTotal` acumulado
em `double`, `Componentes` e `Duracao`. Um grafo desconectado produz uma floresta
mínima, identificada na saída. Isolados contam como componentes; no grafo vazio há
zero componentes, zero arestas e soma zero. O menu mostra tempo, soma e arestas.

O exemplo `grafos/agm_desconectado.txt` tem soma **4**, três componentes (incluindo
um vértice isolado) e três arestas na floresta. O laço de peso −5 não entra no resultado.
O arquivo `../grafo_ponderado.txt` tem AGM de peso **13**.

### Tempo e escala

`Stopwatch` mede cada execução desde a preparação das estruturas auxiliares até
a montagem do resultado, incluindo ordenações e soma/contagem. Leitura de arquivo
e impressão ficam fora da medição. Comparações fazem execuções independentes,
sem aquecimento ou média estatística; tempos muito curtos variam com JIT e ambiente.

As heurísticas usam listas de vizinhos auxiliares. DSATUR seleciona o próximo vértice
por varredura, com custo O(V² + A). Prim usa fila e Kruskal ordena arestas.
A preparação da AGM ainda consulta pesos pela API original: na lista da M1,
essa consulta percorre a lista do vértice e pode custar O(Σ grau(v)²) no total.
A construção da matriz original redimensiona e copia a estrutura a cada vértice,
com custo acumulado O(V³) e memória O(V²). Para milhares de vértices, use lista.
A força bruta não faz parte da comparação automática das heurísticas.

## Validação

`--test` retorna zero quando todos os testes passam e 1 quando algum falha.
A suíte verifica:

- Coloração de vazio, isolados, caminho, ciclos par/ímpar, clique e componentes
  desconectados; conflitos em todas as arestas e mínimos conhecidos por força bruta.
- Correspondência entre lista e matriz; rejeição de direcionados, ponderados na
  coloração e laços na coloração.
- AGM com pesos conhecidos, negativos, empates, laços, isolados, vazio e componentes;
  existência de arestas, pesos, ausência de ciclos e contagem V−componentes.
- Grafos aleatórios pequenos, com AGM conferida por enumeração independente dos
  subconjuntos de arestas, e heurísticas comparadas com coloração exata.
- Leitura dos exemplos, caminho inexistente e saída de cores para nove e dez vértices.
- Caminho com **5.000 vértices** em lista, executando as três heurísticas, Prim e Kruskal,
  verificando duas cores e peso 4.999, sem executar força bruta.
