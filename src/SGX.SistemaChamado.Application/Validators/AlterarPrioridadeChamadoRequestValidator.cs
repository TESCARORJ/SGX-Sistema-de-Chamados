using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AlterarPrioridadeChamadoRequestValidator : AbstractValidator<AlterarPrioridadeChamadoRequest>
{
    public AlterarPrioridadeChamadoRequestValidator()
    {
        RuleFor(x => x.PrioridadeId)
            .NotEmpty().WithMessage("Prioridade obrigatoria.");
    }
}
