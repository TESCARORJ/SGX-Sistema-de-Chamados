using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarTipoSolicitacaoRequestValidator : AbstractValidator<AtualizarTipoSolicitacaoRequest>
{
    public AtualizarTipoSolicitacaoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.Descricao)
            .MaximumLength(500);
    }
}
