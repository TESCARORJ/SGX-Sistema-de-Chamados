using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Repositories;

namespace SGX.SistemaChamado.Tests;

internal static class PortalUseCasesTestFactory
{
    public static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var context = new SGXSistemaChamadoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static Repository<T> Repo<T>(SGXSistemaChamadoDbContext context) where T : class => new(context);

    public static UnitOfWork Uow(SGXSistemaChamadoDbContext context) => new(context);

    public static IOptions<ArquivosOptions> ArquivosOptionsPadrao =>
        Options.Create(new ArquivosOptions
        {
            DiretorioAnexos = "storage/anexos-testes",
            TamanhoMaximoBytes = 10 * 1024 * 1024,
            ExtensoesPermitidas =
            [
                ".pdf",
                ".png",
                ".jpg",
                ".jpeg",
                ".txt",
                ".csv",
                ".zip",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx"
            ],
            ExtensoesBloqueadas =
            [
                ".exe", ".bat", ".cmd", ".ps1", ".sh", ".js", ".vbs", ".msi", ".dll", ".scr", ".com", ".jar", ".hta", ".reg"
            ],
            ContentTypesPermitidos =
            [
                "application/pdf",
                "image/png",
                "image/jpeg",
                "text/plain",
                "text/csv",
                "application/zip",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            ]
        });

    public static IOptions<EmailWorkerOptions> EmailWorkerOptionsPadrao =>
        Options.Create(new EmailWorkerOptions
        {
            ImapHost = "imap.teste.local",
            ImapPorta = 993,
            Usuario = "worker@teste.local",
            Senha = "nao-utilizada-em-testes",
            Pasta = "INBOX",
            SslHabilitado = true,
            TlsHabilitado = false,
            IntervaloSegundos = 60,
            MaxMensagensPorCiclo = 20,
            TamanhoMaximoAnexoMb = 10,
            ExtensoesPermitidas = [".pdf", ".png", ".jpg", ".jpeg", ".txt", ".doc", ".docx", ".xls", ".xlsx"]
        });
}

internal sealed class FakeUsuarioContextoAplicacaoService(UsuarioContextoAplicacao usuario) : IUsuarioContextoAplicacaoService
{
    public Task<UsuarioContextoAplicacao> ObterAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(usuario);
}

internal sealed class FakeArquivoStorageService : IArquivoStorageService
{
    public readonly List<ArquivoStorageRequest> Salvos = [];
    public readonly Dictionary<string, byte[]> Arquivos = new(StringComparer.OrdinalIgnoreCase);

    public Task<ArquivoStorageResult> SalvarAsync(ArquivoStorageRequest request, CancellationToken cancellationToken = default)
    {
        Salvos.Add(request);
        using var memory = new MemoryStream();
        request.Conteudo.Position = 0;
        request.Conteudo.CopyTo(memory);
        var caminho = $"storage/anexos-testes/{request.NomeFisico}";
        Arquivos[caminho] = memory.ToArray();
        return Task.FromResult(new ArquivoStorageResult(caminho, request.NomeFisico));
    }

    public Task<Stream> AbrirLeituraAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        if (!Arquivos.TryGetValue(caminhoRelativo, out var bytes))
        {
            throw new FileNotFoundException("Arquivo fisico nao encontrado.");
        }

        Stream stream = new MemoryStream(bytes, writable: false);
        return Task.FromResult(stream);
    }

    public Task RemoverAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

internal sealed class FakeCodigoChamadoService : ICodigoChamadoService
{
    private int _sequencial = 1;

    public Task<string> GerarAsync(CancellationToken cancellationToken = default)
    {
        var codigo = $"SGX-{DateTime.UtcNow.Year}-{_sequencial:D6}";
        _sequencial++;
        return Task.FromResult(codigo);
    }
}
