using System;
using System.Collections.Generic;

namespace TrabalhoM1.Grafos
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--test")
            {
                return ExecutarTestes();
            }

            Console.WriteLine("MATRIZ - grafo não direcionado e ponderado");
            Grafos matriz = CriarExemplo(new GrafoMatriz(false, true));
            matriz.imprimeGrafo();

            Console.WriteLine();
            Console.WriteLine("LISTA - mesmo grafo");
            Grafos lista = CriarExemplo(new GrafoLista(false, true));
            lista.imprimeGrafo();
            return 0;
        }

        private static Grafos CriarExemplo(Grafos grafo)
        {
            grafo.inserirVertice("A");
            grafo.inserirVertice("B");
            grafo.inserirVertice("C");
            grafo.inserirAresta(0, 1, 2.5f);
            grafo.inserirAresta(1, 2, 4f);
            return grafo;
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
