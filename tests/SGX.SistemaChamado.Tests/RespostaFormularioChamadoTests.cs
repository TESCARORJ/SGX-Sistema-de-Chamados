using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class RespostaFormularioChamadoTests
{
    [Fact]
    public void DeveCriarRespostaValidaComValor()
    {
        var chamadoId = Guid.NewGuid();
        var versaoId = Guid.NewGuid();
        var campoId = Guid.NewGuid();

        var resposta = new RespostaFormularioChamado(
            chamadoId,
            versaoId,
            campoId,
            "  valor unico  ",
            null,
            "teste");

        Assert.Equal(chamadoId, resposta.ChamadoId);
        Assert.Equal(versaoId, resposta.FormularioServicoVersaoId);
        Assert.Equal(campoId, resposta.CampoFormularioServicoId);
        Assert.Equal("valor unico", resposta.Valor);
        Assert.Null(resposta.ValoresJson);
        Assert.Empty(resposta.ObterValores());
        Assert.True(resposta.Ativo);
    }

    [Fact]
    public void DeveCriarRespostaValidaComMultiplosValores()
    {
        var resposta = new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            [" vpn ", "email"],
            "teste");

        Assert.Null(resposta.Valor);
        Assert.NotNull(resposta.ValoresJson);
        Assert.Equal(new[] { "vpn", "email" }, resposta.ObterValores());
    }

    [Fact]
    public void DeveRejeitarRespostaSemChamado()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "valor",
            null,
            "teste"));

        Assert.Equal("chamadoId", exception.ParamName);
    }

    [Fact]
    public void DeveRejeitarRespostaSemVersao()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid(),
            "valor",
            null,
            "teste"));

        Assert.Equal("formularioServicoVersaoId", exception.ParamName);
    }

    [Fact]
    public void DeveRejeitarRespostaSemCampo()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty,
            "valor",
            null,
            "teste"));

        Assert.Equal("campoFormularioServicoId", exception.ParamName);
    }

    [Fact]
    public void DeveRejeitarRespostaSemConteudo()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null,
            "teste"));

        Assert.Equal("valor", exception.ParamName);
    }

    [Fact]
    public void DeveRejeitarRespostaComValorEValoresAoMesmoTempo()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "valor",
            ["email"],
            "teste"));

        Assert.Equal("valor", exception.ParamName);
    }

    [Fact]
    public void DeveRejeitarMultiplosValoresComItemVazio()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            ["email", " "],
            "teste"));

        Assert.Equal("valores", exception.ParamName);
    }

    [Fact]
    public void DeveValidarLimiteDeValor()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new string('x', RespostaFormularioChamado.TamanhoMaximoValor + 1),
            null,
            "teste"));

        Assert.Equal("valor", exception.ParamName);
    }

    [Fact]
    public void DeveValidarLimiteDeCadaItemEmValores()
    {
        var exception = Assert.Throws<ArgumentException>(() => new RespostaFormularioChamado(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            [new string('x', RespostaFormularioChamado.TamanhoMaximoValor + 1)],
            "teste"));

        Assert.Equal("valores", exception.ParamName);
    }
}
