using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MimeKit;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Infrastructure.Email;

namespace SGX.SistemaChamado.Tests;

public sealed class TransportadorEmailNotificacaoTests
{
    [Fact]
    public async Task DeveEnviarMensagemPorSmtpFalsoPreservandoRemetenteDestinatarioAssuntoEHtml()
    {
        await using var servidor = await ServidorSmtpFake.IniciarAsync();
        var transportador = CriarTransportador(new EmailOutboundOptions
        {
            Habilitado = true,
            Host = "127.0.0.1",
            Port = servidor.Porta,
            RemetenteEndereco = "nao-responda@sgx.local",
            RemetenteNome = "SGX Sistema de Chamados",
            UsarSsl = false
        });

        var response = await transportador.EnviarAsync(new MensagemEmailNotificacao(
            "destinatario@cliente.com",
            "Assunto çã",
            "<p>Olá <strong>cliente</strong></p>",
            true,
            "corr-email-001"));

        Assert.True(response.Sucesso);
        Assert.False(response.FalhaTransitoria);
        Assert.False(string.IsNullOrWhiteSpace(response.IdentificadorExterno));

        var mimeMessage = MimeMessage.Load(new MemoryStream(servidor.ObterMensagemRaw()));
        Assert.Equal("destinatario@cliente.com", mimeMessage.To.Mailboxes.Single().Address);
        Assert.Equal("nao-responda@sgx.local", mimeMessage.From.Mailboxes.Single().Address);
        Assert.Equal("Assunto çã", mimeMessage.Subject);
        Assert.Equal("<p>Olá <strong>cliente</strong></p>", mimeMessage.HtmlBody);
        Assert.Equal("corr-email-001", mimeMessage.Headers["X-SGX-Correlation-Key"]);
    }

