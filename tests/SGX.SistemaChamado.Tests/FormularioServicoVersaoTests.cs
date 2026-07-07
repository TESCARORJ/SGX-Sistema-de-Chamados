using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoVersaoTests
{
    [Fact]
    public void DeveCriarVersaoValidaVinculadaAoFormulario()
    {
        var formularioId = Guid.NewGuid();

        var versao = new FormularioServicoVersao(
            formularioId,
            1,
            false,
            null,
            "teste");

        Assert.Equal(formularioId, versao.FormularioServicoId);
        Assert.Equal(1, versao.Numero);
        Assert.False(versao.Publicada);
        Assert.Null(versao.PublicadoEm);
        Assert.True(versao.Ativo);
    }

    [Fact]
    public void DeveImpedirVersaoSemFormulario()
    {
        var exception = Assert.Throws<ArgumentException>(() => new FormularioServicoVersao(
            Guid.Empty,
            1,
            false,
            null,
            "teste"));

        Assert.Equal("formularioServicoId", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirNumeroDeVersaoMenorOuIgualAZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new FormularioServicoVersao(
            Guid.NewGuid(),
            0,
            false,
            null,
            "teste"));

        Assert.Equal("numero", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirPublicadoEmQuandoVersaoNaoEstiverPublicada()
    {
        var exception = Assert.Throws<ArgumentException>(() => new FormularioServicoVersao(
            Guid.NewGuid(),
            1,
            false,
            DateTime.UtcNow,
            "teste"));

        Assert.Equal("publicadoEm", exception.ParamName);
    }
}
