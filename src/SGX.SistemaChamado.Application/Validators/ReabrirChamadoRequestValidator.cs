using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ReabrirChamadoRequestValidator : AbstractValidator<ReabrirChamadoRequest>
{
    public ReabrirChamadoRequestValidator()
    {
        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("Mensagem obrigatoria para reabertura.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Mensagem nao pode ser vazia.")
            .MaximumLength(3000).WithMessage("Mensagem deve ter no maximo 3000 caracteres.");
    }
}
