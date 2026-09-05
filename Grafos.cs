using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace TrabalhoM1.Grafos
{
    /// <summary>
    /// Contrato comum às duas representações de grafo solicitadas no trabalho.
    /// Os vértices são identificados externamente por índices entre 0 e
    /// QuantidadeVertices - 1.
    /// </summary>
    public abstract class Grafos
    {
        private readonly List<string> vertices = new List<string>();

        protected Grafos(bool direcionado, bool ponderado)
        {
            Direcionado = direcionado;
            Ponderado = ponderado;
        }

        public bool Direcionado { get; private set; }

        public bool Ponderado { get; private set; }

        public int QuantidadeVertices
        {
            get { return vertices.Count; }
        }

        public bool inserirVertice(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return false;
            }

            vertices.Add(label);
            AoInserirVertice();
            return true;
        }

        public bool removerVertice(int indice)
        {
            if (!IndiceValido(indice))
            {
                return false;
            }

            AoRemoverVertice(indice);
            vertices.RemoveAt(indice);
            return true;
        }

        public string labelVertice(int indice)
        {
            return IndiceValido(indice) ? vertices[indice] : null;
        }

        public abstract void imprimeGrafo();

        public abstract bool inserirAresta(int origem, int destino, float peso = 1f);

        public abstract bool removerAresta(int origem, int destino);

        public abstract bool existeAresta(int origem, int destino);

        public abstract float pesoAresta(int origem, int destino);

        public abstract List<int> retornarVizinhos(int vertice);

        /// <summary>
        /// Remove todos os vertices e arestas, deixando o grafo vazio.
        /// </summary>
        public void limpar()
        {
            vertices.Clear();
            AoLimpar();
        }

        /// <summary>
        /// Carrega um grafo a partir de um arquivo de texto.
        /// Formato: V A D P seguido de A linhas com arestas.
        /// O tipo do grafo (direcionado e ponderado) vem do proprio cabecalho do
        /// arquivo, e o conteudo anterior do grafo e descartado.
        /// </summary>
        public bool carregarDoArquivo(string caminhoArquivo)
        {
            try
            {
                var linhas = File.ReadAllLines(caminhoArquivo);
                if (linhas.Length == 0)
                    return false;

                var primeiraLinha = SepararCampos(linhas[0]);
                if (primeiraLinha.Length < 4)
                    return false;

                if (!int.TryParse(primeiraLinha[0], out int v) ||
                    !int.TryParse(primeiraLinha[1], out int a) ||
                    !int.TryParse(primeiraLinha[2], out int d) ||
                    !int.TryParse(primeiraLinha[3], out int p))
                    return false;

                if (v < 0 || a < 0)
                    return false;

                // O grafo assume o tipo declarado no arquivo.
                limpar();
                Direcionado = d == 1;
                Ponderado = p == 1;

                // Inserir vértices
                for (int i = 0; i < v; i++)
                {
                    inserirVertice(i.ToString());
                }

                // Inserir arestas: linhas em branco são ignoradas sem consumir uma aresta.
                int arestasLidas = 0;
                for (int i = 1; i < linhas.Length && arestasLidas < a; i++)
                {
                    var partes = SepararCampos(linhas[i]);
                    if (partes.Length == 0)
                        continue;

                    arestasLidas++;

                    if (partes.Length < 2)
                        continue;

                    if (!int.TryParse(partes[0], out int origem) ||
                        !int.TryParse(partes[1], out int destino))
                        continue;

                    float peso = 1f;
                    if (Ponderado && partes.Length > 2)
                    {
                        if (!float.TryParse(partes[2], NumberStyles.Float, CultureInfo.InvariantCulture, out peso))
                            peso = 1f;
                    }

                    inserirAresta(origem, destino, peso);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Quebra a linha em campos, tolerando espaços repetidos, tabulações e
        /// espaços no fim da linha.
        /// </summary>
        private static string[] SepararCampos(string linha)
        {
            return linha.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Busca em Largura (BFS) - retorna lista de vértices visitados a partir da origem
        /// </summary>
        public List<int> buscaLargura(int origem)
        {
            List<int> visitados = new List<int>();
            if (!IndiceValido(origem))
                return visitados;

            Queue<int> fila = new Queue<int>();
            bool[] visitado = new bool[QuantidadeVertices];

            fila.Enqueue(origem);
            visitado[origem] = true;

            while (fila.Count > 0)
            {
                int verticeAtual = fila.Dequeue();
                visitados.Add(verticeAtual);

                foreach (int vizinho in retornarVizinhos(verticeAtual))
                {
                    if (!visitado[vizinho])
                    {
                        visitado[vizinho] = true;
                        fila.Enqueue(vizinho);
                    }
                }
            }

            return visitados;
        }

        /// <summary>
        /// Busca em Profundidade (DFS) - retorna lista de vértices visitados a partir da origem
        /// </summary>
        public List<int> buscaProfundidade(int origem)
        {
            List<int> visitados = new List<int>();
            if (!IndiceValido(origem))
                return visitados;

            bool[] visitado = new bool[QuantidadeVertices];
            DFSRecursiva(origem, visitado, visitados);

            return visitados;
        }

        private void DFSRecursiva(int vertice, bool[] visitado, List<int> visitados)
        {
            visitado[vertice] = true;
            visitados.Add(vertice);

            foreach (int vizinho in retornarVizinhos(vertice))
            {
                if (!visitado[vizinho])
                {
                    DFSRecursiva(vizinho, visitado, visitados);
                }
            }
        }

        /// <summary>
        /// Algoritmo de Dijkstra - retorna menor distância e caminho para cada vértice
        /// Retorna um dicionário onde a chave é o vértice destino e o valor é uma tupla (distância, caminho)
        /// </summary>
        public Dictionary<int, (float distancia, List<int> caminho)> dijkstra(int origem)
        {
            var resultado = new Dictionary<int, (float, List<int>)>();
            
            if (!IndiceValido(origem) || !Ponderado)
                return resultado;

            float[] distancias = new float[QuantidadeVertices];
            int[] anterior = new int[QuantidadeVertices];
            bool[] visitado = new bool[QuantidadeVertices];

            // Inicializar distâncias e anteriores
            for (int i = 0; i < QuantidadeVertices; i++)
            {
                distancias[i] = float.MaxValue;
                anterior[i] = -1;
            }
            distancias[origem] = 0;

            // Algoritmo de Dijkstra
            for (int i = 0; i < QuantidadeVertices; i++)
            {
                // Encontrar vértice não visitado com menor distância
                int u = -1;
                float menorDist = float.MaxValue;

                for (int j = 0; j < QuantidadeVertices; j++)
                {
                    if (!visitado[j] && distancias[j] < menorDist)
                    {
                        u = j;
                        menorDist = distancias[j];
                    }
                }

                if (u == -1 || distancias[u] == float.MaxValue)
                    break;

                visitado[u] = true;

                // Relaxar arestas
                foreach (int v in retornarVizinhos(u))
                {
                    float peso = pesoAresta(u, v);
                    if (!visitado[v] && distancias[u] + peso < distancias[v])
                    {
                        distancias[v] = distancias[u] + peso;
                        anterior[v] = u;
                    }
                }
            }

            // Construir resultado com caminhos
            for (int i = 0; i < QuantidadeVertices; i++)
            {
                var caminho = new List<int>();
                if (distancias[i] != float.MaxValue)
                {
                    // Reconstruir caminho
                    int atual = i;
                    while (atual != -1)
                    {
                        caminho.Insert(0, atual);
                        atual = anterior[atual];
                    }
                }

                resultado[i] = (distancias[i], caminho);
            }

            return resultado;
        }

        protected bool IndicesValidos(int origem, int destino)
        {
            return IndiceValido(origem) && IndiceValido(destino);
        }

        protected bool IndiceValido(int indice)
        {
            return indice >= 0 && indice < vertices.Count;
        }

        protected bool TentarNormalizarPeso(float pesoInformado, out float peso)
        {
            peso = Ponderado ? pesoInformado : 1f;
            return !float.IsNaN(peso) && !float.IsInfinity(peso) && peso != 0f;
        }

        protected string CabecalhoVertice(int indice)
        {
            return indice + "[" + vertices[indice] + "]";
        }

        protected abstract void AoInserirVertice();

        protected abstract void AoLimpar();

        protected abstract void AoRemoverVertice(int indice);
    }
}
