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
        ListarPorIdAsync(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<
            ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse(),
        });
    }
}