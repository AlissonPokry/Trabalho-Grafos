#nullable enable
using System.Diagnostics;
using TrabalhoM1.Grafos;

namespace TrabalhoM2;

public sealed record ResultadoColoracao(int[] Cores, int QuantidadeCores, TimeSpan Duracao);
public readonly record struct ArestaAgm(int Origem, int Destino, float Peso);
public sealed record ResultadoAgm(IReadOnlyList<ArestaAgm> Arestas, double PesoTotal,
    int Componentes, TimeSpan Duracao);

public static class Algoritmos
{
    // Cada execução prepara sua própria vizinhança: esse custo faz parte da medição.
    private static int[][] PrepararColoracao(Grafos grafo)
    {
        if (grafo.Direcionado || grafo.Ponderado)
            throw new ArgumentException("Coloração exige grafo não direcionado e não ponderado.");
        var vizinhos = new int[grafo.QuantidadeVertices][];
        for (int v = 0; v < vizinhos.Length; v++)
        {
            vizinhos[v] = grafo.retornarVizinhos(v).ToArray();
            if (vizinhos[v].Contains(v))
                throw new ArgumentException("Grafo com laço não admite coloração própria.");
        }
        return vizinhos;
    }

    private static ResultadoColoracao Finalizar(int[] cores, Stopwatch relogio)
    {
        int quantidade = cores.Length == 0 ? 0 : cores.Max();
        relogio.Stop();
        return new(cores, quantidade, relogio.Elapsed);
    }

    private static int MenorCor(int v, int[][] vizinhos, int[] cores)
    {
        var usadas = new HashSet<int>();
        foreach (int u in vizinhos[v]) usadas.Add(cores[u]);
        int cor = 1;
        while (usadas.Contains(cor)) cor++;
        return cor;
    }

    public static ResultadoColoracao Gulosa(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var vizinhos = PrepararColoracao(grafo);
        var cores = new int[vizinhos.Length];
        for (int v = 0; v < cores.Length; v++) cores[v] = MenorCor(v, vizinhos, cores);
        return Finalizar(cores, relogio);
    }

    public static ResultadoColoracao WelshPowell(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var vizinhos = PrepararColoracao(grafo);
        var cores = new int[vizinhos.Length];
        var ordem = Enumerable.Range(0, cores.Length)
            .OrderByDescending(v => vizinhos[v].Length).ThenBy(v => v).ToArray();
        int restantes = cores.Length, cor = 0;
        while (restantes > 0)
        {
            cor++;
            foreach (int v in ordem)
            {
                if (cores[v] != 0 || vizinhos[v].Any(u => cores[u] == cor)) continue;
                cores[v] = cor;
                restantes--;
            }
        }
        return Finalizar(cores, relogio);
    }

    public static ResultadoColoracao Dsatur(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var vizinhos = PrepararColoracao(grafo);
        var cores = new int[vizinhos.Length];
        var saturacao = Enumerable.Range(0, cores.Length).Select(_ => new HashSet<int>()).ToArray();
        for (int passo = 0; passo < cores.Length; passo++)
        {
            int escolhido = -1;
            for (int v = 0; v < cores.Length; v++)
            {
                if (cores[v] != 0) continue;
                if (escolhido == -1 || saturacao[v].Count > saturacao[escolhido].Count ||
                    (saturacao[v].Count == saturacao[escolhido].Count &&
                     vizinhos[v].Length > vizinhos[escolhido].Length)) escolhido = v;
            }
            cores[escolhido] = MenorCor(escolhido, vizinhos, cores);
            foreach (int u in vizinhos[escolhido])
                if (cores[u] == 0) saturacao[u].Add(cores[escolhido]);
        }
        return Finalizar(cores, relogio);
    }

    public static ResultadoColoracao ForcaBruta(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var vizinhos = PrepararColoracao(grafo);
        var cores = new int[vizinhos.Length];
        if (cores.Length == 0) return Finalizar(cores, relogio);
        Array.Fill(cores, 1);
        if (vizinhos.All(v => v.Length == 0)) return Finalizar(cores, relogio);

        // Enumeração literal das atribuições completas, sem poda ou heurística.
        // O vetor funciona como um contador em base k, evitando pilha recursiva.
        for (int k = 2; k <= cores.Length; k++)
        {
            Array.Fill(cores, 1);
            while (true)
            {
                bool valida = true;
                for (int v = 0; v < cores.Length && valida; v++)
                    foreach (int u in vizinhos[v])
                        if (cores[v] == cores[u]) { valida = false; break; }
                if (valida) return Finalizar(cores, relogio);

                int posicao = cores.Length - 1;
                while (posicao >= 0 && cores[posicao] == k) cores[posicao--] = 1;
                if (posicao < 0) break;
                cores[posicao]++;
            }
        }
        throw new InvalidOperationException("Não foi possível encontrar uma coloração.");
    }

