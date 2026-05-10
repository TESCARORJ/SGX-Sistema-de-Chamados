using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class LogIntegracaoEmail : AuditableEntity
{
    public string? MessageId { get; private set; }
    public string? InReplyTo { get; private set; }
    public string? References { get; private set; }
    public string Fingerprint { get; private set; } = string.Empty;
    public string Remetente { get; private set; } = string.Empty;
    public string? Destinatario { get; private set; }
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
        string? inReplyTo,
        string? references,
        string fingerprint,
        string remetente,
        string? destinatario,
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
        InReplyTo = string.IsNullOrWhiteSpace(inReplyTo) ? null : inReplyTo.Trim();
        References = string.IsNullOrWhiteSpace(references) ? null : references.Trim();
        Fingerprint = fingerprint.Trim();
        Remetente = remetente.Trim().ToLowerInvariant();
        Destinatario = string.IsNullOrWhiteSpace(destinatario) ? null : destinatario.Trim().ToLowerInvariant();
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
        StatusProcessamento = StatusProcessamentoEmail.Duplicado;
        Erro = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void MarcarIgnorado(DateTime dataProcessamento, string atualizadoPor, string? motivo = null, Guid? chamadoId = null)
    {
        ChamadoId = chamadoId;
        DataProcessamento = dataProcessamento;
        StatusProcessamento = StatusProcessamentoEmail.Ignorado;
        Erro = string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim();
        AtualizarAuditoria(atualizadoPor);
    }

    public void MarcarNaoCorrelacionado(DateTime dataProcessamento, string atualizadoPor, string? detalhe = null)
    {
        DataProcessamento = dataProcessamento;
        StatusProcessamento = StatusProcessamentoEmail.NaoCorrelacionado;
        Erro = string.IsNullOrWhiteSpace(detalhe) ? null : detalhe.Trim();
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

    public void AtualizarObservacao(string observacao, string atualizadoPor)
    {
        Erro = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
        AtualizarAuditoria(atualizadoPor);
    }
}
