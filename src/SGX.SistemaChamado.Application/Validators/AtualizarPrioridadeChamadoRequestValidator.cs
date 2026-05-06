using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarPrioridadeChamadoRequestValidator : AbstractValidator<AtualizarPrioridadeChamadoRequest>
{
    public AtualizarPrioridadeChamadoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.Nivel)
            .InclusiveBetween(1, 4).WithMessage("Nivel deve estar entre 1 e 4.");

        RuleFor(x => x.PrazoPrimeiraRespostaHoras)
            .GreaterThanOrEqualTo(0).WithMessage("Prazo de primeira resposta nao pode ser negativo.");

        RuleFor(x => x.PrazoResolucaoHoras)
            .GreaterThanOrEqualTo(0).WithMessage("Prazo de resolucao nao pode ser negativo.");
    }
}
