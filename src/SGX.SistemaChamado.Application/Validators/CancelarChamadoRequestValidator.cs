using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CancelarChamadoRequestValidator : AbstractValidator<CancelarChamadoRequest>
{
    public CancelarChamadoRequestValidator()
    {
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo obrigatorio para cancelamento.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Motivo nao pode ser vazio.")
            .MaximumLength(3000).WithMessage("Motivo deve ter no maximo 3000 caracteres.");
    }
}
