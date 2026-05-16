using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarPrioridadeChamadoRequestValidator : AbstractValidator<AtualizarPrioridadeChamadoRequest>
{
    private const string CorHexRegex = "^#[0-9A-Fa-f]{6}$";

    public AtualizarPrioridadeChamadoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.Peso)
            .GreaterThan(0).WithMessage("Peso deve ser maior que zero.");

        RuleFor(x => x.Cor)
            .Matches(CorHexRegex)
            .When(x => !string.IsNullOrWhiteSpace(x.Cor))
            .WithMessage("Cor deve estar no formato hexadecimal #RRGGBB.");
    }
}
