using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroCatalogoServicoRequestValidator : AbstractValidator<FiltroCatalogoServicoRequest>
{
    private static readonly string[] CamposOrdenacao = ["nome", "ordem", "criadoem", "atualizadoem"];

    public FiltroCatalogoServicoRequestValidator()
    {
        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.DepartamentoResponsavelId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoResponsavelId informado e invalido.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informado e invalido.");

        RuleFor(x => x.PrioridadePadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PrioridadePadraoId informado e invalido.");

        RuleFor(x => x.SlaPadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SlaPadraoId informado e invalido.");

        RuleFor(x => x.PoliticaSlaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PoliticaSlaId informado e invalido.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .When(x => x.Visibilidade.HasValue)
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser nome, ordem, criadoEm ou atualizadoEm.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}

public sealed class CriarCatalogoServicoRequestValidator : AbstractValidator<CriarCatalogoServicoRequest>
{
    public CriarCatalogoServicoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descricao e obrigatoria.")
            .MaximumLength(4000).WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.InstrucoesSolicitante)
            .MaximumLength(8000)
            .When(x => !string.IsNullOrWhiteSpace(x.InstrucoesSolicitante))
            .WithMessage("InstrucoesSolicitante deve ter no maximo 8000 caracteres.");

        RuleFor(x => x.DepartamentoResponsavelId)
            .NotEqual(Guid.Empty)
            .WithMessage("DepartamentoResponsavelId e obrigatorio.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informado e invalido.");

        RuleFor(x => x.PrioridadePadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PrioridadePadraoId informado e invalido.");

        RuleFor(x => x.SlaPadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SlaPadraoId informado e invalido.");

        RuleFor(x => x.PoliticaSlaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PoliticaSlaId informado e invalido.");

        RuleFor(x => x.ArtigoBaseConhecimentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ArtigoBaseConhecimentoId informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.Ordem)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Ordem nao pode ser negativa.");
    }
}

public sealed class AtualizarCatalogoServicoRequestValidator : AbstractValidator<AtualizarCatalogoServicoRequest>
{
    public AtualizarCatalogoServicoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descricao e obrigatoria.")
            .MaximumLength(4000).WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.InstrucoesSolicitante)
            .MaximumLength(8000)
            .When(x => !string.IsNullOrWhiteSpace(x.InstrucoesSolicitante))
            .WithMessage("InstrucoesSolicitante deve ter no maximo 8000 caracteres.");

        RuleFor(x => x.DepartamentoResponsavelId)
            .NotEqual(Guid.Empty)
            .WithMessage("DepartamentoResponsavelId e obrigatorio.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informado e invalido.");

        RuleFor(x => x.PrioridadePadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PrioridadePadraoId informado e invalido.");

        RuleFor(x => x.SlaPadraoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SlaPadraoId informado e invalido.");

        RuleFor(x => x.PoliticaSlaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("PoliticaSlaId informado e invalido.");

        RuleFor(x => x.ArtigoBaseConhecimentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ArtigoBaseConhecimentoId informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.Ordem)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Ordem nao pode ser negativa.");
    }
}
