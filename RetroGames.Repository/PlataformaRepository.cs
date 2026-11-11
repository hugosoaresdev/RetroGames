
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = @"
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma;
        ";

        var plataformas = new List<Plataforma>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var plataforma = new Plataforma
            {
                Id = leitor.GetInt32("co_seq_plataforma"),
                Nome = leitor.GetString("no_plataforma"),
                Fabricante = leitor.GetString("no_fabricante"),
                AnoLancamento = leitor.GetInt32("nu_ano_lancamento")
            };

            plataformas.Add(plataforma);
        }

        return plataformas;
    }


    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = @"
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return new Plataforma
            {
                Id = leitor.GetInt32("co_seq_plataforma"),
                Nome = leitor.GetString("no_plataforma"),
                Fabricante = leitor.GetString("no_fabricante"),
                AnoLancamento = leitor.GetInt32("nu_ano_lancamento")
            };
        }

        return null;
    }


    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorado = null)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_plataforma
                WHERE no_plataforma = @nome
                AND (@idIgnorado IS NULL OR co_seq_plataforma <> @idIgnorado)
            );
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorado", idIgnorado);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToBoolean(resultado);
    }


    public async Task<bool> PossuiJogosAsync(int idPlataforma)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1
                FROM tb_jogo
                WHERE co_plataforma = @idPlataforma
            );
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPlataforma", idPlataforma);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToBoolean(resultado);
    }


    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = @"
            INSERT INTO tb_plataforma (
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            )
            VALUES (
                @nome,
                @fabricante,
                @anoLancamento
            );
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        const string sqlId = "SELECT LAST_INSERT_ID();";

        await using var comandoId = CriarComando(conexao, sqlId);

        var resultado = await comandoId.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = @"
            UPDATE tb_plataforma
            SET
                no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @anoLancamento
            WHERE co_seq_plataforma = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);
        AdicionarParametro(comando, "@id", plataforma.Id);

        await ExecutarGravacaoAsync(comando);
    }


    public async Task ExcluirAsync(int id)
    {
        const string sql = @"
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }
}
