using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class RegistrarSucessoEntregaNotificacaoRequestValidator : AbstractValidator<RegistrarSucessoEntregaNotificacaoRequest>
{
    public RegistrarSucessoEntregaNotificacaoRequestValidator()
    {
        RuleFor(x => x.NotificacaoId)
            .NotEmpty()
            .WithMessage("A notificacao informada e obrigatoria.");

        RuleFor(x => x.EnviadaEm)
            .Must(data => data != default)
            .WithMessage("A data de envio e obrigatoria.");
    }
}
