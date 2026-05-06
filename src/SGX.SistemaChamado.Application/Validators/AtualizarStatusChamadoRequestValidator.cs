using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarStatusChamadoRequestValidator : AbstractValidator<AtualizarStatusChamadoRequest>
{
    public AtualizarStatusChamadoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.Codigo)
            .InclusiveBetween(1, 99).WithMessage("Codigo de status invalido.");
    }
}
