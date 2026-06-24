using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class RegistrarFalhaEntregaNotificacaoRequestValidator : AbstractValidator<RegistrarFalhaEntregaNotificacaoRequest>
{
    public RegistrarFalhaEntregaNotificacaoRequestValidator()
    {
        RuleFor(x => x.NotificacaoId)
            .NotEmpty()
            .WithMessage("A notificacao informada e obrigatoria.");

        RuleFor(x => x.Erro)
            .NotEmpty()
            .WithMessage("O erro da tentativa e obrigatorio.")
            .MaximumLength(Notificacao.MaximoUltimoErro)
            .WithMessage($"O valor informado deve possuir no maximo {Notificacao.MaximoUltimoErro} caracteres.");

        RuleFor(x => x.FalhouEm)
            .Must(data => data != default)
            .WithMessage("A data da falha e obrigatoria.");
    }
}
