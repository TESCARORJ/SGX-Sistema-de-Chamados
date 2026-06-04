using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapPercentualChecklistRulesTests
{
    [Fact]
    public async Task Sprint2RelacionamentosComChecklistAtivoDeveIniciarComPercentualZeroEContador0De33()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itemLegado = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem36Id);
        Assert.Equal(25, itemLegado.PercentualImplementacao);

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem36Id);

        Assert.Equal("Sprint 2 - Relacionamentos, dependencias e orquestracao ITSM", detalhe.Area);
        Assert.Equal("ITIL/ITSM", detalhe.Categoria);
        Assert.Equal(102, detalhe.Ordem);
        Assert.Equal(33, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(0, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(0, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    [Theory]
    [InlineData(5, 0, 0)]
    [InlineData(8, 4, 50)]
    [InlineData(10, 10, 100)]
    public async Task DeveCalcularPercentualComBaseNosItensAtivosDoChecklist(int totalAtivo, int totalConcluido, int percentualEsperado)
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var roadmap = CriarRoadmapBase(area: $"Roadmap {totalAtivo}-{totalConcluido}", percentualLegado: 25);
        context.RoadmapItsmItens.Add(roadmap);

        for (var ordem = 1; ordem <= totalAtivo; ordem++)
        {
            var concluido = ordem <= totalConcluido;
            context.RoadmapChecklistItens.Add(CriarChecklist(roadmap.Id, ordem, concluido, ativo: true));
        }

        await context.SaveChangesAsync();

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(roadmap.Id);

        Assert.Equal(totalAtivo, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(totalConcluido, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(percentualEsperado, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    [Fact]
    public async Task DeveDesconsiderarItensInativosNoCalculoDoChecklist()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var roadmap = CriarRoadmapBase(area: "Roadmap com itens inativos", percentualLegado: 25);
        context.RoadmapItsmItens.Add(roadmap);

        for (var ordem = 1; ordem <= 8; ordem++)
        {
            var concluido = ordem <= 4;
            context.RoadmapChecklistItens.Add(CriarChecklist(roadmap.Id, ordem, concluido, ativo: true));
        }

        context.RoadmapChecklistItens.Add(CriarChecklist(roadmap.Id, 9, concluido: true, ativo: false));
        context.RoadmapChecklistItens.Add(CriarChecklist(roadmap.Id, 10, concluido: true, ativo: false));

        await context.SaveChangesAsync();

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(roadmap.Id);

        Assert.Equal(8, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(4, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(50, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));

    private static RoadmapItsmItem CriarRoadmapBase(string area, int percentualLegado)
        => new(
            area,
            "ITIL/ITSM",
            "Objetivo",
            SeedData.RoadmapCategoriaItilId,
            "Situacao atual",
            "Atencao tecnica",
            StatusRoadmapItsm.Pendente,
            PrioridadeRoadmapItsm.Alta,
            ImpactoRoadmapItsm.Alto,
            DecisaoRoadmapItsm.DesenvolverAgora,
            null,
            "Time Produto",
            null,
            999,
            StatusImplementacaoRoadmapItsm.Planejado,
            StatusTecnicoRoadmapItsm.NaoAvaliado,
            percentualLegado,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "teste");

    private static RoadmapChecklistItem CriarChecklist(Guid roadmapItemId, int ordem, bool concluido, bool ativo)
    {
        var item = new RoadmapChecklistItem(
            roadmapItemId,
            $"Checklist {ordem}",
            null,
            GrupoRoadmapChecklist.Desenvolvimento,
            ordem,
            concluido,
            true,
            "teste");

        if (!ativo)
        {
            item.Desativar("teste");
        }

        return item;
    }
}
