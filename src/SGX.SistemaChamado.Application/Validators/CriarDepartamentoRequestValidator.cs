using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarDepartamentoRequestValidator : AbstractValidator<CriarDepartamentoRequest>
{
    public CriarDepartamentoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.Sigla)
            .NotEmpty().WithMessage("Sigla obrigatoria.")
            .MaximumLength(20);
    }
}
