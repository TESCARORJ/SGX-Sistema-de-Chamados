using System.Globalization;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Worker.Email.Services;

public sealed class MailKitEmailImapClient(
    IOptions<EmailWorkerOptions> emailWorkerOptions) : IEmailImapClient
{
    public async Task<IReadOnlyCollection<EmailMessageData>> LerMensagensAsync(int maxMensagens, CancellationToken cancellationToken = default)
    {
        var options = emailWorkerOptions.Value;
        using var client = new ImapClient();
        await ConectarAsync(client, options, cancellationToken);

        var inbox = await AbrirPastaAsync(client, options.Pasta, FolderAccess.ReadWrite, cancellationToken);
        var uids = await inbox.SearchAsync(SearchQuery.NotSeen, cancellationToken);

        var selecionadas = uids
            .OrderBy(x => x.Id)
            .Take(Math.Max(1, maxMensagens))
            .ToArray();

        var mensagens = new List<EmailMessageData>(selecionadas.Length);
        foreach (var uid in selecionadas)
        {
            var mimeMessage = await inbox.GetMessageAsync(uid, cancellationToken);
            mensagens.Add(await MapearMensagemAsync(uid, mimeMessage, cancellationToken));
        }

        await client.DisconnectAsync(true, cancellationToken);
        return mensagens;
    }

    public async Task MarcarComoLidaAsync(string identificador, CancellationToken cancellationToken = default)
    {
        var options = emailWorkerOptions.Value;
        using var client = new ImapClient();
        await ConectarAsync(client, options, cancellationToken);

        var inbox = await AbrirPastaAsync(client, options.Pasta, FolderAccess.ReadWrite, cancellationToken);
        if (TryParseUid(identificador, out var uid))
        {
            await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, cancellationToken);
        }

        await client.DisconnectAsync(true, cancellationToken);
    }

    public async Task MoverMensagemAsync(string identificador, string pastaDestino, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pastaDestino))
        {
            return;
        }

        var options = emailWorkerOptions.Value;
        using var client = new ImapClient();
        await ConectarAsync(client, options, cancellationToken);

        var inbox = await AbrirPastaAsync(client, options.Pasta, FolderAccess.ReadWrite, cancellationToken);
        var destino = await ObterOuCriarPastaAsync(client, pastaDestino, cancellationToken);

        if (TryParseUid(identificador, out var uid))
        {
            await inbox.MoveToAsync(uid, destino, cancellationToken);
        }

        await client.DisconnectAsync(true, cancellationToken);
    }

    private async Task ConectarAsync(ImapClient client, EmailWorkerOptions options, CancellationToken cancellationToken)
    {
        var secureSocketOptions = options.SslHabilitado
            ? SecureSocketOptions.SslOnConnect
            : options.TlsHabilitado
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

        await client.ConnectAsync(options.ImapHost, options.ImapPorta, secureSocketOptions, cancellationToken);
        await client.AuthenticateAsync(options.Usuario, options.Senha, cancellationToken);
    }

    private static async Task<IMailFolder> AbrirPastaAsync(ImapClient client, string nomePasta, FolderAccess acesso, CancellationToken cancellationToken)
    {
        var pasta = await client.GetFolderAsync(nomePasta, cancellationToken);
        await pasta.OpenAsync(acesso, cancellationToken);
        return pasta;
    }

    private static async Task<IMailFolder> ObterOuCriarPastaAsync(ImapClient client, string nomePasta, CancellationToken cancellationToken)
    {
        var pastaRaiz = client.GetFolder(client.PersonalNamespaces[0]);

        try
        {
            var existente = await pastaRaiz.GetSubfolderAsync(nomePasta, cancellationToken);
            if (!existente.IsOpen)
            {
                await existente.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
            }

            return existente;
        }
        catch (FolderNotFoundException)
        {
            var criada = await pastaRaiz.CreateAsync(nomePasta, true, cancellationToken);
            if (criada is null)
            {
                throw new InvalidOperationException($"Nao foi possivel criar a pasta IMAP '{nomePasta}'.");
            }

            await criada.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
            return criada;
        }
    }

    private static async Task<EmailMessageData> MapearMensagemAsync(UniqueId uid, MimeMessage mimeMessage, CancellationToken cancellationToken)
    {
        var remetente = mimeMessage.From.Mailboxes.FirstOrDefault();
        var destinatarios = mimeMessage.To.Mailboxes
            .Select(x => x.Address)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var anexos = new List<EmailAttachmentData>();

        foreach (var attachment in mimeMessage.Attachments)
        {
            switch (attachment)
            {
                case MimePart mimePart:
                {
                    await using var stream = new MemoryStream();
                    if (mimePart.Content is not null)
                    {
                        await mimePart.Content.DecodeToAsync(stream, cancellationToken);
                    }

                    var nome = string.IsNullOrWhiteSpace(mimePart.FileName) ? "anexo.bin" : mimePart.FileName;
                    var contentType = mimePart.ContentType?.MimeType ?? "application/octet-stream";
                    var conteudo = stream.ToArray();
                    anexos.Add(new EmailAttachmentData(nome, contentType, conteudo, conteudo.LongLength, mimePart.ContentId));
                    break;
                }
                case MessagePart messagePart:
                {
                    await using var stream = new MemoryStream();
                    if (messagePart.Message is not null)
                    {
                        await messagePart.Message.WriteToAsync(stream, cancellationToken);
                    }

                    var nome = messagePart.ContentDisposition?.FileName ??
                               messagePart.ContentType?.Name ??
                               "mensagem.eml";
                    var conteudo = stream.ToArray();
                    anexos.Add(new EmailAttachmentData(nome, "message/rfc822", conteudo, conteudo.LongLength, messagePart.ContentId));
                    break;
                }
            }
        }

        return new EmailMessageData
        {
            Identificador = uid.Id.ToString(CultureInfo.InvariantCulture),
            MessageId = NormalizarHeader(mimeMessage.MessageId),
            InReplyTo = NormalizarHeader(mimeMessage.InReplyTo),
            References = mimeMessage.References.Select(NormalizarHeader).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()!,
            RemetenteEmail = remetente?.Address ?? string.Empty,
            Destinatario = destinatarios.Length == 0 ? null : string.Join(';', destinatarios),
            RemetenteNome = remetente?.Name,
            Assunto = mimeMessage.Subject ?? string.Empty,
            CorpoTexto = mimeMessage.TextBody,
            CorpoHtml = mimeMessage.HtmlBody,
            DataRecebimento = mimeMessage.Date != DateTimeOffset.MinValue
                ? mimeMessage.Date.UtcDateTime
                : DateTime.UtcNow,
            Anexos = anexos
        };
    }

    private static bool TryParseUid(string identificador, out UniqueId uniqueId)
    {
        uniqueId = default;
        if (!uint.TryParse(identificador, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor))
        {
            return false;
        }

        uniqueId = new UniqueId(valor);
        return true;
    }

    private static string? NormalizarHeader(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().Trim('<', '>');
    }
}
