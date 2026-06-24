using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class SelecionarNotificacoesProcessaveisRequestValidator : AbstractValidator<SelecionarNotificacoesProcessaveisRequest>
{
    public SelecionarNotificacoesProcessaveisRequestValidator()
    {
        RuleFor(x => x.Limite)
            .InclusiveBetween(1, 1000)
            .WithMessage("O limite deve estar entre 1 e 1000 notificacoes.");

        RuleFor(x => x.DataReferencia)
            .Must(data => data != default)
            .WithMessage("A data de referencia do processamento e obrigatoria.");
    }
}
