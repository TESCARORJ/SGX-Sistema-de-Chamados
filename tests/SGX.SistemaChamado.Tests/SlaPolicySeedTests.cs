using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaPolicySeedTests
{
    [Fact]
    public void DeveSeedarPoliticaPadraoDeSlaComMetasPorPrioridade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var politica = context.SlaPoliticas.Single(x => x.Id == SeedData.SlaPoliticaPadraoId);
        var metas = context.SlaMetas
            .Where(x => x.PoliticaSlaId == SeedData.SlaPoliticaPadraoId)
            .ToList();

        Assert.Equal("SLA Padrão", politica.Nome);
        Assert.True(politica.Ativo);
        Assert.Equal(1, politica.Ordem);
        Assert.False(politica.UsarHorarioComercial);
        Assert.True(politica.PausarQuandoAguardandoSolicitante);

        Assert.Equal(4, metas.Count);
        Assert.Contains(metas, x => x.PrioridadeId == SeedData.PrioridadeBaixaId && x.TempoPrimeiraRespostaMinutos == 480 && x.TempoResolucaoMinutos == 2880);
        Assert.Contains(metas, x => x.PrioridadeId == SeedData.PrioridadeMediaId && x.TempoPrimeiraRespostaMinutos == 240 && x.TempoResolucaoMinutos == 1440);
        Assert.Contains(metas, x => x.PrioridadeId == SeedData.PrioridadeAltaId && x.TempoPrimeiraRespostaMinutos == 60 && x.TempoResolucaoMinutos == 480);
        Assert.Contains(metas, x => x.PrioridadeId == SeedData.PrioridadeCriticaId && x.TempoPrimeiraRespostaMinutos == 30 && x.TempoResolucaoMinutos == 240);
    }
}
