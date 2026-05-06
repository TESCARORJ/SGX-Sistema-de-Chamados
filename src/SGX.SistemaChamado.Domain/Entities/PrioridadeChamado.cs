using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PrioridadeChamado : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public PrioridadeChamadoEnum Nivel { get; private set; }
    public string? Descricao { get; private set; }
    public int PrazoPrimeiraRespostaHoras { get; private set; }
    public int PrazoResolucaoHoras { get; private set; }

    public ICollection<Chamado> Chamados { get; private set; } = [];
    public ICollection<SlaConfiguracao> SlaConfiguracoes { get; private set; } = [];

    private PrioridadeChamado()
    {
    }

    public PrioridadeChamado(
        string nome,
        PrioridadeChamadoEnum nivel,
        string? descricao,
        int prazoPrimeiraRespostaHoras,
        int prazoResolucaoHoras,
        string criadoPor)
    {
        DefinirNome(nome);
        Nivel = nivel;
        DefinirDescricao(descricao);
        DefinirPrazos(prazoPrimeiraRespostaHoras, prazoResolucaoHoras);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da prioridade e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirPrazos(int prazoPrimeiraRespostaHoras, int prazoResolucaoHoras)
    {
        if (prazoPrimeiraRespostaHoras < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoPrimeiraRespostaHoras), "O prazo de primeira resposta nao pode ser negativo.");
        }

        if (prazoResolucaoHoras < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoResolucaoHoras), "O prazo de resolucao nao pode ser negativo.");
        }

        PrazoPrimeiraRespostaHoras = prazoPrimeiraRespostaHoras;
        PrazoResolucaoHoras = prazoResolucaoHoras;
    }

    public void DefinirNivel(PrioridadeChamadoEnum nivel, string atualizadoPor)
    {
        Nivel = nivel;
        AtualizarAuditoria(atualizadoPor);
    }
}
