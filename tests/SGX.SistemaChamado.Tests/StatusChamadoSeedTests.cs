using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class StatusChamadoSeedTests
{
    [Fact]
    public void SeedDeStatusNaoDeveDuplicarCodigos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.StatusChamado.Select(x => x.Codigo).ToList();

        Assert.Equal(codigos.Count, codigos.Distinct().Count());
    }

    [Fact]
    public void SeedDeStatusDeveConterStatusItilEspecificos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var codigos = context.StatusChamado.Select(x => x.Codigo).ToHashSet();

        Assert.Contains(StatusChamadoEnum.EmAnalise, codigos);
        Assert.Contains(StatusChamadoEnum.AguardandoAprovacao, codigos);
        Assert.Contains(StatusChamadoEnum.Aprovada, codigos);
        Assert.Contains(StatusChamadoEnum.Reprovada, codigos);
        Assert.Contains(StatusChamadoEnum.EmExecucao, codigos);
        Assert.Contains(StatusChamadoEnum.Concluida, codigos);
        Assert.Contains(StatusChamadoEnum.CausaRaizIdentificada, codigos);
        Assert.Contains(StatusChamadoEnum.SolucaoDeContorno, codigos);
        Assert.Contains(StatusChamadoEnum.Correlacionado, codigos);
        Assert.Contains(StatusChamadoEnum.Tratado, codigos);
        Assert.Contains(StatusChamadoEnum.Planejada, codigos);
    }
}
