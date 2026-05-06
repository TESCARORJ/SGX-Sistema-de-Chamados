using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarParametroSistemaRequestValidator : AbstractValidator<CriarParametroSistemaRequest>
{
    public CriarParametroSistemaRequestValidator()
    {
        RuleFor(x => x.Chave)
            .NotEmpty().WithMessage("Chave obrigatoria.")
            .MaximumLength(180);

        RuleFor(x => x.Valor)
            .NotEmpty().WithMessage("Valor obrigatorio.")
            .MaximumLength(4000);
    }
}