    private static List<ArestaAgm>[] PrepararAgm(Grafos grafo)
    {
        if (grafo.Direcionado) throw new ArgumentException("AGM exige grafo não direcionado.");
        var adjacencias = Enumerable.Range(0, grafo.QuantidadeVertices)
            .Select(_ => new List<ArestaAgm>()).ToArray();
        for (int v = 0; v < adjacencias.Length; v++)
            foreach (int u in grafo.retornarVizinhos(v))
            {
                // Consulta o peso uma vez por aresta, nas duas representações da M1.
                if (v >= u) continue;
                float peso = grafo.pesoAresta(v, u);
                adjacencias[v].Add(new(v, u, peso));
                adjacencias[u].Add(new(u, v, peso));
            }
        return adjacencias;
    }

    private static ResultadoAgm Finalizar(List<ArestaAgm> arestas, int componentes, Stopwatch relogio)
    {
        double peso = arestas.Sum(a => (double)a.Peso);
        relogio.Stop();
        return new(arestas.AsReadOnly(), peso, componentes, relogio.Elapsed);
    }

    public static ResultadoAgm Prim(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var adjacencias = PrepararAgm(grafo);
        var visitado = new bool[adjacencias.Length];
        var resultado = new List<ArestaAgm>();
        var fila = new PriorityQueue<ArestaAgm, (float Peso, int Menor, int Maior)>();
        int componentes = 0;
        void Visitar(int v)
        {
            visitado[v] = true;
            foreach (var a in adjacencias[v])
                if (!visitado[a.Destino])
                    fila.Enqueue(a, (a.Peso, Math.Min(a.Origem, a.Destino), Math.Max(a.Origem, a.Destino)));
        }
        for (int raiz = 0; raiz < visitado.Length; raiz++)
        {
            if (visitado[raiz]) continue;
            componentes++;
            Visitar(raiz);
            while (fila.TryDequeue(out var aresta, out _))
            {
                if (visitado[aresta.Destino]) continue;
                resultado.Add(new(Math.Min(aresta.Origem, aresta.Destino),
                    Math.Max(aresta.Origem, aresta.Destino), aresta.Peso));
                Visitar(aresta.Destino);
            }
        }
        return Finalizar(resultado, componentes, relogio);
    }

    public static ResultadoAgm Kruskal(Grafos grafo)
    {
        var relogio = Stopwatch.StartNew();
        var adjacencias = PrepararAgm(grafo);
        var arestas = adjacencias.SelectMany(a => a).Where(a => a.Origem < a.Destino)
            .OrderBy(a => a.Peso).ThenBy(a => a.Origem).ThenBy(a => a.Destino);
        var conjuntos = new Conjuntos(adjacencias.Length);
        var resultado = new List<ArestaAgm>();
        int componentes = adjacencias.Length;
        foreach (var a in arestas)
            if (conjuntos.Unir(a.Origem, a.Destino)) { resultado.Add(a); componentes--; }
        return Finalizar(resultado, componentes, relogio);
    }

    internal sealed class Conjuntos
    {
        private readonly int[] pais;
        private readonly int[] tamanhos;
        public Conjuntos(int n)
        {
            pais = Enumerable.Range(0, n).ToArray();
            tamanhos = Enumerable.Repeat(1, n).ToArray();
        }
        private int Raiz(int v)
        {
            while (pais[v] != v) { pais[v] = pais[pais[v]]; v = pais[v]; }
            return v;
        }
        public bool Unir(int u, int v)
        {
            u = Raiz(u); v = Raiz(v);
            if (u == v) return false;
            if (tamanhos[u] < tamanhos[v]) (u, v) = (v, u);
            pais[v] = u;
            tamanhos[u] += tamanhos[v];
            return true;
        }
    }
}
