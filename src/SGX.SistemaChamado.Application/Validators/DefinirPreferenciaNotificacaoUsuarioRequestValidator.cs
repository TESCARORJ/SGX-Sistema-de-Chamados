using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class DefinirPreferenciaNotificacaoUsuarioRequestValidator : AbstractValidator<DefinirPreferenciaNotificacaoUsuarioRequest>
{
    public DefinirPreferenciaNotificacaoUsuarioRequestValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty()
            .WithMessage("O usuario da preferencia de notificacao e obrigatorio.");

        RuleFor(x => x.TipoEvento)
            .Must(Enum.IsDefined)
            .WithMessage("O tipo de evento da preferencia de notificacao e invalido.");

        RuleFor(x => x.Canal)
            .Must(Enum.IsDefined)
            .WithMessage("O canal da preferencia de notificacao e invalido.");
    }
}
