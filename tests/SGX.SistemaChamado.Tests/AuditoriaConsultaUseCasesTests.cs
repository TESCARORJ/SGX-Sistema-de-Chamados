using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Auditoria;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AuditoriaConsultaUseCasesTests
{
    [Fact]
    public async Task ListagemRetornaEventosPaginados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        await SeedEventosAsync(context);
        var usuarioContexto = CriarUsuarioAdminContexto();

        var useCase = new ListarEventosAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        var response = await useCase.ExecutarAsync(new FiltroEventosAuditoriaRequest
        {
            Pagina = 1,
            TamanhoPagina = 2
        });

        Assert.Equal(5, response.Total);
        Assert.Equal(2, response.Items.Count);
        Assert.All(response.Items, x => Assert.NotEqual(Guid.Empty, x.Id));
    }

    [Fact]
    public async Task FiltroPorPeriodoUsuarioModuloEntidadeAcaoNivelSucessoCorrelacaoETextoFunciona()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        await SeedEventosAsync(context);
        var usuarioContexto = CriarUsuarioAdminContexto();

        var useCase = new ListarEventosAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        var response = await useCase.ExecutarAsync(new FiltroEventosAuditoriaRequest
        {
            DataInicio = new DateTime(2026, 5, 12, 0, 0, 0, DateTimeKind.Utc),
            DataFim = new DateTime(2026, 5, 12, 23, 59, 59, DateTimeKind.Utc),
            UsuarioEmail = "admin.usuarios@sgx.com",
            Modulo = "Usu",
            Entidade = "Usu",
            Acao = TipoAcaoAuditoria.Edicao,
            Nivel = NivelAuditoria.Alerta,
            Sucesso = false,
            CorrelacaoId = "corr-2",
            Texto = "falha valida"
        });

        var evento = Assert.Single(response.Items);
        Assert.Equal("Usuários", evento.Modulo);
        Assert.Equal(NivelAuditoria.Alerta, evento.Nivel);
        Assert.False(evento.Sucesso);
    }

    [Fact]
    public async Task DetalheRetornaDadosAntesDepoisMetadados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var eventoId = await SeedEventosAsync(context);
        var usuarioContexto = CriarUsuarioAdminContexto();

        var useCase = new ObterEventoAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        var response = await useCase.ExecutarAsync(eventoId);

        Assert.Equal(eventoId, response.Id);
        Assert.Contains("StatusAnterior", response.DadosAntes ?? string.Empty);
        Assert.Contains("StatusNovo", response.DadosDepois ?? string.Empty);
        Assert.Contains("origem", response.Metadados ?? string.Empty);
    }

    [Fact]
    public async Task DetalheInexistenteRetornaKeyNotFound()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        await SeedEventosAsync(context);
        var usuarioContexto = CriarUsuarioAdminContexto();

        var useCase = new ObterEventoAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DashboardCalculaTotaisEAgrupamentos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        await SeedEventosAsync(context);
        var usuarioContexto = CriarUsuarioAdminContexto();

        var useCase = new ObterDashboardAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        var response = await useCase.ExecutarAsync(new FiltroDashboardAuditoriaRequest());

        Assert.Equal(5, response.TotalEventos);
        Assert.Equal(1, response.TotalEventosCriticos);
        Assert.Equal(1, response.TotalEventosAlerta);
        Assert.Equal(3, response.TotalEventosInformacao);
        Assert.Equal(2, response.TotalFalhas);
        Assert.Equal(3, response.TotalSucessos);
        Assert.Contains(response.EventosPorModulo, x => x.Chave == "Chamados");
        Assert.Contains(response.EventosPorAcao, x => x.Chave == TipoAcaoAuditoria.Edicao.ToString());
        Assert.NotEmpty(response.EventosPorUsuario);
        Assert.NotEmpty(response.EventosPorDia);
    }

    [Fact]
    public async Task UsuarioSemPerfilAdminNaoPodeConsultarAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        await SeedEventosAsync(context);
        var usuarioContexto = new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Solicitante",
            "solicitante@sgx.com",
            "solicitante@sgx.com",
            ["Solicitante"]);

        var useCase = new ListarEventosAuditoriaUseCase(
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            new FakeUsuarioContextoAplicacaoService(usuarioContexto));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecutarAsync(new FiltroEventosAuditoriaRequest()));
    }

    private static UsuarioContextoAplicacao CriarUsuarioAdminContexto()
        => new(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.com",
            "admin@sgx.com",
            ["Administrador"]);

    private static async Task<Guid> SeedEventosAsync(DbContext context)
    {
        var set = context.Set<EventoAuditoria>();
        var eventoChamadosId = Guid.Empty;

        var eventos = new[]
        {
            new EventoAuditoria(
                new DateTime(2026, 5, 10, 11, 0, 0, DateTimeKind.Utc),
                Guid.NewGuid(),
                "Admin Local",
                "admin.local@sgx.com",
                "admin.local@sgx.com",
                "127.0.0.1",
                "agent-1",
                "Autenticacao Corporativa",
                "Usuario",
                "user-1",
                TipoAcaoAuditoria.Login,
                "Login local realizado com sucesso.",
                null,
                null,
                "{\"origem\":\"api\"}",
                NivelAuditoria.Informacao,
                true,
                null,
                "corr-1"),
            new EventoAuditoria(
                new DateTime(2026, 5, 11, 14, 0, 0, DateTimeKind.Utc),
                Guid.NewGuid(),
                "Atendente Chamados",
                "atendente.chamados@sgx.com",
                "atendente.chamados@sgx.com",
                "10.0.0.12",
                "agent-2",
                "Chamados",
                "Chamado",
                "c-1",
                TipoAcaoAuditoria.AlteracaoStatus,
                "Status do chamado alterado.",
                "{\"StatusAnterior\":\"Aberto\"}",
                "{\"StatusNovo\":\"EmAtendimento\"}",
                "{\"origem\":\"api\",\"codigo\":\"SGX-2026-100001\"}",
                NivelAuditoria.Informacao,
                true,
                null,
                "corr-1"),
            new EventoAuditoria(
                new DateTime(2026, 5, 12, 9, 30, 0, DateTimeKind.Utc),
                Guid.NewGuid(),
                "Admin Usuários",
                "admin.usuarios@sgx.com",
                "admin.usuarios@sgx.com",
                "10.0.0.21",
                "agent-3",
                "Usuários",
                "Usuario",
                "u-1",
                TipoAcaoAuditoria.Edicao,
                "Falha validação de dados do usuário.",
                "{\"StatusAnterior\":\"Ativo\"}",
                "{\"StatusNovo\":\"Inativo\"}",
                "{\"origem\":\"api\",\"resultado\":\"Falha\"}",
                NivelAuditoria.Alerta,
                false,
                "Falha validação.",
                "corr-2"),
            new EventoAuditoria(
                new DateTime(2026, 5, 13, 8, 0, 0, DateTimeKind.Utc),
                Guid.NewGuid(),
                "Admin SLA",
                "admin.sla@sgx.com",
                "admin.sla@sgx.com",
                "10.0.0.31",
                "agent-4",
                "SLA",
                "PoliticaSla",
                "sla-1",
                TipoAcaoAuditoria.Configuracao,
                "Erro na atualização da política de SLA.",
                "{\"Tempo\":\"240\"}",
                "{\"Tempo\":\"120\"}",
                "{\"origem\":\"api\",\"operacao\":\"AtualizarPolitica\"}",
                NivelAuditoria.Critico,
                false,
                "Erro interno.",
                "corr-3"),
            new EventoAuditoria(
                new DateTime(2026, 5, 14, 16, 15, 0, DateTimeKind.Utc),
                Guid.NewGuid(),
                "Admin Roadmap",
                "admin.roadmap@sgx.com",
                "admin.roadmap@sgx.com",
                "10.0.0.41",
                "agent-5",
                "Roadmap ITSM",
                "RoadmapItsmItem",
                "rm-1",
                TipoAcaoAuditoria.Edicao,
                "Checklist do roadmap atualizado.",
                "{\"concluido\":false}",
                "{\"concluido\":true}",
                "{\"origem\":\"web\"}",
                NivelAuditoria.Informacao,
                true,
                null,
                "corr-4")
        };

        await set.AddRangeAsync(eventos);
        await context.SaveChangesAsync();

        eventoChamadosId = eventos[1].Id;
        return eventoChamadosId;
    }
}
