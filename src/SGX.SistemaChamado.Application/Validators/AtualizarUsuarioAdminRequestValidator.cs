using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarUsuarioAdminRequestValidator : AbstractValidator<AtualizarUsuarioAdminRequest>
{
    public AtualizarUsuarioAdminRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email obrigatorio.")
            .EmailAddress().WithMessage("Email invalido.");

        RuleFor(x => x.Login)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.Login));

        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId invalido.");
    }
}
