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
            .NotEmpty().WithMessage("Categoria obrigatoria.");

        RuleFor(x => x.PrioridadeId)
            .NotEmpty().WithMessage("Prioridade obrigatoria.");

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
