using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint5RegrasFechamentoChecklistTests
{
    [Fact]
    public async Task Sprint5RegrasFechamentoDeveExporChecklistComPercentualRecalculado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem24Id);
        Assert.Equal("Sprint 5 - Regras de fechamento, aceite e reabertura", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, item.StatusTecnico);
        Assert.Equal(100, item.PercentualImplementacao);
        Assert.Equal("Executar homologacao formal da Sprint 5 e iniciar a analise da Sprint 6 - Notificacoes ITSM, sem antecipar implementacao funcional.", item.ProximaAcao);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem24Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(32, checklistAtivo.Length);

        var totalConcluidos = checklistAtivo.Count(x => x.Concluido);
        Assert.Equal(32, totalConcluidos);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 12).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 13).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 14).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 15).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 16).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 17).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 18).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 19).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 20).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 21).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 22).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 23).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 24).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 25).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 26).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 27).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 28).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 29).Concluido);
        Assert.True(checklistAtivo.Single(x => x.Ordem == 30).Concluido);

        Assert.Equal(Enumerable.Range(1, 32), checklistAtivo.Select(x => x.Ordem));

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var expectedPercent = 100; // 32 / 32 = 100%
        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem24Id);

        Assert.Equal(32, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(32, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(expectedPercent, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal("Executar homologacao formal da Sprint 5 e iniciar a analise da Sprint 6 - Notificacoes ITSM, sem antecipar implementacao funcional.", detalhe.ProximaAcao);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
