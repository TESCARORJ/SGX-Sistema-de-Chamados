using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarChamadoRequestValidator : AbstractValidator<CriarChamadoRequest>
{
    public CriarChamadoRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo obrigatorio.")
            .MaximumLength(180).WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descricao obrigatoria.")
            .MaximumLength(4000).WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.CategoriaId)
            .Must((request, valor) => request.CatalogoServicoId.HasValue || !string.IsNullOrWhiteSpace(request.CatalogoServicoSlug) || valor.HasValue)
            .WithMessage("Categoria obrigatoria quando nao houver servico de catalogo.")
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Categoria invalida.");

        RuleFor(x => x.PrioridadeId)
            .Must((request, valor) => request.CatalogoServicoId.HasValue || !string.IsNullOrWhiteSpace(request.CatalogoServicoSlug) || valor.HasValue)
            .WithMessage("Prioridade obrigatoria quando nao houver servico de catalogo.")
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Prioridade invalida.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Servico de catalogo invalido.");

        RuleFor(x => x.CatalogoServicoSlug)
            .MaximumLength(160)
            .WithMessage("Slug do servico de catalogo deve ter no maximo 160 caracteres.");

        RuleFor(x => x.SubcategoriaId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Subcategoria invalida.");

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Tipo de solicitacao invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Local/unidade invalido.");

        RuleFor(x => x.DepartamentoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Departamento invalido.");
    }
}
