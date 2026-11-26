using DotNetEnv;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.Services;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "A configuração ConnectionStrings__Default não foi encontrada no arquivo .env.");

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddAutenticacao();
builder.Services.AddAutorizacao();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services
    .AddControllers(opcoes =>
        opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.InvalidModelStateResponseFactory = contexto =>
            new BadRequestObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage =
                    ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
            });
    });
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var escopo = app.Services.CreateScope())
{
    var autenticacaoService =
        escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();

    var emailAdministrador =
        app.Configuration["AdminInicial:Email"]
        ?? throw new InvalidOperationException(
            "A configuração AdminInicial__Email não foi encontrada no arquivo .env.");

    var senhaAdministrador =
        app.Configuration["AdminInicial:Senha"]
        ?? throw new InvalidOperationException(
            "A configuração AdminInicial__Senha não foi encontrada no arquivo .env.");

    await autenticacaoService.GarantirAdministradorInicialAsync(
        emailAdministrador,
        senhaAdministrador);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.MapControllers();

app.Run();