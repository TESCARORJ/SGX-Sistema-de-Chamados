using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class AdminSlaUseCasesTests
{
    [Fact]
    public async Task CriarPoliticaDeveRejeitarDuplicidadeDePrioridade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var useCase = new CriarPoliticaSlaUseCase(
            PortalUseCasesTestFactory.Repo<PoliticaSla>(context),
            PortalUseCasesTestFactory.Repo<MetaSla>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CalendarioCorporativo>(context),
            new FakeUsuarioContextoAplicacaoService(new(
                Guid.NewGuid(),
                "Administrador",
                "admin@sgx.local",
                "admin@sgx.local",
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

        var request = new CriarPoliticaSlaRequest
        {
            Nome = "SLA duplicado",
            Ordem = 99,
            Metas =
            [
                new MetaSlaUpsertRequest
                {
                    PrioridadeId = SeedData.PrioridadeAltaId,
                    TempoPrimeiraRespostaMinutos = 30,
                    TempoResolucaoMinutos = 120,
                    Ativo = true
                },
                new MetaSlaUpsertRequest
                {
                    PrioridadeId = SeedData.PrioridadeAltaId,
                    TempoPrimeiraRespostaMinutos = 60,
                    TempoResolucaoMinutos = 180,
                    Ativo = true
                }
            ]
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(request));
    }
}
