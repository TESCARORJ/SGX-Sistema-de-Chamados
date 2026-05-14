using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaSprint3Tests
{
    [Fact]
    public async Task SeedPadraoDeConfiguracaoDeAlertaFoiCriado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var config = await context.ConfiguracoesAlertaSla.FindAsync(SeedData.ConfiguracaoAlertaSlaPadraoId);

        Assert.NotNull(config);
        Assert.True(config!.Ativo);
        Assert.Equal(30, config.MinutosAntesVencimentoPrimeiraResposta);
        Assert.Equal(120, config.MinutosAntesVencimentoResolucao);
        Assert.True(config.NotificarAtendente);
    }

    [Fact]
    public async Task ConfiguracaoDeAlertaPodeSerAtualizada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context);
        var useCase = new AtualizarConfiguracaoAlertaSlaUseCase(
            PortalUseCasesTestFactory.Repo<ConfiguracaoAlertaSla>(context),
            new FakeUsuarioContextoAplicacaoService(admin),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new AtualizarConfiguracaoAlertaSlaRequest
        {
            Ativo = false,
            MinutosAntesVencimentoPrimeiraResposta = 10,
            MinutosAntesVencimentoResolucao = 45,
            NotificarAtendente = false,
            NotificarGestor = true,
            NotificarDepartamento = true
        });

        Assert.False(response.Ativo);
        Assert.Equal(10, response.MinutosAntesVencimentoPrimeiraResposta);
        Assert.True(response.NotificarGestor);
    }

    [Fact]
    public async Task SlaAplicadoRegistraEventoUmaUnicaVez()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);
        await CriarPoliticaAsync(context, prioridade.Id, 30, 120);

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow);
        await context.SaveChangesAsync();
        var sla = context.ChamadosSla.First(x => x.ChamadoId == chamado.Id);
        var eventService = new SlaEventService(PortalUseCasesTestFactory.Repo<EventoSla>(context));
        await eventService.RegistrarAsync(sla, TipoEventoSla.SlaAplicado, "SLA aplicado ao chamado.", DateTime.UtcNow, "teste", chaveIdempotencia: $"chamado-sla:{sla.Id}:sla-aplicado");
        await context.SaveChangesAsync();

        Assert.Single(context.EventosSla.Where(x => x.TipoEvento == TipoEventoSla.SlaAplicado));
    }

    [Fact]
    public async Task PrimeiraRespostaResolucaoPausaERetomadaRegistramEventos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);
        await CriarPoliticaAsync(context, prioridade.Id, 30, 120);
        var inicio = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        var statusAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var statusAguardando = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.AguardandoSolicitante);
        await service.AplicarMudancaStatusAsync(chamado, statusAtendimento, statusAguardando, "teste", inicio.AddMinutes(5));
        await service.AplicarMudancaStatusAsync(chamado, statusAguardando, statusAtendimento, "teste", inicio.AddMinutes(10));
        await service.RegistrarEncerramentoAsync(chamado, "teste", inicio.AddMinutes(50));
        await context.SaveChangesAsync();

        Assert.Contains(context.EventosSla, x => x.TipoEvento == TipoEventoSla.PrimeiraRespostaDentroDoPrazo);
        Assert.Contains(context.EventosSla, x => x.TipoEvento == TipoEventoSla.ResolucaoDentroDoPrazo);
        Assert.Contains(context.EventosSla, x => x.TipoEvento == TipoEventoSla.SlaPausado);
        Assert.Contains(context.EventosSla, x => x.TipoEvento == TipoEventoSla.SlaRetomado);
    }

    [Fact]
    public async Task MonitoramentoGeraAlertasEConfigInativaImpedeNovosEventos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (_, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);
        await CriarChamadoComSlaManualAsync(context, prioridade.Id, DateTime.UtcNow.AddMinutes(10), DateTime.UtcNow.AddMinutes(-5));
        var monitoring = CriarMonitoring(context);

        await monitoring.ExecutarVerificacaoAsync();
        await monitoring.ExecutarVerificacaoAsync();

        Assert.Single(context.EventosSla.Where(x => x.TipoEvento == TipoEventoSla.AlertaPrimeiraRespostaProximoVencimento));
        Assert.Single(context.EventosSla.Where(x => x.TipoEvento == TipoEventoSla.AlertaResolucaoVencida));

        var config = context.ConfiguracoesAlertaSla.First();
        config.Desativar("teste");
        await context.SaveChangesAsync();

        await monitoring.ExecutarVerificacaoAsync();
        Assert.Equal(2, context.EventosSla.Count());
    }

    [Fact]
    public async Task DashboardSlaCalculaIndicadoresEAgrupamentos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context);
        var (_, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);
        await CriarChamadoComSlaManualAsync(context, prioridade.Id, DateTime.UtcNow.AddMinutes(-60), DateTime.UtcNow.AddMinutes(-10));

        var useCase = new ObterDashboardSlaUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(admin));

        var response = await useCase.ExecutarAsync(new FiltroDashboardSlaRequest());

        Assert.True(response.TotalComSlaAplicado >= 1);
        Assert.True(response.TotalVencidos >= 1);
        Assert.NotEmpty(response.PorPrioridade);
        Assert.NotEmpty(response.PorCategoria);
        Assert.NotEmpty(response.PorDepartamento);
    }

    private static SlaMonitoringService CriarMonitoring(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ConfiguracaoAlertaSla>(context),
            new SlaEventService(PortalUseCasesTestFactory.Repo<EventoSla>(context)),
            PortalUseCasesTestFactory.Uow(context),
            NullLogger<SlaMonitoringService>.Instance);

    private static async Task<UsuarioContextoAplicacao> CriarAdminAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin SLA 3", $"admin.sla3.{Guid.NewGuid():N}@sgx.local", TipoPerfil.Administrador);
        return AdminUseCasesTestFactory.Contexto(admin, "Administrador");
    }

    private static async Task<(Chamado Chamado, PrioridadeChamado Prioridade)> CriarChamadoBaseAsync(
        SGXSistemaChamadoDbContext context,
        PrioridadeChamadoEnum nivelPrioridade)
    {
        foreach (var politica in context.SlaPoliticas.Where(x => x.Ativo).ToList())
        {
            politica.Desativar("teste");
        }

        foreach (var meta in context.SlaMetas.Where(x => x.Ativo).ToList())
        {
            meta.Desativar("teste");
        }

        await context.SaveChangesAsync();

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {Guid.NewGuid():N}"[..18], $"sol.{Guid.NewGuid():N}@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {Guid.NewGuid():N}"[..18]);
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == nivelPrioridade);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var chamado = new Chamado($"CH-S3-{Guid.NewGuid():N}"[..16], "Chamado SLA 3", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, statusAberto.Id, OrigemChamado.Portal, "teste");
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();

        return (chamado, prioridade);
    }

    private static async Task CriarChamadoComSlaManualAsync(SGXSistemaChamadoDbContext context, Guid prioridadeId, DateTime prazoPrimeiraResposta, DateTime prazoResolucao)
    {
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {Guid.NewGuid():N}"[..18], $"sol.{Guid.NewGuid():N}@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {Guid.NewGuid():N}"[..18]);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var chamado = new Chamado($"CH-M3-{Guid.NewGuid():N}"[..16], "Chamado SLA manual", "Descricao", solicitante.Id, categoria.Id, prioridadeId, statusAberto.Id, OrigemChamado.Portal, "teste");
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();

        context.ChamadosSla.Add(new ChamadoSla(chamado.Id, null, prioridadeId, DateTime.UtcNow.AddHours(-1), prazoPrimeiraResposta, prazoResolucao, true, false, null, "teste"));
        await context.SaveChangesAsync();
    }

    private static async Task CriarPoliticaAsync(SGXSistemaChamadoDbContext context, Guid prioridadeId, int primeiraRespostaMinutos, int resolucaoMinutos)
    {
        var politica = new PoliticaSla("SLA Sprint 3", "Politica de teste", 1, null, null, null, false, true, "teste");
        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();

        context.SlaMetas.Add(new MetaSla(politica.Id, prioridadeId, primeiraRespostaMinutos, resolucaoMinutos, null, null, "teste"));
        await context.SaveChangesAsync();
    }
}
