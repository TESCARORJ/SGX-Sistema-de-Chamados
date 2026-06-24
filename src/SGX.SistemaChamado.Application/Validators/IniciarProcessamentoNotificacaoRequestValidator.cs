using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class IniciarProcessamentoNotificacaoRequestValidator : AbstractValidator<IniciarProcessamentoNotificacaoRequest>
{
    public IniciarProcessamentoNotificacaoRequestValidator()
    {
        RuleFor(x => x.NotificacaoId)
            .NotEmpty()
            .WithMessage("A notificacao informada e obrigatoria.");

        RuleFor(x => x.IniciadaEm)
            .Must(data => data != default)
            .WithMessage("A data de inicio do processamento e obrigatoria.");
    }
}
