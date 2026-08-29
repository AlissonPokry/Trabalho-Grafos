# Trabalho M1 - Parte 1

Implementação em C# das duas representações solicitadas:

- `GrafoMatriz`: matriz de adjacência;
- `GrafoLista`: lista de adjacência com a estrutura auxiliar `Aresta`;
- `Grafos`: classe base comum, configurada no construtor como direcionada ou não e ponderada ou não.

## Operações implementadas

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

## Observação sobre a direção das arestas

A página 76 diz que a ligação de volta deve ser adicionada no grafo direcionado. Isso contradiz a definição apresentada anteriormente e as páginas 69 e 77. A implementação segue a regra correta: a ligação de volta é criada e removida somente em grafos **não direcionados**.

## Executar

Com o SDK do .NET 8 instalado, dentro desta pasta:

```powershell
dotnet run
dotnet run -- --test
```

O primeiro comando mostra as duas representações do mesmo grafo. O segundo executa testes de inserção, remoção, reindexação, direção, peso, vizinhos e self-loop.
