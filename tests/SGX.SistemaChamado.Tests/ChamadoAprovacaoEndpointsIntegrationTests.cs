using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoAprovacaoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ChamadoAprovacaoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveCriarListarAprovarEReprovarAprovacaoViaEndpoint()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.vinc.endpoint.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Vinculada", "Administrador");

        var criarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo = "Aprovacao de mudanca tecnica",
            descricao = "Autorizar execucao planejada.",
            justificativaSolicitacao = "Mudanca exige autorizacao formal."
        });

        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var criada = await criarResponse.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(criada);
        Assert.Equal(StatusAprovacaoChamado.Pendente, criada.Status);

        var listarResponse = await client.GetAsync($"/api/admin/chamados/{chamadoId}/aprovacoes");
        Assert.Equal(HttpStatusCode.OK, listarResponse.StatusCode);
        var lista = await listarResponse.Content.ReadFromJsonAsync<List<ChamadoAprovacaoAdminResponse>>();
        Assert.NotNull(lista);
        Assert.Contains(lista, x => x.Id == criada.Id);

        var aprovarResponse = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{criada.Id}/aprovar",
            new { justificativaDecisao = "Mudanca aprovada." });
        Assert.Equal(HttpStatusCode.OK, aprovarResponse.StatusCode);
        var aprovada = await aprovarResponse.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(aprovada);
        Assert.Equal(StatusAprovacaoChamado.Aprovado, aprovada.Status);
        Assert.NotNull(aprovada.DecididoEm);

        var criarReprovacaoResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo = "Aprovacao de execucao fora de janela"
        });
        Assert.Equal(HttpStatusCode.OK, criarReprovacaoResponse.StatusCode);
        var paraReprovar = await criarReprovacaoResponse.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(paraReprovar);

        var reprovarResponse = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{paraReprovar.Id}/reprovar",
            new { justificativaDecisao = "Janela indisponivel." });
        Assert.Equal(HttpStatusCode.OK, reprovarResponse.StatusCode);
        var reprovada = await reprovarResponse.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(reprovada);
        Assert.Equal(StatusAprovacaoChamado.Reprovado, reprovada.Status);
    }

    [Fact]
    public async Task DeveListarSomenteAprovacoesDoChamadoInformado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.lista.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Lista", "Administrador");

        var aprovacaoChamado = await CriarAprovacaoViaEndpointAsync(client, chamadoId, "Aprovacao do chamado correto");
        var aprovacaoOutroChamado = await CriarAprovacaoViaEndpointAsync(client, outroChamadoId, "Aprovacao de outro chamado");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoId}/aprovacoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var aprovacoes = await response.Content.ReadFromJsonAsync<List<ChamadoAprovacaoAdminResponse>>();
        Assert.NotNull(aprovacoes);
        Assert.Contains(aprovacoes, x => x.Id == aprovacaoChamado.Id);
        Assert.DoesNotContain(aprovacoes, x => x.Id == aprovacaoOutroChamado.Id);
    }

    [Fact]
    public async Task DeveRetornarCampoBloqueiaAvancoAtendimentoNaListagem()
    {
        var chamadoId = await SeedChamadoAsync();
        var aprovacaoId = await CriarAprovacaoDiretaAsync(chamadoId, bloqueiaAvancoAtendimento: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.bloqueia.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Bloqueia", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoId}/aprovacoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var aprovacoes = await response.Content.ReadFromJsonAsync<List<ChamadoAprovacaoAdminResponse>>();
        Assert.NotNull(aprovacoes);
        var aprovacao = Assert.Single(aprovacoes, x => x.Id == aprovacaoId);
        Assert.True(aprovacao.BloqueiaAvancoAtendimento);
    }

    [Fact]
    public async Task DeveRetornar404AoAprovarAprovacaoInexistente()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Inexistente", "Administrador");

        var response = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{Guid.NewGuid()}/aprovar",
            new { justificativaDecisao = "Tentativa inexistente." });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Aprovacao vinculada nao encontrada.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoReprovarAprovacaoDeOutroChamado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.outro.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Outro", "Administrador");
        var aprovacaoOutroChamado = await CriarAprovacaoViaEndpointAsync(client, outroChamadoId, "Aprovacao outro chamado");

        var response = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoOutroChamado.Id}/reprovar",
            new { justificativaDecisao = "Outro chamado." });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Aprovacao vinculada nao pertence ao chamado informado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoCancelarAprovacaoDeOutroChamado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.cancelar.outro.{Guid.NewGuid():N}@empresa.com", "Admin Cancela Aprovacao Outro", "Administrador");
        var aprovacaoOutroChamado = await CriarAprovacaoViaEndpointAsync(client, outroChamadoId, "Aprovacao cancelamento outro chamado");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacaoOutroChamado.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Aprovacao vinculada nao pertence ao chamado informado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoDecidirAprovacaoJaDecidida()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.aprov.ja.decidida.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Ja Decidida", "Administrador");
        var aprovacao = await CriarAprovacaoViaEndpointAsync(client, chamadoId, "Aprovacao ja decidida");

        var aprovarResponse = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacao.Id}/aprovar",
            new { justificativaDecisao = "Primeira decisao." });
        Assert.Equal(HttpStatusCode.OK, aprovarResponse.StatusCode);

        var reprovarResponse = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacao.Id}/reprovar",
            new { justificativaDecisao = "Segunda decisao." });

        Assert.Equal(HttpStatusCode.BadRequest, reprovarResponse.StatusCode);
        var payload = await reprovarResponse.Content.ReadAsStringAsync();
        Assert.Contains("Somente aprovacoes pendentes podem ser decididas.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveCancelarAprovacaoViaEndpoint()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.cancel.aprov.vinc.{Guid.NewGuid():N}@empresa.com", "Admin Cancela Aprovacao", "Administrador");

        var criarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo = "Aprovacao de compra"
        });
        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var criada = await criarResponse.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(criada);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/chamados/{chamadoId}/aprovacoes/{criada.Id}")
        {
            Content = JsonContent.Create(new { motivoCancelamento = "Compra cancelada pela area." })
        };
        var cancelarResponse = await client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, cancelarResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var aprovacao = await dbContext.AprovacoesChamado.SingleAsync(x => x.Id == criada.Id);
        Assert.Equal(StatusAprovacaoChamado.Cancelado, aprovacao.Status);
        Assert.False(aprovacao.Ativo);
        Assert.NotNull(aprovacao.CanceladoEm);
        Assert.Equal("Compra cancelada pela area.", aprovacao.MotivoCancelamento);
    }

    [Fact]
    public async Task DeveBloquearCriacaoViaEndpointParaSolicitante()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.aprov.vinc.endpoint.{Guid.NewGuid():N}@empresa.com", "Solicitante Aprovacao Vinculada", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo = "Aprovacao solicitante"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DevePermitirCriacaoViaEndpointParaAtendente()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.aprov.vinc.endpoint.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao Vinculada", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo = "Aprovacao criada por atendente"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearDecisaoViaEndpointParaSolicitante()
    {
        var chamadoId = await SeedChamadoAsync();

        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.aprov.seed.decisao.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Seed Decisao", "Administrador");
        var aprovacao = await CriarAprovacaoViaEndpointAsync(adminClient, chamadoId, "Aprovacao para bloqueio solicitante");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.aprov.decisao.{Guid.NewGuid():N}@empresa.com", "Solicitante Aprovacao Decisao", "Solicitante");

        var response = await client.PostAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacao.Id}/aprovar",
            new { justificativaDecisao = "Solicitante nao pode decidir." });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearListagemViaEndpointSemAutenticacao()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoId}/aprovacoes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearCancelamentoViaEndpointSemAutenticacao()
    {
        var chamadoId = await SeedChamadoAsync();

        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.aprov.seed.cancelar.{Guid.NewGuid():N}@empresa.com", "Admin Aprovacao Seed Cancelar", "Administrador");
        var aprovacao = await CriarAprovacaoViaEndpointAsync(adminClient, chamadoId, "Aprovacao para cancelar sem auth");

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoId}/aprovacoes/{aprovacao.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<ChamadoAprovacaoAdminResponse> CriarAprovacaoViaEndpointAsync(HttpClient client, Guid chamadoId, string titulo)
    {
        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacoes", new
        {
            titulo
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var aprovacao = await response.Content.ReadFromJsonAsync<ChamadoAprovacaoAdminResponse>();
        Assert.NotNull(aprovacao);
        return aprovacao;
    }

    private async Task<Guid> CriarAprovacaoDiretaAsync(Guid chamadoId, bool bloqueiaAvancoAtendimento, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(x => x.Login.Contains("admin"), cancellationToken);
        var chamado = await dbContext.Chamados.AsNoTracking().FirstAsync(x => x.Id == chamadoId, cancellationToken);

        var aprovacao = new AprovacaoChamado(
            chamadoId,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            chamado.SolicitanteId,
            "Aprovacao direta para endpoint",
            "Validar campo bloqueante",
            "Aprovacao bloqueante direta",
            bloqueiaAvancoAtendimento: bloqueiaAvancoAtendimento);

        dbContext.AprovacoesChamado.Add(aprovacao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return aprovacao.Id;
    }

    private async Task<(Guid ChamadoId, Guid OutroChamadoId)> SeedDoisChamadosAsync(CancellationToken cancellationToken = default)
    {
        var chamadoId = await SeedChamadoAsync(cancellationToken);
        var outroChamadoId = await SeedChamadoAsync(cancellationToken);
        return (chamadoId, outroChamadoId);
    }

    private async Task<Guid> SeedChamadoAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = new Usuario(
            "Solicitante Aprovacao Vinculada Endpoint",
            $"sol.aprov.vinc.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            $"sol.aprov.vinc.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            "integration-test");
        dbContext.Usuarios.Add(solicitante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfilSolicitante = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(solicitante.Id, perfilSolicitante.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Categoria Aprovacao Vinculada Endpoint", "Categoria para aprovacao endpoint", null, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamado = new Chamado(
            $"SGX-AV-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado com aprovacao vinculada",
            "Descricao do chamado com aprovacao vinculada",
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

    private static void AddDevHeaders(HttpClient client, string email, string nome, string role)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Add("X-Dev-User-Email", email);
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", nome);
        client.DefaultRequestHeaders.Add("X-Dev-User-Role", role);
    }

    private static void AddInvalidBearer(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token-invalido");
    }
}
