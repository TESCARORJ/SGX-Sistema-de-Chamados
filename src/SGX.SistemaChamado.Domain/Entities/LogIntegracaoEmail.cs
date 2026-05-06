using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class LogIntegracaoEmail : AuditableEntity
{
    public string? MessageId { get; private set; }
    public string Fingerprint { get; private set; } = string.Empty;
    public string Remetente { get; private set; } = string.Empty;
    public string? NomeRemetente { get; private set; }
    public string? Assunto { get; private set; }
    public DateTime DataRecebimento { get; private set; }
    public DateTime? DataProcessamento { get; private set; }
    public StatusProcessamentoEmail StatusProcessamento { get; private set; }
    public string? Erro { get; private set; }
    public Guid? ChamadoId { get; private set; }
    public int Tentativas { get; private set; }

    public Chamado? Chamado { get; private set; }

    private LogIntegracaoEmail()
    {
    }

    public LogIntegracaoEmail(
        string? messageId,
        string fingerprint,
        string remetente,
        string? nomeRemetente,
        string? assunto,
        DateTime dataRecebimento,
        string criadoPor)
    {
        if (string.IsNullOrWhiteSpace(fingerprint))
        {
            throw new ArgumentException("O fingerprint e obrigatorio.", nameof(fingerprint));
        }

        if (string.IsNullOrWhiteSpace(remetente))
        {
            throw new ArgumentException("O remetente e obrigatorio.", nameof(remetente));
        }

        MessageId = string.IsNullOrWhiteSpace(messageId) ? null : messageId.Trim();
        Fingerprint = fingerprint.Trim();
        Remetente = remetente.Trim().ToLowerInvariant();
        NomeRemetente = string.IsNullOrWhiteSpace(nomeRemetente) ? null : nomeRemetente.Trim();
        Assunto = string.IsNullOrWhiteSpace(assunto) ? null : assunto.Trim();
        DataRecebimento = dataRecebimento;
        StatusProcessamento = StatusProcessamentoEmail.Pendente;
        Tentativas = 0;
        DefinirCriacao(criadoPor);
    }

    public void RegistrarTentativa(string atualizadoPor)
    {
        Tentativas++;
        AtualizarAuditoria(atualizadoPor);
    }

    public void MarcarProcessado(Guid? chamadoId, DateTime dataProcessamento, string atualizadoPor)
    {
        ChamadoId = chamadoId;
        DataProcessamento = dataProcessamento;
        StatusProcessamento = StatusProcessamentoEmail.Processado;
        Erro = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void MarcarDuplicado(Guid? chamadoId, DateTime dataProcessamento, string atualizadoPor)
    {
        ChamadoId = chamadoId;
        DataProcessamento = dataProcessamento;
        StatusProcessamento = StatusProcessamentoEmail.IgnoradoDuplicado;
        Erro = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void MarcarErro(string erro, DateTime dataProcessamento, string atualizadoPor, Guid? chamadoId = null)
    {
        ChamadoId = chamadoId;
        DataProcessamento = dataProcessamento;
        StatusProcessamento = StatusProcessamentoEmail.Erro;
        Erro = string.IsNullOrWhiteSpace(erro) ? "Erro nao informado." : erro.Trim();
        AtualizarAuditoria(atualizadoPor);
    }
}
