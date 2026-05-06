using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ComentarioAdminChamadoRequestValidator : AbstractValidator<ComentarioAdminChamadoRequest>
{
    public ComentarioAdminChamadoRequestValidator()
    {
        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("Mensagem obrigatoria.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Mensagem nao pode ser vazia.")
            .MaximumLength(3000).WithMessage("Mensagem deve ter no maximo 3000 caracteres.");
    }
}
