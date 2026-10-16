#nullable enable
using TrabalhoM1.Grafos;

namespace TrabalhoM2;

internal static class Testes
{
    private static int verificacoes;
    private static readonly Func<Grafos, ResultadoColoracao>[] Coloracoes =
        [Algoritmos.Gulosa, Algoritmos.WelshPowell, Algoritmos.Dsatur, Algoritmos.ForcaBruta];
    private static readonly Func<Grafos, ResultadoAgm>[] Agms = [Algoritmos.Prim, Algoritmos.Kruskal];

    public static int Executar()
    {
        try
        {
            TestarColoracoes();
            TestarAgms();
            TestarAleatorios();
            TestarArquivosESaida();
            TestarGrande();
            Console.WriteLine($"SUCESSO: {verificacoes} verificações passaram.");
            return 0;
        }
        catch (Exception erro)
        {
            Console.Error.WriteLine($"FALHA após {verificacoes} verificações: {erro}");
            return 1;
        }
    }

    private static void Verificar(bool condicao, string mensagem)
    {
        verificacoes++;
        if (!condicao) throw new InvalidOperationException(mensagem);
    }

    private static Grafos Criar(int n, (int U, int V, float P)[] arestas,
        bool matriz = false, bool ponderado = false, bool direcionado = false)
    {
        Grafos g = matriz ? new GrafoMatriz(direcionado, ponderado) : new GrafoLista(direcionado, ponderado);
        for (int v = 0; v < n; v++) g.inserirVertice(v.ToString());
        foreach (var a in arestas)
            if (!g.inserirAresta(a.U, a.V, a.P)) throw new Exception("Fixture inválida.");
        return g;
    }

    private static void ValidarColoracao(Grafos g, ResultadoColoracao r)
    {
        Verificar(r.Cores.Length == g.QuantidadeVertices, "Número de vértices na coloração.");
        Verificar(r.QuantidadeCores == r.Cores.Distinct().Count(), "Contagem de cores.");
        Verificar(r.Cores.All(c => c > 0), "Cores positivas.");
        for (int v = 0; v < g.QuantidadeVertices; v++)
            foreach (int u in g.retornarVizinhos(v))
                Verificar(r.Cores[v] != r.Cores[u], $"Conflito entre {v} e {u}.");
        Verificar(r.Duracao >= TimeSpan.Zero, "Duração da coloração.");
    }

    private static void TestarColoracoes()
    {
        var casos = new (int N, (int, int, float)[] Arestas, int Minimo)[]
        {
            (0, [], 0), (1, [], 1), (5, [], 1),
            (4, [(0,1,1), (1,2,1), (2,3,1)], 2),
            (4, [(0,1,1), (1,2,1), (2,3,1), (3,0,1)], 2),
            (5, [(0,1,1), (1,2,1), (2,3,1), (3,4,1), (4,0,1)], 3),
            (4, [(0,1,1), (0,2,1), (0,3,1), (1,2,1), (1,3,1), (2,3,1)], 4),
            (7, [(0,1,1), (1,2,1), (2,0,1), (3,4,1), (4,5,1)], 3)
        };
        foreach (var caso in casos)
        {
            var lista = Criar(caso.N, caso.Arestas);
            var matriz = Criar(caso.N, caso.Arestas, matriz: true);
            foreach (var algoritmo in Coloracoes)
            {
                var r = algoritmo(lista);
                var m = algoritmo(matriz);
                ValidarColoracao(lista, r);
                ValidarColoracao(matriz, m);
                Verificar(r.Cores.SequenceEqual(m.Cores), "Coloração deve independer da representação.");
                Verificar(r.QuantidadeCores >= caso.Minimo, "Coloração abaixo do mínimo conhecido.");
            }
            Verificar(Algoritmos.ForcaBruta(lista).QuantidadeCores == caso.Minimo, "Mínimo exato.");
        }
        foreach (var algoritmo in Coloracoes)
        {
            EsperarErro(() => algoritmo(Criar(1, [(0,0,1)])));
            EsperarErro(() => algoritmo(Criar(2, [(0,1,2)], ponderado: true)));
            EsperarErro(() => algoritmo(Criar(2, [(0,1,1)], direcionado: true)));
        }
        Console.WriteLine("Coloração: casos conhecidos, validade, representações e rejeições OK.");
    }

