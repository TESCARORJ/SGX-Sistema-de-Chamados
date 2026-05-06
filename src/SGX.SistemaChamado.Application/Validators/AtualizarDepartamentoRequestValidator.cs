using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarDepartamentoRequestValidator : AbstractValidator<AtualizarDepartamentoRequest>
{
    public AtualizarDepartamentoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.Sigla)
            .NotEmpty().WithMessage("Sigla obrigatoria.")
            .MaximumLength(20);
    }
}
