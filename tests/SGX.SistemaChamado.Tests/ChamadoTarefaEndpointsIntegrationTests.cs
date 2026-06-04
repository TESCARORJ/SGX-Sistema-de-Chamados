using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoTarefaEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ChamadoTarefaEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveCriarListarAlterarStatusECancelarTarefaViaEndpoint()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.endpoint.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Endpoint", "Administrador");

        var criarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/tarefas", new
        {
            titulo = "Executar script de correcao",
            descricao = "Rodar script validado em homologacao.",
            prazo = DateTime.UtcNow.AddDays(1)
        });

        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var criada = await criarResponse.Content.ReadFromJsonAsync<ChamadoTarefaAdminResponse>();
        Assert.NotNull(criada);
        Assert.Equal(StatusTarefaChamadoEnum.Pendente, criada.Status);

        var listarResponse = await client.GetAsync($"/api/admin/chamados/{chamadoId}/tarefas");
        Assert.Equal(HttpStatusCode.OK, listarResponse.StatusCode);
        var tarefas = await listarResponse.Content.ReadFromJsonAsync<List<ChamadoTarefaAdminResponse>>();
        Assert.NotNull(tarefas);
        Assert.Contains(tarefas, x => x.Id == criada.Id);

        var statusResponse = await client.PatchAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/tarefas/{criada.Id}/status",
            new { status = StatusTarefaChamadoEnum.EmAndamento });
        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);
        var emAndamento = await statusResponse.Content.ReadFromJsonAsync<ChamadoTarefaAdminResponse>();
        Assert.NotNull(emAndamento);
        Assert.Equal(StatusTarefaChamadoEnum.EmAndamento, emAndamento.Status);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/admin/chamados/{chamadoId}/tarefas/{criada.Id}")
        {
            Content = JsonContent.Create(new { motivoCancelamento = "Atividade substituida por chamado derivado." })
        };
        var cancelarResponse = await client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, cancelarResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var tarefa = await dbContext.ChamadosTarefas.SingleAsync(x => x.Id == criada.Id);
        Assert.Equal(StatusTarefaChamadoEnum.Cancelada, tarefa.Status);
        Assert.False(tarefa.Ativo);
        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoId &&
            x.Tipo == TipoHistoricoChamado.TarefaCancelada &&
            x.Descricao.Contains("Atividade substituida"));
    }

    [Fact]
    public async Task DeveListarSomenteTarefasDoChamadoInformado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.lista.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Lista", "Administrador");

        var tarefaChamado = await CriarTarefaViaEndpointAsync(client, chamadoId, "Tarefa do chamado correto");
        var tarefaOutroChamado = await CriarTarefaViaEndpointAsync(client, outroChamadoId, "Tarefa de outro chamado");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoId}/tarefas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarefas = await response.Content.ReadFromJsonAsync<List<ChamadoTarefaAdminResponse>>();
        Assert.NotNull(tarefas);
        Assert.Contains(tarefas, x => x.Id == tarefaChamado.Id);
        Assert.DoesNotContain(tarefas, x => x.Id == tarefaOutroChamado.Id);
    }

    [Fact]
    public async Task DeveConcluirTarefaViaEndpoint()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.concluir.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Concluir", "Administrador");
        var tarefa = await CriarTarefaViaEndpointAsync(client, chamadoId, "Concluir checklist operacional");

        var response = await client.PatchAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/tarefas/{tarefa.Id}/status",
            new { status = StatusTarefaChamadoEnum.Concluida });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoTarefaAdminResponse>();
        Assert.NotNull(payload);
        Assert.Equal(StatusTarefaChamadoEnum.Concluida, payload.Status);
        Assert.NotNull(payload.ConcluidoEm);
    }

    [Fact]
    public async Task DeveRetornar404AoAlterarStatusDeTarefaInexistente()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Inexistente", "Administrador");

        var response = await client.PatchAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/tarefas/{Guid.NewGuid()}/status",
            new { status = StatusTarefaChamadoEnum.EmAndamento });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tarefa vinculada nao encontrada.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoAlterarStatusDeTarefaDeOutroChamado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.outro.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Outro", "Administrador");
        var tarefaOutroChamado = await CriarTarefaViaEndpointAsync(client, outroChamadoId, "Tarefa de outro chamado");

        var response = await client.PatchAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/tarefas/{tarefaOutroChamado.Id}/status",
            new { status = StatusTarefaChamadoEnum.EmAndamento });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tarefa vinculada nao pertence ao chamado informado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoCancelarTarefaDeOutroChamado()
    {
        var (chamadoId, outroChamadoId) = await SeedDoisChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.tarefa.cancelar.outro.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Cancelar Outro", "Administrador");
        var tarefaOutroChamado = await CriarTarefaViaEndpointAsync(client, outroChamadoId, "Tarefa cancelamento outro chamado");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoId}/tarefas/{tarefaOutroChamado.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Tarefa vinculada nao pertence ao chamado informado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirCriacaoViaEndpointParaAtendente()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.tarefa.endpoint.{Guid.NewGuid():N}@empresa.com", "Atendente Tarefa Endpoint", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/tarefas", new
        {
            titulo = "Tarefa criada por atendente"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearCriacaoViaEndpointParaSolicitante()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.tarefa.endpoint.{Guid.NewGuid():N}@empresa.com", "Solicitante Tarefa Endpoint", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/tarefas", new
        {
            titulo = "Tarefa solicitante"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearAlteracaoStatusViaEndpointParaSolicitante()
    {
        var chamadoId = await SeedChamadoAsync();

        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.tarefa.seed.status.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Seed Status", "Administrador");
        var tarefa = await CriarTarefaViaEndpointAsync(adminClient, chamadoId, "Tarefa para bloquear solicitante");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.tarefa.status.{Guid.NewGuid():N}@empresa.com", "Solicitante Tarefa Status", "Solicitante");

        var response = await client.PatchAsJsonAsync(
            $"/api/admin/chamados/{chamadoId}/tarefas/{tarefa.Id}/status",
            new { status = StatusTarefaChamadoEnum.EmAndamento });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearListagemViaEndpointSemAutenticacao()
    {
        var chamadoId = await SeedChamadoAsync();

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoId}/tarefas");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearCancelamentoViaEndpointSemAutenticacao()
    {
        var chamadoId = await SeedChamadoAsync();

        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.tarefa.seed.cancelar.{Guid.NewGuid():N}@empresa.com", "Admin Tarefa Seed Cancelar", "Administrador");
        var tarefa = await CriarTarefaViaEndpointAsync(adminClient, chamadoId, "Tarefa para cancelar sem auth");

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoId}/tarefas/{tarefa.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<ChamadoTarefaAdminResponse> CriarTarefaViaEndpointAsync(HttpClient client, Guid chamadoId, string titulo)
    {
        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/tarefas", new
        {
            titulo
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tarefa = await response.Content.ReadFromJsonAsync<ChamadoTarefaAdminResponse>();
        Assert.NotNull(tarefa);
        return tarefa;
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
            "Solicitante Tarefa Endpoint",
            $"sol.tarefa.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            $"sol.tarefa.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            "integration-test");
        dbContext.Usuarios.Add(solicitante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfilSolicitante = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(solicitante.Id, perfilSolicitante.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken)
            ?? new CategoriaChamado("Categoria Tarefa Endpoint", "Categoria para tarefa endpoint", null, "integration-test");
        if (categoria.Id == Guid.Empty || dbContext.Entry(categoria).State == EntityState.Detached)
        {
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamado = new Chamado(
            $"SGX-TA-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado com tarefas",
            "Descricao do chamado com tarefas",
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
