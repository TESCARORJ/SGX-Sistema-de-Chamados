using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RelatoriosAvancadosAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private const string EndpointMetadados = "/api/admin/relatorios-avancados/metadados";
    private const string EndpointResumo = "/api/admin/relatorios-avancados/chamados/resumo";
    private const string EndpointProdutividade = "/api/admin/relatorios-avancados/atendimento/produtividade";
    private const string EndpointSlaResumo = "/api/admin/relatorios-avancados/sla/resumo";
    private const string EndpointSlaViolacoes = "/api/admin/relatorios-avancados/sla/violacoes";
    private const string EndpointAprovacoesResumo = "/api/admin/relatorios-avancados/aprovacoes/resumo";
    private const string EndpointAprovacoesPorOrigem = "/api/admin/relatorios-avancados/aprovacoes/por-origem";
    private const string EndpointCatalogoResumo = "/api/admin/relatorios-avancados/catalogo-servicos/resumo";
    private const string EndpointCatalogoPorDepartamento = "/api/admin/relatorios-avancados/catalogo-servicos/por-departamento";
    private const string EndpointInventarioResumo = "/api/admin/relatorios-avancados/inventario-ativos/resumo";
    private const string EndpointBaseResumo = "/api/admin/relatorios-avancados/base-conhecimento/resumo";
    private const string EndpointAuditoriaResumo = "/api/admin/relatorios-avancados/auditoria/resumo";
    private const string EndpointAuditoriaPorUsuario = "/api/admin/relatorios-avancados/auditoria/por-usuario";
    private readonly ApiIntegrationTestFactory _factory;

    public RelatoriosAvancadosAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AtendenteComPermissaoVisualizarAcessaMetadados()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.relatorios.ok.{Guid.NewGuid():N}@empresa.com", "Atendente Relatorios", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(EndpointMetadados);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EndpointResumoSemPermissaoVisualizarRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.resumo.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Visualizar", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        }
    }

    [Fact]
    public async Task EndpointProdutividadeSemPermissaoOperacionalRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosOperacional, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.prod.semop.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Operacional", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointProdutividade);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosOperacional, true);
        }
    }

    [Fact]
    public async Task EndpointRetornaMetadadosComTiposEsperados()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.metadados.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(EndpointMetadados);
        var payload = await response.Content.ReadFromJsonAsync<RelatorioMetadadosDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);

        var tiposEsperados = Enum.GetValues<TipoRelatorioAvancado>();
        Assert.Equal(tiposEsperados.Length, payload!.TiposRelatorioDisponiveis.Count);
        Assert.All(tiposEsperados, tipo => Assert.Contains(tipo, payload.TiposRelatorioDisponiveis));
    }

    [Fact]
    public async Task FiltroDataInicialMaiorQueDataFinalRetornaBadRequest()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.filtro.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync($"{EndpointResumo}?DataInicial=2026-05-10&DataFinal=2026-05-01");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AgrupamentoInvalidoRetornaBadRequest()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.agrup.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/relatorios-avancados/chamados/serie-temporal?Agrupamento=Ano");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EndpointSlaResumoSemPermissaoGerencialRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.sla.semgerencial.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerencial", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointSlaResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        }
    }

    [Fact]
    public async Task EndpointSlaViolacoesComPermissoesCorretasRetornaOk()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosOperacional, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.relatorios.sla.violacoes.{Guid.NewGuid():N}@empresa.com", "Atendente Operacional", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(EndpointSlaViolacoes);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EndpointAprovacoesResumoSemPermissaoGerencialRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.aprovacoes.semgerencial.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerencial", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointAprovacoesResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        }
    }

    [Fact]
    public async Task EndpointAprovacoesPorOrigemSemPermissaoOperacionalRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosOperacional, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.aprovacoes.semoperacional.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Operacional", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointAprovacoesPorOrigem);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosOperacional, true);
        }
    }

    [Fact]
    public async Task EndpointCatalogoResumoSemPermissaoGerencialRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.catalogo.semgerencial.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerencial", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointCatalogoResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        }
    }

    [Fact]
    public async Task EndpointCatalogoPorDepartamentoComPermissaoVisualizarRetornaOk()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.catalogo.pordepartamento.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(EndpointCatalogoPorDepartamento);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EndpointAprovacoesComEnumInvalidoRetornaBadRequest()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.aprovacoes.enuminvalido.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync($"{EndpointAprovacoesResumo}?StatusAprovacao=Invalido");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EndpointInventarioResumoSemPermissaoGerencialRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.inventario.semgerencial.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerencial", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointInventarioResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        }
    }

    [Fact]
    public async Task EndpointBaseResumoSemPermissaoGerencialRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.base.semgerencial.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Gerencial", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointBaseResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        }
    }

    [Fact]
    public async Task EndpointAuditoriaResumoSemPermissaoAuditoriaRetornaForbidden()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosAuditoria, false);

        try
        {
            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"atendente.relatorios.auditoria.semauditoria.{Guid.NewGuid():N}@empresa.com", "Atendente Sem Auditoria", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.GetAsync(EndpointAuditoriaResumo);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosAuditoria, true);
        }
    }

    [Fact]
    public async Task EndpointAuditoriaPorUsuarioComPermissoesCorretasRetornaOk()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosAuditoria, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.auditoria.ok.{Guid.NewGuid():N}@empresa.com", "Admin Auditoria", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync(EndpointAuditoriaPorUsuario);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task EndpointInventarioComEnumInvalidoRetornaBadRequest()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.inventario.enuminvalido.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync($"{EndpointInventarioResumo}?StatusOperacional=Inexistente");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/admin/relatorios-avancados/chamados/resumo")]
    [InlineData("/api/admin/relatorios-avancados/sla/resumo")]
    [InlineData("/api/admin/relatorios-avancados/aprovacoes/resumo")]
    [InlineData("/api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados")]
    [InlineData("/api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes")]
    [InlineData("/api/admin/relatorios-avancados/auditoria/resumo")]
    public async Task EndpointsDoDashboardComPeriodoUtcValidoNaoRetornamBadRequest(string endpoint)
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosVisualizar, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosGerencial, true);
        await DefinirPermissaoAtivaAsync(PermissoesConstants.RelatoriosAvancadosAuditoria, true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.relatorios.periodoutc.{Guid.NewGuid():N}@empresa.com", "Admin Relatorios", "Administrador");
        _ = await client.GetAsync("/api/me");

        const string dataInicialUtc = "2026-04-25T00:00:00.000Z";
        const string dataFinalUtc = "2026-05-25T23:59:59.999Z";
        var query = endpoint.Contains("mais-solicitados", StringComparison.OrdinalIgnoreCase)
            || endpoint.Contains("chamados-recorrentes", StringComparison.OrdinalIgnoreCase)
            || endpoint.Contains("auditoria/resumo", StringComparison.OrdinalIgnoreCase)
            ? $"?DataInicial={dataInicialUtc}&DataFinal={dataFinalUtc}&LimiteRanking=5"
            : $"?DataInicial={dataInicialUtc}&DataFinal={dataFinalUtc}";

        var response = await client.GetAsync($"{endpoint}{query}");

        Assert.NotEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task DefinirPermissaoAtivaAsync(string permissaoCodigo, bool ativa, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var permissao = await dbContext.PermissoesSistema
            .FirstAsync(x => x.Codigo == permissaoCodigo, cancellationToken);

        if (permissao.Ativo == ativa)
        {
            return;
        }

        if (ativa)
        {
            permissao.Ativar("integration-test");
        }
        else
        {
            permissao.Desativar("integration-test");
        }

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
