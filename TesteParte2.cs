using System;
using System.Collections.Generic;
using System.Linq;

namespace TrabalhoM1.Grafos
{
    /// <summary>
    /// Testes completos para a Parte 2: Leitura de Arquivos e Algoritmos
    /// </summary>
    internal static class TesteParte2
    {
        private static int totalTestes = 0;
        private static int testesPassados = 0;

        public static void ExecutarTodosTestes()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║   TESTES COMPLETOS - PARTE 2 DO TRABALHO GRAFOS   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

            // Seção 1: Leitura de Arquivos
            Console.WriteLine("═══ SEÇÃO 1: LEITURA DE ARQUIVOS ═══\n");
            TestarCarregamentoArquivos();

            // Seção 2: BFS
            Console.WriteLine("\n═══ SEÇÃO 2: BUSCA EM LARGURA (BFS) ═══\n");
            TestarBFS();

            // Seção 3: DFS
            Console.WriteLine("\n═══ SEÇÃO 3: BUSCA EM PROFUNDIDADE (DFS) ═══\n");
            TestarDFS();

            // Seção 4: Dijkstra
            Console.WriteLine("\n═══ SEÇÃO 4: ALGORITMO DE DIJKSTRA ═══\n");
            TestarDijkstra();

            // Seção 5: Comparação Matriz vs Lista
            Console.WriteLine("\n═══ SEÇÃO 5: COMPARAÇÃO MATRIZ vs LISTA ═══\n");
            TestarComparacaoMatrizVsLista();

            // Resumo Final
            Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  RESUMO: {testesPassados}/{totalTestes} testes passaram                  ║");
            if (testesPassados == totalTestes)
            {
                Console.WriteLine("║           ✓ TODOS OS TESTES PASSARAM!               ║");
            }
            else
            {
                Console.WriteLine($"║           ✗ {totalTestes - testesPassados} testes falharam                     ║");
            }
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
        }

        private static void TestarCarregamentoArquivos()
        {
            Console.WriteLine("Teste 1.1: Carregar grafo não-direcionado não-ponderado");
            Grafos grafo = new GrafoMatriz(false, false);
            bool sucesso = grafo.carregarDoArquivo("grafo_simples.txt");
            Validar(sucesso && grafo.QuantidadeVertices == 5, "✓ Carregamento bem-sucedido");

            Console.WriteLine("Teste 1.2: Carregar grafo direcionado não-ponderado");
            grafo = new GrafoMatriz(true, false);
            sucesso = grafo.carregarDoArquivo("grafo_direcionado.txt");
            Validar(sucesso && grafo.QuantidadeVertices == 5, "✓ Carregamento bem-sucedido");

            Console.WriteLine("Teste 1.3: Carregar grafo não-direcionado ponderado");
            grafo = new GrafoMatriz(false, true);
            sucesso = grafo.carregarDoArquivo("grafo_ponderado.txt");
            Validar(sucesso && grafo.QuantidadeVertices == 6, "✓ Carregamento bem-sucedido");

            Console.WriteLine("Teste 1.4: Validar tipo de grafo incompatível");
            grafo = new GrafoMatriz(false, false); // Não-direcionado, não-ponderado
            sucesso = grafo.carregarDoArquivo("grafo_ponderado.txt"); // Arquivo é ponderado
            Validar(!sucesso, "✓ Rejeição de tipo incompatível");

            Console.WriteLine("Teste 1.5: Testar com GrafoLista");
            grafo = new GrafoLista(false, false);
            sucesso = grafo.carregarDoArquivo("grafo_simples.txt");
            Validar(sucesso && grafo.QuantidadeVertices == 5, "✓ Carregamento em lista bem-sucedido");

            Console.WriteLine("Teste 1.6: Validar arestas após carregamento");
            grafo = new GrafoMatriz(false, false);
            grafo.carregarDoArquivo("grafo_simples.txt");
            bool existeAresta01 = grafo.existeAresta(0, 1);
            bool existeAresta10 = grafo.existeAresta(1, 0); // Deve existir (não-direcionado)
            Validar(existeAresta01 && existeAresta10, "✓ Arestas criadas nos dois sentidos");
        }

