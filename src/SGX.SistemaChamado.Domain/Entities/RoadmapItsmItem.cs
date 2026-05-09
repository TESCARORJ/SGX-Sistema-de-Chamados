using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class RoadmapItsmItem : AuditableEntity
{
    public string Area { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public Guid? RoadmapCategoriaId { get; private set; }
    public string SituacaoAtual { get; private set; } = string.Empty;
    public string AtencaoTecnica { get; private set; } = string.Empty;
    public StatusRoadmapItsm Status { get; private set; }
    public PrioridadeRoadmapItsm Prioridade { get; private set; }
    public ImpactoRoadmapItsm Impacto { get; private set; }
    public DecisaoRoadmapItsm Decisao { get; private set; }
    public string? Observacao { get; private set; }
    public string? Responsavel { get; private set; }
    public DateTime? PrazoAlvo { get; private set; }
    public int Ordem { get; private set; }
    public StatusImplementacaoRoadmapItsm StatusImplementacao { get; private set; } = StatusImplementacaoRoadmapItsm.NaoIniciado;
    public StatusTecnicoRoadmapItsm StatusTecnico { get; private set; } = StatusTecnicoRoadmapItsm.NaoAvaliado;
    public int PercentualImplementacao { get; private set; }
    public string? PendenciasTecnicas { get; private set; }
    public string? PendenciasHomologacao { get; private set; }
    public string? EvidenciaImplementacao { get; private set; }
    public DateTime? DataConclusaoTecnica { get; private set; }
    public DateTime? DataHomologacao { get; private set; }
    public string? CriterioAceite { get; private set; }
    public string? ProximaAcao { get; private set; }

    public ICollection<RoadmapImplementacaoFutura> ImplementacoesFuturas { get; private set; } = [];
    public ICollection<RoadmapChecklistItem> ChecklistItens { get; private set; } = [];
    public RoadmapCategoria? RoadmapCategoria { get; private set; }

    private RoadmapItsmItem()
    {
    }

    public RoadmapItsmItem(
        string area,
        string categoria,
        Guid? roadmapCategoriaId,
        string situacaoAtual,
        string atencaoTecnica,
        StatusRoadmapItsm status,
        PrioridadeRoadmapItsm prioridade,
        ImpactoRoadmapItsm impacto,
        DecisaoRoadmapItsm decisao,
        string? observacao,
        string? responsavel,
        DateTime? prazoAlvo,
        int ordem,
        StatusImplementacaoRoadmapItsm statusImplementacao,
        StatusTecnicoRoadmapItsm statusTecnico,
        int percentualImplementacao,
        string? pendenciasTecnicas,
        string? pendenciasHomologacao,
        string? evidenciaImplementacao,
        DateTime? dataConclusaoTecnica,
        DateTime? dataHomologacao,
        string? criterioAceite,
        string? proximaAcao,
        string criadoPor)
    {
        DefinirArea(area);
        DefinirCategoria(categoria);
        DefinirRoadmapCategoria(roadmapCategoriaId);
        DefinirSituacaoAtual(situacaoAtual);
        DefinirAtencaoTecnica(atencaoTecnica);
        DefinirStatus(status);
        DefinirPrioridade(prioridade);
        DefinirImpacto(impacto);
        DefinirDecisao(decisao);
        DefinirObservacao(observacao);
        DefinirResponsavel(responsavel);
        DefinirPrazoAlvo(prazoAlvo);
        DefinirOrdem(ordem);
        DefinirStatusImplementacao(statusImplementacao);
        DefinirStatusTecnico(statusTecnico);
        DefinirPercentualImplementacao(percentualImplementacao);
        DefinirPendenciasTecnicas(pendenciasTecnicas);
        DefinirPendenciasHomologacao(pendenciasHomologacao);
        DefinirEvidenciaImplementacao(evidenciaImplementacao);
        DefinirDataConclusaoTecnica(dataConclusaoTecnica);
        DefinirDataHomologacao(dataHomologacao);
        DefinirCriterioAceite(criterioAceite);
        DefinirProximaAcao(proximaAcao);
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        string area,
        string categoria,
        Guid? roadmapCategoriaId,
        string situacaoAtual,
        string atencaoTecnica,
        StatusRoadmapItsm status,
        PrioridadeRoadmapItsm prioridade,
        ImpactoRoadmapItsm impacto,
        DecisaoRoadmapItsm decisao,
        string? observacao,
        string? responsavel,
        DateTime? prazoAlvo,
        int ordem,
        StatusImplementacaoRoadmapItsm statusImplementacao,
        StatusTecnicoRoadmapItsm statusTecnico,
        int percentualImplementacao,
        string? pendenciasTecnicas,
        string? pendenciasHomologacao,
        string? evidenciaImplementacao,
        DateTime? dataConclusaoTecnica,
        DateTime? dataHomologacao,
        string? criterioAceite,
        string? proximaAcao,
        string atualizadoPor)
    {
        DefinirArea(area);
        DefinirCategoria(categoria);
        DefinirRoadmapCategoria(roadmapCategoriaId);
        DefinirSituacaoAtual(situacaoAtual);
        DefinirAtencaoTecnica(atencaoTecnica);
        DefinirStatus(status);
        DefinirPrioridade(prioridade);
        DefinirImpacto(impacto);
        DefinirDecisao(decisao);
        DefinirObservacao(observacao);
        DefinirResponsavel(responsavel);
        DefinirPrazoAlvo(prazoAlvo);
        DefinirOrdem(ordem);
        DefinirStatusImplementacao(statusImplementacao);
        DefinirStatusTecnico(statusTecnico);
        DefinirPercentualImplementacao(percentualImplementacao);
        DefinirPendenciasTecnicas(pendenciasTecnicas);
        DefinirPendenciasHomologacao(pendenciasHomologacao);
        DefinirEvidenciaImplementacao(evidenciaImplementacao);
        DefinirDataConclusaoTecnica(dataConclusaoTecnica);
        DefinirDataHomologacao(dataHomologacao);
        DefinirCriterioAceite(criterioAceite);
        DefinirProximaAcao(proximaAcao);
        AtualizarAuditoria(atualizadoPor);
    }

    public void AtualizarClassificacao(
        StatusRoadmapItsm status,
        PrioridadeRoadmapItsm prioridade,
        DecisaoRoadmapItsm decisao,
        string? responsavel,
        DateTime? prazoAlvo,
        string? observacao,
        string atualizadoPor)
    {
        DefinirStatus(status);
        DefinirPrioridade(prioridade);
        DefinirDecisao(decisao);
        DefinirResponsavel(responsavel);
        DefinirPrazoAlvo(prazoAlvo);
        DefinirObservacao(observacao);
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirArea(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
        {
            throw new ArgumentException("A area do roadmap e obrigatoria.", nameof(area));
        }

        Area = area.Trim();
    }

    public void DefinirCategoria(string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
        {
            throw new ArgumentException("A categoria do roadmap e obrigatoria.", nameof(categoria));
        }

        Categoria = categoria.Trim();
    }

    public void DefinirRoadmapCategoria(Guid? roadmapCategoriaId)
    {
        if (roadmapCategoriaId.HasValue && roadmapCategoriaId.Value == Guid.Empty)
        {
            throw new ArgumentException("RoadmapCategoriaId invalido.", nameof(roadmapCategoriaId));
        }

        RoadmapCategoriaId = roadmapCategoriaId;
    }

    public void DefinirSituacaoAtual(string situacaoAtual)
    {
        if (string.IsNullOrWhiteSpace(situacaoAtual))
        {
            throw new ArgumentException("A situacao atual e obrigatoria.", nameof(situacaoAtual));
        }

        SituacaoAtual = situacaoAtual.Trim();
    }

    public void DefinirAtencaoTecnica(string atencaoTecnica)
    {
        if (string.IsNullOrWhiteSpace(atencaoTecnica))
        {
            throw new ArgumentException("A atencao tecnica e obrigatoria.", nameof(atencaoTecnica));
        }

        AtencaoTecnica = atencaoTecnica.Trim();
    }

    public void DefinirStatus(StatusRoadmapItsm status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Status do roadmap invalido.", nameof(status));
        }

        Status = status;
    }

    public void DefinirPrioridade(PrioridadeRoadmapItsm prioridade)
    {
        if (!Enum.IsDefined(prioridade))
        {
            throw new ArgumentException("Prioridade do roadmap invalida.", nameof(prioridade));
        }

        Prioridade = prioridade;
    }

    public void DefinirImpacto(ImpactoRoadmapItsm impacto)
    {
        if (!Enum.IsDefined(impacto))
        {
            throw new ArgumentException("Impacto do roadmap invalido.", nameof(impacto));
        }

        Impacto = impacto;
    }

    public void DefinirDecisao(DecisaoRoadmapItsm decisao)
    {
        if (!Enum.IsDefined(decisao))
        {
            throw new ArgumentException("Decisao do roadmap invalida.", nameof(decisao));
        }

        Decisao = decisao;
    }

    public void DefinirObservacao(string? observacao)
    {
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
    }

    public void DefinirResponsavel(string? responsavel)
    {
        Responsavel = string.IsNullOrWhiteSpace(responsavel) ? null : responsavel.Trim();
    }

    public void DefinirPrazoAlvo(DateTime? prazoAlvo)
    {
        if (!prazoAlvo.HasValue)
        {
            PrazoAlvo = null;
            return;
        }

        var data = prazoAlvo.Value.Date;
        PrazoAlvo = DateTime.SpecifyKind(data, DateTimeKind.Utc);
    }

    public void DefinirOrdem(int ordem)
    {
        if (ordem <= 0)
        {
            throw new ArgumentException("A ordem do roadmap deve ser maior que zero.", nameof(ordem));
        }

        Ordem = ordem;
    }

    public void DefinirStatusImplementacao(StatusImplementacaoRoadmapItsm statusImplementacao)
    {
        if (!Enum.IsDefined(statusImplementacao))
        {
            throw new ArgumentException("Status de implementacao invalido.", nameof(statusImplementacao));
        }

        StatusImplementacao = statusImplementacao;
    }

    public void DefinirStatusTecnico(StatusTecnicoRoadmapItsm statusTecnico)
    {
        if (!Enum.IsDefined(statusTecnico))
        {
            throw new ArgumentException("Status tecnico invalido.", nameof(statusTecnico));
        }

        StatusTecnico = statusTecnico;
    }

    public void DefinirPercentualImplementacao(int percentualImplementacao)
    {
        if (percentualImplementacao < 0 || percentualImplementacao > 100)
        {
            throw new ArgumentException("Percentual de implementacao deve estar entre 0 e 100.", nameof(percentualImplementacao));
        }

        PercentualImplementacao = percentualImplementacao;
    }

    public void DefinirPendenciasTecnicas(string? pendenciasTecnicas)
    {
        PendenciasTecnicas = string.IsNullOrWhiteSpace(pendenciasTecnicas) ? null : pendenciasTecnicas.Trim();
    }

    public void DefinirPendenciasHomologacao(string? pendenciasHomologacao)
    {
        PendenciasHomologacao = string.IsNullOrWhiteSpace(pendenciasHomologacao) ? null : pendenciasHomologacao.Trim();
    }

    public void DefinirEvidenciaImplementacao(string? evidenciaImplementacao)
    {
        EvidenciaImplementacao = string.IsNullOrWhiteSpace(evidenciaImplementacao) ? null : evidenciaImplementacao.Trim();
    }

    public void DefinirDataConclusaoTecnica(DateTime? dataConclusaoTecnica)
    {
        if (!dataConclusaoTecnica.HasValue)
        {
            DataConclusaoTecnica = null;
            return;
        }

        var data = dataConclusaoTecnica.Value.Date;
        DataConclusaoTecnica = DateTime.SpecifyKind(data, DateTimeKind.Utc);
    }

    public void DefinirDataHomologacao(DateTime? dataHomologacao)
    {
        if (!dataHomologacao.HasValue)
        {
            DataHomologacao = null;
            return;
        }

        var data = dataHomologacao.Value.Date;
        DataHomologacao = DateTime.SpecifyKind(data, DateTimeKind.Utc);
    }

    public void DefinirCriterioAceite(string? criterioAceite)
    {
        CriterioAceite = string.IsNullOrWhiteSpace(criterioAceite) ? null : criterioAceite.Trim();
    }

    public void DefinirProximaAcao(string? proximaAcao)
    {
        ProximaAcao = string.IsNullOrWhiteSpace(proximaAcao) ? null : proximaAcao.Trim();
    }

    public void RecalcularPercentualImplementacao(IEnumerable<RoadmapChecklistItem>? checklistItens)
    {
        if (checklistItens is null)
        {
            return;
        }

        var ativos = checklistItens.Where(x => x.Ativo).ToArray();
        if (ativos.Length == 0)
        {
            return;
        }

        var concluidos = ativos.Count(x => x.Concluido);
        PercentualImplementacao = (int)Math.Round((concluidos * 100.0) / ativos.Length, MidpointRounding.AwayFromZero);
    }
}
