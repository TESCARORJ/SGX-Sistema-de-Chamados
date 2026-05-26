using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapChecklistUseCasesTests
{
    [Fact]
    public async Task ListarChecklistDeveOrdenarPorOrdemGrupoETitulo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var roadmap = CriarRoadmapBase("Checklist Ordenacao");
        context.RoadmapItsmItens.Add(roadmap);

        var itemA = new RoadmapChecklistItem(roadmap.Id, "Zulu", null, GrupoRoadmapChecklist.Desenvolvimento, 2, false, true, "teste");
        var itemB = new RoadmapChecklistItem(roadmap.Id, "Alpha", null, GrupoRoadmapChecklist.Planejamento, 1, false, true, "teste");
        var itemC = new RoadmapChecklistItem(roadmap.Id, "Beta", null, GrupoRoadmapChecklist.Desenvolvimento, 1, false, true, "teste");
        var itemD = new RoadmapChecklistItem(roadmap.Id, "Omega", null, GrupoRoadmapChecklist.Desenvolvimento, 1, false, true, "teste");

        context.RoadmapChecklistItens.AddRange(itemA, itemB, itemC, itemD);
        await context.SaveChangesAsync();

        var useCase = new ListarRoadmapChecklistPorItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapChecklistItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var resultado = await useCase.ExecutarAsync(roadmap.Id);

        Assert.Equal(new[] { itemB.Id, itemC.Id, itemD.Id, itemA.Id }, resultado.Select(x => x.Id).ToArray());
    }

    [Fact]
    public async Task InativarEReativarChecklistDevemRecalcularPercentualERegistrarAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();

        var roadmap = CriarRoadmapBase("Checklist Percentual");
        context.RoadmapItsmItens.Add(roadmap);

        var concluido = new RoadmapChecklistItem(roadmap.Id, "Concluido", null, GrupoRoadmapChecklist.Testes, 1, true, true, "teste");
        var pendente = new RoadmapChecklistItem(roadmap.Id, "Pendente", null, GrupoRoadmapChecklist.Testes, 2, false, true, "teste");
        context.RoadmapChecklistItens.AddRange(concluido, pendente);
        roadmap.RecalcularPercentualImplementacao(new[] { concluido, pendente });
        await context.SaveChangesAsync();

        var inativar = new InativarRoadmapChecklistItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapChecklistItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await inativar.ExecutarAsync(pendente.Id);

        var roadmapAposInativacao = await context.RoadmapItsmItens.FindAsync(roadmap.Id);
        Assert.NotNull(roadmapAposInativacao);
        Assert.Equal(100, roadmapAposInativacao!.PercentualImplementacao);

        var eventoInativacao = Assert.Single(auditoria.Eventos);
        Assert.Equal(TipoAcaoAuditoria.Inativacao, eventoInativacao.Acao);
        Assert.Contains("\"roadmapItemId\"", eventoInativacao.Metadados ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(roadmap.Id.ToString(), eventoInativacao.Metadados ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"Ativo\":true", eventoInativacao.DadosAntes ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"Ativo\":false", eventoInativacao.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        var reativar = new ReativarRoadmapChecklistItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapChecklistItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await reativar.ExecutarAsync(pendente.Id);

        var roadmapAposReativacao = await context.RoadmapItsmItens.FindAsync(roadmap.Id);
        Assert.NotNull(roadmapAposReativacao);
        Assert.Equal(50, roadmapAposReativacao!.PercentualImplementacao);

        var eventoReativacao = auditoria.Eventos.Last();
        Assert.Equal(TipoAcaoAuditoria.Ativacao, eventoReativacao.Acao);
        Assert.Contains("\"Ativo\":false", eventoReativacao.DadosAntes ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("\"Ativo\":true", eventoReativacao.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));

    private static RoadmapItsmItem CriarRoadmapBase(string area)
        => new(
            area,
            "Categoria",
            "Objetivo",
            null,
            "Situacao",
            "Atencao",
            StatusRoadmapItsm.Pendente,
            PrioridadeRoadmapItsm.Media,
            ImpactoRoadmapItsm.Medio,
            DecisaoRoadmapItsm.DesenvolverAgora,
            null,
            null,
            null,
            1,
            StatusImplementacaoRoadmapItsm.EmDesenvolvimento,
            StatusTecnicoRoadmapItsm.Parcial,
            0,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "teste");
}
