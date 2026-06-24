using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class MaterializarConteudoNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveSelecionarTemplateAtivoPorEventoCanalEVersaoMaisRecente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(nome: "Template v1", versao: 1));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(nome: "Template v2", versao: 2, conteudoTemplate: "Versao {{chamado.codigo}} v2"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(CriarRequest());

        Assert.Equal("Template v2", response.TemplateNome);
        Assert.Equal(2, response.TemplateVersao);
        Assert.Equal("Versao CH-001 v2", response.Conteudo);
    }

    [Fact]
    public async Task DeveRespeitarVigenciaESelecionarTemplateAplicavel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            nome: "Futuro",
            versao: 3,
            vigenteDe: new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc)));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            nome: "Atual",
            versao: 2,
            vigenteDe: new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc),
            vigenteAte: new DateTime(2026, 6, 21, 23, 59, 59, DateTimeKind.Utc)));
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(CriarRequest());

        Assert.Equal("Atual", response.TemplateNome);
        Assert.Equal(2, response.TemplateVersao);
    }

    [Fact]
    public async Task DeveMaterializarAssuntoEConteudoComMultiplasVariaveis()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            assuntoTemplate: "Chamado {{chamado.codigo}} - {{chamado.titulo}}",
            conteudoTemplate: "O chamado {{chamado.codigo}} / {{chamado.titulo}} foi atualizado.",
            variaveisPermitidas: ["chamado.codigo", "chamado.titulo"]));
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(CriarRequest(new Dictionary<string, string>
        {
            ["chamado.codigo"] = "CH-001",
            ["chamado.titulo"] = "Impressora"
        }));

        Assert.Equal("Chamado CH-001 - Impressora", response.Assunto);
        Assert.Equal("O chamado CH-001 / Impressora foi atualizado.", response.Conteudo);
        Assert.Equal(["chamado.codigo", "chamado.titulo"], response.VariaveisUtilizadas);
    }

    [Fact]
    public async Task DevePreservarTextoSemPlaceholdersEAceitarVariavelExtraPermitida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            assuntoTemplate: "Aviso fixo",
            conteudoTemplate: "Conteudo fixo",
            variaveisPermitidas: ["chamado.codigo", "extra.permitida"]));
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(CriarRequest(new Dictionary<string, string>
        {
            ["chamado.codigo"] = "CH-001",
            ["extra.permitida"] = "nao usada"
        }));

        Assert.Equal("Aviso fixo", response.Assunto);
        Assert.Equal("Conteudo fixo", response.Conteudo);
        Assert.Empty(response.VariaveisUtilizadas);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DevePermitirSelecaoExplicitaPorTemplateId()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var template = CriarTemplate(nome: "Explicito", versao: 9);
        context.Set<TemplateNotificacao>().Add(template);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(CriarRequest(
            templateNotificacaoId: template.Id));

        Assert.Equal(template.Id, response.TemplateNotificacaoId);
        Assert.Equal("Explicito", response.TemplateNome);
    }

    [Fact]
    public async Task DeveRejeitarTemplateInexistenteOuInativoOuForaDaVigencia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var inativo = CriarTemplate(nome: "Inativo");
        inativo.DesativarTemplate(Guid.NewGuid(), "teste");
        var foraVigencia = CriarTemplate(
            nome: "ForaVigencia",
            vigenteAte: new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc));

        context.Set<TemplateNotificacao>().Add(inativo);
        context.Set<TemplateNotificacao>().Add(foraVigencia);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(templateNotificacaoId: Guid.NewGuid())));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(templateNotificacaoId: inativo.Id)));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(templateNotificacaoId: foraVigencia.Id)));
    }

    [Fact]
    public async Task DeveRejeitarVariavelAusenteNaoPermitidaOuPlaceholderDesconhecido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate());
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            nome: "Desconhecido",
            versao: 2,
            conteudoTemplate: "Valor {{nao.declarada}}",
            variaveisPermitidas: ["chamado.codigo"]));
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(new Dictionary<string, string>())));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(new Dictionary<string, string>
            {
                ["nao.permitida"] = "x"
            })));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(
                new Dictionary<string, string> { ["chamado.codigo"] = "CH-001" },
                templateNotificacaoId: context.Set<TemplateNotificacao>().Single(x => x.Nome == "Desconhecido").Id)));
    }

    [Fact]
    public async Task DeveRejeitarPlaceholderMalformadoOuExpressao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(nome: "Malformado", conteudoTemplate: "Valor {{ chamado.codigo "));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(nome: "Expressao", versao: 2, conteudoTemplate: "Valor {{chamado.codigo + 1}}"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var malformadoId = context.Set<TemplateNotificacao>().Single(x => x.Nome == "Malformado").Id;
        var expressaoId = context.Set<TemplateNotificacao>().Single(x => x.Nome == "Expressao").Id;

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(CriarRequest(templateNotificacaoId: malformadoId)));
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(CriarRequest(templateNotificacaoId: expressaoId)));
    }

    [Fact]
    public async Task DeveEscaparHtmlNoConteudoDoCanalEmailESemAlterarTemplateOuCriarNotificacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var template = CriarTemplate(
            canal: CanalNotificacao.Email,
            assuntoTemplate: null,
            conteudoTemplate: "<strong>{{solicitante.nome}}</strong>",
            variaveisPermitidas: ["solicitante.nome"]);
        context.Set<TemplateNotificacao>().Add(template);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(CriarRequest(
            variaveis: new Dictionary<string, string> { ["solicitante.nome"] = "<script>alert(1)</script>" },
            canal: CanalNotificacao.Email,
            tipoEvento: TipoEventoNotificacao.EventoChamado,
            templateNotificacaoId: template.Id));

        Assert.Equal("<strong>&lt;script&gt;alert(1)&lt;/script&gt;</strong>", response.Conteudo);
        Assert.Null((await context.Set<TemplateNotificacao>().SingleAsync(x => x.Id == template.Id)).AssuntoTemplate);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveRejeitarConteudoFinalAcimaDoLimite()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            assuntoTemplate: null,
            conteudoTemplate: "{{valor.grande.1}}{{valor.grande.2}}{{valor.grande.3}}",
            variaveisPermitidas: ["valor.grande.1", "valor.grande.2", "valor.grande.3"]));
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(new Dictionary<string, string>
            {
                ["valor.grande.1"] = new string('x', 4000),
                ["valor.grande.2"] = new string('x', 4000),
                ["valor.grande.3"] = new string('x', 4000)
            })));
    }

    [Fact]
    public async Task DeveRespeitarCancellationToken()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        context.Set<TemplateNotificacao>().Add(CriarTemplate());
        await context.SaveChangesAsync();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            CriarUseCase(context).ExecutarAsync(CriarRequest(), cts.Token));
    }

    private static MaterializarConteudoNotificacaoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(PortalUseCasesTestFactory.Repo<TemplateNotificacao>(context));

    private static MaterializarConteudoNotificacaoRequest CriarRequest(
        IReadOnlyDictionary<string, string>? variaveis = null,
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email,
        Guid? templateNotificacaoId = null)
    {
        return new MaterializarConteudoNotificacaoRequest(
            tipoEvento,
            canal,
            new DateTime(2026, 6, 21, 22, 0, 0, DateTimeKind.Utc),
            variaveis ?? new Dictionary<string, string>
            {
                ["chamado.codigo"] = "CH-001"
            },
            templateNotificacaoId);
    }

    private static TemplateNotificacao CriarTemplate(
        string nome = "Template v1",
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email,
        int versao = 1,
        string? assuntoTemplate = "Assunto {{chamado.codigo}}",
        string conteudoTemplate = "Conteudo {{chamado.codigo}}",
        IReadOnlyCollection<string>? variaveisPermitidas = null,
        DateTime? vigenteDe = null,
        DateTime? vigenteAte = null)
    {
        return new TemplateNotificacao(
            nome,
            tipoEvento,
            canal,
            versao,
            conteudoTemplate,
            Guid.NewGuid(),
            "test.materializacao",
            variaveisPermitidas ?? ["chamado.codigo"],
            assuntoTemplate,
            "Descricao",
            vigenteDe,
            vigenteAte);
    }
}
