using RetroGames.Model.Model;
using RetroGames.Repository;
using System.Transactions;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? nome,
        int? idPlataforma,
        int? idGenero,
        bool incluirInativos,
        int pagina,
        int quantidadePorPagina)
    {
        List<int>? idsJogos = null;

        if (idGenero.HasValue)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();

            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == idGenero.Value)
                .Select(vinculo => vinculo.IdJogo)
                .Distinct()
                .ToList();

            if (idsJogos.Count == 0)
            {
                return (new List<JogoDetalheServiceModel>(), 0);
            }
        }

        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            nome,
            idPlataforma,
            idsJogos,
            incluirInativos,
            pagina,
            quantidadePorPagina);

        var detalhes = await MontarDetalhesAsync(jogos);

        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });

        return detalhes[0];
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var idsJogos = jogos
            .Select(jogo => jogo.Id)
            .ToList();

        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(idsJogos);

        var plataformasPorId = plataformas.ToDictionary(
            plataforma => plataforma.Id);

        var condicoesPorId = condicoes.ToDictionary(
            condicao => condicao.Id);

        var generosPorId = generos.ToDictionary(
            genero => genero.Id);

        return jogos
            .Select(jogo => new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataformasPorId[jogo.IdPlataforma].Nome,
                DescricaoCondicaoJogo =
                    condicoesPorId[jogo.IdCondicaoJogo].Descricao,
                Generos = vinculos
                    .Where(vinculo => vinculo.IdJogo == jogo.Id)
                    .Select(vinculo => generosPorId[vinculo.IdGenero])
                    .ToList()
            })
            .ToList();
    }

    public async Task<Jogo> InserirAsync(Jogo jogo, List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);

            var vinculos = idsGeneros
                .Distinct()
                .Select(idGenero => new JogoGenero
                {
                    IdJogo = jogo.Id,
                    IdGenero = idGenero
                })
                .ToList();

            if (vinculos.Count > 0)
            {
                await repositorio.JogoGenero.InserirAsync(vinculos);
            }

            transactionScope.Complete();
        }

        return jogo;
    }

    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        List<int> idsGeneros)
    {
        var plataforma = await repositorio.Plataforma
            .BuscarPorIdAsync(jogo.IdPlataforma);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(condicao =>
                condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA);
        }

        var generos = await repositorio.Genero.ListarAsync();
        var idsGenerosExistentes = generos
            .Select(genero => genero.Id)
            .ToHashSet();

        if (idsGeneros.Any(idGenero =>
                !idsGenerosExistentes.Contains(idGenero)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    public async Task AlterarAsync(Jogo jogo, List<int> idsGeneros)
    {
        var jogoExistente = await repositorio.Jogo
            .BuscarPorIdAsync(jogo.Id);

        if (jogoExistente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        await ValidarReferenciasAsync(jogo, idsGeneros);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

            var vinculos = idsGeneros
                .Distinct()
                .Select(idGenero => new JogoGenero
                {
                    IdJogo = jogo.Id,
                    IdGenero = idGenero
                })
                .ToList();

            if (vinculos.Count > 0)
            {
                await repositorio.JogoGenero.InserirAsync(vinculos);
            }

            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        await repositorio.Jogo.DesativarAsync(id);
    }
}