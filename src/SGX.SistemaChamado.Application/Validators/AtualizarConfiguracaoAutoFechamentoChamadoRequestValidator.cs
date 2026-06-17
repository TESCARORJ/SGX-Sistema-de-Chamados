using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator : AbstractValidator<AtualizarConfiguracaoAutoFechamentoChamadoRequest>
{
    public AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator()
    {
        RuleFor(x => x.PrazoAutoFechamentoHoras)
            .GreaterThanOrEqualTo(ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMinimoHoras)
            .WithMessage($"PrazoAutoFechamentoHoras deve ser maior ou igual a {ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMinimoHoras}.")
            .LessThanOrEqualTo(ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMaximoHoras)
            .WithMessage($"PrazoAutoFechamentoHoras deve ser menor ou igual a {ConfiguracaoAutoFechamentoChamadoConstantes.PrazoMaximoHoras}.");

        RuleFor(x => x.ObservacaoAlteracao)
            .MaximumLength(500)
            .WithMessage("ObservacaoAlteracao deve ter no maximo 500 caracteres.");
    }
}
