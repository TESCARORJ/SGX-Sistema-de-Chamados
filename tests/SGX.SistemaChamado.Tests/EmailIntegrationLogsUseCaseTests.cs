using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class EmailIntegrationLogsUseCaseTests
{
    [Fact]
    public async Task ListaLogsComFiltroPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs", "admin.logs@sgx.local", TipoPerfil.Administrador);
        await SeedLogsAsync(context);

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Status = StatusProcessamentoEmail.Processado });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Equal(StatusProcessamentoEmail.Processado, x.StatusProcessamento));
    }

    [Fact]
    public async Task ListaLogsComFiltroPorRemetenteCaseInsensitive()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 2", "admin.logs2@sgx.local", TipoPerfil.Administrador);
        await SeedLogsAsync(context);

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Remetente = "FILTRO.REMETENTE@SGX.LOCAL" });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Contains("filtro.remetente@sgx.local", x.Remetente));
    }

    [Fact]
    public async Task ListaLogsComFiltroPorAssuntoCaseInsensitive()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 3", "admin.logs3@sgx.local", TipoPerfil.Administrador);
        await SeedLogsAsync(context);

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Assunto = "BACKUP" });

        Assert.Single(response.Items);
        Assert.Contains("Backup", response.Items.Single().Assunto, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListaLogsComFiltroPorMessageId()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 4", "admin.logs4@sgx.local", TipoPerfil.Administrador);
        await SeedLogsAsync(context);

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { MessageId = "msg-backup-01" });

        Assert.Single(response.Items);
        Assert.Equal("msg-backup-01", response.Items.Single().MessageId);
    }

    [Fact]
    public async Task ListaLogsComFiltroPorCodigoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 5", "admin.logs5@sgx.local", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Logs", "sol.logs@sgx.local", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "900");

        var log = new LogIntegracaoEmail("msg-chamado-01", null, null, "fp-chamado", "codigo@sgx.local", null, "Codigo", "Referencia", DateTime.UtcNow.AddMinutes(-2), "teste");
        log.RegistrarTentativa("teste");
        log.MarcarProcessado(chamado.Id, DateTime.UtcNow.AddMinutes(-1), "teste");

        context.LogsIntegracaoEmail.Add(log);
        await context.SaveChangesAsync();

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { CodigoChamado = "CH-ADMIN-900" });

        Assert.Single(response.Items);
        Assert.Equal(chamado.Codigo, response.Items.Single().ChamadoCodigo);
    }

    [Fact]
    public async Task ListaLogsUsaOrdenacaoPadraoDesc()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 6", "admin.logs6@sgx.local", TipoPerfil.Administrador);

        var antigo = new LogIntegracaoEmail("msg-antigo", null, null, "fp-antigo", "antigo@sgx.local", null, "Antigo", "Antigo", DateTime.UtcNow.AddMinutes(-20), "teste");
        antigo.RegistrarTentativa("teste");
        antigo.MarcarProcessado(null, DateTime.UtcNow.AddMinutes(-10), "teste");

        var recente = new LogIntegracaoEmail("msg-recente", null, null, "fp-recente", "recente@sgx.local", null, "Recente", "Recente", DateTime.UtcNow.AddMinutes(-4), "teste");
        recente.RegistrarTentativa("teste");
        recente.MarcarProcessado(null, DateTime.UtcNow.AddMinutes(-1), "teste");

        context.LogsIntegracaoEmail.AddRange(antigo, recente);
        await context.SaveChangesAsync();

        var useCase = CriarUseCaseLista(context, admin);

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest());

        Assert.Equal("msg-recente", response.Items.First().MessageId);
    }

    [Fact]
    public async Task DetalheRetornaCamposTecnicos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 7", "admin.logs7@sgx.local", TipoPerfil.Administrador);

        var erro = new LogIntegracaoEmail("erro-msg", "<reply@sgx>", "<a@sgx> <b@sgx>", "fp-erro", "erro@sgx.local", "suporte@sgx.local", "Erro", "Falha", DateTime.UtcNow, "teste");
        erro.RegistrarTentativa("teste");
        erro.MarcarErro("StackTrace: teste erro tecnico", DateTime.UtcNow, "teste");
        context.LogsIntegracaoEmail.Add(erro);
        await context.SaveChangesAsync();

        var useCase = new ObterLogIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(erro.Id);

        Assert.Equal("erro-msg", response.MessageId);
        Assert.Equal("<reply@sgx>", response.InReplyTo);
        Assert.Equal("<a@sgx> <b@sgx>", response.References);
        Assert.Contains("StackTrace", response.Erro);
    }

    [Fact]
    public async Task SolicitanteNaoDeveAcessarLogs()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Logs", "solicitante.logs@sgx.local", TipoPerfil.Solicitante);

        var useCase = new ListarLogsIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante")));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(new FiltroLogsEmailRequest()));
    }

    [Fact]
    public async Task StatusNaoCorrelacionadoRetornaLabelAmigavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 8", "admin.logs8@sgx.local", TipoPerfil.Administrador);

        var log = new LogIntegracaoEmail("msg-nao-corr", null, null, "fp-nao-corr", "fora@sgx.local", null, null, "Sem correlacao", DateTime.UtcNow, "teste");
        log.RegistrarTentativa("teste");
        log.MarcarNaoCorrelacionado(DateTime.UtcNow, "teste", "Nao foi possivel correlacionar.");

        context.LogsIntegracaoEmail.Add(log);
        await context.SaveChangesAsync();

        var useCase = CriarUseCaseLista(context, admin);
        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Status = StatusProcessamentoEmail.NaoCorrelacionado });

        var item = Assert.Single(response.Items);
        Assert.Equal("Não correlacionado", item.StatusProcessamentoLabel);
    }

    private static ListarLogsIntegracaoEmailUseCase CriarUseCaseLista(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Usuario admin)
    {
        return new ListarLogsIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));
    }

    private static async Task SeedLogsAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var logProcessado = new LogIntegracaoEmail("msg-1", null, null, "fp-1", "filtro.remetente@sgx.local", null, "Filtro", "Assunto 1", DateTime.UtcNow.AddMinutes(-5), "teste");
        logProcessado.RegistrarTentativa("teste");
        logProcessado.MarcarProcessado(null, DateTime.UtcNow.AddMinutes(-4), "teste");

        var logErro = new LogIntegracaoEmail("msg-backup-01", null, null, "fp-2", "outro@sgx.local", null, "Outro", "Falha no Backup", DateTime.UtcNow.AddMinutes(-3), "teste");
        logErro.RegistrarTentativa("teste");
        logErro.MarcarErro("Erro de processamento", DateTime.UtcNow.AddMinutes(-2), "teste");

        context.LogsIntegracaoEmail.AddRange(logProcessado, logErro);
        await context.SaveChangesAsync();
    }
}
