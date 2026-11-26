using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>>>
        ListarAsync()
    {
        var generos = await _generoService.ListarAsync();
        var dados = generos
            .Select(genero => genero.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<
            List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados,
        });
    }
}