using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class EncerrarChamadoRequestValidator : AbstractValidator<EncerrarChamadoRequest>
{
    public EncerrarChamadoRequestValidator()
    {
        RuleFor(x => x.Solucao)
            .NotEmpty().WithMessage("Solucao obrigatoria para encerramento.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Solucao nao pode ser vazia.")
            .MaximumLength(3000).WithMessage("Solucao deve ter no maximo 3000 caracteres.");
    }
}
