using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoStatusEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private const string MensagemBloqueioDependencia =
        "Este chamado possui dependencia ativa e nao pode ser fechado enquanto estiver bloqueado por outro chamado.";

    private const string MensagemBloqueioAprovacao =
        "Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.";

    private readonly ApiIntegrationTestFactory _factory;

    public ChamadoStatusEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveBloquearAlteracaoParaStatusFinalQuandoDependenciaAtiva()
    {
        var (chamadoId, bloqueadorId) = await SeedDoisChamadosAsync();
        await CriarRelacionamentoAsync(bloqueadorId, chamadoId, TipoRelacionamentoChamadoEnum.Bloqueia);
        var statusResolvidoId = await ObterStatusIdAsync(StatusChamadoEnum.Resolvido);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.status.dep.{Guid.NewGuid():N}@empresa.com", "Admin Status Dependencia", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/alterar-status", new
        {
            statusId = statusResolvidoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(MensagemBloqueioDependencia, payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirAlteracaoParaStatusIntermediarioQuandoDependenciaAtiva()
    {
        var (chamadoId, bloqueadorId) = await SeedDoisChamadosAsync();
        await CriarRelacionamentoAsync(bloqueadorId, chamadoId, TipoRelacionamentoChamadoEnum.Bloqueia);
        var statusEmAtendimentoId = await ObterStatusIdAsync(StatusChamadoEnum.EmAtendimento);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.status.inter.{Guid.NewGuid():N}@empresa.com", "Admin Status Intermediario", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/alterar-status", new
        {
            statusId = statusEmAtendimentoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Em Atendimento", payload.Status);
    }

    [Fact]
    public async Task DeveBloquearEncerramentoQuandoDependenciaAtiva()
    {
        var (chamadoId, bloqueadorId) = await SeedDoisChamadosAsync();
        await CriarRelacionamentoAsync(bloqueadorId, chamadoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.enc.dep.{Guid.NewGuid():N}@empresa.com", "Admin Encerrar Dependencia", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/encerrar", new
        {
            solucao = "Tentativa bloqueada por dependencia ativa.",
            comentarioInterno = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(MensagemBloqueioDependencia, payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearAlteracaoParaStatusFinalQuandoAprovacaoPendenteBloqueante()
    {
        var chamadoId = await SeedChamadoAsync();
        await CriarAprovacaoAsync(chamadoId, bloqueiaAvancoAtendimento: true);
        var statusResolvidoId = await ObterStatusIdAsync(StatusChamadoEnum.Resolvido);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.status.aprov.bloq.{Guid.NewGuid():N}@empresa.com", "Admin Status Aprovacao Bloqueante", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/alterar-status", new
        {
            statusId = statusResolvidoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(MensagemBloqueioAprovacao, payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearEncerramentoQuandoAprovacaoPendenteBloqueante()
    {
        var chamadoId = await SeedChamadoAsync();
        await CriarAprovacaoAsync(chamadoId, bloqueiaAvancoAtendimento: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.enc.aprov.bloq.{Guid.NewGuid():N}@empresa.com", "Admin Encerrar Aprovacao Bloqueante", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/encerrar", new
        {
            solucao = "Tentativa bloqueada por aprovacao pendente.",
            comentarioInterno = true
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(MensagemBloqueioAprovacao, payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevePermitirAlteracaoParaStatusFinalQuandoAprovacaoPendenteNaoBloqueante()
    {
        var chamadoId = await SeedChamadoAsync();
        await CriarAprovacaoAsync(chamadoId, bloqueiaAvancoAtendimento: false);
        var statusResolvidoId = await ObterStatusIdAsync(StatusChamadoEnum.Resolvido);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.status.aprov.naobloq.{Guid.NewGuid():N}@empresa.com", "Admin Status Aprovacao Nao Bloqueante", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/alterar-status", new
        {
            statusId = statusResolvidoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Resolvido", payload.Status);
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
            "Solicitante Status Endpoint",
            $"sol.status.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            $"sol.status.endpoint.seed.{Guid.NewGuid():N}@empresa.com",
            "integration-test");
        dbContext.Usuarios.Add(solicitante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfilSolicitante = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(solicitante.Id, perfilSolicitante.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento("Operacoes Status Endpoint", "OSE", "Departamento status endpoint", "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Categoria Status Endpoint", "Categoria para status endpoint", departamento.Id, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamado = new Chamado(
            $"SGX-ST-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para status endpoint",
            "Descricao do chamado para status endpoint",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test",
            naturezaChamado: NaturezaChamadoEnum.Requisicao);

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);
        return chamado.Id;
    }

    private async Task<Guid> ObterStatusIdAsync(StatusChamadoEnum codigo, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        return await dbContext.StatusChamado
            .Where(x => x.Codigo == codigo)
            .Select(x => x.Id)
            .FirstAsync(cancellationToken);
    }

    private async Task CriarRelacionamentoAsync(
        Guid chamadoOrigemId,
        Guid chamadoDestinoId,
        TipoRelacionamentoChamadoEnum tipo,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(x => x.Login.Contains("admin"), cancellationToken);

        dbContext.ChamadosRelacionamentos.Add(new ChamadoRelacionamento(
            chamadoOrigemId,
            chamadoDestinoId,
            tipo,
            admin.Id,
            admin.Login,
            "Dependencia ativa para teste de endpoint."));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CriarAprovacaoAsync(Guid chamadoId, bool bloqueiaAvancoAtendimento, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(x => x.Login.Contains("admin"), cancellationToken);
        var chamado = await dbContext.Chamados.AsNoTracking().FirstAsync(x => x.Id == chamadoId, cancellationToken);

        dbContext.AprovacoesChamado.Add(new AprovacaoChamado(
            chamadoId,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            chamado.SolicitanteId,
            "Aprovacao para bloqueio de endpoint",
            "Aprovacao pendente para validar status final.",
            "Aprovacao pendente",
            bloqueiaAvancoAtendimento: bloqueiaAvancoAtendimento));

        await dbContext.SaveChangesAsync(cancellationToken);
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
