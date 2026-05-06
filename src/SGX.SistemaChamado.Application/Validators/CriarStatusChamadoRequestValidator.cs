using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarStatusChamadoRequestValidator : AbstractValidator<CriarStatusChamadoRequest>
{
    public CriarStatusChamadoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.Codigo)
            .InclusiveBetween(1, 99).WithMessage("Codigo de status invalido.");
    }
}
