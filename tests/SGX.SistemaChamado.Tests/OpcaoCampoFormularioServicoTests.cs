using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class OpcaoCampoFormularioServicoTests
{
    [Fact]
    public void DeveCriarOpcaoValidaAssociadaAoCampo()
    {
        var opcao = new OpcaoCampoFormularioServico(
            Guid.NewGuid(),
            "infra",
            "Infraestrutura",
            1,
            "teste");

        Assert.Equal("infra", opcao.Valor);
        Assert.Equal("Infraestrutura", opcao.Rotulo);
        Assert.Equal(1, opcao.Ordem);
        Assert.True(opcao.Ativo);
    }

    [Fact]
    public void DeveImpedirOpcaoSemCampo()
    {
        var exception = Assert.Throws<ArgumentException>(() => new OpcaoCampoFormularioServico(
            Guid.Empty,
            "infra",
            "Infraestrutura",
            1,
            "teste"));

        Assert.Equal("campoFormularioServicoId", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirValorVazio()
    {
        var exception = Assert.Throws<ArgumentException>(() => new OpcaoCampoFormularioServico(
            Guid.NewGuid(),
            " ",
            "Infraestrutura",
            1,
            "teste"));

        Assert.Equal("valor", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirRotuloVazio()
    {
        var exception = Assert.Throws<ArgumentException>(() => new OpcaoCampoFormularioServico(
            Guid.NewGuid(),
            "infra",
            " ",
            1,
            "teste"));

        Assert.Equal("rotulo", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirOrdemMenorOuIgualAZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new OpcaoCampoFormularioServico(
            Guid.NewGuid(),
            "infra",
            "Infraestrutura",
            0,
            "teste"));

        Assert.Equal("ordem", exception.ParamName);
    }

    [Fact]
    public void DevePermitirInativarEReativarOpcao()
    {
        var opcao = new OpcaoCampoFormularioServico(
            Guid.NewGuid(),
            "infra",
            "Infraestrutura",
            1,
            "teste");

        opcao.Inativar("teste");
        Assert.False(opcao.Ativo);
        Assert.NotNull(opcao.AtualizadoEm);

        opcao.Reativar("teste");
        Assert.True(opcao.Ativo);
        Assert.NotNull(opcao.AtualizadoEm);
    }
}
