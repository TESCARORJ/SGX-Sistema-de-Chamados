using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarLocalUnidadeRequestValidator : AbstractValidator<AtualizarLocalUnidadeRequest>
{
    public AtualizarLocalUnidadeRequestValidator()
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
