using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AlterarCategoriaChamadoRequestValidator : AbstractValidator<AlterarCategoriaChamadoRequest>
{
    public AlterarCategoriaChamadoRequestValidator()
    {
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria obrigatoria.");

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
