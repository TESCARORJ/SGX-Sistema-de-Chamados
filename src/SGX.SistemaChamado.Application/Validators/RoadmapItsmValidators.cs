using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroRoadmapItsmRequestValidator : AbstractValidator<FiltroRoadmapItsmRequest>
{
    public FiltroRoadmapItsmRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .When(x => x.Prioridade.HasValue)
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Impacto)
            .IsInEnum()
            .When(x => x.Impacto.HasValue)
            .WithMessage("Impacto informado e invalido.");

        RuleFor(x => x.Categoria)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.Categoria))
            .WithMessage("Categoria deve ter no maximo 120 caracteres.");
    }
}

public sealed class CriarRoadmapItsmItemRequestValidator : AbstractValidator<CriarRoadmapItsmItemRequest>
{
    public CriarRoadmapItsmItemRequestValidator()
    {
        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("Area e obrigatoria.")
            .MaximumLength(180).WithMessage("Area deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Categoria)
            .MaximumLength(120).WithMessage("Categoria deve ter no maximo 120 caracteres.");

        RuleFor(x => x.RoadmapCategoriaId)
            .NotEmpty().WithMessage("RoadmapCategoriaId e obrigatorio.");

        RuleFor(x => x.SituacaoAtual)
            .NotEmpty().WithMessage("Situacao atual e obrigatoria.")
            .MaximumLength(800).WithMessage("Situacao atual deve ter no maximo 800 caracteres.");

        RuleFor(x => x.AtencaoTecnica)
            .NotEmpty().WithMessage("Atencao tecnica e obrigatoria.")
            .MaximumLength(1200).WithMessage("Atencao tecnica deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Impacto)
            .IsInEnum()
            .WithMessage("Impacto informado e invalido.");

        RuleFor(x => x.Decisao)
            .IsInEnum()
            .WithMessage("Decisao informada e invalida.");

        RuleFor(x => x.Observacao)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");

        RuleFor(x => x.StatusImplementacao)
            .IsInEnum()
            .WithMessage("Status da implementacao informado e invalido.");

        RuleFor(x => x.StatusTecnico)
            .IsInEnum()
            .WithMessage("Status tecnico informado e invalido.");

        RuleFor(x => x.PercentualImplementacao)
            .InclusiveBetween(0, 100)
            .When(x => x.PercentualImplementacao.HasValue)
            .WithMessage("Percentual de implementacao deve estar entre 0 e 100.");

        RuleFor(x => x.PendenciasTecnicas)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.PendenciasTecnicas))
            .WithMessage("Pendencias tecnicas deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.PendenciasHomologacao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.PendenciasHomologacao))
            .WithMessage("Pendencias de homologacao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.EvidenciaImplementacao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.EvidenciaImplementacao))
            .WithMessage("Evidencia da implementacao deve ter no maximo 1000 caracteres.");

        RuleFor(x => x.CriterioAceite)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.CriterioAceite))
            .WithMessage("Criterio de aceite deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.ProximaAcao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.ProximaAcao))
            .WithMessage("Proxima acao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class AtualizarRoadmapItsmItemRequestValidator : AbstractValidator<AtualizarRoadmapItsmItemRequest>
{
    public AtualizarRoadmapItsmItemRequestValidator()
    {
        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("Area e obrigatoria.")
            .MaximumLength(180).WithMessage("Area deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Categoria)
            .MaximumLength(120).WithMessage("Categoria deve ter no maximo 120 caracteres.");

        RuleFor(x => x.RoadmapCategoriaId)
            .NotEmpty().WithMessage("RoadmapCategoriaId e obrigatorio.");

        RuleFor(x => x.SituacaoAtual)
            .NotEmpty().WithMessage("Situacao atual e obrigatoria.")
            .MaximumLength(800).WithMessage("Situacao atual deve ter no maximo 800 caracteres.");

        RuleFor(x => x.AtencaoTecnica)
            .NotEmpty().WithMessage("Atencao tecnica e obrigatoria.")
            .MaximumLength(1200).WithMessage("Atencao tecnica deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Impacto)
            .IsInEnum()
            .WithMessage("Impacto informado e invalido.");

        RuleFor(x => x.Decisao)
            .IsInEnum()
            .WithMessage("Decisao informada e invalida.");

        RuleFor(x => x.Observacao)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 1200 caracteres.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");

        RuleFor(x => x.StatusImplementacao)
            .IsInEnum()
            .WithMessage("Status da implementacao informado e invalido.");

        RuleFor(x => x.StatusTecnico)
            .IsInEnum()
            .WithMessage("Status tecnico informado e invalido.");

        RuleFor(x => x.PercentualImplementacao)
            .InclusiveBetween(0, 100)
            .When(x => x.PercentualImplementacao.HasValue)
            .WithMessage("Percentual de implementacao deve estar entre 0 e 100.");

        RuleFor(x => x.PendenciasTecnicas)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.PendenciasTecnicas))
            .WithMessage("Pendencias tecnicas deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.PendenciasHomologacao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.PendenciasHomologacao))
            .WithMessage("Pendencias de homologacao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.EvidenciaImplementacao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.EvidenciaImplementacao))
            .WithMessage("Evidencia da implementacao deve ter no maximo 1000 caracteres.");

        RuleFor(x => x.CriterioAceite)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.CriterioAceite))
            .WithMessage("Criterio de aceite deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.ProximaAcao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.ProximaAcao))
            .WithMessage("Proxima acao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class AtualizarStatusRoadmapItsmRequestValidator : AbstractValidator<AtualizarStatusRoadmapItsmRequest>
{
    public AtualizarStatusRoadmapItsmRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Decisao)
            .IsInEnum()
            .WithMessage("Decisao informada e invalida.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(1200)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 1200 caracteres.");
    }
}

public sealed class FiltroRoadmapImplementacaoFuturaRequestValidator : AbstractValidator<FiltroRoadmapImplementacaoFuturaRequest>
{
    public FiltroRoadmapImplementacaoFuturaRequestValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum()
            .When(x => x.Tipo.HasValue)
            .WithMessage("Tipo informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .When(x => x.Prioridade.HasValue)
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Texto)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Texto))
            .WithMessage("Texto deve ter no maximo 250 caracteres.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");
    }
}

public sealed class CriarRoadmapImplementacaoFuturaRequestValidator : AbstractValidator<CriarRoadmapImplementacaoFuturaRequest>
{
    public CriarRoadmapImplementacaoFuturaRequestValidator()
    {
        RuleFor(x => x.RoadmapItemId)
            .NotEmpty()
            .WithMessage("RoadmapItemId e obrigatorio.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(250).WithMessage("Titulo deve ter no maximo 250 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo informado e invalido.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");
    }
}

public sealed class AtualizarRoadmapImplementacaoFuturaRequestValidator : AbstractValidator<AtualizarRoadmapImplementacaoFuturaRequest>
{
    public AtualizarRoadmapImplementacaoFuturaRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(250).WithMessage("Titulo deve ter no maximo 250 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo informado e invalido.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("Prioridade informada e invalida.");

        RuleFor(x => x.Responsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Responsavel))
            .WithMessage("Responsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");
    }
}

public sealed class FiltroRoadmapCategoriaRequestValidator : AbstractValidator<FiltroRoadmapCategoriaRequest>
{
    public FiltroRoadmapCategoriaRequestValidator()
    {
        RuleFor(x => x.Texto)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.Texto))
            .WithMessage("Texto deve ter no maximo 120 caracteres.");
    }
}

public sealed class CriarRoadmapCategoriaRequestValidator : AbstractValidator<CriarRoadmapCategoriaRequest>
{
    public CriarRoadmapCategoriaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(120).WithMessage("Nome deve ter no maximo 120 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 1000 caracteres.");

