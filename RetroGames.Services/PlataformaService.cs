using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<Plataforma>> ListarAsync()
    {
        return repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        return plataforma;
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            plataforma.Id = await repositorio.Plataforma.InserirAsync(plataforma);
            return plataforma;
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);

        plataforma.Nome = plataforma.Nome.Trim();

        if (await repositorio.Plataforma.ExisteComNomeAsync(
                plataforma.Nome, plataforma.Id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            await repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await repositorio.Plataforma.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_POSSUI_JOGOS);
        }

        await repositorio.Plataforma.ExcluirAsync(id);
    }
}