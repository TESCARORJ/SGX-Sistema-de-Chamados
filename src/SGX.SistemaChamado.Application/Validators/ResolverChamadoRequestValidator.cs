using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ResolverChamadoRequestValidator : AbstractValidator<ResolverChamadoRequest>
{
    public ResolverChamadoRequestValidator()
    {
        RuleFor(x => x.Solucao)
            .NotEmpty().WithMessage("Solucao tecnica obrigatoria para resolucao.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Solucao nao pode ser vazia.")
            .MaximumLength(3000).WithMessage("Solucao deve ter no maximo 3000 caracteres.");
    }
}
