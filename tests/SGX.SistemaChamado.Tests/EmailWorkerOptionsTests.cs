using Microsoft.Extensions.Configuration;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailWorkerOptionsTests
{
    [Fact]
    public void DeveCarregarConfiguracoesObrigatoriasViaVariaveisDeAmbiente()
    {
        const string prefixo = "SGX_TEST_EMAIL_WORKER_";
        var variaveis = new Dictionary<string, string?>
        {
            [$"{prefixo}EmailWorker__ImapHost"] = "imap.sgx.local",
            [$"{prefixo}EmailWorker__ImapPorta"] = "993",
            [$"{prefixo}EmailWorker__Usuario"] = "worker@sgx.local",
            [$"{prefixo}EmailWorker__Senha"] = "segredo",
            [$"{prefixo}EmailWorker__Pasta"] = "INBOX",
            [$"{prefixo}EmailWorker__SslHabilitado"] = "true",
            [$"{prefixo}EmailWorker__TlsHabilitado"] = "false",
            [$"{prefixo}EmailWorker__IntervaloSegundos"] = "45",
            [$"{prefixo}EmailWorker__MaxMensagensPorCiclo"] = "15",
            [$"{prefixo}EmailWorker__CategoriaPadraoId"] = "44444444-4444-4444-4444-444444444441",
            [$"{prefixo}EmailWorker__PrioridadePadraoId"] = "55555555-5555-5555-5555-555555555552",
            [$"{prefixo}EmailWorker__DepartamentoPadraoId"] = "66666666-6666-6666-6666-666666666601",
            [$"{prefixo}EmailWorker__DominiosPermitidos__0"] = "sgx.local",
            [$"{prefixo}EmailWorker__DominiosPermitidos__1"] = "empresa.com",
            [$"{prefixo}EmailWorker__TamanhoMaximoAnexoMb"] = "12",
            [$"{prefixo}EmailWorker__ExtensoesPermitidas__0"] = ".pdf",
            [$"{prefixo}EmailWorker__ExtensoesPermitidas__1"] = ".txt"
        };

        try
        {
            foreach (var (chave, valor) in variaveis)
            {
                Environment.SetEnvironmentVariable(chave, valor);
            }

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: prefixo)
                .Build();

            var options = new EmailWorkerOptions();
            configuration.GetSection(EmailWorkerOptions.SectionName).Bind(options);

            Assert.Equal("imap.sgx.local", options.ImapHost);
            Assert.Equal(993, options.ImapPorta);
            Assert.Equal("worker@sgx.local", options.Usuario);
            Assert.Equal("segredo", options.Senha);
            Assert.Equal("INBOX", options.Pasta);
            Assert.True(options.SslHabilitado);
            Assert.False(options.TlsHabilitado);
            Assert.Equal(45, options.IntervaloSegundos);
            Assert.Equal(15, options.MaxMensagensPorCiclo);
            Assert.Equal(Guid.Parse("44444444-4444-4444-4444-444444444441"), options.CategoriaPadraoId);
            Assert.Equal(Guid.Parse("55555555-5555-5555-5555-555555555552"), options.PrioridadePadraoId);
            Assert.Equal(Guid.Parse("66666666-6666-6666-6666-666666666601"), options.DepartamentoPadraoId);
            Assert.Equal(["sgx.local", "empresa.com"], options.DominiosPermitidos);
            Assert.Equal(12, options.TamanhoMaximoAnexoMb);
            Assert.Equal([".pdf", ".txt"], options.ExtensoesPermitidas);
            Assert.True(options.Configurado);
        }
        finally
        {
            foreach (var chave in variaveis.Keys)
            {
                Environment.SetEnvironmentVariable(chave, null);
            }
        }
    }
}
