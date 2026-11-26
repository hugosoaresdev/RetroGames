using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _condicaoJogoService;

    public CondicaoJogoController(ICondicaoJogoService condicaoJogoService)
    {
        _condicaoJogoService = condicaoJogoService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>>
        ListarAsync()
    {
        var condicoes = await _condicaoJogoService.ListarAsync();
        var dados = condicoes
            .Select(condicao => condicao.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<
            List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados,
        });
    }
}