using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services;

public sealed class PrioridadeChamadoMatrizService(
    IRepository<PrioridadeChamado> prioridadeRepository) : IPrioridadeChamadoMatrizService
{
    public PrioridadeChamadoEnum CalcularNivel(ImpactoChamadoEnum impactoChamado, UrgenciaChamadoEnum urgenciaChamado)
    {
        if (!Enum.IsDefined(impactoChamado))
        {
            throw new ArgumentException("Impacto do chamado invalido.", nameof(impactoChamado));
        }

        if (!Enum.IsDefined(urgenciaChamado))
        {
            throw new ArgumentException("Urgencia do chamado invalida.", nameof(urgenciaChamado));
        }

        return (impactoChamado, urgenciaChamado) switch
        {
            (ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Alta) => PrioridadeChamadoEnum.Critica,
            (ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Media) => PrioridadeChamadoEnum.Alta,
            (ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Baixa) => PrioridadeChamadoEnum.Media,
            (ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Alta) => PrioridadeChamadoEnum.Alta,
            (ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Media) => PrioridadeChamadoEnum.Media,
            (ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Baixa) => PrioridadeChamadoEnum.Baixa,
            (ImpactoChamadoEnum.Baixo, UrgenciaChamadoEnum.Alta) => PrioridadeChamadoEnum.Media,
            (ImpactoChamadoEnum.Baixo, UrgenciaChamadoEnum.Media) => PrioridadeChamadoEnum.Baixa,
            _ => PrioridadeChamadoEnum.Baixa
        };
    }

    public Task<PrioridadeChamado?> ObterPrioridadeAsync(
        ImpactoChamadoEnum impactoChamado,
        UrgenciaChamadoEnum urgenciaChamado,
        CancellationToken cancellationToken = default)
    {
        var nivel = CalcularNivel(impactoChamado, urgenciaChamado);
        return prioridadeRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Nivel == nivel, cancellationToken);
    }
}