    private static void EsperarErro(Action executar)
    {
        bool rejeitou = false;
        try { executar(); } catch (ArgumentException) { rejeitou = true; }
        Verificar(rejeitou, "Entrada incompatível deveria ser rejeitada.");
    }

    private static void ValidarAgm(Grafos g, ResultadoAgm r, double peso, int componentes)
    {
        Verificar(Math.Abs(r.PesoTotal - peso) < 0.00001, $"Peso esperado {peso}; recebido {r.PesoTotal}.");
        Verificar(r.Componentes == componentes, "Número de componentes da floresta.");
        Verificar(r.Arestas.Count == g.QuantidadeVertices - componentes, "Quantidade de arestas da floresta.");
        var conjuntos = new Algoritmos.Conjuntos(g.QuantidadeVertices);
        double soma = 0;
        foreach (var a in r.Arestas)
        {
            Verificar(g.existeAresta(a.Origem, a.Destino), "Aresta deve existir no grafo.");
            Verificar(a.Peso == g.pesoAresta(a.Origem, a.Destino), "Peso original da aresta.");
            Verificar(conjuntos.Unir(a.Origem, a.Destino), "Resultado não pode ter ciclos ou laços.");
            soma += a.Peso;
        }
        Verificar(Math.Abs(soma - r.PesoTotal) < 0.00001, "Soma das arestas.");
        Verificar(r.Duracao >= TimeSpan.Zero, "Duração da AGM.");
    }

    private static void TestarAgms()
    {
        var casos = new (int N, (int,int,float)[] Arestas, double Peso, int Componentes)[]
        {
            (0, [], 0, 0), (1, [], 0, 1), (4, [], 0, 4),
            (4, [(0,1,4), (0,2,2), (1,2,-1), (1,3,5), (2,3,3)], 4, 1),
            (6, [(0,1,4), (0,2,2), (1,2,-1), (3,4,3), (4,4,-5)], 4, 3),
            (4, [(0,1,1), (1,2,1), (2,3,1), (3,0,1), (0,2,1)], 3, 1),
            (3, [(0,1,0), (1,2,1), (0,2,10)], 1, 1),
            (5, [(0,1,0), (1,2,0), (3,4,-2)], -2, 2)
        };
        foreach (var caso in casos)
            foreach (bool matriz in new[] { false, true })
            {
                var g = Criar(caso.N, caso.Arestas, matriz, ponderado: true);
                foreach (var algoritmo in Agms)
                {
                    var r = algoritmo(g);
                    ValidarAgm(g, r, caso.Peso, caso.Componentes);
                    Verificar(r.Arestas.SequenceEqual(algoritmo(g).Arestas), "AGM determinística.");
                }
            }
        foreach (var algoritmo in Agms)
        {
            ValidarAgm(Criar(3, [(0,1,1), (1,2,1)]),
                algoritmo(Criar(3, [(0,1,1), (1,2,1)])), 2, 1);
            EsperarErro(() => algoritmo(Criar(2, [(0,1,1)], direcionado: true)));
        }
        Console.WriteLine("AGM: pesos conhecidos, florestas, ciclos, laços e empates OK.");
    }

    // Oráculo independente: enumera subconjuntos e testa conectividade por BFS.
    private static double PesoOtimo(int n, (int U, int V, float P)[] arestas)
    {
        double melhor = double.PositiveInfinity;
        for (int mascara = 0; mascara < (1 << arestas.Length); mascara++)
        {
            var escolhidas = arestas.Where((_, i) => (mascara & (1 << i)) != 0).ToArray();
            if (escolhidas.Length != n - 1) continue;
            var visitados = new HashSet<int> { 0 };
            var fila = new Queue<int>();
            fila.Enqueue(0);
            while (fila.TryDequeue(out int v))
                foreach (var a in escolhidas)
                {
                    int u = a.U == v ? a.V : a.V == v ? a.U : -1;
                    if (u >= 0 && visitados.Add(u)) fila.Enqueue(u);
                }
            if (visitados.Count == n) melhor = Math.Min(melhor, escolhidas.Sum(a => (double)a.P));
        }
        return melhor;
    }

