using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.DTOs.Email;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Worker.Email.Services;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailIngestionServiceTests
{
    [Fact]
    public async Task FalhaEmUmaMensagemNaoInterrompeProcessamentoDoCiclo()
    {
        var mensagens = new List<EmailMessageData>
        {
            new()
            {
                Identificador = "1",
                MessageId = "msg-falha",
                RemetenteEmail = "falha@sgx.local",
                Assunto = "Falha",
                DataRecebimento = DateTime.UtcNow
            },
            new()
            {
                Identificador = "2",
                MessageId = "msg-ok",
                RemetenteEmail = "ok@sgx.local",
                Assunto = "OK",
                DataRecebimento = DateTime.UtcNow
            }
        };

        var fakeImap = new FakeEmailImapClient(mensagens);
        var fakeUseCase = new FakeProcessarEmailRecebidoUseCase();
        var options = Options.Create(new EmailWorkerOptions
        {
            ImapHost = "imap.sgx.local",
            ImapPorta = 993,
            Usuario = "worker@sgx.local",
            Senha = "segredo",
            Pasta = "INBOX",
            IntervaloSegundos = 10,
            MaxMensagensPorCiclo = 10
        });

        var service = new EmailIngestionService(fakeImap, fakeUseCase, options, NullLogger<EmailIngestionService>.Instance);

        var exception = await Record.ExceptionAsync(() => service.ProcessarMensagensAsync(CancellationToken.None));

        Assert.Null(exception);
        Assert.Equal(2, fakeUseCase.Processadas.Count);
        Assert.Equal(["1", "2"], fakeImap.MensagensMarcadasComoLidas);
    }

    private sealed class FakeEmailImapClient(IReadOnlyCollection<EmailMessageData> mensagens) : IEmailImapClient
    {
        public List<string> MensagensMarcadasComoLidas { get; } = [];

        public Task<IReadOnlyCollection<EmailMessageData>> LerMensagensAsync(int maxMensagens, CancellationToken cancellationToken = default)
            => Task.FromResult((IReadOnlyCollection<EmailMessageData>)mensagens.Take(maxMensagens).ToArray());

        public Task MarcarComoLidaAsync(string identificador, CancellationToken cancellationToken = default)
        {
            MensagensMarcadasComoLidas.Add(identificador);
            return Task.CompletedTask;
        }

        public Task MoverMensagemAsync(string identificador, string pastaDestino, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeProcessarEmailRecebidoUseCase : IProcessarEmailRecebidoUseCase
    {
        public List<string> Processadas { get; } = [];

        public Task<EmailProcessingResult> ExecutarAsync(EmailMessageDto mensagem, CancellationToken cancellationToken = default)
        {
            Processadas.Add(mensagem.Identificador);

            if (mensagem.Identificador == "1")
            {
                throw new InvalidOperationException("Erro forçado para teste.");
            }

            return Task.FromResult(new EmailProcessingResult(EmailProcessingStatus.Processado, Guid.NewGuid(), null));
        }
    }
}
