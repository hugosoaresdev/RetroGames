using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order => -3000;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext contexto,
        ActionExecutionDelegate proximo)
    {
        var possuiArgumentoNulo =
            contexto.ActionArguments.Values.Any(argumento => argumento is null);

        var corpoVazio =
            contexto.HttpContext.Request.ContentLength is null or 0;

        if (possuiArgumentoNulo && corpoVazio)
        {
            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage,
                });

            return;
        }

        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
            {
                continue;
            }

            var tipoValidador = typeof(IValidator<>)
                .MakeGenericType(argumento.GetType());

            if (contexto.HttpContext.RequestServices
                    .GetService(tipoValidador) is not IValidator validador)
            {
                continue;
            }

            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                contexto.HttpContext.RequestAborted);

            if (resultado.IsValid)
            {
                continue;
            }

            var erros = resultado.Errors
                .GroupBy(erro =>
                    JsonNamingPolicy.CamelCase.ConvertName(erro.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Select(erro => erro.ErrorMessage).ToArray());

            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<Dictionary<string, string[]>>
                {
                    ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    Data = erros,
                    ErrorMessage =
                        ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                });

            return;
        }

        await proximo();
    }
}