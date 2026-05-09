using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class RoadmapImplementacaoFutura : AuditableEntity
{
    public Guid RoadmapItemId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public TipoRoadmapImplementacaoFutura Tipo { get; private set; }
    public PrioridadeRoadmapImplementacaoFutura Prioridade { get; private set; }
    public StatusRoadmapImplementacaoFutura Status { get; private set; }
    public string? Responsavel { get; private set; }
    public DateTime? PrazoAlvo { get; private set; }
    public DateTime? DataConclusao { get; private set; }
    public string? Observacao { get; private set; }

    public RoadmapItsmItem RoadmapItem { get; private set; } = null!;

    private RoadmapImplementacaoFutura()
    {
    }

    public RoadmapImplementacaoFutura(
        Guid roadmapItemId,
        string titulo,
        string? descricao,
        TipoRoadmapImplementacaoFutura tipo,
        PrioridadeRoadmapImplementacaoFutura prioridade,
        StatusRoadmapImplementacaoFutura status,
        string? responsavel,
        DateTime? prazoAlvo,
        DateTime? dataConclusao,
        string? observacao,
        string criadoPor)
    {
        DefinirRoadmapItemId(roadmapItemId);
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);
        DefinirTipo(tipo);
        DefinirPrioridade(prioridade);
        DefinirStatus(status);
        DefinirResponsavel(responsavel);
        DefinirPrazoAlvo(prazoAlvo);
        DefinirDataConclusao(dataConclusao);
        DefinirObservacao(observacao);
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        string titulo,
        string? descricao,
        TipoRoadmapImplementacaoFutura tipo,
        PrioridadeRoadmapImplementacaoFutura prioridade,
        StatusRoadmapImplementacaoFutura status,
        string? responsavel,
        DateTime? prazoAlvo,
        DateTime? dataConclusao,
        string? observacao,
        string atualizadoPor)
    {
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);
        DefinirTipo(tipo);
        DefinirPrioridade(prioridade);
        DefinirStatus(status);
        DefinirResponsavel(responsavel);
        DefinirPrazoAlvo(prazoAlvo);
        DefinirDataConclusao(dataConclusao);
        DefinirObservacao(observacao);
        AtualizarAuditoria(atualizadoPor);
    }

    public void Concluir(DateTime? dataConclusao, string atualizadoPor)
    {
        DefinirStatus(StatusRoadmapImplementacaoFutura.Concluido);
        DefinirDataConclusao(dataConclusao ?? DateTime.UtcNow);
        AtualizarAuditoria(atualizadoPor);
    }

    private void DefinirRoadmapItemId(Guid roadmapItemId)
    {
        if (roadmapItemId == Guid.Empty)
        {
            throw new ArgumentException("RoadmapItemId e obrigatorio.", nameof(roadmapItemId));
        }

        RoadmapItemId = roadmapItemId;
    }

    public void DefinirTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("Titulo e obrigatorio.", nameof(titulo));
        }

        Titulo = titulo.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirTipo(TipoRoadmapImplementacaoFutura tipo)
    {
        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentException("Tipo invalido.", nameof(tipo));
        }

        Tipo = tipo;
    }

    public void DefinirPrioridade(PrioridadeRoadmapImplementacaoFutura prioridade)
    {
        if (!Enum.IsDefined(prioridade))
        {
            throw new ArgumentException("Prioridade invalida.", nameof(prioridade));
        }

        Prioridade = prioridade;
    }

    public void DefinirStatus(StatusRoadmapImplementacaoFutura status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Status invalido.", nameof(status));
        }

        Status = status;
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

    public void DefinirDataConclusao(DateTime? dataConclusao)
    {
        if (!dataConclusao.HasValue)
        {
            DataConclusao = null;
            return;
        }

        var data = dataConclusao.Value.Date;
        DataConclusao = DateTime.SpecifyKind(data, DateTimeKind.Utc);
    }

    public void DefinirObservacao(string? observacao)
    {
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
    }
}
