using Microsoft.AspNetCore.Http;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private System.Security.Claims.ClaimsPrincipal? UsuarioAtual =>
        _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado =>
        UsuarioAtual?.Identity?.IsAuthenticated ?? false;

    public int? IdUsuario =>
        EstaAutenticado
            ? UsuarioAtual!.ObterIdUsuario()
            : null;

    public bool EhAdministrador =>
        EstaAutenticado
        && UsuarioAtual!.IsInRole(nameof(PerfilIds.Administrador));
}