using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacao(
        this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = contexto =>
                    {
                        contexto.Response.StatusCode =
                            StatusCodes.Status401Unauthorized;

                        return contexto.Response.WriteAsJsonAsync(
                            new RetroGamesResponseApiModel<object?>
                            {
                                ResponseCode =
                                    ResponseCodes.NAO_AUTENTICADO.Code,
                                ErrorMessage =
                                    ResponseCodes.NAO_AUTENTICADO.DefaultMessage,
                                Data = null,
                            });
                    },

                    OnRedirectToAccessDenied = contexto =>
                    {
                        contexto.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        return contexto.Response.WriteAsJsonAsync(
                            new RetroGamesResponseApiModel<object?>
                            {
                                ResponseCode =
                                    ResponseCodes.ACESSO_NEGADO.Code,
                                ErrorMessage =
                                    ResponseCodes.ACESSO_NEGADO.DefaultMessage,
                                Data = null,
                            });
                    },
                };
            });

        return services;
    }
}