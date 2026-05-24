using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalCatalogoServicosIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public PortalCatalogoServicosIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListagemRetornaSomenteServicosPublicadosAtivosEVisiveis()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        await SeedServicosAsync(prefixo, adminEmail);

        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos?termo={prefixo}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PortalListaCatalogoServicosResponse>();

        Assert.NotNull(payload);
        Assert.Single(payload!.Items);
        Assert.Contains(prefixo, payload.Items.Single().Nome);
    }

    [Fact]
    public async Task DetalhePorSlugRetorna404QuandoServicoNaoPublicado()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoRascunhoAsync(prefixo, adminEmail);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DetalhePorSlugRespeitaVisibilidade()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";
        var atendenteEmail = $"aten.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();
        using var clientAtendente = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");
        AddDevHeaders(clientAtendente, atendenteEmail, "Atendente Catalogo", "Atendente");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");
        _ = await clientAtendente.GetAsync("/api/me");

        var slug = await SeedServicoVisibilidadeAtendenteAsync(prefixo, adminEmail);

        var responseSolicitante = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}");
        var responseAtendente = await clientAtendente.GetAsync($"/api/portal/catalogo-servicos/{slug}");

        Assert.Equal(HttpStatusCode.NotFound, responseSolicitante.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseAtendente.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetornaServicoValido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, permiteAberturaChamado: true);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PortalPrepararChamadoCatalogoServicoDto>();
        Assert.NotNull(payload);
        Assert.Equal(slug, payload!.Slug);
        Assert.True(payload.PermiteAberturaChamado);
    }

    [Fact]
    public async Task PrepararChamadoRetorna404QuandoServicoInexistente()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");
        _ = await clientSolicitante.GetAsync("/api/me");

        var response = await clientSolicitante.GetAsync("/api/portal/catalogo-servicos/inexistente/preparar-chamado");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetorna400QuandoServicoNaoPermiteAbertura()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, permiteAberturaChamado: false);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PrepararChamadoRetorna404QuandoServicoSemVisibilidade()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.catalogo.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.catalogo.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Catalogo", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Catalogo", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedServicoParaPreparacaoAsync(prefixo, adminEmail, visibilidade: VisibilidadeCatalogoServico.Atendente);
        var response = await clientSolicitante.GetAsync($"/api/portal/catalogo-servicos/{slug}/preparar-chamado");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task SeedServicosAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");

        var publicado = new CatalogoServico(
            $"{prefixo}-publicado",
            $"{prefixo}-publicado",
            "descricao publicada",
            "instrucao publicada",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        publicado.Publicar(admin.Id, "integration-test");
        dbContext.CatalogosServico.Add(publicado);

        dbContext.CatalogosServico.Add(new CatalogoServico(
            $"{prefixo}-rascunho",
            $"{prefixo}-rascunho",
            "descricao rascunho",
            "instrucao rascunho",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test"));

        var inativo = new CatalogoServico(
            $"{prefixo}-inativo",
            $"{prefixo}-inativo",
            "descricao inativa",
            "instrucao inativa",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        inativo.Publicar(admin.Id, "integration-test");
        inativo.Desativar("integration-test");
        dbContext.CatalogosServico.Add(inativo);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> SeedServicoRascunhoAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-rascunho-detalhe";

        dbContext.CatalogosServico.Add(new CatalogoServico(
            "Rascunho detalhe",
            slug,
            "descricao rascunho",
            "instrucao rascunho",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "integration-test"));

        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<string> SeedServicoVisibilidadeAtendenteAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-atendente-detalhe";

        var servico = new CatalogoServico(
            "Servico atendente",
            slug,
            "descricao atendente",
            "instrucao atendente",
            departamentoId,
            categoria.Id,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Atendente,
            true,
            false,
            1,
            admin.Id,
            "integration-test");
        servico.Publicar(admin.Id, "integration-test");

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<string> SeedServicoParaPreparacaoAsync(
        string prefixo,
        string emailAdmin,
        bool permiteAberturaChamado = true,
        VisibilidadeCatalogoServico visibilidade = VisibilidadeCatalogoServico.Solicitante,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo, cancellationToken);
        var departamentoId = categoria.DepartamentoId ?? throw new InvalidOperationException("Categoria sem departamento para teste de catalogo.");
        var slug = $"{prefixo}-preparar-chamado";

        var servico = new CatalogoServico(
            "Servico preparar chamado",
            slug,
            "descricao",
            "instrucoes",
            departamentoId,
            categoria.Id,
            null,
            prioridade.Id,
            null,
            null,
            visibilidade,
            permiteAberturaChamado,
            false,
            1,
            admin.Id,
            "integration-test");
        servico.Publicar(admin.Id, "integration-test");

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private static async Task<CategoriaChamado> ObterOuCriarCategoriaAsync(SGXSistemaChamadoDbContext dbContext, string prefixo, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(x => x.Nome == $"Categoria Catalogo {prefixo}", cancellationToken);
        if (categoria is not null)
        {
            return categoria;
        }

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(x => x.Nome == $"Departamento Catalogo {prefixo}", cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento($"Departamento Catalogo {prefixo}", "DCA", null, "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        categoria = new CategoriaChamado($"Categoria Catalogo {prefixo}", null, departamento.Id, "integration-test");
        dbContext.CategoriasChamado.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);
        return categoria;
    }

    private static void AddDevHeaders(HttpClient client, string email, string nome, string role)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Add("X-Dev-User-Email", email);
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", nome);
        client.DefaultRequestHeaders.Add("X-Dev-User-Role", role);
    }
}
