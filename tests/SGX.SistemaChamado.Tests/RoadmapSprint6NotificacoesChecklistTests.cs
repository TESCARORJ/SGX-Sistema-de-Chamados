using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint6NotificacoesChecklistTests
{
    [Fact]
    public async Task RoadmapSprint6DevePermanecerAbertoComChecklistInicial()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem25Id);
        Assert.Equal("Sprint 6 - Notificacoes ITSM", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.Planejado, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.NaoAvaliado, item.StatusTecnico);
        Assert.Equal(25, item.PercentualImplementacao);
        Assert.Equal("Modelar entidade Notificacao e pipeline de eventos.", item.ProximaAcao);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem25Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(4, checklistAtivo.Length);
        Assert.Equal(1, checklistAtivo.Count(x => x.Concluido));
        Assert.True(checklistAtivo.Single(x => x.Ordem == 1).Concluido);
        Assert.False(checklistAtivo.Single(x => x.Ordem == 2).Concluido);
        Assert.False(checklistAtivo.Single(x => x.Ordem == 3).Concluido);
        Assert.False(checklistAtivo.Single(x => x.Ordem == 4).Concluido);
        Assert.Equal(Enumerable.Range(1, 4), checklistAtivo.Select(x => x.Ordem));

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem25Id);

        Assert.Equal(4, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(1, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(25, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal("Modelar entidade Notificacao e pipeline de eventos.", detalhe.ProximaAcao);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
