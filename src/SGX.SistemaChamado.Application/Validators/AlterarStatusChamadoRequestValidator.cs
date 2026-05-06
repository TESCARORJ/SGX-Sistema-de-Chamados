using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AlterarStatusChamadoRequestValidator : AbstractValidator<AlterarStatusChamadoRequest>
{
    public AlterarStatusChamadoRequestValidator()
    {
        RuleFor(x => x.StatusId)
            .NotEmpty().WithMessage("Status obrigatorio.");
    }
}
