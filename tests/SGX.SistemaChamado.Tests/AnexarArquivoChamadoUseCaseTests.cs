using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AnexarArquivoChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAceitarArquivoPermitido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Equal("evidencia.pdf", response.NomeArquivo);
        Assert.Single(context.AnexosChamado);
    }

    [Fact]
    public async Task DeveAceitarArquivoComContentTypeComParametros()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.txt",
            ContentType = "text/plain; charset=utf-8",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Equal("evidencia.txt", response.NomeArquivo);
        Assert.Single(context.AnexosChamado);
    }

    [Fact]
    public async Task DeveAceitarArquivoComContentTypeOctetStreamQuandoExtensaoForPermitida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.txt",
            ContentType = "application/octet-stream",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Equal("evidencia.txt", response.NomeArquivo);
        Assert.Single(context.AnexosChamado);
    }

    [Fact]
    public async Task DeveRejeitarArquivoAcimaDoLimite()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream(new byte[11 * 1024 * 1024]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "grande.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task DeveRejeitarTipoNaoPermitido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "script.exe",
            ContentType = "application/octet-stream",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task DeveCriarHistorico()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();

        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        await useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.txt",
            ContentType = "text/plain",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.AnexoAdicionado);
    }

    [Fact]
    public async Task SolicitanteNaoDeveAnexarEmChamadoDeOutroSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.ChamadoOutro.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task DeveRejeitarNomeDeArquivoComPathTraversal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedDados(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarUseCase(context, dados.UsuarioContexto, storage);
        var stream = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoProprio.Id, new UploadAnexoChamadoRequest
        {
            NomeArquivo = "..\\..\\passwd.txt",
            ContentType = "text/plain",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    private static AnexarArquivoChamadoUseCase CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuarioContexto,
        FakeArquivoStorageService storage)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            storage,
            new FakeUsuarioContextoAplicacaoService(usuarioContexto),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao,
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado ChamadoProprio, Chamado ChamadoOutro, UsuarioContextoAplicacao UsuarioContexto)> SeedDados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");

        var usuario = new Usuario("Usuario 1", "u1@empresa.com", "u1", "teste");
        var usuarioOutro = new Usuario("Usuario 2", "u2@empresa.com", "u2", "teste");
        context.CategoriasChamado.Add(categoria);
        context.Usuarios.AddRange(usuario, usuarioOutro);
        await context.SaveChangesAsync();

        var chamadoProprio = new Chamado("CH-A1", "Chamado A1", "Descricao", usuario.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        var chamadoOutro = new Chamado("CH-A2", "Chamado A2", "Descricao", usuarioOutro.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(chamadoProprio, chamadoOutro);
        await context.SaveChangesAsync();

        return (
            chamadoProprio,
            chamadoOutro,
            new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, ["Solicitante"]));
    }
}
