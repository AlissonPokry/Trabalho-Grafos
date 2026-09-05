# Trabalho M1 - Grafos em C#

## Parte 1 - Representações de Grafos

Implementação em C# das duas representações solicitadas:

- `GrafoMatriz`: matriz de adjacência;
- `GrafoLista`: lista de adjacência com a estrutura auxiliar `Aresta`;
- `Grafos`: classe base comum, configurada no construtor como direcionada ou não e ponderada ou não.

### Operações implementadas (Parte 1)

As duas representações implementam os métodos especificados nos slides:

```csharp
bool inserirVertice(string label)
bool removerVertice(int indice)
string labelVertice(int indice)
void imprimeGrafo()
bool inserirAresta(int origem, int destino, float peso = 1)
bool removerAresta(int origem, int destino)
bool existeAresta(int origem, int destino)
float pesoAresta(int origem, int destino)
List<int> retornarVizinhos(int vertice)
```

Os índices começam em zero. Métodos `bool` retornam `false` para índices ou dados inválidos e para inserções duplicadas. `labelVertice` retorna `null` para um índice inválido; `pesoAresta` retorna `0` quando a aresta não existe. Em grafos não ponderados, toda aresta recebe peso `1`.

Peso zero é rejeitado, pois nos slides ele representa ausência de aresta. Pesos negativos são aceitos para não limitar os próximos algoritmos do trabalho.

### Observação sobre a direção das arestas

A página 76 diz que a ligação de volta deve ser adicionada no grafo direcionado. Isso contradiz a definição apresentada anteriormente e as páginas 69 e 77. A implementação segue a regra correta: a ligação de volta é criada e removida somente em grafos **não direcionados**.

## Parte 2 - Leitura de Arquivos e Algoritmos de Busca

### Leitura de Arquivos

Novo método para carregar grafos de um arquivo de texto:

```csharp
bool carregarDoArquivo(string caminhoArquivo)
```

**Formato do arquivo:**
```
V A D P
Ao Ad Ap
...
```

Onde:
- **V**: número de vértices
- **A**: número de arestas
- **D**: 0 (não direcionado) ou 1 (direcionado)
- **P**: 0 (não ponderado) ou 1 (ponderado)
- **Ao**: vértice de origem da aresta
- **Ad**: vértice de destino da aresta
- **Ap**: peso da aresta (opcional, apenas para grafos ponderados)

O tipo do grafo vem do **próprio cabeçalho do arquivo**: `carregarDoArquivo` lê `D` e `P` e
reconfigura o grafo (descartando o conteúdo anterior), então não é preciso saber de antemão se
o arquivo é direcionado ou ponderado — qualquer arquivo válido abre em qualquer instância.
O método retorna `false` apenas quando o arquivo não existe ou o cabeçalho não está no formato.

A leitura tolera espaços repetidos, tabulações e linhas em branco, e aceita arquivos com
quebra de linha do Windows (CRLF).

**Exemplos de arquivos inclusos:**
- `grafo_simples.txt`: grafo não direcionado e não ponderado
- `grafo_direcionado.txt`: grafo direcionado e não ponderado
- `grafo_ponderado.txt`: grafo não direcionado e ponderado
- `grafos/`: arquivos dos slides e o `espacoaereo.txt` (333 vértices, ponderado)

### Menu interativo

Rodar sem argumentos (`dotnet run`) abre um menu que lista automaticamente os arquivos `.txt`
da pasta do projeto e da subpasta `grafos/`, mostrando o tipo de cada um. Depois de escolher o
arquivo, o grafo é carregado **nas duas representações** e ambas são impressas (matriz de
adjacência e lista de adjacência); em seguida é possível rodar BFS, DFS e Dijkstra sobre ele.
Grafos com mais de 30 vértices pedem confirmação antes de imprimir a matriz.

Outros modos: `dotnet run -- --demo` (demonstração fixa antiga), `dotnet run -- --test`
(testes da Parte 1) e `dotnet run -- --teste-parte2` (testes da Parte 2).

### Algoritmos Implementados

Todos os três algoritmos funcionam tanto na representação de **matriz** quanto na de **lista**, usando o mesmo código na classe base `Grafos`.

#### 1. Busca em Largura (BFS - Breadth-First Search)

```csharp
List<int> buscaLargura(int origem)
```

Retorna uma lista com a ordem de visitação dos vértices começando da origem, visitando todos os vértices alcançáveis em ordem de proximidade.

**Características:**
- Usa fila (Queue) para controlar a ordem de visitação
- Visita vértices por nível (distância)
- Tempo: O(V + A), onde V é número de vértices e A é número de arestas

#### 2. Busca em Profundidade (DFS - Depth-First Search)

```csharp
List<int> buscaProfundidade(int origem)
```

Retorna uma lista com a ordem de visitação dos vértices começando da origem, visitando todos os vértices alcançáveis em profundidade.

**Características:**
- Usa recursão e stack implícito
- Explora o mais profundo possível antes de retroceder
- Tempo: O(V + A)

#### 3. Algoritmo de Dijkstra

```csharp
Dictionary<int, (float distancia, List<int> caminho)> dijkstra(int origem)
```

Calcula a menor distância e o caminho da origem para todos os outros vértices em um grafo ponderado.

**Retorno:**
- Dicionário onde a chave é o vértice destino
- Valor é uma tupla com:
  - `distancia`: distância mínima da origem
  - `caminho`: lista ordenada de vértices do caminho mais curto

**Características:**
- Funciona com grafos ponderados (não ponderados tratam todas as arestas com peso 1)
- Grafo deve ser não direcionado e ponderado conforme criado
- Tempo: O(V²) na implementação atual
- Retorna `float.MaxValue` para vértices inalcançáveis

## Executar

Com o SDK do .NET 8 instalado, dentro desta pasta:

```powershell
dotnet run
dotnet run -- --test
```

O primeiro comando mostra demonstrações dos algoritmos BFS, DFS e Dijkstra com vários tipos de grafos. O segundo executa testes da Parte 1 (inserção, remoção, reindexação, direção, peso, vizinhos e self-loop).
