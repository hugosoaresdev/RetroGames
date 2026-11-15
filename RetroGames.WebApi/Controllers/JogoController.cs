using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    [HttpGet]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<
            PaginaResponseApiModel<ListarJogoResponseApiModel>>>>
        ListarAsync([FromQuery] ListarJogoRequestApiModel requisicao)
    {
        var filtro = requisicao.ParaFiltro();

        var (jogos, total) = await _jogoService.ListarAsync(
            filtro.Nome,
            filtro.IdPlataforma,
            filtro.IdGenero,
            filtro.IncluirInativos,
            filtro.Pagina,
            filtro.QuantidadePorPagina);

        var dadosPagina = new PaginaResponseApiModel<
            ListarJogoResponseApiModel>
        {
            Itens = jogos
                .Select(jogo => jogo.ParaListarResponse())
                .ToList(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.QuantidadePorPagina,
            TotalItens = total,
            TotalPaginas = filtro.QuantidadePorPagina > 0
                ? (int)Math.Ceiling(
                    total / (double)filtro.QuantidadePorPagina)
                : 0,
        };

        return Ok(new RetroGamesResponseApiModel<
            PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dadosPagina,
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        ListarPorIdAsync(int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaDetalheResponse(),
        });
    }
}