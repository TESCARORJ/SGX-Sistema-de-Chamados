using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarLocalUnidadeRequestValidator : AbstractValidator<CriarLocalUnidadeRequest>
{
    public CriarLocalUnidadeRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.Descricao)
            .MaximumLength(500);

        RuleFor(x => x.Endereco)
            .MaximumLength(500);
    }
}
