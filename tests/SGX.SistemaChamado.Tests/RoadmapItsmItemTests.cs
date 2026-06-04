using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapItsmItemTests
{
    [Fact]
    public void DeveNormalizarPrazoAlvoParaUtcAoCriar()
    {
        var prazoSemKind = new DateTime(2026, 5, 9, 13, 45, 0, DateTimeKind.Unspecified);

        var item = new RoadmapItsmItem(
            "Seguranca",
            "Validacao",
            "Validar controles de acesso",
            null,
            "Em andamento",
            "Ajustar permissao por rota",
            StatusRoadmapItsm.EmValidacao,
            PrioridadeRoadmapItsm.Alta,
            ImpactoRoadmapItsm.Alto,
            DecisaoRoadmapItsm.DesenvolverAgora,
            null,
            null,
            prazoSemKind,
            1,
            StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente,
            StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas,
            90,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "teste");

        Assert.NotNull(item.PrazoAlvo);
        Assert.Equal(DateTimeKind.Utc, item.PrazoAlvo!.Value.Kind);
        Assert.Equal(new DateTime(2026, 5, 9, 0, 0, 0, DateTimeKind.Utc), item.PrazoAlvo.Value);
    }

    [Fact]
    public void DeveZerarPercentualQuandoChecklistNaoPossuirItensAtivos()
    {
        var roadmap = new RoadmapItsmItem(
            "Sprint 2 - Relacionamentos",
            "ITIL/ITSM",
            "Objetivo",
            null,
            "Situacao atual",
            "Atencao tecnica",
            StatusRoadmapItsm.Pendente,
            PrioridadeRoadmapItsm.Alta,
            ImpactoRoadmapItsm.Alto,
            DecisaoRoadmapItsm.DesenvolverAgora,
            null,
            null,
            null,
            102,
            StatusImplementacaoRoadmapItsm.Planejado,
            StatusTecnicoRoadmapItsm.NaoAvaliado,
            25,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "teste");

        var checklistInativo = new RoadmapChecklistItem(
            roadmap.Id,
            "Checklist inativo",
            null,
            GrupoRoadmapChecklist.Desenvolvimento,
            1,
            true,
            true,
            "teste");
        checklistInativo.Desativar("teste");

        roadmap.RecalcularPercentualImplementacao(new[] { checklistInativo });

        Assert.Equal(0, roadmap.PercentualImplementacao);
    }
}
