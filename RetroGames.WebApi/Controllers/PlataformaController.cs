using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    [HttpGet]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>>
        ListarAsync()
    {
        var plataformas = await _plataformaService.ListarAsync();
        var dados = plataformas
            .Select(plataforma => plataforma.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<
            List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados,
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>>
        ListarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<
            ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse(),
        });
    }

        [HttpPost]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>>
        InserirAsync([FromBody] InserirPlataformaRequestApiModel requisicao)
    {
        var plataforma = requisicao.ParaModel();
        var criada = await _plataformaService.InserirAsync(plataforma);

        var resposta = new RetroGamesResponseApiModel<
            ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criada.ParaResponse(),
        };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criada.Id },
            resposta);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object?>>>
        AlterarAsync(
            int id,
            [FromBody] AlterarPlataformaRequestApiModel requisicao)
    {
        var plataforma = requisicao.ParaModel(id);
        await _plataformaService.AlterarAsync(plataforma);

        return Ok(new RetroGamesResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object?>>>
        ExcluirAsync(int id)
    {
        await _plataformaService.ExcluirAsync(id);

        return Ok(new RetroGamesResponseApiModel<object?>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = null,
        });
    }
}