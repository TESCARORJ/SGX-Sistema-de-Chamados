using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class MetaSla : AuditableEntity
{
    public Guid PoliticaSlaId { get; private set; }
    public Guid PrioridadeId { get; private set; }
    public int TempoPrimeiraRespostaMinutos { get; private set; }
    public int TempoResolucaoMinutos { get; private set; }
    public int? TempoAtualizacaoMinutos { get; private set; }
    public int? TempoRespostaSubsequenteMinutos { get; private set; }

    public PoliticaSla PoliticaSla { get; private set; } = default!;
    public PrioridadeChamado Prioridade { get; private set; } = default!;

    private MetaSla()
    {
    }

    public MetaSla(
        Guid politicaSlaId,
        Guid prioridadeId,
        int tempoPrimeiraRespostaMinutos,
        int tempoResolucaoMinutos,
        int? tempoAtualizacaoMinutos,
        int? tempoRespostaSubsequenteMinutos,
        string criadoPor)
    {
        if (politicaSlaId == Guid.Empty)
        {
            throw new ArgumentException("A politica de SLA e obrigatoria.", nameof(politicaSlaId));
        }

        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade da meta de SLA e obrigatoria.", nameof(prioridadeId));
        }

        ValidarTempo(tempoPrimeiraRespostaMinutos, nameof(tempoPrimeiraRespostaMinutos));
        ValidarTempo(tempoResolucaoMinutos, nameof(tempoResolucaoMinutos));
        ValidarTempoOpcional(tempoAtualizacaoMinutos, nameof(tempoAtualizacaoMinutos));
        ValidarTempoOpcional(tempoRespostaSubsequenteMinutos, nameof(tempoRespostaSubsequenteMinutos));

        PoliticaSlaId = politicaSlaId;
        PrioridadeId = prioridadeId;
        TempoPrimeiraRespostaMinutos = tempoPrimeiraRespostaMinutos;
        TempoResolucaoMinutos = tempoResolucaoMinutos;
        TempoAtualizacaoMinutos = tempoAtualizacaoMinutos;
        TempoRespostaSubsequenteMinutos = tempoRespostaSubsequenteMinutos;
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        Guid prioridadeId,
        int tempoPrimeiraRespostaMinutos,
        int tempoResolucaoMinutos,
        int? tempoAtualizacaoMinutos,
        int? tempoRespostaSubsequenteMinutos,
        bool ativo,
        string atualizadoPor)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade da meta de SLA e obrigatoria.", nameof(prioridadeId));
        }

        ValidarTempo(tempoPrimeiraRespostaMinutos, nameof(tempoPrimeiraRespostaMinutos));
        ValidarTempo(tempoResolucaoMinutos, nameof(tempoResolucaoMinutos));
        ValidarTempoOpcional(tempoAtualizacaoMinutos, nameof(tempoAtualizacaoMinutos));
        ValidarTempoOpcional(tempoRespostaSubsequenteMinutos, nameof(tempoRespostaSubsequenteMinutos));

        PrioridadeId = prioridadeId;
        TempoPrimeiraRespostaMinutos = tempoPrimeiraRespostaMinutos;
        TempoResolucaoMinutos = tempoResolucaoMinutos;
        TempoAtualizacaoMinutos = tempoAtualizacaoMinutos;
        TempoRespostaSubsequenteMinutos = tempoRespostaSubsequenteMinutos;

        if (ativo)
        {
            Ativar(atualizadoPor);
        }
        else
        {
            Desativar(atualizadoPor);
        }
    }

    private static void ValidarTempo(int valor, string parametro)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(parametro, "Os tempos de SLA devem ser maiores que zero.");
        }
    }

    private static void ValidarTempoOpcional(int? valor, string parametro)
    {
        if (valor.HasValue && valor.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(parametro, "Os tempos opcionais de SLA devem ser maiores que zero.");
        }
    }
}
