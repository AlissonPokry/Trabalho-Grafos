using System;
using System.Collections.Generic;
using System.Globalization;

namespace TrabalhoM1.Grafos
{
    public sealed class GrafoMatriz : Grafos
    {
        // null significa ausência de aresta. Assim, a representação não confunde
        // uma posição vazia com os pesos positivos ou negativos aceitos.
        private float?[,] matriz = new float?[0, 0];

        public GrafoMatriz(bool direcionado, bool ponderado)
            : base(direcionado, ponderado)
        {
        }

        public override bool inserirAresta(int origem, int destino, float peso = 1f)
        {
            float pesoNormalizado;
            if (!IndicesValidos(origem, destino) ||
                !TentarNormalizarPeso(peso, out pesoNormalizado) ||
                matriz[origem, destino].HasValue)
            {
                return false;
            }

            matriz[origem, destino] = pesoNormalizado;
            if (!Direcionado)
            {
                matriz[destino, origem] = pesoNormalizado;
            }

            return true;
        }

        public override bool removerAresta(int origem, int destino)
        {
            if (!IndicesValidos(origem, destino) || !matriz[origem, destino].HasValue)
            {
                return false;
            }

            matriz[origem, destino] = null;
            if (!Direcionado)
            {
                matriz[destino, origem] = null;
            }

            return true;
        }

        public override bool existeAresta(int origem, int destino)
        {
            return IndicesValidos(origem, destino) && matriz[origem, destino].HasValue;
        }

        public override float pesoAresta(int origem, int destino)
        {
            return existeAresta(origem, destino) ? matriz[origem, destino].Value : 0f;
        }

        public override List<int> retornarVizinhos(int vertice)
        {
            List<int> vizinhos = new List<int>();
            if (!IndiceValido(vertice))
            {
                return vizinhos;
            }

            for (int destino = 0; destino < QuantidadeVertices; destino++)
            {
                if (matriz[vertice, destino].HasValue)
                {
                    vizinhos.Add(destino);
                }
            }

            return vizinhos;
        }

        public override void imprimeGrafo()
        {
            Console.Write("       ");
            for (int coluna = 0; coluna < QuantidadeVertices; coluna++)
            {
                Console.Write(string.Format(CultureInfo.InvariantCulture, "{0,8}", coluna));
            }

            Console.WriteLine();
            for (int linha = 0; linha < QuantidadeVertices; linha++)
            {
                Console.Write(string.Format(CultureInfo.InvariantCulture, "{0,-7}", CabecalhoVertice(linha)));
                for (int coluna = 0; coluna < QuantidadeVertices; coluna++)
                {
                    string valor = matriz[linha, coluna].HasValue
                        ? matriz[linha, coluna].Value.ToString("0.###", CultureInfo.InvariantCulture)
                        : "0";
                    Console.Write(string.Format(CultureInfo.InvariantCulture, "{0,8}", valor));
                }

                Console.WriteLine();
            }
        }

        protected override void AoInserirVertice()
        {
            int tamanhoNovo = QuantidadeVertices;
            float?[,] matrizNova = new float?[tamanhoNovo, tamanhoNovo];

            for (int linha = 0; linha < tamanhoNovo - 1; linha++)
            {
                for (int coluna = 0; coluna < tamanhoNovo - 1; coluna++)
                {
                    matrizNova[linha, coluna] = matriz[linha, coluna];
                }
            }

            matriz = matrizNova;
        }

        protected override void AoRemoverVertice(int indice)
        {
            int tamanhoNovo = QuantidadeVertices - 1;
            float?[,] matrizNova = new float?[tamanhoNovo, tamanhoNovo];
            int linhaNova = 0;

            for (int linha = 0; linha < QuantidadeVertices; linha++)
            {
                if (linha == indice)
                {
                    continue;
                }

                int colunaNova = 0;
                for (int coluna = 0; coluna < QuantidadeVertices; coluna++)
                {
                    if (coluna == indice)
                    {
                        continue;
                    }

                    matrizNova[linhaNova, colunaNova] = matriz[linha, coluna];
                    colunaNova++;
                }

                linhaNova++;
            }

            matriz = matrizNova;
        }
    }
}
