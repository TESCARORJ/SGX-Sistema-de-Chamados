using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequestValidator : AbstractValidator<ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest>
{
    public ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("ChamadoId e obrigatorio.");

        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.UsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("UsuarioId informado e invalido.");

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("Motivo e obrigatorio para registrar a reavaliacao.")
            .MaximumLength(4000)
            .WithMessage("Motivo deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.EscopoAnteriorSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoAnteriorSnapshot))
            .WithMessage("EscopoAnteriorSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.EscopoNovoSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoNovoSnapshot))
            .WithMessage("EscopoNovoSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.CustoAnterior)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CustoAnterior.HasValue)
            .WithMessage("CustoAnterior nao pode ser negativo.");

        RuleFor(x => x.CustoNovo)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CustoNovo.HasValue)
            .WithMessage("CustoNovo nao pode ser negativo.");

        RuleFor(x => x.NivelRiscoAnterior)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NivelRiscoAnterior.HasValue)
            .WithMessage("NivelRiscoAnterior nao pode ser negativo.");

        RuleFor(x => x.NivelRiscoNovo)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NivelRiscoNovo.HasValue)
            .WithMessage("NivelRiscoNovo nao pode ser negativo.");
    }
}
