using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapItilSprintsConsistencyTests
{
    [Fact]
    public void DeveManterTrilhaAtivaComSprintsItil()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var sprintsAtivas = context.RoadmapItsmItens
            .Where(x => x.Ativo && x.RoadmapCategoriaId == SeedData.RoadmapCategoriaItilId)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(21, sprintsAtivas.Length);
        Assert.Equal("Sprint 1 - Fundacao ITSM do chamado", sprintsAtivas.First().Area);
        Assert.Equal("Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM", sprintsAtivas[1].Area);
        Assert.Equal("Sprint 21 - Produto, implantacao e operacao", sprintsAtivas.Last().Area);
        Assert.All(sprintsAtivas, item => Assert.StartsWith("Sprint ", item.Area));
    }
}
