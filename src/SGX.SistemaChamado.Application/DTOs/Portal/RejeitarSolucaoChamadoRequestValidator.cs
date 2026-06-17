using FluentValidation;

namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class RejeitarSolucaoChamadoRequestValidator : AbstractValidator<RejeitarSolucaoChamadoRequest>
{
    public RejeitarSolucaoChamadoRequestValidator()
    {
        RuleFor(x => x.MotivoRejeicao)
            .NotEmpty().WithMessage("O motivo da rejeição é obrigatório.")
            .MaximumLength(2000).WithMessage("O motivo da rejeição não pode exceder 2000 caracteres.");
    }
}
