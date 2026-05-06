using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class SlaConfiguracao : AuditableEntity
{
    public Guid? DepartamentoId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid PrioridadeId { get; private set; }
    public int PrazoPrimeiraRespostaHoras { get; private set; }
    public int PrazoResolucaoHoras { get; private set; }

    public Departamento? Departamento { get; private set; }
    public CategoriaChamado? Categoria { get; private set; }
    public PrioridadeChamado Prioridade { get; private set; } = default!;

    private SlaConfiguracao()
    {
    }

    public SlaConfiguracao(
        Guid prioridadeId,
        int prazoPrimeiraRespostaHoras,
        int prazoResolucaoHoras,
        string criadoPor,
        Guid? departamentoId = null,
        Guid? categoriaId = null)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade do SLA e obrigatoria.", nameof(prioridadeId));
        }

        if (prazoPrimeiraRespostaHoras < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoPrimeiraRespostaHoras));
        }

        if (prazoResolucaoHoras < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoResolucaoHoras));
        }

        PrioridadeId = prioridadeId;
        PrazoPrimeiraRespostaHoras = prazoPrimeiraRespostaHoras;
        PrazoResolucaoHoras = prazoResolucaoHoras;
        DepartamentoId = departamentoId;
        CategoriaId = categoriaId;
        DefinirCriacao(criadoPor);
    }
}