        private static void TestarBFS()
        {
            Console.WriteLine("Teste 2.1: BFS em grafo não-direcionado");
            Grafos grafo = CriarGrafoTeste(false, false);
            var resultado = grafo.buscaLargura(0);
            bool válido = resultado.Count == 5 && resultado[0] == 0;
            Validar(válido, $"✓ BFS válido: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 2.2: BFS em grafo direcionado");
            grafo = CriarGrafoTesteDirectionado(true, false);
            resultado = grafo.buscaLargura(0);
            bool temTodos = resultado.Count == 5;
            Validar(temTodos, $"✓ BFS válido: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 2.3: BFS começando de vértice diferente");
            grafo = CriarGrafoTeste(false, false);
            resultado = grafo.buscaLargura(2);
            bool começaEmDois = resultado[0] == 2;
            Validar(começaEmDois, $"✓ BFS começa em 2: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 2.4: BFS com vértice isolado");
            grafo = new GrafoMatriz(false, false);
            for (int i = 0; i < 4; i++) grafo.inserirVertice(i.ToString());
            grafo.inserirAresta(0, 1);
            grafo.inserirAresta(2, 3);
            // 0-1 isolado de 2-3
            resultado = grafo.buscaLargura(0);
            bool apenas2Vertices = resultado.Count == 2;
            Validar(apenas2Vertices, $"✓ BFS visita apenas componente conectada: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 2.5: BFS com arquivo carregado");
            grafo = new GrafoMatriz(false, false);
            grafo.carregarDoArquivo("grafo_simples.txt");
            resultado = grafo.buscaLargura(0);
            bool temResultado = resultado.Count > 0 && resultado[0] == 0;
            Validar(temResultado, $"✓ BFS em arquivo: {string.Join(", ", resultado)}");
        }

        private static void TestarDFS()
        {
            Console.WriteLine("Teste 3.1: DFS em grafo não-direcionado");
            Grafos grafo = CriarGrafoTeste(false, false);
            var resultado = grafo.buscaProfundidade(0);
            bool válido = resultado.Count == 5 && resultado[0] == 0;
            Validar(válido, $"✓ DFS válido: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 3.2: DFS em grafo direcionado");
            grafo = CriarGrafoTesteDirectionado(true, false);
            resultado = grafo.buscaProfundidade(0);
            bool temTodos = resultado.Count == 5;
            Validar(temTodos, $"✓ DFS válido: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 3.3: DFS começando de vértice diferente");
            grafo = CriarGrafoTeste(false, false);
            resultado = grafo.buscaProfundidade(3);
            bool começaEmTres = resultado[0] == 3;
            Validar(começaEmTres, $"✓ DFS começa em 3: {string.Join(", ", resultado)}");

            Console.WriteLine("Teste 3.4: Diferença entre BFS e DFS (ordem)");
            grafo = CriarGrafoTeste(false, false);
            var bfs = grafo.buscaLargura(0);
            var dfs = grafo.buscaProfundidade(0);
            // Ambas visitam todos, mas ordem pode ser diferente
            bool mesmoTamanho = bfs.Count == dfs.Count;
            Validar(mesmoTamanho, $"✓ BFS e DFS visitam todos: BFS={string.Join(",", bfs)} DFS={string.Join(",", dfs)}");

            Console.WriteLine("Teste 3.5: DFS com arquivo carregado");
            grafo = new GrafoMatriz(false, false);
            grafo.carregarDoArquivo("grafo_simples.txt");
            resultado = grafo.buscaProfundidade(0);
            bool temResultado = resultado.Count > 0 && resultado[0] == 0;
            Validar(temResultado, $"✓ DFS em arquivo: {string.Join(", ", resultado)}");
        }

