using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaCalculator(
    IRepository<PoliticaSla> politicaRepository,
    IRepository<CalendarioCorporativo> calendarioRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    ILogger<SlaCalculator> logger) : ISlaCalculator
{
    public async Task<SlaPrazosAplicados?> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        CancellationToken cancellationToken = default)
        => await CalcularPrazosAsync(prioridadeId, categoriaId, departamentoId, null, cancellationToken);

    public async Task<SlaPrazosAplicados?> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        Guid? politicaSlaIdPreferencial,
        CancellationToken cancellationToken = default)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade informada para calculo de SLA e invalida.", nameof(prioridadeId));
        }

        if (politicaSlaIdPreferencial.HasValue)
        {
            var politicaPreferencial = await politicaRepository.Query()
                .AsNoTracking()
                .Where(x => x.Ativo && x.Id == politicaSlaIdPreferencial.Value)
                .Include(x => x.CalendarioCorporativo)
                    .ThenInclude(x => x!.HorariosAtendimento)
                .Include(x => x.CalendarioCorporativo)
                    .ThenInclude(x => x!.Excecoes)
                .Include(x => x.Metas.Where(m => m.Ativo && m.PrioridadeId == prioridadeId))
                .FirstOrDefaultAsync(cancellationToken);

            var metaPreferencial = politicaPreferencial?.Metas.FirstOrDefault();
            if (politicaPreferencial is not null && metaPreferencial is not null)
            {
                var calendarioPreferencial = await ResolverCalendarioAsync(politicaPreferencial, cancellationToken);
                return new SlaPrazosAplicados(
                    politicaPreferencial.Id,
                    politicaPreferencial.Nome,
                    prioridadeId,
                    metaPreferencial.TempoPrimeiraRespostaMinutos,
                    metaPreferencial.TempoResolucaoMinutos,
                    politicaPreferencial.UsarHorarioComercial,
                    calendarioPreferencial?.Id,
                    calendarioPreferencial?.Nome,
                    calendarioPreferencial,
                    politicaPreferencial.PausarQuandoAguardandoSolicitante,
                    $"PoliticaPreferencial:{politicaPreferencial.Nome}");
            }
        }

        var politicas = await politicaRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Include(x => x.CalendarioCorporativo)
                .ThenInclude(x => x!.HorariosAtendimento)
            .Include(x => x.CalendarioCorporativo)
                .ThenInclude(x => x!.Excecoes)
            .Include(x => x.Metas.Where(m => m.Ativo && m.PrioridadeId == prioridadeId))
            .ToListAsync(cancellationToken);

        var candidata = politicas
            .Select(p => new
            {
                Politica = p,
                Meta = p.Metas.FirstOrDefault(),
                Score = CalcularScore(p, categoriaId, departamentoId),
                Match = EhCompativel(p, categoriaId, departamentoId)
            })
            .Where(x => x.Match && x.Meta is not null)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Politica.Ordem)
            .ThenBy(x => x.Politica.Nome)
            .FirstOrDefault();

        if (candidata?.Meta is not null)
        {
            var calendario = await ResolverCalendarioAsync(candidata.Politica, cancellationToken);

            return new SlaPrazosAplicados(
                candidata.Politica.Id,
                candidata.Politica.Nome,
                prioridadeId,
                candidata.Meta.TempoPrimeiraRespostaMinutos,
                candidata.Meta.TempoResolucaoMinutos,
                candidata.Politica.UsarHorarioComercial,
                calendario?.Id,
                calendario?.Nome,
                calendario,
                candidata.Politica.PausarQuandoAguardandoSolicitante,
                $"Politica:{candidata.Politica.Nome}");
        }

        _ = prioridadeRepository;
        return null;
    }

    private async Task<CalendarioCorporativo?> ResolverCalendarioAsync(PoliticaSla politica, CancellationToken cancellationToken)
    {
        if (!politica.UsarHorarioComercial)
        {
            return null;
        }

        if (politica.CalendarioCorporativo is { Ativo: true } calendarioVinculado)
        {
            return calendarioVinculado;
        }

        var calendarioPadrao = await calendarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.HorariosAtendimento)
            .Include(x => x.Excecoes)
            .Where(x => x.Ativo && x.Padrao)
            .OrderBy(x => x.Nome)
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarioPadrao is null)
        {
            logger.LogWarning(
                "Politica SLA {PoliticaSlaId} configurada para horario comercial sem calendario vinculado e sem calendario padrao ativo. O calculo usara minutos corridos.",
                politica.Id);
        }

        return calendarioPadrao;
    }

    private static int CalcularScore(PoliticaSla politica, Guid? categoriaId, Guid? departamentoId)
    {
        var score = 0;
        if (politica.CategoriaId.HasValue && politica.CategoriaId == categoriaId)
        {
            score += 2;
        }

        if (politica.DepartamentoId.HasValue && politica.DepartamentoId == departamentoId)
        {
            score += 1;
        }

        return score;
    }

    private static bool EhCompativel(PoliticaSla politica, Guid? categoriaId, Guid? departamentoId)
    {
        if (politica.CategoriaId.HasValue && politica.CategoriaId != categoriaId)
        {
            return false;
        }

        if (politica.DepartamentoId.HasValue && politica.DepartamentoId != departamentoId)
        {
            return false;
        }

        return true;
    }
}
