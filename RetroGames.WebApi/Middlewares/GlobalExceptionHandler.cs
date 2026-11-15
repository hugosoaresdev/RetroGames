using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is BusinessRuleException regra)
        {
            httpContext.Response.StatusCode = (int)regra.Erro.Status;

            await httpContext.Response.WriteAsJsonAsync(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = regra.Erro.Code,
                    ErrorMessage = regra.Erro.DefaultMessage,
                },
                cancellationToken);

            return true;
        }

        _logger.LogError(
            exception,
            "Erro não tratado ao processar a requisição.");

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro interno no servidor.",
            },
            cancellationToken);

        return true;
    }
}