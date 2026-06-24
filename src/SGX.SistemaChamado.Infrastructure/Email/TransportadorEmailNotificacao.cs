using System.Net.Mail;
using System.Net.Sockets;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using MailKitSmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKitSmtpStatusCode = MailKit.Net.Smtp.SmtpStatusCode;

namespace SGX.SistemaChamado.Infrastructure.Email;

public sealed class TransportadorEmailNotificacao(
    IOptions<EmailOutboundOptions> emailOutboundOptions,
    ILogger<TransportadorEmailNotificacao> logger) : ITransportadorEmailNotificacao
{
    public async Task<ResultadoTransporteEmailNotificacao> EnviarAsync(
        MensagemEmailNotificacao mensagem,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = emailOutboundOptions.Value;
        var erroConfiguracao = ValidarConfiguracao(options);
        if (erroConfiguracao is not null)
        {
            logger.LogWarning("Envio outbound de e-mail bloqueado por configuracao invalida ou desabilitada.");
            return new ResultadoTransporteEmailNotificacao(false, false, null, erroConfiguracao);
        }

        var erroMensagem = ValidarMensagem(mensagem);
        if (erroMensagem is not null)
        {
            return new ResultadoTransporteEmailNotificacao(false, false, null, erroMensagem);
        }

        var mimeMessage = CriarMimeMessage(options, mensagem);

        try
        {
            using var client = new MailKitSmtpClient();
            await client.ConnectAsync(
                options.Host,
                options.Port,
                options.UsarSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(options.Usuario))
            {
                await client.AuthenticateAsync(options.Usuario, options.Senha ?? string.Empty, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation(
                "Mensagem outbound de e-mail enviada. Destinatario={DestinatarioMascarado}",
                MascararEndereco(mensagem.Destinatario));

            return new ResultadoTransporteEmailNotificacao(
                true,
                false,
                mimeMessage.MessageId,
                null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (SmtpCommandException ex)
        {
            var falhaTransitoria = EhFalhaTransitoria(ex.StatusCode);
            logger.LogWarning(
                ex,
                "Falha SMTP ao enviar notificacao outbound. Destinatario={DestinatarioMascarado} FalhaTransitoria={FalhaTransitoria}",
                MascararEndereco(mensagem.Destinatario),
                falhaTransitoria);

            return new ResultadoTransporteEmailNotificacao(
                false,
                falhaTransitoria,
                mimeMessage.MessageId,
                $"Falha SMTP: {ex.Message}");
        }
        catch (ServiceNotAuthenticatedException ex)
        {
            logger.LogWarning(ex, "Falha de autenticacao SMTP ao enviar notificacao outbound.");
            return new ResultadoTransporteEmailNotificacao(false, false, mimeMessage.MessageId, $"Falha de autenticacao SMTP: {ex.Message}");
        }
        catch (SslHandshakeException ex)
        {
            logger.LogWarning(ex, "Falha de configuracao SSL/TLS no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, false, mimeMessage.MessageId, $"Falha SSL/TLS: {ex.Message}");
        }
        catch (SaslException ex)
        {
            logger.LogWarning(ex, "Falha SASL no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, false, mimeMessage.MessageId, $"Falha de autenticacao SMTP: {ex.Message}");
        }
        catch (ServiceNotConnectedException ex)
        {
            logger.LogWarning(ex, "Falha de conexao SMTP no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, true, mimeMessage.MessageId, $"Falha de conexao SMTP: {ex.Message}");
        }
        catch (SmtpProtocolException ex)
        {
            logger.LogWarning(ex, "Falha de protocolo SMTP no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, true, mimeMessage.MessageId, $"Falha de protocolo SMTP: {ex.Message}");
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, "Falha de socket no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, true, mimeMessage.MessageId, $"Falha de conectividade SMTP: {ex.Message}");
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, "Falha de IO no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, true, mimeMessage.MessageId, $"Falha de IO SMTP: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            logger.LogWarning(ex, "Timeout no envio outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, true, mimeMessage.MessageId, $"Timeout SMTP: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha inesperada no transporte outbound de e-mail.");
            return new ResultadoTransporteEmailNotificacao(false, false, mimeMessage.MessageId, $"Falha inesperada no transporte de e-mail: {ex.Message}");
        }
    }

    private static MimeMessage CriarMimeMessage(EmailOutboundOptions options, MensagemEmailNotificacao mensagem)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse($"{options.RemetenteNome} <{options.RemetenteEndereco}>"));
        mimeMessage.To.Add(MailboxAddress.Parse(mensagem.Destinatario));
        mimeMessage.Subject = mensagem.Assunto.Trim();

        if (!string.IsNullOrWhiteSpace(mensagem.ChaveCorrelacao))
        {
            mimeMessage.Headers.Add("X-SGX-Correlation-Key", mensagem.ChaveCorrelacao);
        }

        var bodyBuilder = new BodyBuilder();
        if (mensagem.ConteudoHtml)
        {
            bodyBuilder.HtmlBody = mensagem.Conteudo;
        }
        else
        {
            bodyBuilder.TextBody = mensagem.Conteudo;
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();
        return mimeMessage;
    }

    private static string? ValidarConfiguracao(EmailOutboundOptions options)
    {
        if (!options.Habilitado)
        {
            return "O transporte outbound de e-mail esta desabilitado.";
        }

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return "A configuracao de envio outbound exige host SMTP.";
        }

        if (options.Port <= 0 || options.Port > 65535)
        {
            return "A configuracao de envio outbound exige porta SMTP valida.";
        }

        if (string.IsNullOrWhiteSpace(options.RemetenteEndereco) || !EhEnderecoValido(options.RemetenteEndereco))
        {
            return "A configuracao de envio outbound exige remetente valido.";
        }

        if (!string.IsNullOrWhiteSpace(options.Usuario) && string.IsNullOrWhiteSpace(options.Senha))
        {
            return "A configuracao de envio outbound exige senha quando usuario SMTP for informado.";
        }

        return null;
    }

    private static string? ValidarMensagem(MensagemEmailNotificacao mensagem)
    {
        if (string.IsNullOrWhiteSpace(mensagem.Destinatario) || !EhEnderecoValido(mensagem.Destinatario))
        {
            return "A mensagem outbound exige destinatario com endereco valido.";
        }

        if (string.IsNullOrWhiteSpace(mensagem.Assunto))
        {
            return "A mensagem outbound exige assunto.";
        }

        if (string.IsNullOrWhiteSpace(mensagem.Conteudo))
        {
            return "A mensagem outbound exige conteudo.";
        }

        return null;
    }

    private static bool EhEnderecoValido(string endereco)
    {
        try
        {
            _ = new MailAddress(endereco);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool EhFalhaTransitoria(MailKitSmtpStatusCode statusCode)
    {
        var codigo = (int)statusCode;
        return codigo >= 400 && codigo < 500;
    }

    private static string MascararEndereco(string endereco)
    {
        var partes = endereco.Split('@', 2, StringSplitOptions.TrimEntries);
        if (partes.Length != 2 || partes[0].Length <= 2)
        {
            return "***";
        }

        return $"{partes[0][..2]}***@{partes[1]}";
    }
}
