using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapBaseConhecimentoConsistencyTests
{
    [Fact]
    public void DeveManterItemUnicoBaseConhecimentoComStatusSincronizado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itens = context.RoadmapItsmItens
            .Where(x => x.Area == "Base de conhecimento" && x.Categoria == "Conhecimento")
            .ToArray();

        Assert.Single(itens);

        var item = itens[0];
        Assert.Equal(SeedData.RoadmapItsmItem10Id, item.Id);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, item.StatusTecnico);
        Assert.Equal(90, item.PercentualImplementacao);
    }

    [Fact]
    public async Task DetalheDoRoadmapDeveExibirAvaliacaoDeHomologacaoFuncionalPreparada()
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

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem10Id);

        Assert.Equal("Base de conhecimento", detalhe.Area);
        Assert.Equal("Conhecimento", detalhe.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, detalhe.StatusTecnico);
        Assert.Equal("Homologação funcional preparada", detalhe.StatusTecnicoDescricao);
        Assert.Equal(0, detalhe.PercentualImplementacao);
        Assert.Equal(0, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(0, detalhe.QuantidadeChecklistConcluido);
        Assert.False(detalhe.PercentualCalculadoPorChecklist);
    }
}