    private static void TestarAleatorios()
    {
        var aleatorio = new Random(2026);
        for (int caso = 0; caso < 20; caso++)
        {
            var arestas = new List<(int, int, float)>();
            for (int u = 0; u < 5; u++)
                for (int v = u + 1; v < 5; v++)
                    if (v == u + 1 || aleatorio.Next(2) == 0)
                    {
                        int peso = aleatorio.Next(-5, 6);
                        arestas.Add((u, v, peso == 0 ? 1 : peso));
                    }
            double otimo = PesoOtimo(5, arestas.ToArray());
            foreach (bool matriz in new[] { false, true })
            {
                var g = Criar(5, arestas.ToArray(), matriz, ponderado: true);
                foreach (var algoritmo in Agms) ValidarAgm(g, algoritmo(g), otimo, 1);
                var simples = Criar(5, arestas.ToArray(), matriz);
                int minimo = Algoritmos.ForcaBruta(simples).QuantidadeCores;
                foreach (var algoritmo in Coloracoes.Take(3))
                {
                    var r = algoritmo(simples);
                    ValidarColoracao(simples, r);
                    Verificar(r.QuantidadeCores >= minimo, "Heurística versus solução exata.");
                }
            }
        }
        Console.WriteLine("Grafos aleatórios: AGM versus enumeração independente e heurísticas versus força bruta OK.");
    }

    private static void TestarArquivosESaida()
    {
        string pasta = Path.Combine(AppContext.BaseDirectory, "grafos");
        var g = new GrafoLista(false, false);
        Verificar(g.carregarDoArquivo(Path.Combine(pasta, "ciclo_impar.txt")), "Leitura de exemplo.");
        Verificar(Algoritmos.ForcaBruta(g).QuantidadeCores == 3, "Arquivo de ciclo ímpar.");
        Verificar(g.carregarDoArquivo(Path.Combine(pasta, "agm_desconectado.txt")), "Leitura ponderada.");
        ValidarAgm(g, Algoritmos.Prim(g), 4, 3);
        Verificar(!g.carregarDoArquivo(Path.Combine(pasta, "inexistente.txt")), "Arquivo inexistente.");
        foreach (int n in new[] { 9, 10 })
        {
            var simples = Criar(n, []);
            string saida = Capturar(() => Program.Exibir(simples, Algoritmos.Gulosa(simples)));
            Verificar(saida.Contains("Cores utilizadas: 1") && saida.Contains("Tempo de execução:"), "Resumo da coloração.");
            Verificar(saida.Split('\n').Count(l => l.StartsWith("Vértice ")) == (n < 10 ? n : 0), "Limiar de impressão de cores.");
        }
        string floresta = Capturar(() => Program.Exibir(Algoritmos.Kruskal(g)));
        Verificar(floresta.Contains("Floresta geradora mínima: 3 componentes") && floresta.Contains("Soma dos pesos: 4"), "Saída da floresta.");
        Console.WriteLine("Arquivos e saída: leitura, cores para nove/dez vértices e floresta OK.");
    }

    private static string Capturar(Action acao)
    {
        var original = Console.Out;
        using var texto = new StringWriter();
        try { Console.SetOut(texto); acao(); }
        finally { Console.SetOut(original); }
        return texto.ToString();
    }

    private static void TestarGrande()
    {
        const int n = 5000;
        var arestas = Enumerable.Range(0, n - 1).Select(v => (v, v + 1, 1f)).ToArray();
        var g = Criar(n, arestas);
        foreach (var algoritmo in Coloracoes.Take(3))
        {
            var r = algoritmo(g);
            ValidarColoracao(g, r);
            Verificar(r.QuantidadeCores == 2, "Caminho grande usa duas cores.");
            Console.WriteLine($"5000 vértices — {algoritmo.Method.Name}: {r.Duracao.TotalMilliseconds:F3} ms");
        }
        foreach (var algoritmo in Agms)
        {
            var r = algoritmo(g);
            ValidarAgm(g, r, n - 1, 1);
            Console.WriteLine($"5000 vértices — {algoritmo.Method.Name}: {r.Duracao.TotalMilliseconds:F3} ms");
        }
    }
}
