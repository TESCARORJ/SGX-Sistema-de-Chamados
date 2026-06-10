using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint4MotorAprovacoesChecklistTests
{
    [Fact]
    public async Task Sprint4MotorAprovacoesDeveExporChecklistDetalhadoComPercentualRecalculado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem22Id);
        Assert.Equal("Sprint 4 - Motor de Aprovacoes ITSM", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, item.StatusTecnico);
        Assert.Equal(94, item.PercentualImplementacao);
        Assert.Equal("Preparar roteiro de homologacao de casos sensiveis.", item.ProximaAcao);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem22Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(68, checklistAtivo.Length);
        
        var totalConcluidos = checklistAtivo.Count(x => x.Concluido);
        Assert.Equal(64, totalConcluidos);
        
        Assert.DoesNotContain(checklistAtivo, x => x.Grupo == GrupoRoadmapChecklist.Homologacao && x.Concluido);
        Assert.Equal(Enumerable.Range(1, 68), checklistAtivo.Select(x => x.Ordem));

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var expectedPercent = 94; // 64 / 68 = 94.11% => 94%
        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem22Id);

        Assert.Equal(68, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(64, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(expectedPercent, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
