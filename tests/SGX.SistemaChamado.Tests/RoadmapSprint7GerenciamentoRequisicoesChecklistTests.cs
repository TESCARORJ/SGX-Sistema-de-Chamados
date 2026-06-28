using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint7GerenciamentoRequisicoesChecklistTests
{
    [Fact]
    public async Task RoadmapSprint7DeveRefletirFluxoParcialSemMarcarEntregasCentraisGenericasComoConcluidas()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem18Id);
        Assert.Equal("Sprint 7 - Gerenciamento de Requisicoes", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.EmDesenvolvimento, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.Parcial, item.StatusTecnico);
        Assert.Equal(90, item.PercentualImplementacao);
        Assert.Equal(
            "Registrar aceite formal somente com evidencia",
            RemoverAcentos(item.ProximaAcao));

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem18Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(39, checklistAtivo.Length);
        Assert.All(checklistAtivo, x => Assert.True(x.Ativo));
        Assert.All(checklistAtivo, x => Assert.True(x.Obrigatorio));
        Assert.Equal(35, checklistAtivo.Count(x => x.Concluido));
        Assert.Equal(4, checklistAtivo.Count(x => !x.Concluido));
        Assert.Equal(Enumerable.Range(1, 39), checklistAtivo.Select(x => x.Ordem));

        Assert.Equal(
            new[]
            {
                "Diagnosticar estado atual da Sprint 7 e inconsistencias do roadmap",
                "Confirmar representacao da requisicao de servico como Chamado com NaturezaChamadoEnum.Requisicao",
                "Validar vinculo existente entre Chamado e Catalogo de Servicos",
                "Definir menor escopo seguro da abertura guiada por catalogo",
                "Implementar ou ajustar contrato de consulta do servico para abertura",
                "Implementar ou ajustar contrato de abertura guiada por catalogo com semantica de requisicao",
                "Criar validator dedicado para abertura guiada por catalogo",
                "Implementar use case dedicado de abertura de requisicao de servico via catalogo",
                "Aplicar classificacao vinda do catalogo no backend",
                "Aplicar grupo responsavel configurado no catalogo",
                "Aplicar SLA configurado ou fallback existente",
                "Persistir vinculo entre chamado e servico do catalogo",
                "Implementar ou reutilizar formulario por servico",
                "Validar e persistir respostas do formulario",
                "Gerar aprovacao obrigatoria quando a regra aplicavel exigir",
                "Preservar aprovacao legada sem duplicidade",
                "Preservar abertura de incidentes e chamados sem catalogo",
                "Criar ou ajustar endpoints do portal para catalogo e abertura guiada",
                "Implementar tela de catalogo no portal",
                "Implementar detalhe do servico no portal",
                "Implementar formulario guiado de abertura",
                "Implementar confirmacao e acompanhamento da requisicao aberta",
                "Garantir seguranca, autorizacao e ownership dos endpoints",
                "Registrar historico e auditoria dos eventos relevantes",
                "Testar abertura por catalogo sem aprovacao",
                "Testar abertura por catalogo com aprovacao obrigatoria",
                "Testar formulario obrigatorio e respostas invalidas",
                "Testar grupo responsavel e SLA",
                "Testar regressao de abertura legada, incidente e atendimento",
                "Testar regressao de aprovacao legada e motor novo",
                "Executar build backend e testes direcionados",
                "Executar build frontend e validacao TypeScript",
                "Verificar EF pending model changes",
                "Criar ou revisar migrations estruturais, se necessarias",
                "Criar migration de dados ou checklist, se aplicavel",
                "Atualizar documentacao principal da Sprint 7",
                "Registrar homologacao funcional",
                "Registrar homologacao visual responsiva",
                "Registrar aceite formal somente com evidencia"
            },
            checklistAtivo.Select(x => RemoverAcentos(x.Titulo)).ToArray());

        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Planejar escopo e criterios de aceite");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Implementar entregas centrais da sprint");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Executar testes funcionais e tecnicos");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Registrar homologacao e aceite");

        Assert.Equal(
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38 },
            checklistAtivo.Where(x => x.Concluido).Select(x => x.Ordem).ToArray());

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem18Id);

        Assert.Equal(39, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(35, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(90, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal(
            "Registrar aceite formal somente com evidencia",
            RemoverAcentos(detalhe.ProximaAcao));

        var percentualEsperado = (int)Math.Round((35 * 100.0) / 39, MidpointRounding.AwayFromZero);
        Assert.Equal(90, percentualEsperado);
        Assert.Equal(percentualEsperado, detalhe.PercentualImplementacao);
    }

    private static string RemoverAcentos(string valor)
        => valor
            .Replace("Ã¡", "a", StringComparison.Ordinal)
            .Replace("Ã ", "a", StringComparison.Ordinal)
            .Replace("Ã£", "a", StringComparison.Ordinal)
            .Replace("Ã¢", "a", StringComparison.Ordinal)
            .Replace("Ã©", "e", StringComparison.Ordinal)
            .Replace("Ãª", "e", StringComparison.Ordinal)
            .Replace("Ã­", "i", StringComparison.Ordinal)
            .Replace("Ã³", "o", StringComparison.Ordinal)
            .Replace("Ã´", "o", StringComparison.Ordinal)
            .Replace("Ãµ", "o", StringComparison.Ordinal)
            .Replace("Ãº", "u", StringComparison.Ordinal)
            .Replace("Ã§", "c", StringComparison.Ordinal)
            .Replace("Ã", "A", StringComparison.Ordinal)
            .Replace("Ã€", "A", StringComparison.Ordinal)
            .Replace("Ãƒ", "A", StringComparison.Ordinal)
            .Replace("Ã‚", "A", StringComparison.Ordinal)
            .Replace("Ã‰", "E", StringComparison.Ordinal)
            .Replace("ÃŠ", "E", StringComparison.Ordinal)
            .Replace("Ã", "I", StringComparison.Ordinal)
            .Replace("Ã“", "O", StringComparison.Ordinal)
            .Replace("Ã”", "O", StringComparison.Ordinal)
            .Replace("Ã•", "O", StringComparison.Ordinal)
            .Replace("Ãš", "U", StringComparison.Ordinal)
            .Replace("Ã‡", "C", StringComparison.Ordinal);

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
