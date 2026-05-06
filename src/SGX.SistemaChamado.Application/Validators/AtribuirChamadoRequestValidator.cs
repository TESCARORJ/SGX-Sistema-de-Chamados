using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtribuirChamadoRequestValidator : AbstractValidator<AtribuirChamadoRequest>
{
    public AtribuirChamadoRequestValidator()
    {
        RuleFor(x => x.ResponsavelId)
            .NotEmpty().WithMessage("Responsavel obrigatorio.");
    }
}
