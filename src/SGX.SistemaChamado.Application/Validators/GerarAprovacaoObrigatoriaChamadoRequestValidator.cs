using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class GerarAprovacaoObrigatoriaChamadoRequestValidator : AbstractValidator<GerarAprovacaoObrigatoriaChamadoRequest>
{
    public GerarAprovacaoObrigatoriaChamadoRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .NotEmpty()
            .WithMessage("ChamadoId e obrigatorio.");

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("TipoSolicitacaoId informado e invalido.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CatalogoServicoId informado e invalido.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informada e invalida.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informada e invalida.");

        RuleFor(x => x.SolicitanteId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SolicitanteId informado e invalido.");

        RuleFor(x => x)
            .Must(x => !x.SubcategoriaId.HasValue || x.CategoriaId.HasValue)
            .WithMessage("CategoriaId e obrigatoria quando SubcategoriaId for informada.");

        RuleFor(x => x.Custo)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.Custo.HasValue)
            .WithMessage("Custo nao pode ser negativo.");

        RuleFor(x => x.NivelRisco)
            .GreaterThan(0)
            .When(x => x.NivelRisco.HasValue)
            .WithMessage("NivelRisco deve ser maior que zero.");

        RuleFor(x => x.OrigemSolicitacao)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.OrigemSolicitacao))
            .WithMessage("OrigemSolicitacao deve ter no maximo 200 caracteres.");
    }
}
