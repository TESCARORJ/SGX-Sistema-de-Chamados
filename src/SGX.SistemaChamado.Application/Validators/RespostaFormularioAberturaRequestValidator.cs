using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class RespostaFormularioAberturaRequestValidator : AbstractValidator<RespostaFormularioAberturaRequest>
{
    public const int TamanhoMaximoValor = 4000;

    public RespostaFormularioAberturaRequestValidator()
    {
        RuleFor(x => x.CampoFormularioServicoId)
            .NotEqual(Guid.Empty)
            .WithMessage("CampoFormularioServicoId e obrigatorio.");

        RuleFor(x => x)
            .Must(TerConteudoInformado)
            .WithMessage("Informe Valor ou Valores.");

        RuleFor(x => x)
            .Must(TerSomenteUmaFormaDeConteudo)
            .WithMessage("Informe apenas Valor ou Valores.");

        RuleFor(x => x.Valor)
            .MaximumLength(TamanhoMaximoValor)
            .When(x => !string.IsNullOrWhiteSpace(x.Valor))
            .WithMessage($"Valor deve ter no maximo {TamanhoMaximoValor} caracteres.");

        RuleForEach(x => x.Valores)
            .NotEmpty()
            .WithMessage("Valores nao pode conter itens vazios.")
            .MaximumLength(TamanhoMaximoValor)
            .WithMessage($"Cada item de Valores deve ter no maximo {TamanhoMaximoValor} caracteres.")
            .When(x => x.Valores is not null);
    }

    private static bool TerConteudoInformado(RespostaFormularioAberturaRequest request)
        => !string.IsNullOrWhiteSpace(request.Valor) || request.Valores?.Count > 0;

    private static bool TerSomenteUmaFormaDeConteudo(RespostaFormularioAberturaRequest request)
        => string.IsNullOrWhiteSpace(request.Valor) || request.Valores is null || request.Valores.Count == 0;
}
