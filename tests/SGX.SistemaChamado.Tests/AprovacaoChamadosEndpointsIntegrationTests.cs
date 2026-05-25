using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AprovacaoChamadosEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public AprovacaoChamadosEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FluxoAdministrativoSolicitarListarDetalharEAprovarFunciona()
    {
        var chamadoId = await CriarChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.endpoint.{Guid.NewGuid():N}@empresa.com", "Administrador Aprovacao", "Administrador");
        _ = await client.GetAsync("/api/me");

        var solicitarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacao/solicitar", new
        {
            tipoOrigem = (int)TipoOrigemAprovacaoChamado.Manual,
            origemDescricao = "Solicitacao manual",
            justificativaSolicitacao = "Aguardando aprovacao administrativa"
        });

        Assert.Equal(HttpStatusCode.OK, solicitarResponse.StatusCode);
        var aprovacaoId = await ObterGuidDaRespostaAsync(solicitarResponse, "id");

        var listagemResponse = await client.GetAsync("/api/admin/aprovacao-chamados?chamadoId=" + chamadoId);
        Assert.Equal(HttpStatusCode.OK, listagemResponse.StatusCode);
        var listagemPayload = await listagemResponse.Content.ReadAsStringAsync();
        Assert.Contains(aprovacaoId.ToString(), listagemPayload, StringComparison.OrdinalIgnoreCase);

        var detalheResponse = await client.GetAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}");
        Assert.Equal(HttpStatusCode.OK, detalheResponse.StatusCode);
        var detalhePayload = await detalheResponse.Content.ReadAsStringAsync();
        Assert.Contains(aprovacaoId.ToString(), detalhePayload, StringComparison.OrdinalIgnoreCase);

        var aprovarResponse = await client.PostAsJsonAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}/aprovar", new
        {
            justificativaDecisao = "Aprovado para execucao"
        });

        Assert.Equal(HttpStatusCode.OK, aprovarResponse.StatusCode);
        using var aprovarJson = JsonDocument.Parse(await aprovarResponse.Content.ReadAsStringAsync());
        Assert.Equal((int)StatusAprovacaoChamado.Aprovado, aprovarJson.RootElement.GetProperty("status").GetInt32());
    }

    private async Task<Guid> CriarChamadoAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = new Usuario(
            "Solicitante Endpoints",
            $"sol.endpoints.{Guid.NewGuid():N}@empresa.com",
            $"sol.endpoints.{Guid.NewGuid():N}@empresa.com",
            "integration-test");
        dbContext.Usuarios.Add(solicitante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfilSolicitante = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(solicitante.Id, perfilSolicitante.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento("Operacoes", "OPS", "Departamento de operacoes", "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Solicitacoes Gerais", "Categoria de apoio", departamento.Id, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamado = new Chamado(
            $"SGX-APR-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para fluxo administrativo de aprovacao",
            "Descricao de teste para endpoints de aprovacao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        return chamado.Id;
    }

    private static async Task<Guid> ObterGuidDaRespostaAsync(HttpResponseMessage response, string propriedade)
    {
        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        return json.RootElement.GetProperty(propriedade).GetGuid();
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
