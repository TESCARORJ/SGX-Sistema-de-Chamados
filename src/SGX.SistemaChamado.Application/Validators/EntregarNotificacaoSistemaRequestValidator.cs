using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class EntregarNotificacaoSistemaRequestValidator : AbstractValidator<EntregarNotificacaoSistemaRequest>
{
    public EntregarNotificacaoSistemaRequestValidator()
    {
        RuleFor(x => x.NotificacaoId)
            .NotEmpty()
            .WithMessage("A notificacao informada e obrigatoria.");

        RuleFor(x => x.EntregueEm)
            .Must(data => data != default)
            .WithMessage("A data de entrega e obrigatoria.");
    }
}
