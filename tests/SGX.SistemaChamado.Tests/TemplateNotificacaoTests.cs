using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class TemplateNotificacaoTests
{
    [Fact]
    public void DeveCriarTemplateValido()
    {
        var template = CriarTemplate(
            nome: "Aviso chamado",
            assuntoTemplate: "Chamado {{chamado.codigo}}",
            conteudoTemplate: "O chamado {{chamado.codigo}} foi atualizado.",
            variaveisPermitidas: ["chamado.codigo", "chamado.titulo"]);

        Assert.Equal("Aviso chamado", template.Nome);
        Assert.Equal("Chamado {{chamado.codigo}}", template.AssuntoTemplate);
        Assert.Equal("O chamado {{chamado.codigo}} foi atualizado.", template.ConteudoTemplate);
        Assert.Equal(TipoEventoNotificacao.EventoChamado, template.TipoEvento);
        Assert.Equal(CanalNotificacao.Email, template.Canal);
        Assert.Equal(1, template.Versao);
        Assert.True(template.Ativo);
        Assert.Equal(["chamado.codigo", "chamado.titulo"], template.VariaveisPermitidas);
    }

    [Fact]
    public void DeveRejeitarNomeObrigatorio()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(nome: " "));
    }

    [Fact]
    public void DeveRejeitarConteudoObrigatorio()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(conteudoTemplate: " "));
    }

    [Fact]
    public void DeveRejeitarVersaoNaoPositiva()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CriarTemplate(versao: 0));
    }

    [Fact]
    public void DeveRejeitarCanalInvalido()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(canal: (CanalNotificacao)999));
    }

    [Fact]
    public void DeveRejeitarEventoInvalido()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(tipoEvento: (TipoEventoNotificacao)999));
    }

    [Fact]
    public void DeveRejeitarVigenciaInvalida()
    {
        var inicio = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        var fim = inicio.AddMinutes(-1);

        Assert.Throws<InvalidOperationException>(() => CriarTemplate(vigenteDe: inicio, vigenteAte: fim));
    }

    [Fact]
    public void DeveNormalizarStringsEVariaveis()
    {
        var template = CriarTemplate(
            nome: "  Aviso  ",
            descricao: "  Descricao  ",
            assuntoTemplate: "  Assunto {{chamado.codigo}}  ",
            conteudoTemplate: "  Conteudo {{chamado.codigo}}  ",
            variaveisPermitidas: [" chamado.codigo ", "CHAMADO.TITULO"]);

        Assert.Equal("Aviso", template.Nome);
        Assert.Equal("Descricao", template.Descricao);
        Assert.Equal("Assunto {{chamado.codigo}}", template.AssuntoTemplate);
        Assert.Equal("Conteudo {{chamado.codigo}}", template.ConteudoTemplate);
        Assert.Equal(["chamado.codigo", "chamado.titulo"], template.VariaveisPermitidas);
    }

    [Fact]
    public void DevePermitirAtivarEDesativarTemplate()
    {
        var template = CriarTemplate();
        var usuarioId = Guid.NewGuid();

        template.DesativarTemplate(usuarioId, "teste");
        Assert.False(template.Ativo);
        Assert.Equal(usuarioId, template.AtualizadoPorUsuarioId);

        template.AtivarTemplate(usuarioId, "teste");
        Assert.True(template.Ativo);
        Assert.Equal(usuarioId, template.AtualizadoPorUsuarioId);
    }

    [Fact]
    public void DevePermitirNovaVersaoSemMutarTemplateAnterior()
    {
        var v1 = CriarTemplate(nome: "Aviso", versao: 1, conteudoTemplate: "Conteudo v1");
        var v2 = CriarTemplate(nome: "Aviso", versao: 2, conteudoTemplate: "Conteudo v2");

        Assert.Equal(1, v1.Versao);
        Assert.Equal(2, v2.Versao);
        Assert.Equal("Conteudo v1", v1.ConteudoTemplate);
        Assert.Equal("Conteudo v2", v2.ConteudoTemplate);
    }

    [Fact]
    public void DeveRejeitarVariavelPermitidaInvalidaOuDuplicada()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(variaveisPermitidas: ["chamado.codigo", "chamado.codigo"]));
        Assert.Throws<ArgumentException>(() => CriarTemplate(variaveisPermitidas: ["chamado.{{codigo}}"]));
    }

    [Fact]
    public void DeveRejeitarLimitesDeCampos()
    {
        Assert.Throws<ArgumentException>(() => CriarTemplate(nome: new string('n', TemplateNotificacao.MaximoNome + 1)));
        Assert.Throws<ArgumentException>(() => CriarTemplate(descricao: new string('d', TemplateNotificacao.MaximoDescricao + 1)));
        Assert.Throws<ArgumentException>(() => CriarTemplate(assuntoTemplate: new string('a', TemplateNotificacao.MaximoAssuntoTemplate + 1)));
        Assert.Throws<ArgumentException>(() => CriarTemplate(conteudoTemplate: new string('c', TemplateNotificacao.MaximoConteudoTemplate + 1)));
        Assert.Throws<ArgumentException>(() => CriarTemplate(variaveisPermitidas: [new string('v', TemplateNotificacao.MaximoVariavelPermitida + 1)]));
    }

    private static TemplateNotificacao CriarTemplate(
        string nome = "Template teste",
        TipoEventoNotificacao tipoEvento = TipoEventoNotificacao.EventoChamado,
        CanalNotificacao canal = CanalNotificacao.Email,
        int versao = 1,
        string conteudoTemplate = "Conteudo {{chamado.codigo}}",
        IReadOnlyCollection<string>? variaveisPermitidas = null,
        string? assuntoTemplate = "Assunto {{chamado.codigo}}",
        string? descricao = "Descricao de teste",
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
            "test.template",
            variaveisPermitidas ?? ["chamado.codigo"],
            assuntoTemplate,
            descricao,
            vigenteDe,
            vigenteAte);
    }
}
