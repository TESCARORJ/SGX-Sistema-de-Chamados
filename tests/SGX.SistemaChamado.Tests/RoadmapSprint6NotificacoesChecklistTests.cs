using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint6NotificacoesChecklistTests
{
    [Fact]
    public async Task RoadmapSprint6DeveRefletirIntegracaoEventosConcluidaSemAnteciparAceiteFinal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem25Id);
        Assert.Equal("Sprint 6 - Notificacoes ITSM", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, item.StatusTecnico);
        Assert.Equal(94, item.PercentualImplementacao);
        Assert.Equal("Executar homologacao visual/manual (320px/375px/768px/desktop) e registrar aceite formal da Sprint 6.", RemoverAcentos(item.ProximaAcao));

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem25Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(16, checklistAtivo.Length);
        Assert.All(checklistAtivo, x => Assert.True(x.Ativo));
        Assert.All(checklistAtivo, x => Assert.True(x.Obrigatorio));
        Assert.Equal(15, checklistAtivo.Count(x => x.Concluido));
        Assert.Equal(1, checklistAtivo.Count(x => !x.Concluido));
        Assert.All(checklistAtivo.Where(x => x.Ordem <= 15), x => Assert.True(x.Concluido));
        Assert.All(checklistAtivo.Where(x => x.Ordem >= 16), x => Assert.False(x.Concluido));
        Assert.Equal(Enumerable.Range(1, 16), checklistAtivo.Select(x => x.Ordem));

        Assert.Equal(
            new[]
            {
                "Planejar escopo e criterios de aceite",
                "Diagnosticar estruturas existentes de notificacoes e eventos",
                "Modelar entidade Notificacao e contrato de eventos",
                "Criar configuracao EF e migration estrutural de notificacoes",
                "Testar dominio e estrutura persistente de notificacoes",
                "Criar servico de geracao idempotente de notificacoes",
                "Implementar resolucao de destinatarios por participacao e perfil",
                "Modelar templates e materializacao de conteudo",
                "Implementar preferencias de notificacao por usuario e evento",
                "Implementar processamento e controle de tentativas de entrega",
                "Implementar entrega pelo canal Sistema",
                "Implementar entrega pelo canal E-mail",
                "Criar API de consulta, leitura e marcacao como nao lida",
                "Implementar central de notificacoes no frontend",
                "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao",
                "Documentar, homologar e registrar aceite da Sprint 6"
            },
            checklistAtivo.Select(x => RemoverAcentos(x.Titulo)).ToArray());

        Assert.Equal("Implementar preferencias de notificacao por usuario e evento", RemoverAcentos(checklistAtivo.Single(x => x.Ordem == 9).Titulo));
        Assert.Equal("Implementar processamento e controle de tentativas de entrega", checklistAtivo.Single(x => x.Ordem == 10).Titulo);
        Assert.Equal("Implementar entrega pelo canal Sistema", checklistAtivo.Single(x => x.Ordem == 11).Titulo);
        Assert.Equal("Implementar entrega pelo canal E-mail", checklistAtivo.Single(x => x.Ordem == 12).Titulo);
        Assert.Equal("Criar API de consulta, leitura e marcacao como nao lida", RemoverAcentos(checklistAtivo.Single(x => x.Ordem == 13).Titulo));
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Implementar entregas centrais da sprint");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Executar testes funcionais e tecnicos");
        Assert.DoesNotContain(checklistAtivo, x => x.Titulo == "Registrar homologacao e aceite");

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem25Id);

        Assert.Equal(16, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(15, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(94, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal("Executar homologacao visual/manual (320px/375px/768px/desktop) e registrar aceite formal da Sprint 6.", RemoverAcentos(detalhe.ProximaAcao));

        var percentualEsperado = (int)Math.Round((15 * 100.0) / 16, MidpointRounding.AwayFromZero);
        Assert.Equal(94, percentualEsperado);
        Assert.Equal(percentualEsperado, detalhe.PercentualImplementacao);

        var sprint5 = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem24Id);
        Assert.Equal(100, sprint5.PercentualImplementacao);
    }

    private static string RemoverAcentos(string valor)
        => valor
            .Replace("á", "a", StringComparison.Ordinal)
            .Replace("à", "a", StringComparison.Ordinal)
            .Replace("ã", "a", StringComparison.Ordinal)
            .Replace("â", "a", StringComparison.Ordinal)
            .Replace("é", "e", StringComparison.Ordinal)
            .Replace("ê", "e", StringComparison.Ordinal)
            .Replace("í", "i", StringComparison.Ordinal)
            .Replace("ó", "o", StringComparison.Ordinal)
            .Replace("ô", "o", StringComparison.Ordinal)
            .Replace("õ", "o", StringComparison.Ordinal)
            .Replace("ú", "u", StringComparison.Ordinal)
            .Replace("ç", "c", StringComparison.Ordinal)
            .Replace("Á", "A", StringComparison.Ordinal)
            .Replace("À", "A", StringComparison.Ordinal)
            .Replace("Ã", "A", StringComparison.Ordinal)
            .Replace("Â", "A", StringComparison.Ordinal)
            .Replace("É", "E", StringComparison.Ordinal)
            .Replace("Ê", "E", StringComparison.Ordinal)
            .Replace("Í", "I", StringComparison.Ordinal)
            .Replace("Ó", "O", StringComparison.Ordinal)
            .Replace("Ô", "O", StringComparison.Ordinal)
            .Replace("Õ", "O", StringComparison.Ordinal)
            .Replace("Ú", "U", StringComparison.Ordinal)
            .Replace("Ç", "C", StringComparison.Ordinal);

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
