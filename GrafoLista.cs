using System;
using System.Collections.Generic;
using System.Globalization;

namespace TrabalhoM1.Grafos
{
    public struct Aresta
    {
        public Aresta(int destino, float peso)
            : this()
        {
            Destino = destino;
            Peso = peso;
        }

        public int Destino { get; private set; }

        public float Peso { get; private set; }
    }

    public sealed class GrafoLista : Grafos
    {
        private readonly List<List<Aresta>> adjacencias = new List<List<Aresta>>();

        public GrafoLista(bool direcionado, bool ponderado)
            : base(direcionado, ponderado)
        {
        }

        public override bool inserirAresta(int origem, int destino, float peso = 1f)
        {
            float pesoNormalizado;
            if (!IndicesValidos(origem, destino) ||
                !TentarNormalizarPeso(peso, out pesoNormalizado) ||
                IndiceDaAresta(origem, destino) >= 0)
            {
                return false;
            }

            adjacencias[origem].Add(new Aresta(destino, pesoNormalizado));
            if (!Direcionado && origem != destino)
            {
                adjacencias[destino].Add(new Aresta(origem, pesoNormalizado));
            }

            return true;
        }

        public override bool removerAresta(int origem, int destino)
        {
            if (!IndicesValidos(origem, destino))
            {
                return false;
            }

            int indiceAresta = IndiceDaAresta(origem, destino);
            if (indiceAresta < 0)
            {
                return false;
            }

            adjacencias[origem].RemoveAt(indiceAresta);
            if (!Direcionado && origem != destino)
            {
                int indiceRetorno = IndiceDaAresta(destino, origem);
                if (indiceRetorno >= 0)
                {
                    adjacencias[destino].RemoveAt(indiceRetorno);
                }
            }

            return true;
        }

        public override bool existeAresta(int origem, int destino)
        {
            return IndicesValidos(origem, destino) && IndiceDaAresta(origem, destino) >= 0;
        }

        public override float pesoAresta(int origem, int destino)
        {
            if (!IndicesValidos(origem, destino))
            {
                return 0f;
            }

            int indiceAresta = IndiceDaAresta(origem, destino);
            return indiceAresta >= 0 ? adjacencias[origem][indiceAresta].Peso : 0f;
        }

        public override List<int> retornarVizinhos(int vertice)
        {
            List<int> vizinhos = new List<int>();
            if (!IndiceValido(vertice))
            {
                return vizinhos;
            }

            foreach (Aresta aresta in adjacencias[vertice])
            {
                vizinhos.Add(aresta.Destino);
            }

            return vizinhos;
        }

        public override void imprimeGrafo()
        {
            for (int origem = 0; origem < QuantidadeVertices; origem++)
            {
                Console.Write(CabecalhoVertice(origem) + ":");
                foreach (Aresta aresta in adjacencias[origem])
                {
                    Console.Write(" " + aresta.Destino);
                    if (Ponderado)
                    {
                        Console.Write("(" + aresta.Peso.ToString("0.###", CultureInfo.InvariantCulture) + ")");
                    }
                }

                Console.WriteLine();
            }
        }

        protected override void AoInserirVertice()
        {
            adjacencias.Add(new List<Aresta>());
        }

        protected override void AoLimpar()
        {
            adjacencias.Clear();
        }

        protected override void AoRemoverVertice(int indice)
        {
            adjacencias.RemoveAt(indice);

            for (int origem = 0; origem < adjacencias.Count; origem++)
            {
                for (int posicao = adjacencias[origem].Count - 1; posicao >= 0; posicao--)
                {
                    Aresta aresta = adjacencias[origem][posicao];
                    if (aresta.Destino == indice)
                    {
                        adjacencias[origem].RemoveAt(posicao);
                    }
                    else if (aresta.Destino > indice)
                    {
                        adjacencias[origem][posicao] = new Aresta(aresta.Destino - 1, aresta.Peso);
                    }
                }
            }
        }

        private int IndiceDaAresta(int origem, int destino)
        {
            for (int indice = 0; indice < adjacencias[origem].Count; indice++)
            {
                if (adjacencias[origem][indice].Destino == destino)
                {
                    return indice;
                }
            }

            return -1;
        }
    }
}
