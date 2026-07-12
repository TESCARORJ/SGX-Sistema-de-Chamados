using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint9GerenciamentoIncidentesChecklistTests
{
    [Fact]
    public async Task Sprint9GerenciamentoIncidentesDeveExporChecklistTecnicoComPercentualRecalculado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem17Id);
        Assert.Equal("Sprint 9 - Gerenciamento de Incidentes", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.EmDesenvolvimento, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.Parcial, item.StatusTecnico);
        Assert.Equal(36, item.PercentualImplementacao);
        Assert.Equal(
            "Implementar os itens pendentes de modelagem, backend, API, frontend, testes, seguranca, governanca e homologacao do fluxo de incidente.",
            item.ProximaAcao);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem17Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(50, checklistAtivo.Length);
        Assert.All(checklistAtivo, x => Assert.True(x.Ativo));
        Assert.All(checklistAtivo, x => Assert.True(x.Obrigatorio));
        Assert.Equal(18, checklistAtivo.Count(x => x.Concluido));
        Assert.Equal(32, checklistAtivo.Count(x => !x.Concluido));
        Assert.Equal(Enumerable.Range(1, 50), checklistAtivo.Select(x => x.Ordem));

        Assert.Equal(
            new[]
            {
                "Diagnosticar estado atual dos chamados operacionais",
                "Confirmar escopo funcional da Sprint 9",
                "Definir criterios de aceite para Gerenciamento de Incidentes",
                "Documentar diferenca entre incidente, requisicao e chamado legado",
                "Registrar limites atuais do fluxo de incidente",
                "Registrar dependencias e riscos da Sprint 9, incluindo CMDB, SLA e autorizacao",
                "Confirmar existencia da natureza Incidente no modelo ITSM",
                "Validar matriz de status permitidos para Incidente",
                "Exigir impacto e urgencia na criacao de incidentes",
                "Registrar classificacao de incidente por e-mail",
                "Registrar compatibilidade de Incidente nos filtros do dashboard administrativo",
                "Registrar compatibilidade de Incidente nos relatorios administrativos",
                "Registrar compatibilidade de Incidente nas acoes disponiveis do chamado",
                "Registrar compatibilidade de Incidente na abertura legada do chamado",
                "Sincronizar SeedData, teste, migration e documentacao da Sprint 9",
                "Registrar regra de fechamento",
                "Registrar compatibilidade com status atual do chamado",
                "Registrar limitacao de SLA se ainda reutilizar SLA existente",
                "Registrar prioridade por impacto e urgencia",
                "Registrar pendencia para DTOs de abertura de incidente",
                "Registrar pendencia para validators de incidente",
                "Registrar pendencia para use case de abertura",
                "Registrar pendencia para use case de triagem",
                "Registrar pendencia para use case de atendimento",
                "Registrar pendencia para use case de diagnostico",
                "Registrar pendencia para use case de workaround",
                "Registrar pendencia para use case de resolucao",
                "Registrar pendencia para use case de reabertura",
                "Registrar pendencia para use case de fechamento",
                "Registrar pendencia para historico de diagnostico, workaround e resolucao",
                "Registrar pendencia para auditoria minima",
                "Registrar pendencia para endpoints de abertura/consulta de incidente",
                "Registrar pendencia para endpoints de atendimento",
                "Registrar pendencia para endpoints de resolucao",
                "Registrar pendencia para endpoints de reabertura",
                "Registrar pendencia para endpoints de fechamento",
                "Registrar pendencia para contratos sem expor detalhes internos do dominio",
                "Registrar pendencia para abertura de incidente",
                "Registrar pendencia para tela de atendimento",
                "Registrar pendencia para diagnostico e workaround",
                "Registrar pendencia para resolucao",
                "Registrar pendencia para reabertura",
                "Registrar autorizacao por acao operacional de incidente",
                "Registrar protecao de payload e integridade de metadados",
                "Registrar testes de abertura e triagem de incidente",
                "Registrar testes de atendimento e diagnostico de incidente",
                "Registrar testes de workaround e resolucao de incidente",
                "Registrar testes de reabertura e fechamento de incidente",
                "Registrar documentacao tecnica e rastreabilidade da Sprint 9",
                "Registrar homologacao funcional, visual e aceite formal"
            },
            checklistAtivo.Select(x => x.Titulo).ToArray());

        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Planejar escopo e criterios de aceite");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Implementar entregas centrais da sprint");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Executar testes funcionais e tecnicos");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Registrar homologacao e aceite");

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem17Id);

        Assert.Equal(50, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(18, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(36, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal(
            "Implementar os itens pendentes de modelagem, backend, API, frontend, testes, seguranca, governanca e homologacao do fluxo de incidente.",
            detalhe.ProximaAcao);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
