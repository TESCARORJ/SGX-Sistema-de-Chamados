using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class EntregarNotificacaoEmailRequestValidator : AbstractValidator<EntregarNotificacaoEmailRequest>
{
    public EntregarNotificacaoEmailRequestValidator()
    {
        RuleFor(x => x.NotificacaoId)
            .NotEmpty()
            .WithMessage("A notificacao informada e obrigatoria.");

        RuleFor(x => x.EntregueEm)
            .Must(x => x != default)
            .WithMessage("A data de entrega e obrigatoria.")
            .Must(x => x.Kind == DateTimeKind.Utc)
            .WithMessage("A data de entrega deve estar em UTC.");
    }
}
