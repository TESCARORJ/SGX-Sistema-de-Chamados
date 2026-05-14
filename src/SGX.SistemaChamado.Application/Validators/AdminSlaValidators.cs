using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroPoliticaSlaRequestValidator : AbstractValidator<FiltroPoliticaSlaRequest>
{
    public FiltroPoliticaSlaRequestValidator()
    {
        RuleFor(x => x.Texto)
            .MaximumLength(160)
            .When(x => !string.IsNullOrWhiteSpace(x.Texto));
    }
}

public sealed class MetaSlaUpsertRequestValidator : AbstractValidator<MetaSlaUpsertRequest>
{
    public MetaSlaUpsertRequestValidator()
    {
        RuleFor(x => x.PrioridadeId)
            .NotEmpty()
            .WithMessage("Prioridade e obrigatoria.");

        RuleFor(x => x.TempoPrimeiraRespostaMinutos)
            .GreaterThan(0)
            .WithMessage("Tempo de primeira resposta deve ser maior que zero.");

        RuleFor(x => x.TempoResolucaoMinutos)
            .GreaterThan(0)
            .WithMessage("Tempo de resolucao deve ser maior que zero.");

        RuleFor(x => x.TempoAtualizacaoMinutos)
            .GreaterThan(0)
            .When(x => x.TempoAtualizacaoMinutos.HasValue)
            .WithMessage("Tempo de atualizacao deve ser maior que zero quando informado.");

        RuleFor(x => x.TempoRespostaSubsequenteMinutos)
            .GreaterThan(0)
            .When(x => x.TempoRespostaSubsequenteMinutos.HasValue)
            .WithMessage("Tempo de resposta subsequente deve ser maior que zero quando informado.");
    }
}

public sealed class CriarPoliticaSlaRequestValidator : AbstractValidator<CriarPoliticaSlaRequest>
{
    public CriarPoliticaSlaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da politica de SLA e obrigatorio.")
            .MaximumLength(160);

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem da politica de SLA deve ser maior que zero.");

        RuleFor(x => x.Metas)
            .NotNull()
            .Must(metas => metas is { Count: > 0 })
            .WithMessage("A politica de SLA deve possuir ao menos uma meta.");

        RuleForEach(x => x.Metas).SetValidator(new MetaSlaUpsertRequestValidator());
    }
}

public sealed class AtualizarPoliticaSlaRequestValidator : AbstractValidator<AtualizarPoliticaSlaRequest>
{
    public AtualizarPoliticaSlaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da politica de SLA e obrigatorio.")
            .MaximumLength(160);

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao));

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem da politica de SLA deve ser maior que zero.");

        RuleFor(x => x.Metas)
            .NotNull()
            .Must(metas => metas is { Count: > 0 })
            .WithMessage("A politica de SLA deve possuir ao menos uma meta.");

        RuleForEach(x => x.Metas).SetValidator(new MetaSlaUpsertRequestValidator());
    }
}
