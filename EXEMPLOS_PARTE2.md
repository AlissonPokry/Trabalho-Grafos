# Exemplos de Uso - Parte 2 do Trabalho

## Exemplo 1: Usando BFS e DFS

```csharp
// Criar um grafo não-direcionado e não-ponderado
Grafos grafo = new GrafoMatriz(false, false);

// Inserir vértices
for (int i = 0; i < 5; i++)
{
    grafo.inserirVertice(i.ToString());
}

// Inserir arestas
grafo.inserirAresta(0, 1);
grafo.inserirAresta(0, 2);
grafo.inserirAresta(1, 3);
grafo.inserirAresta(2, 3);
grafo.inserirAresta(3, 4);

// Executar BFS começando do vértice 0
List<int> visitasBFS = grafo.buscaLargura(0);
Console.WriteLine("BFS: " + string.Join(", ", visitasBFS));
// Saída: BFS: 0, 1, 2, 3, 4

// Executar DFS começando do vértice 0
List<int> visitasDFS = grafo.buscaProfundidade(0);
Console.WriteLine("DFS: " + string.Join(", ", visitasDFS));
// Saída: DFS: 0, 1, 3, 4, 2 (ou similar, dependendo da ordem)
```

## Exemplo 2: Usando Dijkstra

```csharp
// Criar um grafo não-direcionado e ponderado
Grafos grafo = new GrafoMatriz(false, true);

// Inserir vértices
for (int i = 0; i < 4; i++)
{
    grafo.inserirVertice(i.ToString());
}

// Inserir arestas com pesos
grafo.inserirAresta(0, 1, 1.0f);
grafo.inserirAresta(0, 2, 4.0f);
grafo.inserirAresta(1, 2, 2.0f);
grafo.inserirAresta(1, 3, 5.0f);
grafo.inserirAresta(2, 3, 1.0f);

// Executar Dijkstra começando do vértice 0
var resultado = grafo.dijkstra(0);

// Acessar resultado
foreach (var kvp in resultado)
{
    int destino = kvp.Key;
    float distancia = kvp.Value.distancia;
    List<int> caminho = kvp.Value.caminho;

    if (distancia == float.MaxValue)
    {
        Console.WriteLine($"Vértice {destino}: INALCANÇÁVEL");
    }
    else
    {
        string caminhoStr = string.Join(" -> ", caminho);
        Console.WriteLine($"Vértice {destino}: distância = {distancia}, caminho = {caminhoStr}");
    }
}

/* Saída esperada:
Vértice 0: distância = 0, caminho = 0
Vértice 1: distância = 1, caminho = 0 -> 1
Vértice 2: distância = 3, caminho = 0 -> 1 -> 2
Vértice 3: distância = 4, caminho = 0 -> 1 -> 2 -> 3
*/
```

## Exemplo 3: Carregando Grafo de Arquivo

```csharp
// Criar um grafo com as mesmas propriedades do arquivo
Grafos grafo = new GrafoMatriz(false, false);

// Carregar do arquivo
if (grafo.carregarDoArquivo("grafo_simples.txt"))
{
    Console.WriteLine("Grafo carregado com sucesso!");
    
    // Usar os algoritmos
    Console.WriteLine("BFS: " + string.Join(", ", grafo.buscaLargura(0)));
    Console.WriteLine("DFS: " + string.Join(", ", grafo.buscaProfundidade(0)));
}
else
{
    Console.WriteLine("Erro ao carregar arquivo!");
}
```

## Exemplo 4: Comparando Matriz vs Lista

```csharp
// Ambas as representações funcionam identicamente para os mesmos algoritmos
Grafos matriz = new GrafoMatriz(false, true);
Grafos lista = new GrafoLista(false, true);

// Populando
for (int i = 0; i < 5; i++)
{
    matriz.inserirVertice(i.ToString());
    lista.inserirVertice(i.ToString());
}

matriz.inserirAresta(0, 1, 2.0f);
lista.inserirAresta(0, 1, 2.0f);

// Os resultados são idênticos
var resBFS_Matriz = matriz.buscaLargura(0);
var resBFS_Lista = lista.buscaLargura(0);
Console.WriteLine("BFS resultados iguais: " + SequenciaIgual(resBFS_Matriz, resBFS_Lista));
```

## Notas Importantes

1. **BFS e DFS** funcionam com qualquer tipo de grafo:
   - Direcionado ou não
   - Ponderado ou não

2. **Dijkstra** requer:
   - Grafo ponderado (`Ponderado = true`)
   - Preferência por grafos não-direcionados (embora possa funcionar com direcionados)
   - Todos os pesos devem ser positivos

3. **Carregamento de Arquivo**:
   - O arquivo deve ter o número correto de vértices e arestas
   - Os vértices são criados automaticamente como índices (0, 1, 2, ...)
   - Em grafos não-direcionados, as arestas são criadas em ambos os sentidos automaticamente

4. **Complexidade:**
   - BFS: O(V + A)
   - DFS: O(V + A)
   - Dijkstra: O(V²) com a implementação atual
