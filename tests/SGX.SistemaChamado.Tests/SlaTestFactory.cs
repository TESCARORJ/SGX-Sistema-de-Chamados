using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

internal static class SlaTestFactory
{
    public static ISlaService CriarService(SGXSistemaChamadoDbContext context)
    {
        var calculator = new SlaCalculator(
            PortalUseCasesTestFactory.Repo<SlaConfiguracao>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context));

        return new SlaService(
            calculator,
            PortalUseCasesTestFactory.Repo<SlaControle>(context));
    }
}
