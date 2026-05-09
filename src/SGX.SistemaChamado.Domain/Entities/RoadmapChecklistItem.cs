using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class RoadmapChecklistItem : AuditableEntity
{
    public Guid RoadmapItemId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public GrupoRoadmapChecklist Grupo { get; private set; }
    public int Ordem { get; private set; }
    public bool Concluido { get; private set; }
    public bool Obrigatorio { get; private set; }

    public RoadmapItsmItem RoadmapItem { get; private set; } = null!;

    private RoadmapChecklistItem()
    {
    }

    public RoadmapChecklistItem(
        Guid roadmapItemId,
        string titulo,
        string? descricao,
        GrupoRoadmapChecklist grupo,
        int ordem,
        bool concluido,
        bool obrigatorio,
        string criadoPor)
    {
        DefinirRoadmapItemId(roadmapItemId);
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);
        DefinirGrupo(grupo);
        DefinirOrdem(ordem);
        Concluido = concluido;
        Obrigatorio = obrigatorio;
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        string titulo,
        string? descricao,
        GrupoRoadmapChecklist grupo,
        int ordem,
        bool concluido,
        bool obrigatorio,
        string atualizadoPor)
    {
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);
        DefinirGrupo(grupo);
        DefinirOrdem(ordem);
        Concluido = concluido;
        Obrigatorio = obrigatorio;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Concluir(string atualizadoPor)
    {
        Concluido = true;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Reabrir(string atualizadoPor)
    {
        Concluido = false;
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

    public void DefinirGrupo(GrupoRoadmapChecklist grupo)
    {
        if (!Enum.IsDefined(grupo))
        {
            throw new ArgumentException("Grupo do checklist invalido.", nameof(grupo));
        }

        Grupo = grupo;
    }

    public void DefinirOrdem(int ordem)
    {
        if (ordem <= 0)
        {
            throw new ArgumentException("Ordem deve ser maior que zero.", nameof(ordem));
        }

        Ordem = ordem;
    }
}
