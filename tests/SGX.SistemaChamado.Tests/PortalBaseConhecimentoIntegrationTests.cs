using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class PortalBaseConhecimentoIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public PortalBaseConhecimentoIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ListagemRetornaSomentePublicadosAtivosEResumoSemConteudo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.bc.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.bc.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Portal BC", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Portal BC", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        await SeedArtigosAsync(prefixo, adminEmail);

        var response = await clientSolicitante.GetAsync($"/api/portal/base-conhecimento/artigos?termo={prefixo}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var raw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("\"conteudo\"", raw, StringComparison.OrdinalIgnoreCase);

        var payload = await response.Content.ReadFromJsonAsync<PortalListaBaseConhecimentoArtigosResponse>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!.Items);
        Assert.All(payload.Items, item => Assert.Contains(prefixo, item.Titulo));
    }

    [Fact]
    public async Task DetalhePorSlugRetornaConteudoCompletoParaPublicadoAtivo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.bc.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.bc.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Portal BC", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Portal BC", "Solicitante");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");

        var slug = await SeedArtigoPublicadoSolicitanteAsync(prefixo, adminEmail);

        var response = await clientSolicitante.GetAsync($"/api/portal/base-conhecimento/artigos/{slug}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PortalBaseConhecimentoArtigoDetalheDto>();
        Assert.NotNull(payload);
        Assert.Contains(prefixo, payload!.Conteudo);
    }

    [Fact]
    public async Task VisibilidadeAtendenteBloqueiaSolicitanteNoDetalhe()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var adminEmail = $"admin.portal.bc.int.{prefixo}@empresa.com";
        var solicitanteEmail = $"sol.portal.bc.int.{prefixo}@empresa.com";
        var atendenteEmail = $"aten.portal.bc.int.{prefixo}@empresa.com";

        using var clientAdmin = _factory.CreateClient();
        using var clientSolicitante = _factory.CreateClient();
        using var clientAtendente = _factory.CreateClient();

        AddDevHeaders(clientAdmin, adminEmail, "Admin Portal BC", "Administrador");
        AddDevHeaders(clientSolicitante, solicitanteEmail, "Solicitante Portal BC", "Solicitante");
        AddDevHeaders(clientAtendente, atendenteEmail, "Atendente Portal BC", "Atendente");

        _ = await clientAdmin.GetAsync("/api/me");
        _ = await clientSolicitante.GetAsync("/api/me");
        _ = await clientAtendente.GetAsync("/api/me");

        var slug = await SeedArtigoPublicadoAtendenteAsync(prefixo, adminEmail);

        var responseSolicitante = await clientSolicitante.GetAsync($"/api/portal/base-conhecimento/artigos/{slug}");
        var responseAtendente = await clientAtendente.GetAsync($"/api/portal/base-conhecimento/artigos/{slug}");

        Assert.Equal(HttpStatusCode.NotFound, responseSolicitante.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseAtendente.StatusCode);
    }

    private async Task SeedArtigosAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);

        dbContext.BaseConhecimentoArtigos.Add(new BaseConhecimentoArtigo(
            $"{prefixo}-publicado",
            $"{prefixo}-publicado",
            "resumo",
            $"conteudo {prefixo}",
            categoria.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Solicitante,
            "tag",
            admin.Id,
            "integration-test"));

        dbContext.BaseConhecimentoArtigos.Add(new BaseConhecimentoArtigo(
            $"{prefixo}-rascunho",
            $"{prefixo}-rascunho",
            "resumo",
            $"conteudo {prefixo}",
            categoria.Id,
            StatusArtigoConhecimento.Rascunho,
            VisibilidadeArtigoConhecimento.Solicitante,
            "tag",
            admin.Id,
            "integration-test"));

        var inativo = new BaseConhecimentoArtigo(
            $"{prefixo}-inativo",
            $"{prefixo}-inativo",
            "resumo",
            $"conteudo {prefixo}",
            categoria.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Solicitante,
            "tag",
            admin.Id,
            "integration-test");
        inativo.Desativar("integration-test");
        dbContext.BaseConhecimentoArtigos.Add(inativo);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> SeedArtigoPublicadoSolicitanteAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);

        var slug = $"{prefixo}-detalhe-solicitante";
        dbContext.BaseConhecimentoArtigos.Add(new BaseConhecimentoArtigo(
            $"Detalhe {prefixo}",
            slug,
            "resumo",
            $"conteudo completo {prefixo}",
            categoria.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Solicitante,
            "tag",
            admin.Id,
            "integration-test"));

        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private async Task<string> SeedArtigoPublicadoAtendenteAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios.FirstAsync(x => x.Email == emailAdmin, cancellationToken);
        var categoria = await ObterOuCriarCategoriaAsync(dbContext, prefixo, cancellationToken);

        var slug = $"{prefixo}-detalhe-atendente";
        dbContext.BaseConhecimentoArtigos.Add(new BaseConhecimentoArtigo(
            $"Atendente {prefixo}",
            slug,
            "resumo",
            $"conteudo atendente {prefixo}",
            categoria.Id,
            StatusArtigoConhecimento.Publicado,
            VisibilidadeArtigoConhecimento.Atendente,
            "tag",
            admin.Id,
            "integration-test"));

        await dbContext.SaveChangesAsync(cancellationToken);
        return slug;
    }

    private static async Task<CategoriaChamado> ObterOuCriarCategoriaAsync(SGXSistemaChamadoDbContext dbContext, string prefixo, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(x => x.Nome == $"Categoria BC {prefixo}", cancellationToken);
        if (categoria is not null)
        {
            return categoria;
        }

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(x => x.Nome == $"Departamento BC {prefixo}", cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento($"Departamento BC {prefixo}", "DBC", null, "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        categoria = new CategoriaChamado($"Categoria BC {prefixo}", null, departamento.Id, "integration-test");
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