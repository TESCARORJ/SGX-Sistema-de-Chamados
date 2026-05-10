using SGX.SistemaChamado.Application.DTOs.Email;
using SGX.SistemaChamado.Application.Interfaces.Email;

namespace SGX.SistemaChamado.Application.Services.Email;

public sealed class EmailParaChamadoService(
    IEmailMessageProcessor emailMessageProcessor) : IEmailParaChamadoService
{
    public async Task<EmailProcessingResult> ProcessarAsync(EmailMessageDto mensagem, CancellationToken cancellationToken = default)
    {
        var resultado = await emailMessageProcessor.ProcessarAsync(
            new EmailMessageData
            {
                Identificador = mensagem.Identificador,
                MessageId = mensagem.MessageId,
                InReplyTo = mensagem.InReplyTo,
                References = mensagem.References,
                RemetenteEmail = mensagem.RemetenteEmail,
                RemetenteNome = mensagem.RemetenteNome,
                Destinatario = mensagem.Destinatario,
                Assunto = mensagem.Assunto,
                CorpoTexto = mensagem.CorpoTexto,
                CorpoHtml = mensagem.CorpoHtml,
                DataRecebimento = mensagem.DataRecebimento,
                Anexos = mensagem.Anexos
                    .Select(x => new EmailAttachmentData(x.NomeArquivo, x.ContentType, x.Conteudo, x.TamanhoBytes, x.ContentId))
                    .ToArray()
            },
            cancellationToken);

        return new EmailProcessingResult(
            resultado.Status switch
            {
                EmailMensagemProcessamentoStatus.Processado => EmailProcessingStatus.Processado,
                EmailMensagemProcessamentoStatus.Ignorado => EmailProcessingStatus.Ignorado,
                EmailMensagemProcessamentoStatus.Erro => EmailProcessingStatus.Erro,
                EmailMensagemProcessamentoStatus.Duplicado => EmailProcessingStatus.Duplicado,
                _ => EmailProcessingStatus.NaoCorrelacionado
            },
            resultado.ChamadoId,
            resultado.Erro);
    }
}
