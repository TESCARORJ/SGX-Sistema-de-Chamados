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

        var useCase = new ListarLogsIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Status = StatusProcessamentoEmail.Processado });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Equal(StatusProcessamentoEmail.Processado, x.StatusProcessamento));
    }

    [Fact]
    public async Task ListaLogsComFiltroPorRemetente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 2", "admin.logs2@sgx.local", TipoPerfil.Administrador);
        await SeedLogsAsync(context);

        var useCase = new ListarLogsIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroLogsEmailRequest { Remetente = "filtro.remetente@sgx.local" });

        Assert.NotEmpty(response.Items);
        Assert.All(response.Items, x => Assert.Contains("filtro.remetente@sgx.local", x.Remetente));
    }

    [Fact]
    public async Task DetalheRetornaErroTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Logs 3", "admin.logs3@sgx.local", TipoPerfil.Administrador);
        var erro = new LogIntegracaoEmail("erro-msg", "fp-erro", "erro@sgx.local", "Erro", "Falha", DateTime.UtcNow, "teste");
        erro.RegistrarTentativa("teste");
        erro.MarcarErro("StackTrace: teste erro tecnico", DateTime.UtcNow, "teste");
        context.LogsIntegracaoEmail.Add(erro);
        await context.SaveChangesAsync();

        var useCase = new ObterLogIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(erro.Id);

        Assert.Contains("StackTrace", response.Erro);
    }

    [Fact]
    public async Task SolicitanteNaoDeveAcessarLogs()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Logs", "sol.logs@sgx.local", TipoPerfil.Solicitante);

        var useCase = new ListarLogsIntegracaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<LogIntegracaoEmail>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante")));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(new FiltroLogsEmailRequest()));
    }

    private static async Task SeedLogsAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var logProcessado = new LogIntegracaoEmail("msg-1", "fp-1", "filtro.remetente@sgx.local", "Filtro", "Assunto 1", DateTime.UtcNow.AddMinutes(-5), "teste");
        logProcessado.RegistrarTentativa("teste");
        logProcessado.MarcarProcessado(null, DateTime.UtcNow.AddMinutes(-4), "teste");

        var logErro = new LogIntegracaoEmail("msg-2", "fp-2", "outro@sgx.local", "Outro", "Assunto 2", DateTime.UtcNow.AddMinutes(-3), "teste");
        logErro.RegistrarTentativa("teste");
        logErro.MarcarErro("Erro de processamento", DateTime.UtcNow.AddMinutes(-2), "teste");

        context.LogsIntegracaoEmail.AddRange(logProcessado, logErro);
        await context.SaveChangesAsync();
    }
}
