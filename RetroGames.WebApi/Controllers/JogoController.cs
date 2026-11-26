using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;

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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        ListarPorId(int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaDetalheResponse(),
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        InserirAsync([FromBody] InserirJogoRequestApiModel requisicao)
    {
        var jogo = requisicao.ParaModel();

        var criado = await _jogoService.InserirAsync(
            jogo,
            requisicao.IdsGeneros);

        var detalhe = await _jogoService.BuscarPorIdAsync(criado.Id);

        var resposta = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaDetalheResponse(),
        };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criado.Id },
            resposta);
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object?>>>
        AlterarAsync(
            int id,
            [FromBody] AlterarJogoRequestApiModel requisicao)
    {
        var jogo = requisicao.ParaModel(id);

        await _jogoService.AlterarAsync(
            jogo,
            requisicao.IdsGeneros);

        return Ok(new RetroGamesResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object?>>>
        ExcluirAsync(int id)
    {
        await _jogoService.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
        });
    }
}