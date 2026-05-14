using Microsoft.Extensions.Logging.Abstractions;
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
            PortalUseCasesTestFactory.Repo<PoliticaSla>(context),
            PortalUseCasesTestFactory.Repo<CalendarioCorporativo>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            NullLogger<SlaCalculator>.Instance);

        return new SlaService(
            calculator,
            PortalUseCasesTestFactory.Repo<ChamadoSla>(context),
            PortalUseCasesTestFactory.Repo<CalendarioCorporativo>(context),
            new SlaBusinessTimeCalculator(),
            new SlaEventService(PortalUseCasesTestFactory.Repo<EventoSla>(context)));
    }
}
