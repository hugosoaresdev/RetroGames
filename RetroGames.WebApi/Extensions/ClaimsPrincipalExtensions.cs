using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil switch
        {
            PerfilIds.Administrador => "Administrador",
            PerfilIds.Cliente => "Cliente",
            _ => usuario.IdPerfil.ToString(),
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil),
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identidade);
    }

    public static int ObterIdUsuario(this ClaimsPrincipal usuarioAtual)
    {
        var idUsuario = usuarioAtual
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        if (!int.TryParse(idUsuario, out var id))
        {
            throw new InvalidOperationException(
                "O usuário autenticado não possui um id válido nas claims.");
        }

        return id;
    }
}