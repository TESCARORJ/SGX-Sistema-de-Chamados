using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapItil20SprintsConsistencyTests
{
    [Fact]
    public void DeveManterTrilhaAtivaCom20SprintsItil()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var sprintsAtivas = context.RoadmapItsmItens
            .Where(x => x.Ativo && x.RoadmapCategoriaId == SeedData.RoadmapCategoriaItilId)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(20, sprintsAtivas.Length);
        Assert.Equal("Sprint 1 - Fundacao ITSM do chamado", sprintsAtivas.First().Area);
        Assert.Equal("Sprint 20 - Produto, implantacao e operacao", sprintsAtivas.Last().Area);
        Assert.All(sprintsAtivas, item => Assert.StartsWith("Sprint ", item.Area));
    }
}
