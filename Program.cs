using System;
using System.Collections.Generic;
using System.IO;

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
                else if (args[0] == "--demo")
                {
                    DemonstraracaoBuscas();
                    return 0;
                }
            }

            MenuPrincipal();

            return 0;
        }

        // ------------------------------------------------------------------
        // Menu interativo: escolhe um arquivo .txt, carrega nas duas
        // representações e permite rodar os algoritmos sobre ele.
        // ------------------------------------------------------------------

        private const int LimiteImpressaoAutomatica = 30;

        private static void MenuPrincipal()
        {
            Console.WriteLine("=== TRABALHO M1 - GRAFOS ===\n");

            while (true)
            {
                List<string> arquivos = LocalizarArquivos();

                if (arquivos.Count == 0)
                {
                    Console.WriteLine("Nenhum arquivo .txt encontrado em " + Directory.GetCurrentDirectory());
                    Console.WriteLine("Coloque os arquivos de grafo na pasta do projeto e rode novamente.");
                    return;
                }

                Console.WriteLine("Arquivos encontrados:");
                for (int i = 0; i < arquivos.Count; i++)
                {
                    Console.WriteLine(string.Format("{0,3}) {1,-28} {2}",
                        i + 1,
                        CaminhoRelativo(arquivos[i]),
                        DescreverArquivo(arquivos[i])));
                }

                Console.WriteLine("  0) sair");

                int escolha = LerInteiro("\nArquivo: ", 0, arquivos.Count);
                if (escolha <= 0)
                {
                    Console.WriteLine("Até mais.");
                    return;
                }

                string caminho = arquivos[escolha - 1];

                Grafos matriz = new GrafoMatriz(false, false);
                Grafos lista = new GrafoLista(false, false);

                if (!matriz.carregarDoArquivo(caminho) || !lista.carregarDoArquivo(caminho))
                {
                    Console.WriteLine("\nNão foi possível carregar \"" + CaminhoRelativo(caminho) +
                                      "\". Verifique se a primeira linha está no formato: V A D P\n");
                    continue;
                }

                Console.WriteLine("\nGrafo carregado: " + CaminhoRelativo(caminho));
                ImprimirResumo(matriz);
                ImprimirRepresentacoes(matriz, lista);

                MenuGrafo(matriz, lista, caminho);
            }
        }

        private static void MenuGrafo(Grafos matriz, Grafos lista, string caminho)
        {
            while (true)
            {
                Console.WriteLine("\n--- " + CaminhoRelativo(caminho) + " ---");
                Console.WriteLine("1) Imprimir grafo (matriz e lista)");
                Console.WriteLine("2) Busca em largura (BFS)");
                Console.WriteLine("3) Busca em profundidade (DFS)");
                Console.WriteLine("4) Dijkstra");
                Console.WriteLine("5) Escolher outro arquivo");
                Console.WriteLine("0) Sair");

                int opcao = LerInteiro("Opção: ", 0, 5);

                switch (opcao)
                {
                    case 1:
                        ImprimirRepresentacoes(matriz, lista);
                        break;

                    case 2:
                    case 3:
                        int origemBusca = LerInteiro(
                            "Vértice de origem (0 a " + (matriz.QuantidadeVertices - 1) + "): ",
                            0, matriz.QuantidadeVertices - 1);
                        if (opcao == 2)
                        {
                            Console.WriteLine("MATRIZ - BFS: " + string.Join(", ", matriz.buscaLargura(origemBusca)));
                            Console.WriteLine("LISTA  - BFS: " + string.Join(", ", lista.buscaLargura(origemBusca)));
                        }
                        else
                        {
                            Console.WriteLine("MATRIZ - DFS: " + string.Join(", ", matriz.buscaProfundidade(origemBusca)));
                            Console.WriteLine("LISTA  - DFS: " + string.Join(", ", lista.buscaProfundidade(origemBusca)));
                        }
                        break;

                    case 4:
                        if (!matriz.Ponderado)
                        {
                            Console.WriteLine("Dijkstra exige um grafo ponderado; este arquivo não é ponderado.");
                            break;
                        }

                        int origemDijkstra = LerInteiro(
                            "Vértice de origem (0 a " + (matriz.QuantidadeVertices - 1) + "): ",
                            0, matriz.QuantidadeVertices - 1);
                        Console.WriteLine("\nMATRIZ - Dijkstra a partir de " + origemDijkstra + ":");
                        ImprimirResultadoDijkstra(matriz.dijkstra(origemDijkstra));
                        Console.WriteLine("\nLISTA - Dijkstra a partir de " + origemDijkstra + ":");
                        ImprimirResultadoDijkstra(lista.dijkstra(origemDijkstra));
                        break;

                    case 5:
                        Console.WriteLine();
                        return;

                    default:
                        Console.WriteLine("Até mais.");
                        Environment.Exit(0);
                        return;
                }
            }
        }

        /// <summary>
        /// Procura arquivos .txt no diretório atual e na subpasta "grafos".
        /// Se nada for encontrado, tenta a pasta do executável.
        /// </summary>
        private static List<string> LocalizarArquivos()
        {
            List<string> encontrados = ProcurarEm(Directory.GetCurrentDirectory());

            if (encontrados.Count == 0)
            {
                encontrados = ProcurarEm(AppContext.BaseDirectory);
            }

            return encontrados;
        }

        private static List<string> ProcurarEm(string raiz)
        {
            List<string> encontrados = new List<string>();

            try
            {
                if (!Directory.Exists(raiz))
                {
                    return encontrados;
                }

                string[] naRaiz = Directory.GetFiles(raiz, "*.txt");
                Array.Sort(naRaiz, StringComparer.OrdinalIgnoreCase);
                encontrados.AddRange(naRaiz);

                string subpasta = Path.Combine(raiz, "grafos");
                if (Directory.Exists(subpasta))
                {
                    string[] naSubpasta = Directory.GetFiles(subpasta, "*.txt");
                    Array.Sort(naSubpasta, StringComparer.OrdinalIgnoreCase);
                    encontrados.AddRange(naSubpasta);
                }
            }
            catch (Exception)
            {
                // diretório inacessível: segue com o que já foi encontrado
            }

            return encontrados;
        }

        /// <summary>
        /// Lê a primeira linha do arquivo e descreve o grafo (V, A e tipo).
        /// </summary>
        private static string DescreverArquivo(string caminho)
        {
            try
            {
                using (StreamReader leitor = new StreamReader(caminho))
                {
                    string primeira = leitor.ReadLine();
                    if (primeira == null)
                    {
                        return "(arquivo vazio)";
                    }

                    string[] campos = primeira.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (campos.Length < 4 ||
                        !int.TryParse(campos[0], out int v) ||
                        !int.TryParse(campos[1], out int a) ||
                        !int.TryParse(campos[2], out int d) ||
                        !int.TryParse(campos[3], out int p))
                    {
                        return "(formato não reconhecido)";
                    }

                    return string.Format("{0,4} vértices, {1,5} arestas  [{2}]", v, a, DescreverTipo(d == 1, p == 1));
                }
            }
            catch (Exception)
            {
                return "(não foi possível ler)";
            }
        }

        private static string DescreverTipo(bool direcionado, bool ponderado)
        {
            if (direcionado && ponderado) return "direcionado e ponderado";
            if (direcionado) return "direcionado";
            if (ponderado) return "ponderado";
            return "simples";
        }

        private static string CaminhoRelativo(string caminho)
        {
            try
            {
                return Path.GetRelativePath(Directory.GetCurrentDirectory(), caminho);
            }
            catch (Exception)
            {
                return caminho;
            }
        }

        private static void ImprimirResumo(Grafos grafo)
        {
            Console.WriteLine("Vértices: " + grafo.QuantidadeVertices +
                              " | Tipo: " + DescreverTipo(grafo.Direcionado, grafo.Ponderado));
        }

        /// <summary>
        /// Imprime a matriz de adjacência e a lista de adjacência. Grafos grandes
        /// só são impressos após confirmação, para não inundar a tela.
        /// </summary>
        private static void ImprimirRepresentacoes(Grafos matriz, Grafos lista)
        {
            if (matriz.QuantidadeVertices > LimiteImpressaoAutomatica)
            {
                Console.Write("O grafo tem " + matriz.QuantidadeVertices +
                              " vértices e a matriz vai ocupar muitas linhas. Imprimir mesmo assim? (s/N): ");
                string resposta = Console.ReadLine();
                if (resposta == null || !resposta.Trim().StartsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Impressão do grafo ignorada.");
                    return;
                }
            }

            Console.WriteLine("\nMATRIZ DE ADJACÊNCIA:");
            matriz.imprimeGrafo();

            Console.WriteLine("\nLISTA DE ADJACÊNCIA:");
            lista.imprimeGrafo();
        }

        /// <summary>
        /// Lê um inteiro entre minimo e maximo, repetindo a pergunta em caso de
        /// entrada inválida. Devolve o mínimo se a entrada terminar (Ctrl+D).
        /// </summary>
        private static int LerInteiro(string pergunta, int minimo, int maximo)
        {
            while (true)
            {
                Console.Write(pergunta);
                string entrada = Console.ReadLine();

                if (entrada == null)
                {
                    Console.WriteLine();
                    return minimo;
                }

                if (int.TryParse(entrada.Trim(), out int valor) && valor >= minimo && valor <= maximo)
                {
                    return valor;
                }

                Console.WriteLine("Entrada inválida. Informe um número entre " + minimo + " e " + maximo + ".");
            }
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
            Exigir(grafo.inserirAresta(0, 2, 0f), "ponderado deve aceitar peso zero");
            Exigir(grafo.existeAresta(0, 2) && grafo.pesoAresta(0, 2) == 0f, "aresta de peso zero deve existir");
            Exigir(grafo.removerAresta(0, 2), "deve remover aresta de peso zero");
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
            Exigir(grafo.inserirAresta(1, 0, 0f) && grafo.pesoAresta(1, 0) == 1f,
                "não ponderado não deve armazenar peso zero");
            Exigir(grafo.removerAresta(1, 0), "deve remover arco de retorno do teste");
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
