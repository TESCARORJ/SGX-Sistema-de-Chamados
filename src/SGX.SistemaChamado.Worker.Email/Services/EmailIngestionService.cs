using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Worker.Email.Services;

public sealed class EmailIngestionService(
    IEmailImapClient emailImapClient,
    IEmailMessageProcessor emailMessageProcessor,
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
            EmailMensagemProcessamentoResultado resultado;
            try
            {
                resultado = await emailMessageProcessor.ProcessarAsync(mensagem, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha inesperada no processamento de mensagem. Identificador={Identificador}", mensagem.Identificador);
                resultado = new EmailMensagemProcessamentoResultado(EmailMensagemProcessamentoStatus.Erro, null, ex.Message);
            }

            try
            {
                if (options.MarcarComoLidaAoProcessar)
                {
                    await emailImapClient.MarcarComoLidaAsync(mensagem.Identificador, cancellationToken);
                }

                if (resultado.Status == EmailMensagemProcessamentoStatus.Erro)
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
