using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PrioridadeChamado : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public PrioridadeChamadoEnum Nivel { get; private set; }
    public string? Descricao { get; private set; }
    public int Peso { get; private set; }
    public string? Cor { get; private set; } = "#1976D2";
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
        DefinirPesoECor((int)nivel, ObterCorPadrao(nivel));
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

    public void DefinirPesoECor(int peso, string? cor)
    {
        if (peso <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(peso), "O peso da prioridade deve ser maior que zero.");
        }

        Peso = peso;
        Cor = string.IsNullOrWhiteSpace(cor) ? null : cor.Trim().ToUpperInvariant();
    }

    public void DefinirNivel(PrioridadeChamadoEnum nivel, string atualizadoPor)
    {
        Nivel = nivel;
        DefinirPesoECor((int)nivel, ObterCorPadrao(nivel));
        AtualizarAuditoria(atualizadoPor);
    }

    private static string ObterCorPadrao(PrioridadeChamadoEnum nivel)
        => nivel switch
        {
            PrioridadeChamadoEnum.Baixa => "#2E7D32",
            PrioridadeChamadoEnum.Media => "#F9A825",
            PrioridadeChamadoEnum.Alta => "#EF6C00",
            PrioridadeChamadoEnum.Critica => "#C62828",
            _ => "#1976D2"
        };
}
