using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaCalculator(
    IRepository<SlaConfiguracao> slaConfiguracaoRepository,
    IRepository<PrioridadeChamado> prioridadeRepository) : ISlaCalculator
{
    public async Task<SlaPrazos> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        CancellationToken cancellationToken = default)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade informada para calculo de SLA e invalida.", nameof(prioridadeId));
        }

        var configuracoes = await slaConfiguracaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo && x.PrioridadeId == prioridadeId)
            .ToListAsync(cancellationToken);

        var porDepartamentoCategoria = configuracoes.FirstOrDefault(x =>
            x.DepartamentoId == departamentoId &&
            x.CategoriaId == categoriaId &&
            departamentoId.HasValue &&
            categoriaId.HasValue);

        if (porDepartamentoCategoria is not null)
        {
            return new SlaPrazos(
                porDepartamentoCategoria.PrazoPrimeiraRespostaHoras,
                porDepartamentoCategoria.PrazoResolucaoHoras,
                "Departamento+Categoria+Prioridade");
        }

        var porCategoria = configuracoes.FirstOrDefault(x =>
            x.DepartamentoId is null &&
            x.CategoriaId == categoriaId &&
            categoriaId.HasValue);

        if (porCategoria is not null)
        {
            return new SlaPrazos(
                porCategoria.PrazoPrimeiraRespostaHoras,
                porCategoria.PrazoResolucaoHoras,
                "Categoria+Prioridade");
        }

        var porDepartamento = configuracoes.FirstOrDefault(x =>
            x.DepartamentoId == departamentoId &&
            x.CategoriaId is null &&
            departamentoId.HasValue);

        if (porDepartamento is not null)
        {
            return new SlaPrazos(
                porDepartamento.PrazoPrimeiraRespostaHoras,
                porDepartamento.PrazoResolucaoHoras,
                "Departamento+Prioridade");
        }

        var porPrioridade = configuracoes.FirstOrDefault(x => x.DepartamentoId is null && x.CategoriaId is null);
        if (porPrioridade is not null)
        {
            return new SlaPrazos(
                porPrioridade.PrazoPrimeiraRespostaHoras,
                porPrioridade.PrazoResolucaoHoras,
                "Prioridade");
        }

        var prioridade = await prioridadeRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == prioridadeId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Prioridade nao encontrada para fallback de SLA.");

        return new SlaPrazos(
            prioridade.PrazoPrimeiraRespostaHoras,
            prioridade.PrazoResolucaoHoras,
            "FallbackPrioridade");
    }
}