    [Fact]
    public async Task DeveRetornarFalhaDefinitivaQuandoConfiguracaoForInvalida()
    {
        var transportador = CriarTransportador(new EmailOutboundOptions
        {
            Habilitado = true,
            Host = "",
            Port = 0,
            RemetenteEndereco = "invalido",
            RemetenteNome = "SGX",
            UsarSsl = false
        });

        var response = await transportador.EnviarAsync(new MensagemEmailNotificacao(
            "destinatario@cliente.com",
            "Assunto",
            "Conteudo",
            false,
            null));

        Assert.False(response.Sucesso);
        Assert.False(response.FalhaTransitoria);
        Assert.Contains("host", response.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornarFalhaDefinitivaQuandoDestinatarioDaMensagemForInvalido()
    {
        await using var servidor = await ServidorSmtpFake.IniciarAsync();
        var transportador = CriarTransportador(new EmailOutboundOptions
        {
            Habilitado = true,
            Host = "127.0.0.1",
            Port = servidor.Porta,
            RemetenteEndereco = "nao-responda@sgx.local",
            RemetenteNome = "SGX",
            UsarSsl = false
        });

        var response = await transportador.EnviarAsync(new MensagemEmailNotificacao(
            "destinatario-invalido",
            "Assunto",
            "Conteudo",
            false,
            null));

        Assert.False(response.Sucesso);
        Assert.False(response.FalhaTransitoria);
        Assert.Contains("destinatario", response.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveClassificarRespostaSmtp4xxComoFalhaTransitoria()
    {
        await using var servidor = await ServidorSmtpFake.IniciarAsync(respostaRcpt: "450 4.2.0 Mailbox busy");
        var transportador = CriarTransportador(new EmailOutboundOptions
        {
            Habilitado = true,
            Host = "127.0.0.1",
            Port = servidor.Porta,
            RemetenteEndereco = "nao-responda@sgx.local",
            RemetenteNome = "SGX",
            UsarSsl = false
        });

        var response = await transportador.EnviarAsync(new MensagemEmailNotificacao(
            "destinatario@cliente.com",
            "Assunto",
            "Conteudo",
            false,
            null));

        Assert.False(response.Sucesso);
        Assert.True(response.FalhaTransitoria);
        Assert.Contains("SMTP", response.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveClassificarRespostaSmtp5xxComoFalhaDefinitiva()
    {
        await using var servidor = await ServidorSmtpFake.IniciarAsync(respostaRcpt: "550 5.1.1 Mailbox unavailable");
        var transportador = CriarTransportador(new EmailOutboundOptions
        {
            Habilitado = true,
            Host = "127.0.0.1",
            Port = servidor.Porta,
            RemetenteEndereco = "nao-responda@sgx.local",
            RemetenteNome = "SGX",
            UsarSsl = false
        });

        var response = await transportador.EnviarAsync(new MensagemEmailNotificacao(
            "destinatario@cliente.com",
            "Assunto",
            "Conteudo",
            false,
            null));

        Assert.False(response.Sucesso);
        Assert.False(response.FalhaTransitoria);
        Assert.Contains("SMTP", response.Erro!, StringComparison.OrdinalIgnoreCase);
    }

    private static TransportadorEmailNotificacao CriarTransportador(EmailOutboundOptions options)
        => new(Options.Create(options), NullLogger<TransportadorEmailNotificacao>.Instance);

    private sealed class ServidorSmtpFake : IAsyncDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _loop;
        private readonly string _respostaRcpt;
        private readonly string _respostaDataFinal;
        private readonly List<byte> _mensagem = [];

        private ServidorSmtpFake(TcpListener listener, string respostaRcpt, string respostaDataFinal)
        {
            _listener = listener;
            _respostaRcpt = respostaRcpt;
            _respostaDataFinal = respostaDataFinal;
            _loop = Task.Run(ProcessarAsync);
        }

        public int Porta => ((IPEndPoint)_listener.LocalEndpoint).Port;

        public static Task<ServidorSmtpFake> IniciarAsync(
            string respostaRcpt = "250 2.1.5 OK",
            string respostaDataFinal = "250 2.0.0 Queued")
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return Task.FromResult(new ServidorSmtpFake(listener, respostaRcpt, respostaDataFinal));
        }

        public byte[] ObterMensagemRaw() => _mensagem.ToArray();

        private async Task ProcessarAsync()
        {
            try
            {
                using var client = await _listener.AcceptTcpClientAsync(_cts.Token);
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
                await using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true)
                {
                    NewLine = "\r\n",
                    AutoFlush = true
                };

                await writer.WriteLineAsync("220 localhost ESMTP SGX");
                var emData = false;

                while (!_cts.IsCancellationRequested)
                {
                    var linha = await reader.ReadLineAsync();
                    if (linha is null)
                    {
                        break;
                    }

                    if (emData)
                    {
                        if (linha == ".")
                        {
                            emData = false;
                            await writer.WriteLineAsync(_respostaDataFinal);
                            continue;
                        }

                        var bytes = Encoding.ASCII.GetBytes(linha + "\r\n");
                        _mensagem.AddRange(bytes);
                        continue;
                    }

                    if (linha.StartsWith("EHLO", StringComparison.OrdinalIgnoreCase) ||
                        linha.StartsWith("HELO", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("250-localhost");
                        await writer.WriteLineAsync("250 PIPELINING");
                        continue;
                    }

                    if (linha.StartsWith("MAIL FROM:", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("250 2.1.0 Sender OK");
                        continue;
                    }

                    if (linha.StartsWith("RCPT TO:", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync(_respostaRcpt);
                        continue;
                    }

                    if (linha.StartsWith("DATA", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("354 End data with <CR><LF>.<CR><LF>");
                        emData = true;
                        continue;
                    }

                    if (linha.StartsWith("QUIT", StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("221 2.0.0 Bye");
                        break;
                    }

                    await writer.WriteLineAsync("250 OK");
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _listener.Stop();
            try
            {
                await _loop;
            }
            catch
            {
            }
            _cts.Dispose();
        }
    }
}