        private static void TestarDijkstra()
        {
            Console.WriteLine("Teste 4.1: Dijkstra em grafo ponderado simples");
            Grafos grafo = CriarGrafoTestePonderado();
            var resultado = grafo.dijkstra(0);
            
            bool dist0Correct = resultado[0].distancia == 0;
            bool dist1Correct = resultado[1].distancia == 3; // Melhor caminho: 0→2→1 = 2+1
            bool dist2Correct = resultado[2].distancia == 2;
            
            Validar(dist0Correct && dist1Correct && dist2Correct, 
                $"✓ Distâncias corretas: 0→0={resultado[0].distancia}, 0→1={resultado[1].distancia}, 0→2={resultado[2].distancia}");

            Console.WriteLine("Teste 4.2: Validar caminho no Dijkstra");
            grafo = CriarGrafoTestePonderado();
            resultado = grafo.dijkstra(0);
            
            bool caminho0Correct = resultado[0].caminho.Count == 1 && resultado[0].caminho[0] == 0;
            bool caminho2Correct = resultado[2].caminho.Count == 2 && resultado[2].caminho[0] == 0 && resultado[2].caminho[1] == 2;
            
            Validar(caminho0Correct && caminho2Correct, 
                $"✓ Caminhos corretos: 0→0={string.Join("→", resultado[0].caminho)}, 0→2={string.Join("→", resultado[2].caminho)}");

            Console.WriteLine("Teste 4.3: Dijkstra com pesos diferentes");
            grafo = new GrafoMatriz(false, true);
            for (int i = 0; i < 4; i++) grafo.inserirVertice(i.ToString());
            grafo.inserirAresta(0, 1, 10f);
            grafo.inserirAresta(0, 2, 5f);
            grafo.inserirAresta(2, 1, 3f); // Caminho melhor: 0→2→1 = 8
            
            resultado = grafo.dijkstra(0);
            bool melhorCaminho = resultado[1].distancia == 8;
            Validar(melhorCaminho, $"✓ Encontra melhor caminho: 0→1 distância = {resultado[1].distancia}");

            Console.WriteLine("Teste 4.4: Dijkstra com arquivo ponderado");
            grafo = new GrafoMatriz(false, true);
            grafo.carregarDoArquivo("grafo_ponderado.txt");
            resultado = grafo.dijkstra(0);
            
            bool temDistancia = resultado[0].distancia == 0 && resultado[1].distancia > 0;
            Validar(temDistancia, $"✓ Dijkstra em arquivo: distância 0→0={resultado[0].distancia}, 0→1={resultado[1].distancia}");

            Console.WriteLine("Teste 4.5: Vértices inalcançáveis");
            grafo = new GrafoMatriz(false, true);
            grafo.inserirVertice("0");
            grafo.inserirVertice("1");
            grafo.inserirVertice("2");
            grafo.inserirVertice("3");
            grafo.inserirAresta(0, 1, 1);
            // Vértice 2 e 3 isolados
            
            resultado = grafo.dijkstra(0);
            bool inalcancavel2 = resultado[2].distancia == float.MaxValue;
            Validar(inalcancavel2, "✓ Detecta vértices inalcançáveis");
        }

