using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces;

public interface IPrioridadeChamadoMatrizService
{
    PrioridadeChamadoEnum CalcularNivel(ImpactoChamadoEnum impactoChamado, UrgenciaChamadoEnum urgenciaChamado);
    Task<PrioridadeChamado?> ObterPrioridadeAsync(ImpactoChamadoEnum impactoChamado, UrgenciaChamadoEnum urgenciaChamado, CancellationToken cancellationToken = default);
}
