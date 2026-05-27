using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class PrioridadeChamadoMatrizServiceTests
{
    [Theory]
    [InlineData(ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Alta, PrioridadeChamadoEnum.Critica)]
    [InlineData(ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Media, PrioridadeChamadoEnum.Alta)]
    [InlineData(ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Baixa, PrioridadeChamadoEnum.Media)]
    [InlineData(ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Alta, PrioridadeChamadoEnum.Alta)]
    [InlineData(ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Media, PrioridadeChamadoEnum.Media)]
    [InlineData(ImpactoChamadoEnum.Medio, UrgenciaChamadoEnum.Baixa, PrioridadeChamadoEnum.Baixa)]
    [InlineData(ImpactoChamadoEnum.Baixo, UrgenciaChamadoEnum.Alta, PrioridadeChamadoEnum.Media)]
    [InlineData(ImpactoChamadoEnum.Baixo, UrgenciaChamadoEnum.Media, PrioridadeChamadoEnum.Baixa)]
    [InlineData(ImpactoChamadoEnum.Baixo, UrgenciaChamadoEnum.Baixa, PrioridadeChamadoEnum.Baixa)]
    public void DeveCalcularNivelConformeMatrizOficial(
        ImpactoChamadoEnum impacto,
        UrgenciaChamadoEnum urgencia,
        PrioridadeChamadoEnum prioridadeEsperada)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context));

        var nivel = service.CalcularNivel(impacto, urgencia);

        Assert.Equal(prioridadeEsperada, nivel);
    }

    [Fact]
    public async Task DeveResolverPrioridadePersistidaConformeMatriz()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context));

        var prioridade = await service.ObterPrioridadeAsync(ImpactoChamadoEnum.Alto, UrgenciaChamadoEnum.Alta);

        Assert.NotNull(prioridade);
        Assert.Equal(PrioridadeChamadoEnum.Critica, prioridade!.Nivel);
    }
}