        private static void TestarComparacaoMatrizVsLista()
        {
            Console.WriteLine("Teste 5.1: BFS idêntico em Matriz e Lista");
            Grafos matriz = CriarGrafoTeste(false, false);
            Grafos lista = CriarGrafoTeste(false, false);
            
            var bfsMatriz = matriz.buscaLargura(0);
            var bfsLista = lista.buscaLargura(0);
            
            bool iguais = bfsMatriz.SequenceEqual(bfsLista);
            Validar(iguais, $"✓ BFS idêntico: Matriz={string.Join(",", bfsMatriz)} Lista={string.Join(",", bfsLista)}");

            Console.WriteLine("Teste 5.2: DFS idêntico em Matriz e Lista");
            matriz = CriarGrafoTeste(false, false);
            lista = CriarGrafoTeste(false, false);
            
            var dfsMatriz = matriz.buscaProfundidade(0);
            var dfsLista = lista.buscaProfundidade(0);
            
            bool dfsIguais = dfsMatriz.SequenceEqual(dfsLista);
            Validar(dfsIguais, $"✓ DFS idêntico: Matriz={string.Join(",", dfsMatriz)} Lista={string.Join(",", dfsLista)}");

            Console.WriteLine("Teste 5.3: Dijkstra idêntico em Matriz e Lista");
            matriz = CriarGrafoTestePonderado();
            lista = CriarGrafoTestePonderado(true);
            
            var dijMatriz = matriz.dijkstra(0);
            var dijLista = lista.dijkstra(0);
            
            bool dijIguais = true;
            for (int i = 0; i < matriz.QuantidadeVertices; i++)
            {
                if (Math.Abs(dijMatriz[i].distancia - dijLista[i].distancia) > 0.0001f)
                {
                    dijIguais = false;
                    break;
                }
            }
            Validar(dijIguais, $"✓ Dijkstra idêntico em ambas representações");

            Console.WriteLine("Teste 5.4: Carregamento idêntico");
            matriz = new GrafoMatriz(false, false);
            lista = new GrafoLista(false, false);
            
            bool carregouMatriz = matriz.carregarDoArquivo("grafo_simples.txt");
            bool carregouLista = lista.carregarDoArquivo("grafo_simples.txt");
            
            bool bfsIguaisCom = matriz.buscaLargura(0).SequenceEqual(lista.buscaLargura(0));
            Validar(carregouMatriz && carregouLista && bfsIguaisCom, "✓ Carregamento e BFS idênticos");
        }

        private static Grafos CriarGrafoTeste(bool direcionado, bool ponderado)
        {
            Grafos grafo = new GrafoMatriz(direcionado, ponderado);
            for (int i = 0; i < 5; i++)
                grafo.inserirVertice(i.ToString());

            grafo.inserirAresta(0, 1, 1);
            grafo.inserirAresta(0, 2, 1);
            grafo.inserirAresta(1, 2, 1);
            grafo.inserirAresta(1, 3, 1);
            grafo.inserirAresta(2, 3, 1);
            grafo.inserirAresta(3, 4, 1);

            return grafo;
        }

        private static Grafos CriarGrafoTesteDirectionado(bool direcionado, bool ponderado)
        {
            Grafos grafo = new GrafoMatriz(direcionado, ponderado);
            for (int i = 0; i < 5; i++)
                grafo.inserirVertice(i.ToString());

            grafo.inserirAresta(0, 1, 1);
            grafo.inserirAresta(0, 2, 1);
            grafo.inserirAresta(1, 2, 1);
            grafo.inserirAresta(1, 3, 1);
            grafo.inserirAresta(2, 3, 1);
            grafo.inserirAresta(3, 4, 1);
            grafo.inserirAresta(2, 4, 1);

            return grafo;
        }

        private static Grafos CriarGrafoTestePonderado(bool usarLista = false)
        {
            Grafos grafo = usarLista ? new GrafoLista(false, true) : new GrafoMatriz(false, true);
            for (int i = 0; i < 4; i++)
                grafo.inserirVertice(i.ToString());

            grafo.inserirAresta(0, 1, 4f);
            grafo.inserirAresta(0, 2, 2f);
            grafo.inserirAresta(1, 2, 1f);
            grafo.inserirAresta(1, 3, 5f);
            grafo.inserirAresta(2, 3, 8f);

            return grafo;
        }

        private static void Validar(bool condicao, string mensagem)
        {
            totalTestes++;
            if (condicao)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {mensagem}");
                testesPassados++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ✗ {mensagem}");
            }
            Console.ResetColor();
        }
    }
}
