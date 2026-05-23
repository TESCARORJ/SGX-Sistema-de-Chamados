using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroBaseConhecimentoArtigoRequestValidator : AbstractValidator<FiltroBaseConhecimentoArtigoRequest>
{
    private static readonly string[] CamposOrdenacao = ["criadoem", "atualizadoem"];

    public FiltroBaseConhecimentoArtigoRequestValidator()
    {
        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .When(x => x.Visibilidade.HasValue)
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser criadoEm ou atualizadoEm.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                          string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}

public sealed class CriarBaseConhecimentoArtigoRequestValidator : AbstractValidator<CriarBaseConhecimentoArtigoRequest>
{
    public CriarBaseConhecimentoArtigoRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Resumo)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Resumo))
            .WithMessage("Resumo deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Conteudo)
            .NotEmpty().WithMessage("Conteudo e obrigatorio.")
            .MaximumLength(20000).WithMessage("Conteudo deve ter no maximo 20000 caracteres.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.Tags)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Tags))
            .WithMessage("Tags deve ter no maximo 1200 caracteres.");
    }
}

public sealed class AtualizarBaseConhecimentoArtigoRequestValidator : AbstractValidator<AtualizarBaseConhecimentoArtigoRequest>
{
    public AtualizarBaseConhecimentoArtigoRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Resumo)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Resumo))
            .WithMessage("Resumo deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Conteudo)
            .NotEmpty().WithMessage("Conteudo e obrigatorio.")
            .MaximumLength(20000).WithMessage("Conteudo deve ter no maximo 20000 caracteres.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.Visibilidade)
            .IsInEnum()
            .WithMessage("Visibilidade informada e invalida.");

        RuleFor(x => x.Tags)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Tags))
            .WithMessage("Tags deve ter no maximo 1200 caracteres.");
    }
}