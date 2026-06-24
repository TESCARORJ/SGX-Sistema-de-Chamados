using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AvaliarPreferenciaNotificacaoRequestValidator : AbstractValidator<AvaliarPreferenciaNotificacaoRequest>
{
    public AvaliarPreferenciaNotificacaoRequestValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty()
            .WithMessage("O usuario da avaliacao de preferencia de notificacao e obrigatorio.");

        RuleFor(x => x.TipoEvento)
            .Must(Enum.IsDefined)
            .WithMessage("O tipo de evento da avaliacao de preferencia de notificacao e invalido.");

        RuleFor(x => x.Canal)
            .Must(Enum.IsDefined)
            .WithMessage("O canal da avaliacao de preferencia de notificacao e invalido.");
    }
}
