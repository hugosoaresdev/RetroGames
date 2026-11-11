
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        List<int>? ids,
        bool incluirInativos,
        int pagina,
        int quantidadePorPagina)
    {
        // Prepara os parâmetros do filtro de IDs.
        var parametrosIds = new List<string>();

        if (ids != null)
        {
            if (ids.Count == 0)
            {
                return (new List<Jogo>(), 0);
            }

            for (int i = 0; i < ids.Count; i++)
            {
                parametrosIds.Add($"@id{i}");
            }
        }

        string filtroIdsSql = ids == null
            ? ""
            : $"AND co_seq_jogo IN ({string.Join(", ", parametrosIds)})";

        // Calcula a quantidade de registros a ignorar.
        int deslocamento = (pagina - 1) * quantidadePorPagina;

        await using var conexao = await AbrirConexaoAsync();

        // Consulta a quantidade total de jogos.
        string sqlContagem = $@"
            SELECT COUNT(*)
            FROM tb_jogo
            WHERE (@nome IS NULL OR no_jogo LIKE @nome)
              AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
              AND (@incluirInativos = 1 OR st_ativo = 1)
              {filtroIdsSql};
        ";

        await using var comandoContagem = CriarComando(conexao, sqlContagem);

        AdicionarParametro(
            comandoContagem,
            "@nome",
            string.IsNullOrWhiteSpace(nome) ? null : $"%{nome}%"
        );

        AdicionarParametro(comandoContagem, "@idPlataforma", idPlataforma);
        AdicionarParametro(comandoContagem, "@incluirInativos", incluirInativos);

        if (ids != null)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                AdicionarParametro(comandoContagem, $"@id{i}", ids[i]);
            }
        }

        var resultado = await comandoContagem.ExecuteScalarAsync();
        int total = Convert.ToInt32(resultado);

        // Consulta os jogos aplicando os mesmos filtros e a paginação.
        string sqlJogos = $@"
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE (@nome IS NULL OR no_jogo LIKE @nome)
              AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
              AND (@incluirInativos = 1 OR st_ativo = 1)
              {filtroIdsSql}
            ORDER BY co_seq_jogo
            LIMIT @limite OFFSET @deslocamento;
        ";

        await using var comandoJogos = CriarComando(conexao, sqlJogos);

        AdicionarParametro(
            comandoJogos,
            "@nome",
            string.IsNullOrWhiteSpace(nome) ? null : $"%{nome}%"
        );

        AdicionarParametro(comandoJogos, "@idPlataforma", idPlataforma);
        AdicionarParametro(comandoJogos, "@incluirInativos", incluirInativos);

        if (ids != null)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                AdicionarParametro(comandoJogos, $"@id{i}", ids[i]);
            }
        }

        AdicionarParametro(comandoJogos, "@limite", quantidadePorPagina);
        AdicionarParametro(comandoJogos, "@deslocamento", deslocamento);

        var jogos = new List<Jogo>();

        await using var leitor = await comandoJogos.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var jogo = new Jogo
            {
                Id = leitor.GetInt32("co_seq_jogo"),
                Nome = leitor.GetString("no_jogo"),
                Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
                    ? null
                    : leitor.GetString("ds_jogo"),
                AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
                Preco = leitor.GetDecimal("vl_preco"),
                QuantidadeEstoque = leitor.GetInt32("qt_estoque"),
                Ativo = leitor.GetBoolean("st_ativo"),
                DataCadastro = leitor.GetDateTime("dt_cadastro"),
                IdPlataforma = leitor.GetInt32("co_plataforma"),
                IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
            };

            jogos.Add(jogo);
        }

        return (jogos, total);
    }


    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
                ? null
                : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            QuantidadeEstoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }


    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = @"
            INSERT INTO tb_jogo (
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            )
            VALUES (
                @nome,
                @descricao,
                @anoLancamento,
                @preco,
                @quantidadeEstoque,
                @ativo,
                @dataCadastro,
                @idPlataforma,
                @idCondicaoJogo
            );

            SELECT LAST_INSERT_ID();
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@quantidadeEstoque", jogo.QuantidadeEstoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }


    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @quantidadeEstoque,
                st_ativo = @ativo,
                dt_cadastro = @dataCadastro,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@quantidadeEstoque", jogo.QuantidadeEstoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await comando.ExecuteNonQueryAsync();
    }


    public async Task DesativarAsync(int id)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }
}