        RuleFor(x => x.Cor)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Cor))
            .WithMessage("Cor deve ter no maximo 30 caracteres.");

        RuleFor(x => x.Icone)
            .MaximumLength(80)
            .When(x => !string.IsNullOrWhiteSpace(x.Icone))
            .WithMessage("Icone deve ter no maximo 80 caracteres.");
    }
}

public sealed class AtualizarRoadmapCategoriaRequestValidator : AbstractValidator<AtualizarRoadmapCategoriaRequest>
{
    public AtualizarRoadmapCategoriaRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(120).WithMessage("Nome deve ter no maximo 120 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 1000 caracteres.");

        RuleFor(x => x.Cor)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Cor))
            .WithMessage("Cor deve ter no maximo 30 caracteres.");

        RuleFor(x => x.Icone)
            .MaximumLength(80)
            .When(x => !string.IsNullOrWhiteSpace(x.Icone))
            .WithMessage("Icone deve ter no maximo 80 caracteres.");
    }
}

public sealed class CriarRoadmapChecklistItemRequestValidator : AbstractValidator<CriarRoadmapChecklistItemRequest>
{
    public CriarRoadmapChecklistItemRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(250).WithMessage("Titulo deve ter no maximo 250 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.Grupo)
            .IsInEnum()
            .WithMessage("Grupo informado e invalido.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");
    }
}

public sealed class AtualizarRoadmapChecklistItemRequestValidator : AbstractValidator<AtualizarRoadmapChecklistItemRequest>
{
    public AtualizarRoadmapChecklistItemRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(250).WithMessage("Titulo deve ter no maximo 250 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.Grupo)
            .IsInEnum()
            .WithMessage("Grupo informado e invalido.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");
    }
}
