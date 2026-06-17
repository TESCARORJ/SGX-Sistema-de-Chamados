using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint6NotificacoesChecklistTests
{
    [Fact]
    public void Sprint6NotificacoesDevePermanecerPlanejadaAteChecklistSerConsolidado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem25Id);

        Assert.Equal("Sprint 6 - Notificacoes ITSM", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.Planejado, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.NaoAvaliado, item.StatusTecnico);
        Assert.Equal(25, item.PercentualImplementacao);
        Assert.Equal("Modelar entidade Notificacao e pipeline de eventos.", item.ProximaAcao);
    }
}
