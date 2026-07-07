using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicosEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public CatalogoServicosEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CriarDeveAceitarGrupoTecnicoIdERetornarContratoAdministrativo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.catalogo.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Catalogo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedDependenciasAsync(prefixo);

        var response = await client.PostAsJsonAsync("/api/admin/catalogo-servicos", new
        {
            nome = $"Servico {prefixo}",
            descricao = "Descricao valida.",
            departamentoResponsavelId = dados.DepartamentoId,
            grupoTecnicoId = dados.GrupoTecnicoId,
            visibilidade = (int)VisibilidadeCatalogoServico.Interno
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CatalogoServicoDetalheDto>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoTecnicoId, payload!.GrupoTecnicoId);
        Assert.Equal(dados.GrupoTecnicoNome, payload.NomeGrupoTecnico);
    }

    [Fact]
    public async Task AtualizarDevePermitirRemoverGrupoTecnicoDoContratoAdministrativo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.catalogo.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Catalogo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedDependenciasAsync(prefixo);
        var servicoId = await SeedServicoAsync(prefixo, email, dados.DepartamentoId, dados.GrupoTecnicoId);

        var response = await client.PutAsJsonAsync($"/api/admin/catalogo-servicos/{servicoId}", new
        {
            nome = $"Servico atualizado {prefixo}",
            descricao = "Descricao atualizada.",
            departamentoResponsavelId = dados.DepartamentoId,
            grupoTecnicoId = (Guid?)null,
            visibilidade = (int)VisibilidadeCatalogoServico.Interno,
            permiteAberturaChamado = true,
            requerAprovacao = false,
            ordem = 2,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CatalogoServicoDetalheDto>();
        Assert.NotNull(payload);
        Assert.Null(payload!.GrupoTecnicoId);
        Assert.Null(payload.NomeGrupoTecnico);
    }

    [Fact]
    public async Task ListarDeveExporGrupoTecnicoNoContratoAdministrativo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.catalogo.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Catalogo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedDependenciasAsync(prefixo);
        var servicoId = await SeedServicoAsync(prefixo, email, dados.DepartamentoId, dados.GrupoTecnicoId);

        var response = await client.GetAsync($"/api/admin/catalogo-servicos?termo={prefixo}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedResultResponse<CatalogoServicoListagemDto>>();
        Assert.NotNull(payload);

        var item = Assert.Single(payload!.Items, x => x.Id == servicoId);
        Assert.Equal(dados.GrupoTecnicoId, item.GrupoTecnicoId);
        Assert.Equal(dados.GrupoTecnicoNome, item.NomeGrupoTecnico);
    }

    [Fact]
    public async Task ObterDeveExporGrupoTecnicoNoDetalheAdministrativo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.catalogo.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Catalogo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedDependenciasAsync(prefixo);
        var servicoId = await SeedServicoAsync(prefixo, email, dados.DepartamentoId, dados.GrupoTecnicoId);

        var response = await client.GetAsync($"/api/admin/catalogo-servicos/{servicoId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CatalogoServicoDetalheDto>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoTecnicoId, payload!.GrupoTecnicoId);
        Assert.Equal(dados.GrupoTecnicoNome, payload.NomeGrupoTecnico);
    }

    private async Task<(Guid DepartamentoId, Guid GrupoTecnicoId, string GrupoTecnicoNome)> SeedDependenciasAsync(string prefixo, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var departamento = new Departamento($"Departamento {prefixo}", "DCT", null, "integration-test");
        var grupoTecnico = new GrupoTecnico($"Grupo {prefixo}", null, "integration-test");

        dbContext.Departamentos.Add(departamento);
        dbContext.GruposTecnicos.Add(grupoTecnico);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (departamento.Id, grupoTecnico.Id, grupoTecnico.Nome);
    }

    private async Task<Guid> SeedServicoAsync(string prefixo, string emailAdmin, Guid departamentoId, Guid grupoTecnicoId, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios
            .AsNoTracking()
            .FirstAsync(x => x.Email == emailAdmin, cancellationToken);

        var servico = new CatalogoServico(
            $"Servico {prefixo}",
            $"servico-{prefixo}",
            "Descricao original.",
            null,
            departamentoId,
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            admin.Id,
            "integration-test",
            grupoTecnicoId);

        dbContext.CatalogosServico.Add(servico);
        await dbContext.SaveChangesAsync(cancellationToken);
        return servico.Id;
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
