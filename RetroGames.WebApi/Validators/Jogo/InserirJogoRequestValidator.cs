using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class InserirJogoRequestValidator
    : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(requisicao => requisicao.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter até 150 caracteres.");

        RuleFor(requisicao => requisicao.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter até 1000 caracteres.");

        RuleFor(requisicao => requisicao.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(requisicao => requisicao.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .Must(preco => preco == decimal.Round(preco, 2))
            .WithMessage("O preço deve ter no máximo duas casas decimais.");

        RuleFor(requisicao => requisicao.QuantidadeEstoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");

        RuleFor(requisicao => requisicao.IdPlataforma)
            .GreaterThan(0)
            .WithMessage("Informe a plataforma.");

        RuleFor(requisicao => requisicao.IdCondicaoJogo)
            .GreaterThan(0)
            .WithMessage("Informe a condição do jogo.");

        RuleFor(requisicao => requisicao.IdsGeneros)
            .NotEmpty()
            .WithMessage("Informe ao menos um gênero.");
    }
}