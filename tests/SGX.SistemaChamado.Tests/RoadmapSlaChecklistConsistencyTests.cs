using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSlaChecklistConsistencyTests
{
    [Fact]
    public void SeedDoItemSlaDeveTerChecklistCom69ItensSemDuplicidade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var checklistSla = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem05Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        var duplicidadeOrdem = checklistSla
            .GroupBy(x => x.Ordem)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        var duplicidadeOrdemDescricao = checklistSla
            .GroupBy(x => new { x.Ordem, Descricao = (x.Descricao ?? string.Empty).Trim().ToUpperInvariant() })
            .Where(x => x.Count() > 1)
            .Select(x => x.Key.Ordem)
            .ToArray();

        Assert.Equal(69, checklistSla.Length);
        Assert.All(checklistSla, item => Assert.True(item.Concluido));
        Assert.Empty(duplicidadeOrdem);
        Assert.Empty(duplicidadeOrdemDescricao);
    }

    [Fact]
    public async Task ItemSlaDeveExporPercentualCemEStatusConsolidadosNoUseCase()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Administrador Teste",
                "admin@sgx.local",
                "admin.teste",
                ["Administrador"])));

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem05Id);

        Assert.Equal(69, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(69, detalhe.QuantidadeChecklistConcluido);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal(100, detalhe.PercentualImplementacao);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, detalhe.StatusTecnico);
    }
}
