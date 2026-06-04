using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint2RelacionamentosChecklistConsistencyTests
{
    [Fact]
    public void SeedDaSprint2RelacionamentosDeveTer33ItensAtivosSemDuplicidade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var checklist = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem36Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        var duplicidadeOrdem = checklist
            .GroupBy(x => x.Ordem)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        Assert.Equal(33, checklist.Length);
        Assert.Empty(duplicidadeOrdem);
        Assert.All(checklist, item => Assert.False(item.Concluido));
    }

    [Fact]
    public async Task DetalheDaSprint2RelacionamentosDeveExibirPercentualZeroEContador0De33()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem36Id);

        Assert.Equal("Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM", detalhe.Area);
        Assert.Equal(102, detalhe.Ordem);
        Assert.Equal(33, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(0, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(0, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    [Fact]
    public async Task ListagemDoChecklistDaSprint2RelacionamentosDeveRetornar33Itens()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var useCase = new ListarRoadmapChecklistPorItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapChecklistItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var checklist = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem36Id);

        Assert.Equal(33, checklist.Count);
    }

    [Fact]
    public async Task ComUmItemConcluidoNaSprint2RelacionamentosPercentualDeveSerTres()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem36Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .First();

        item.Concluir("teste");
        await context.SaveChangesAsync();

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem36Id);

        Assert.Equal(33, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(1, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(3, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador Teste",
            "admin@sgx.local",
            "admin.teste",
            ["Administrador"]));
}
