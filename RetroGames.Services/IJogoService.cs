using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
        int pagina,
        int quantidadePorPagina);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<Jogo> InserirAsync(Jogo jogo, List<int> idsGeneros);

    Task AlterarAsync(Jogo jogo, List<int> idsGeneros);

    Task DesativarAsync(int id);
}