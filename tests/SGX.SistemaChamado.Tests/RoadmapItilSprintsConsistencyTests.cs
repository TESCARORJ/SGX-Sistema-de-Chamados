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
        Assert.Equal("Sprint 6 - Notificacoes ITSM", sprintsAtivas.First().Area);
        Assert.Equal("Sprint 1 - Fundacao ITSM do chamado", sprintsAtivas[1].Area);
        Assert.Equal("Sprint 19 - Pesquisa de satisfacao", sprintsAtivas.Last().Area);
        Assert.All(sprintsAtivas, item => Assert.StartsWith("Sprint ", item.Area));
    }
}
