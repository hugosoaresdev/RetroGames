using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<
        RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>>
        LoginAsync([FromBody] LoginRequestApiModel requisicao)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(
            requisicao.Email,
            requisicao.Senha);

        if (usuario is null)
        {
            return Unauthorized(
                new RetroGamesResponseApiModel<object?>
                {
                    ResponseCode =
                        ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                    ErrorMessage =
                        ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage,
                    Data = null,
                });
        }

        var principal = usuario.ParaClaimsPrincipal();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Ok(
            new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = MontarRespostaUsuario(principal),
            });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<RetroGamesResponseApiModel<object?>>>
        LogoutAsync()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok(
            new RetroGamesResponseApiModel<object?>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = null,
            });
    }

    [Authorize]
    [HttpGet("eu")]
    public ActionResult<
        RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>
        Eu()
    {
        return Ok(
            new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = MontarRespostaUsuario(User),
            });
    }

    private static UsuarioLogadoResponseApiModel MontarRespostaUsuario(
        ClaimsPrincipal usuario)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = usuario.ObterIdUsuario(),
            Nome = usuario.FindFirst(ClaimTypes.Name)?.Value
                ?? string.Empty,
            Email = usuario.FindFirst(ClaimTypes.Email)?.Value
                ?? string.Empty,
            Perfil = usuario.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty,
        };
    }
}