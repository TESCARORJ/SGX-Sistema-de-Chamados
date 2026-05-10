using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Email;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Worker.Email.Services;

public sealed class EmailIngestionService(
    IEmailImapClient emailImapClient,
    IProcessarEmailRecebidoUseCase processarEmailRecebidoUseCase,
    IOptions<EmailWorkerOptions> emailWorkerOptions,
    ILogger<EmailIngestionService> logger)
{
    private static bool _jaLogouAvisoNaoConfigurado;

    public async Task ProcessarMensagensAsync(CancellationToken cancellationToken)
    {
        var options = emailWorkerOptions.Value;
        if (!options.Configurado)
        {
            if (!_jaLogouAvisoNaoConfigurado)
            {
                logger.LogWarning("Worker de e-mail iniciado sem configuracao IMAP completa. Nenhuma mensagem sera processada ate configurar EmailWorker__* .");
                _jaLogouAvisoNaoConfigurado = true;
            }

            return;
        }

        var mensagens = await emailImapClient.LerMensagensAsync(Math.Max(1, options.MaxMensagensPorCiclo), cancellationToken);
        if (mensagens.Count == 0)
        {
            return;
        }

        foreach (var mensagem in mensagens)
        {
            EmailProcessingResult resultado;
            try
            {
                resultado = await processarEmailRecebidoUseCase.ExecutarAsync(new EmailMessageDto
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
                        .Select(x => new EmailAttachmentDto(x.NomeArquivo, x.ContentType, x.Conteudo, x.TamanhoBytes, x.ContentId))
                        .ToArray()
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha inesperada no processamento de mensagem. Identificador={Identificador}", mensagem.Identificador);
                resultado = new EmailProcessingResult(EmailProcessingStatus.Erro, null, ex.Message);
            }

            try
            {
                if (options.MarcarComoLidaAoProcessar)
                {
                    await emailImapClient.MarcarComoLidaAsync(mensagem.Identificador, cancellationToken);
                }

                if (resultado.Status == EmailProcessingStatus.Erro)
                {
                    if (options.MoverComErro)
                    {
                        await emailImapClient.MoverMensagemAsync(mensagem.Identificador, options.PastaErro, cancellationToken);
                    }
                }
                else if (options.MoverProcessadas)
                {
                    await emailImapClient.MoverMensagemAsync(mensagem.Identificador, options.PastaProcessadas, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha ao aplicar pos-processamento IMAP (marcar/mover). Identificador={Identificador}", mensagem.Identificador);
            }
        }
    }
}
