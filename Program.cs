using System;
using System.Collections.Generic;

namespace TrabalhoM1.Grafos
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--test")
                {
                    return ExecutarTestes();
                }
                else if (args[0] == "--teste-parte2" || args[0] == "--teste-completo")
                {
                    TesteParte2.ExecutarTodosTestes();
                    return 0;
                }
            }

            DemonstraracaoBuscas();
            
            return 0;
        }

        private static void DemonstraracaoBuscas()
        {
            Console.WriteLine("=== DEMONSTRAÇÃO DE BUSCAS E DIJKSTRA ===\n");

            // Teste 1: Busca em Largura e Profundidade
            Console.WriteLine("--- Teste 1: Grafo Não-Direcionado (BFS e DFS) ---");
            Grafos grafo1Matriz = new GrafoMatriz(false, false);
            Grafos grafo1Lista = new GrafoLista(false, false);

            for (int i = 0; i < 5; i++)
            {
                grafo1Matriz.inserirVertice(i.ToString());
                grafo1Lista.inserirVertice(i.ToString());
            }

            int[][] arestas1 = new int[][] 
            {
                new int[] { 0, 1 },
                new int[] { 0, 2 },
                new int[] { 1, 2 },
                new int[] { 1, 3 },
                new int[] { 2, 3 },
                new int[] { 3, 4 }
            };

            foreach (var aresta in arestas1)
            {
                grafo1Matriz.inserirAresta(aresta[0], aresta[1]);
                grafo1Lista.inserirAresta(aresta[0], aresta[1]);
            }

            Console.WriteLine("MATRIZ:");
            Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafo1Matriz.buscaLargura(0)));
            Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafo1Matriz.buscaProfundidade(0)));

            Console.WriteLine("\nLISTA:");
            Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafo1Lista.buscaLargura(0)));
            Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafo1Lista.buscaProfundidade(0)));

            // Teste 2: Grafo Direcionado
            Console.WriteLine("\n--- Teste 2: Grafo Direcionado (BFS e DFS) ---");
            Grafos grafo2Matriz = new GrafoMatriz(true, false);
            Grafos grafo2Lista = new GrafoLista(true, false);

            for (int i = 0; i < 5; i++)
            {
                grafo2Matriz.inserirVertice(i.ToString());
                grafo2Lista.inserirVertice(i.ToString());
            }

            int[][] arestas2 = new int[][]
            {
                new int[] { 0, 1 },
                new int[] { 0, 2 },
                new int[] { 1, 2 },
                new int[] { 1, 3 },
                new int[] { 2, 3 },
                new int[] { 3, 4 },
                new int[] { 2, 4 }
            };

            foreach (var aresta in arestas2)
            {
                grafo2Matriz.inserirAresta(aresta[0], aresta[1]);
                grafo2Lista.inserirAresta(aresta[0], aresta[1]);
            }

            Console.WriteLine("MATRIZ:");
            Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafo2Matriz.buscaLargura(0)));
            Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafo2Matriz.buscaProfundidade(0)));

            Console.WriteLine("\nLISTA:");
            Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafo2Lista.buscaLargura(0)));
            Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafo2Lista.buscaProfundidade(0)));

            // Teste 3: Dijkstra com Grafo Ponderado
            Console.WriteLine("\n--- Teste 3: Algoritmo de Dijkstra (Grafo Ponderado) ---");
            Grafos grafo3Matriz = new GrafoMatriz(false, true);
            Grafos grafo3Lista = new GrafoLista(false, true);

            for (int i = 0; i < 6; i++)
            {
                grafo3Matriz.inserirVertice(i.ToString());
                grafo3Lista.inserirVertice(i.ToString());
            }

            (int origem, int destino, float peso)[] arestas3 = new[]
            {
                (0, 1, 4f),
                (0, 2, 2f),
                (1, 2, 1f),
                (1, 3, 5f),
                (2, 3, 8f),
                (2, 4, 10f),
                (3, 4, 2f),
                (3, 5, 6f),
                (4, 5, 3f)
            };

            foreach (var (orig, dest, p) in arestas3)
            {
                grafo3Matriz.inserirAresta(orig, dest, p);
                grafo3Lista.inserirAresta(orig, dest, p);
            }

            Console.WriteLine("MATRIZ - Dijkstra a partir de 0:");
            ImprimirResultadoDijkstra(grafo3Matriz.dijkstra(0));

            Console.WriteLine("\nLISTA - Dijkstra a partir de 0:");
            ImprimirResultadoDijkstra(grafo3Lista.dijkstra(0));

            // Teste 4: Carregar de arquivo
            Console.WriteLine("\n--- Teste 4: Carregar Grafo de Arquivo ---");
            try
            {
                Grafos grafoArquivoMatriz = new GrafoMatriz(false, false);
                if (grafoArquivoMatriz.carregarDoArquivo("grafo_simples.txt"))
                {
                    Console.WriteLine("Grafo carregado com sucesso da MATRIZ!");
                    Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafoArquivoMatriz.buscaLargura(0)));
                    Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafoArquivoMatriz.buscaProfundidade(0)));
                }
                else
                {
                    Console.WriteLine("Erro ao carregar grafo na MATRIZ");
                }

                Grafos grafoArquivoLista = new GrafoLista(false, false);
                if (grafoArquivoLista.carregarDoArquivo("grafo_simples.txt"))
                {
                    Console.WriteLine("\nGrafo carregado com sucesso da LISTA!");
                    Console.WriteLine("BFS começando de 0: " + string.Join(", ", grafoArquivoLista.buscaLargura(0)));
                    Console.WriteLine("DFS começando de 0: " + string.Join(", ", grafoArquivoLista.buscaProfundidade(0)));
                }
                else
                {
                    Console.WriteLine("Erro ao carregar grafo na LISTA");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao carregar arquivo: " + ex.Message);
            }
        }

        private static void ImprimirResultadoDijkstra(Dictionary<int, (float distancia, List<int> caminho)> resultado)
        {
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
        }

        private static int ExecutarTestes()
        {
            try
            {
                TestarRepresentacao(new GrafoMatriz(false, true));
                TestarRepresentacao(new GrafoLista(false, true));
                TestarDirecionado(new GrafoMatriz(true, false));
                TestarDirecionado(new GrafoLista(true, false));
                TestarLaco(new GrafoMatriz(false, false));
                TestarLaco(new GrafoLista(false, false));
                Console.WriteLine("Todos os testes passaram.");
                return 0;
            }
            catch (Exception erro)
            {
                Console.Error.WriteLine("Falha: " + erro.Message);
                return 1;
            }
        }

        private static void TestarRepresentacao(Grafos grafo)
        {
            Exigir(grafo.inserirVertice("A"), "deve inserir A");
            Exigir(grafo.inserirVertice("B"), "deve inserir B");
            Exigir(grafo.inserirVertice("C"), "deve inserir C");
            Exigir(!grafo.inserirVertice(" "), "não deve inserir label vazio");
            Exigir(grafo.inserirAresta(0, 1, -2.5f), "deve aceitar peso não nulo");
            Exigir(grafo.inserirAresta(1, 2, 4f), "deve inserir segunda aresta");
            Exigir(grafo.existeAresta(1, 0), "não direcionado deve criar retorno");
            Exigir(grafo.pesoAresta(1, 0) == -2.5f, "deve preservar o peso");
            Exigir(!grafo.inserirAresta(0, 1, 8f), "não deve duplicar aresta");
            Exigir(!grafo.inserirAresta(0, 2, 0f), "peso zero deve ser inválido");
            Exigir(SequenciaIgual(grafo.retornarVizinhos(1), 0, 2), "vizinhos incorretos");
            Exigir(grafo.removerVertice(1), "deve remover vértice");
            Exigir(grafo.QuantidadeVertices == 2, "quantidade após remoção incorreta");
            Exigir(grafo.labelVertice(1) == "C", "índices devem ser reorganizados");
            Exigir(!grafo.existeAresta(0, 1), "arestas incidentes devem ser removidas");
            Exigir(grafo.labelVertice(99) == null, "índice inválido deve retornar null");
            Exigir(grafo.pesoAresta(0, 1) == 0f, "aresta inexistente deve ter peso zero");
        }

        private static void TestarDirecionado(Grafos grafo)
        {
            grafo.inserirVertice("A");
            grafo.inserirVertice("B");
            Exigir(grafo.inserirAresta(0, 1, 99f), "deve inserir arco");
            Exigir(grafo.pesoAresta(0, 1) == 1f, "não ponderado deve usar peso um");
            Exigir(!grafo.existeAresta(1, 0), "direcionado não deve criar retorno");
            Exigir(grafo.removerAresta(0, 1), "deve remover arco");
            Exigir(!grafo.removerAresta(0, 1), "não deve remover arco inexistente");
        }

        private static void TestarLaco(Grafos grafo)
        {
            grafo.inserirVertice("A");
            Exigir(grafo.inserirAresta(0, 0), "deve inserir self-loop");
            Exigir(SequenciaIgual(grafo.retornarVizinhos(0), 0), "self-loop não deve duplicar");
            Exigir(grafo.removerAresta(0, 0), "deve remover self-loop");
        }

        private static bool SequenciaIgual(List<int> atual, params int[] esperada)
        {
            if (atual.Count != esperada.Length)
            {
                return false;
            }

            for (int indice = 0; indice < atual.Count; indice++)
            {
                if (atual[indice] != esperada[indice])
                {
                    return false;
                }
            }

            return true;
        }

        private static void Exigir(bool condicao, string mensagem)
        {
            if (!condicao)
            {
                throw new InvalidOperationException(mensagem);
            }
        }
    }
}
