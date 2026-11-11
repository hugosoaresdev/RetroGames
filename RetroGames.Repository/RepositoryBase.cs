namespace RetroGames.Repository;

using MySqlConnector;

public abstract class RepositoryBase
{
    protected readonly RepositoryConfig Config;

    protected RepositoryBase(RepositoryConfig config)
    {
        Config = config;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(Config.ConnectionString);

        await conexao.OpenAsync();

        return conexao;
    }

    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException();
        }
    }
}