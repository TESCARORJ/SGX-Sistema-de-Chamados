using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint8CatalogoServicosChecklistTests
{
    [Fact]
    public async Task RoadmapSprint8DeveRefletirCatalogo20ComoBacklogEstruturalRastreavel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem19Id);
        Assert.Equal("Sprint 8 - Catalogo de Servicos 2.0", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.EmDesenvolvimento, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.Parcial, item.StatusTecnico);
        Assert.Equal(96, item.PercentualImplementacao);
        Assert.Equal(
            "Registrar homologacao funcional.",
            RemoverAcentos(item.ProximaAcao));

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem19Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(76, checklistAtivo.Length);
        Assert.All(checklistAtivo, x => Assert.True(x.Ativo));
        Assert.All(checklistAtivo, x => Assert.True(x.Obrigatorio));
        Assert.Equal(73, checklistAtivo.Count(x => x.Concluido));
        Assert.Equal(3, checklistAtivo.Count(x => !x.Concluido));
        Assert.Equal(Enumerable.Range(1, 76), checklistAtivo.Select(x => x.Ordem));

        Assert.Equal(
            new[]
            {
                "Diagnosticar estado atual do Catalogo 2.0 e pendencias transferidas da Sprint 7",
                "Confirmar escopo estrutural do Catalogo 2.0",
                "Definir criterios de aceite para motor de abertura guiada por servico",
                "Documentar decisao de transferencia dos itens 10, 13 e 14 da Sprint 7",
                "Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel",
                "Configurar EF Core para vinculo entre catalogo e grupo tecnico",
                "Criar migration estrutural para grupo tecnico no catalogo",
                "Ajustar contratos administrativos do catalogo para grupo tecnico responsavel",
                "Ajustar validators administrativos do catalogo para grupo tecnico responsavel",
                "Ajustar use cases administrativos do catalogo para grupo tecnico responsavel",
                "Expor grupo tecnico responsavel na consulta administrativa do catalogo",
                "Aplicar grupo tecnico responsavel na abertura guiada por catalogo",
                "Preservar fallback de grupo quando servico nao possuir grupo configurado",
                "Testar aplicacao de grupo tecnico configurado no catalogo",
                "Testar fallback de grupo sem configuracao no catalogo",
                "Modelar entidade de formulario por servico",
                "Modelar campos do formulario por servico",
                "Modelar tipos de campo permitidos",
                "Modelar obrigatoriedade, ordem, ajuda e visibilidade dos campos",
                "Modelar opcoes de campos enumerados, se aplicavel",
                "Modelar versionamento de formulario por servico",
                "Configurar EF Core para formulario e campos",
                "Criar migration estrutural para formulario dinamico",
                "Ajustar contratos administrativos para manutencao de formulario do servico",
                "Criar validators administrativos para formulario do servico",
                "Criar use cases administrativos para configurar formulario do servico",
                "Criar endpoints administrativos para formulario do servico",
                "Ajustar frontend administrativo do catalogo para configurar formulario",
                "Testar configuracao administrativa de formulario por servico",
                "Expor campos do formulario no endpoint de preparacao da abertura",
                "Ajustar contrato de abertura guiada para receber respostas do formulario",
                "Criar validator de respostas do formulario na abertura guiada",
                "Validar obrigatoriedade dos campos no backend",
                "Validar tipos e formatos das respostas no backend",
                "Impedir respostas de campos inexistentes ou de outro servico",
                "Preservar abertura guiada sem formulario configurado",
                "Ajustar frontend do portal para renderizar formulario dinamico",
                "Ajustar frontend do portal para enviar respostas do formulario",
                "Testar abertura guiada com formulario valido",
                "Testar abertura guiada com campos obrigatorios ausentes",
                "Testar abertura guiada com respostas invalidas",
                "Testar abertura guiada sem formulario configurado",
                "Modelar persistencia das respostas do formulario no chamado",
                "Configurar EF Core para respostas do formulario",
                "Criar migration estrutural para respostas do formulario",
                "Persistir respostas do formulario na abertura guiada",
                "Exibir respostas do formulario no detalhe do chamado",
                "Exibir respostas do formulario no portal do solicitante",
                "Exibir respostas do formulario na area administrativa de atendimento",
                "Registrar historico da abertura com formulario preenchido",
                "Registrar auditoria tecnica das respostas persistidas",
                "Testar persistencia das respostas do formulario",
                "Testar exibicao das respostas no portal",
                "Testar exibicao das respostas no atendimento administrativo",
                "Garantir aplicacao de tipo, categoria, subcategoria e prioridade do catalogo",
                "Garantir aplicacao de SLA padrao do catalogo",
                "Garantir aplicacao de aprovacao por servico",
                "Garantir compatibilidade com abertura legada sem catalogo",
                "Garantir compatibilidade com incidentes",
                "Garantir compatibilidade com aprovacao legada e motor novo",
                "Testar regressao de abertura guiada com SLA, grupo e aprovacao",
                "Testar regressao de abertura legada e incidente",
                "Garantir autorizacao para manutencao administrativa do formulario",
                "Garantir que solicitante nao manipule grupo, SLA, aprovacao ou classificacao",
                "Garantir que solicitante so envie respostas permitidas para o servico",
                "Testar seguranca do formulario e respostas",
                "Atualizar documentacao tecnica da Sprint 8",
                "Atualizar docs/ROADMAP.md e docs/ROADMAP-ITSM.md",
                "Atualizar SeedData e testes de checklist da Sprint 8",
                "Criar migration de checklist da Sprint 8",
                "Executar build backend e testes direcionados",
                "Executar build frontend e validacao TypeScript",
                "Verificar EF pending model changes",
                "Registrar homologacao funcional",
                "Registrar homologacao visual responsiva",
                "Registrar aceite formal somente com evidencia"
            },
            checklistAtivo.Select(x => RemoverAcentos(x.Titulo)).ToArray());

        Assert.Equal(
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73 },
            checklistAtivo.Where(x => x.Concluido).Select(x => x.Ordem).ToArray());

        Assert.Contains(checklistAtivo, x => x.Titulo == "Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel");
        Assert.Contains(checklistAtivo, x => x.Titulo == "Modelar entidade de formulario por servico");
        Assert.Contains(checklistAtivo, x => x.Titulo == "Persistir respostas do formulario na abertura guiada");
        Assert.Contains(checklistAtivo, x => x.Titulo == "Documentar decisao de transferencia dos itens 10, 13 e 14 da Sprint 7");

        var checklistSprint7 = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem18Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Contains(checklistSprint7, x => x.Ordem == 10 && x.Titulo == "Aplicar grupo responsavel configurado no catalogo");
        Assert.Contains(checklistSprint7, x => x.Ordem == 13 && x.Titulo == "Implementar ou reutilizar formulario por servico");
        Assert.Contains(checklistSprint7, x => x.Ordem == 14 && x.Titulo == "Validar e persistir respostas do formulario");

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem19Id);

        Assert.Equal(76, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(73, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal(96, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal(
            "Registrar homologacao funcional.",
            RemoverAcentos(detalhe.ProximaAcao));

        var percentualEsperado = (int)Math.Round((73 * 100.0) / 76, MidpointRounding.AwayFromZero);
        Assert.Equal(96, percentualEsperado);
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
