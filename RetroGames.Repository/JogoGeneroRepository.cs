using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = @"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
        ";

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos)
    {
        if (idsJogos.Count == 0)
            return new List<JogoGenero>();

        var parametros = idsJogos
            .Select((id, indice) => $"@id{indice}")
            .ToList();

        string sql = $@"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", parametros)})
            ORDER BY co_jogo, co_genero;
        ";

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < idsJogos.Count; i++)
            AdicionarParametro(comando, $"@id{i}", idsJogos[i]);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }

    public async Task InserirAsync(List<JogoGenero> vinculos)
    {
        if (vinculos.Count == 0)
            return;

        var valores = vinculos
            .Select((vinculo, indice) =>
                $"(@jogo{indice}, @genero{indice})")
            .ToList();

        string sql = $@"
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", valores)};
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < vinculos.Count; i++)
        {
            AdicionarParametro(comando, $"@jogo{i}", vinculos[i].IdJogo);
            AdicionarParametro(comando, $"@genero{i}", vinculos[i].IdGenero);
        }

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = @"
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }
}