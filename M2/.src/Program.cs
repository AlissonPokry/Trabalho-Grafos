#nullable enable
using System.Globalization;
using TrabalhoM1.Grafos;

namespace TrabalhoM2;

internal static class Program
{
    private static readonly string[] Nomes =
        ["Gulosa", "Welsh–Powell", "DSATUR", "Força bruta", "Prim", "Kruskal"];

    private static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        if (args is ["--test"]) return Testes.Executar();
        if (args.Length > 0)
        {
            Console.WriteLine("Uso: dotnet run [-- --test]");
            return 1;
        }
        Console.WriteLine("=== TRABALHO M2 — GRAFOS ===");
        while (true)
        {
            var arquivos = LocalizarArquivos();
            Console.WriteLine("\nArquivos disponíveis:");
            for (int i = 0; i < arquivos.Count; i++)
                Console.WriteLine($"{i + 1}) {Path.GetRelativePath(Directory.GetCurrentDirectory(), arquivos[i])}");
            Console.WriteLine("C) Informar caminho\n0) Sair");
            Console.Write("Arquivo: ");
            string? entrada = Console.ReadLine();
            if (entrada is null or "0") return 0;
            string caminho;
            if (entrada.Equals("c", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Caminho do arquivo: ");
                caminho = (Console.ReadLine() ?? "").Trim().Trim('"');
            }
            else if (int.TryParse(entrada, out int indice) && indice >= 1 && indice <= arquivos.Count)
                caminho = arquivos[indice - 1];
            else { Console.WriteLine("Opção inválida."); continue; }

            Console.WriteLine("Representação: 1) Lista (padrão)  2) Matriz");
            Console.Write("Escolha [1]: ");
            string? representacao = Console.ReadLine();
            if (representacao is null) return 0;
            if (representacao is not ("" or "1" or "2"))
            { Console.WriteLine("Representação inválida."); continue; }
            Grafos grafo = representacao == "2" ? new GrafoMatriz(false, false) : new GrafoLista(false, false);
            if (!grafo.carregarDoArquivo(caminho))
            {
                Console.WriteLine("Não foi possível carregar o arquivo. Cabeçalho esperado: V A D P.");
                continue;
            }
            Console.WriteLine($"\n{grafo.QuantidadeVertices} vértices; " +
                $"{(grafo.Direcionado ? "direcionado" : "não direcionado")}; " +
                $"{(grafo.Ponderado ? "ponderado" : "não ponderado")}; " +
                $"representação {(representacao == "2" ? "matriz" : "lista")}.");
            if (!MenuGrafo(grafo)) return 0;
        }
    }

    private static bool MenuGrafo(Grafos grafo)
    {
        while (true)
        {
            Console.WriteLine("\n1) Gulosa\n2) Welsh–Powell\n3) DSATUR\n4) Força bruta" +
                "\n5) Prim\n6) Kruskal\n7) Comparar três heurísticas\n8) Comparar Prim e Kruskal" +
                "\n9) Escolher outro arquivo\n0) Sair");
            Console.Write("Opção: ");
            string? entrada = Console.ReadLine();
            if (entrada is null or "0") return false;
            if (!int.TryParse(entrada, out int opcao) || opcao < 1 || opcao > 9)
            { Console.WriteLine("Opção inválida."); continue; }
            if (opcao == 9) return true;
            if (opcao == 4)
                Console.WriteLine("Força bruta: custo exponencial, sem limite automático. Ctrl+C encerra o programa.");
            int[] executar = opcao switch { 7 => [1, 2, 3], 8 => [5, 6], _ => [opcao] };
            try
            {
                foreach (int algoritmo in executar)
                {
                    Console.WriteLine($"\n{Nomes[algoritmo - 1]}:");
                    switch (algoritmo)
                    {
                        case 1: Exibir(grafo, Algoritmos.Gulosa(grafo)); break;
                        case 2: Exibir(grafo, Algoritmos.WelshPowell(grafo)); break;
                        case 3: Exibir(grafo, Algoritmos.Dsatur(grafo)); break;
                        case 4: Exibir(grafo, Algoritmos.ForcaBruta(grafo)); break;
                        case 5: Exibir(Algoritmos.Prim(grafo)); break;
                        case 6: Exibir(Algoritmos.Kruskal(grafo)); break;
                    }
                }
            }
            catch (ArgumentException erro) { Console.WriteLine(erro.Message); }
        }
    }

    internal static void Exibir(Grafos grafo, ResultadoColoracao resultado)
    {
        Console.WriteLine($"Cores utilizadas: {resultado.QuantidadeCores}");
        Console.WriteLine($"Tempo de execução: {resultado.Duracao.TotalMilliseconds:F6} ms");
        if (grafo.QuantidadeVertices < 10)
            for (int v = 0; v < resultado.Cores.Length; v++)
                Console.WriteLine($"Vértice {v} [{grafo.labelVertice(v)}]: cor {resultado.Cores[v]}");
    }

    internal static void Exibir(ResultadoAgm resultado)
    {
        Console.WriteLine(resultado.Componentes > 1
            ? $"Floresta geradora mínima: {resultado.Componentes} componentes."
            : resultado.Componentes == 0 ? "Grafo vazio: nenhuma árvore." : "Árvore geradora mínima.");
        Console.WriteLine($"Arestas selecionadas: {resultado.Arestas.Count}");
        Console.WriteLine($"Soma dos pesos: {resultado.PesoTotal:G12}");
        Console.WriteLine($"Tempo de execução: {resultado.Duracao.TotalMilliseconds:F6} ms");
        foreach (var a in resultado.Arestas)
            Console.WriteLine($"{a.Origem} -- {a.Destino}: {a.Peso:G9}");
    }

    private static List<string> LocalizarArquivos()
    {
        var encontrados = new HashSet<string>(StringComparer.Ordinal);
        // O caminho do projeto, derivado do executável, permite rodar com --project da pasta pai.
        string projeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
        var raizes = new[] { Directory.GetCurrentDirectory(), projeto, Path.GetDirectoryName(projeto)! };
        foreach (string raiz in raizes.Distinct())
            foreach (string pasta in new[] { raiz, Path.Combine(raiz, "grafos") })
            {
                if (!Directory.Exists(pasta)) continue;
                try
                {
                    foreach (string arquivo in Directory.EnumerateFiles(pasta, "*.txt"))
                        encontrados.Add(Path.GetFullPath(arquivo));
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        return encontrados.OrderBy(p => p, StringComparer.Ordinal).ToList();
    }
}
