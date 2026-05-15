using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SGX.SistemaChamado.Api.Controllers;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AnexosChamadoUseCasesTests
{
    [Fact]
    public async Task SolicitanteEnviaAnexoEmChamadoProprio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3, 4]);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
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
    public async Task SolicitanteNaoEnviaAnexoEmChamadoDeOutroUsuario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(dados.ChamadoOutroSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "evidencia.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task AtendenteEnviaAnexoEmChamadoAcessivel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.AtendenteContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "atendimento.txt",
            ContentType = "text/plain",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Equal("atendimento.txt", response.NomeArquivo);
    }

    [Fact]
    public async Task AdministradorEnviaAnexoEmQualquerChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.AdminContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        var response = await useCase.ExecutarAsync(dados.ChamadoOutroSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "admin.docx",
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        Assert.Equal("admin.docx", response.NomeArquivo);
    }

    [Fact]
    public async Task UploadRejeitaArquivoVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        await using var stream = new MemoryStream([]);
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "vazio.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = 0,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task UploadRejeitaExtensaoBloqueada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "script.ps1",
            ContentType = "application/octet-stream",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task UploadRejeitaAcimaDoLimite()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        var bytes = new byte[(10 * 1024 * 1024) + 1];
        await using var stream = new MemoryStream(bytes);
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "grande.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        }));
    }

    [Fact]
    public async Task ListagemNaoRetornaCaminhoFisicoNemNomeArmazenado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.AnexosChamado.Add(new AnexoChamado(
            dados.ChamadoSolicitante.Id,
            "evidencia.pdf",
            "abcd1234.pdf",
            "application/pdf",
            128,
            "storage/anexos/abcd1234.pdf",
            dados.Solicitante.Id,
            "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Single(response);
        var propriedades = response.First().GetType().GetProperties().Select(x => x.Name).ToArray();
        Assert.DoesNotContain("Caminho", propriedades);
        Assert.DoesNotContain("NomeArquivoArmazenado", propriedades);
    }

    [Fact]
    public async Task DownloadFuncionaParaUsuarioAutorizado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var adicionar = CriarAdicionarUseCase(context, dados.SolicitanteContexto, storage);

        await using var stream = new MemoryStream([9, 8, 7]);
        var criado = await adicionar.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "download.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        var baixar = CriarBaixarUseCase(context, dados.SolicitanteContexto, storage);
        var arquivo = await baixar.ExecutarAsync(dados.ChamadoSolicitante.Id, criado.Id);

        Assert.Equal("download.pdf", arquivo.NomeArquivo);
        Assert.Equal("application/pdf", arquivo.ContentType);
    }

    [Fact]
    public async Task DownloadBloqueadoSemAcessoAoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var adicionar = CriarAdicionarUseCase(context, dados.AdminContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        var criado = await adicionar.ExecutarAsync(dados.ChamadoOutroSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "privado.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        var baixar = CriarBaixarUseCase(context, dados.SolicitanteContexto, storage);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => baixar.ExecutarAsync(dados.ChamadoOutroSolicitante.Id, criado.Id));
    }

    [Fact]
    public async Task DownloadRetorna404QuandoAnexoNaoPertenceAoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();
        var adicionar = CriarAdicionarUseCase(context, dados.AdminContexto, storage);

        await using var stream = new MemoryStream([1, 2, 3]);
        var criado = await adicionar.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "nao-pertence.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        var baixar = CriarBaixarUseCase(context, dados.AdminContexto, storage);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => baixar.ExecutarAsync(dados.ChamadoOutroSolicitante.Id, criado.Id));
    }

    [Fact]
    public async Task DownloadRetorna404QuandoArquivoFisicoNaoExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();

        var anexo = new AnexoChamado(
            dados.ChamadoSolicitante.Id,
            "ausente.pdf",
            "nao-existe.pdf",
            "application/pdf",
            111,
            "storage/anexos/nao-existe.pdf",
            dados.Solicitante.Id,
            "teste");
        context.AnexosChamado.Add(anexo);
        await context.SaveChangesAsync();

        var baixar = CriarBaixarUseCase(context, dados.AdminContexto, storage);
        await Assert.ThrowsAsync<FileNotFoundException>(() => baixar.ExecutarAsync(dados.ChamadoSolicitante.Id, anexo.Id));
    }

    [Fact]
    public void NaoExisteFluxoDeExclusaoDeAnexo()
    {
        var interfaceExcluir = typeof(IListarAnexosChamadoUseCase).Assembly
            .GetTypes()
            .FirstOrDefault(x => x.Name.Contains("ExcluirAnexoChamadoUseCase", StringComparison.OrdinalIgnoreCase));

        Assert.Null(interfaceExcluir);
    }

    [Fact]
    public void NenhumEndpointDeleteDeAnexoEstaExposto()
    {
        var controllers = new[] { typeof(ChamadosController), typeof(PortalController) };
        var possuiDeleteAnexo = controllers
            .SelectMany(controller => controller.GetMethods())
            .SelectMany(method => method.GetCustomAttributes(typeof(HttpMethodAttribute), inherit: true).Cast<HttpMethodAttribute>())
            .Any(attribute =>
                attribute.HttpMethods.Any(http => string.Equals(http, "DELETE", StringComparison.OrdinalIgnoreCase)) &&
                (attribute.Template?.Contains("anexo", StringComparison.OrdinalIgnoreCase) ?? false));

        Assert.False(possuiDeleteAnexo);
    }

    private static ListarAnexosChamadoUseCase CriarListarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario));

    private static AdicionarAnexoChamadoUseCase CriarAdicionarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuario,
        FakeArquivoStorageService storage)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            storage,
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao,
            PortalUseCasesTestFactory.Uow(context));

    private static BaixarAnexoChamadoUseCase CriarBaixarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuario,
        FakeArquivoStorageService storage)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            storage,
            new FakeUsuarioContextoAplicacaoService(usuario));

    private static async Task<(
        Usuario Solicitante,
        Chamado ChamadoSolicitante,
        Chamado ChamadoOutroSolicitante,
        UsuarioContextoAplicacao SolicitanteContexto,
        UsuarioContextoAplicacao AtendenteContexto,
        UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.anexo@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "atendente.anexo@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "solicitante.anexo@empresa.com", TipoPerfil.Solicitante);
        var outroSolicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro Solicitante", "outro.anexo@empresa.com", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Arquivos");
        var chamadoSolicitante = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "ANX-001");
        var chamadoOutro = await AdminUseCasesTestFactory.CriarChamadoAsync(context, outroSolicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "ANX-002");

        return (
            solicitante,
            chamadoSolicitante,
            chamadoOutro,
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"),
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
