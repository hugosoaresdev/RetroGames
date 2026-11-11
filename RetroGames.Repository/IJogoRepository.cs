using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        List<int>? ids,
        bool incluirInativos,
        int pagina,
        int quantidadePorPagina
    );

    Task<Jogo?> BuscarPorIdAsync(int id);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task DesativarAsync(int id);
}