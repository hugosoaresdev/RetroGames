using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (
        string? Nome,
        int? IdPlataforma,
        int? IdGenero,
        bool IncluirInativos,
        int Pagina,
        int QuantidadePorPagina)
        ParaFiltro(this ListarJogoRequestApiModel requisicao)
    {
        return (
            requisicao.Busca,
            requisicao.IdPlataforma,
            requisicao.IdGenero,
            false,
            requisicao.Pagina,
            requisicao.TamanhoPagina);
    }

    public static ListarJogoResponseApiModel ParaListarResponse(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            QuantidadeEstoque = detalhe.Jogo.QuantidadeEstoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicaoJogo = detalhe.DescricaoCondicaoJogo,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList(),
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            QuantidadeEstoque = detalhe.Jogo.QuantidadeEstoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicaoJogo = detalhe.DescricaoCondicaoJogo,
            Generos = detalhe.Generos
                .Select(genero => genero.ParaResponse())
                .ToList(),
        };
    }
}