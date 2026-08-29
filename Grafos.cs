using System;
using System.Collections.Generic;

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

        protected abstract void AoRemoverVertice(int indice);
    }
}
