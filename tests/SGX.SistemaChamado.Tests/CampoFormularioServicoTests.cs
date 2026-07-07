using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CampoFormularioServicoTests
{
    [Fact]
    public void DeveCriarCampoValidoAssociadoAoFormulario()
    {
        var formularioServicoVersaoId = Guid.NewGuid();

        var campo = new CampoFormularioServico(
            formularioServicoVersaoId,
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            "Informe a matricula institucional.",
            true,
            "teste");

        Assert.Equal(formularioServicoVersaoId, campo.FormularioServicoVersaoId);
        Assert.Equal("matriculaServidor", campo.Nome);
        Assert.Equal("Matricula do servidor", campo.Rotulo);
        Assert.Equal(TipoCampoFormularioServico.TextoCurto, campo.Tipo);
        Assert.True(campo.Obrigatorio);
        Assert.Equal(1, campo.Ordem);
        Assert.Equal("Informe a matricula institucional.", campo.TextoAjuda);
        Assert.True(campo.Visivel);
        Assert.True(campo.Ativo);
    }

    [Fact]
    public void DeveCriarCampoOpcional()
    {
        var campo = new CampoFormularioServico(
            Guid.NewGuid(),
            "observacoes",
            "Observacoes",
            TipoCampoFormularioServico.TextoLongo,
            false,
            2,
            null,
            false,
            "teste");

        Assert.False(campo.Obrigatorio);
        Assert.Equal(2, campo.Ordem);
        Assert.Null(campo.TextoAjuda);
        Assert.False(campo.Visivel);
    }

    [Fact]
    public void DeveImpedirCampoSemVersaoDeFormulario()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CampoFormularioServico(
            Guid.Empty,
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            null,
            true,
            "teste"));

        Assert.Equal("formularioServicoVersaoId", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirNomeTecnicoVazio()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CampoFormularioServico(
            Guid.NewGuid(),
            " ",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            null,
            true,
            "teste"));

        Assert.Equal("nome", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirRotuloVazio()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CampoFormularioServico(
            Guid.NewGuid(),
            "matriculaServidor",
            " ",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            null,
            true,
            "teste"));

        Assert.Equal("rotulo", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirCriacaoComTipoInvalido()
    {
        var exception = Assert.Throws<ArgumentException>(() => new CampoFormularioServico(
            Guid.NewGuid(),
            "matriculaServidor",
            "Matricula do servidor",
            (TipoCampoFormularioServico)999,
            true,
            1,
            null,
            true,
            "teste"));

        Assert.Equal("tipo", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirOrdemMenorOuIgualAZero()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new CampoFormularioServico(
            Guid.NewGuid(),
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            0,
            null,
            true,
            "teste"));

        Assert.Equal("ordem", exception.ParamName);
    }

    [Fact]
    public void DeveImpedirTextoAjudaAcimaDoLimite()
    {
        var textoAjuda = new string('a', 501);

        var exception = Assert.Throws<ArgumentException>(() => new CampoFormularioServico(
            Guid.NewGuid(),
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            textoAjuda,
            true,
            "teste"));

        Assert.Equal("textoAjuda", exception.ParamName);
    }

    [Fact]
    public void DevePermitirInativarEReativarCampo()
    {
        var campo = new CampoFormularioServico(
            Guid.NewGuid(),
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            "Informe a matricula institucional.",
            true,
            "teste");

        campo.Inativar("teste");
        Assert.False(campo.Ativo);
        Assert.NotNull(campo.AtualizadoEm);

        campo.Reativar("teste");
        Assert.True(campo.Ativo);
        Assert.NotNull(campo.AtualizadoEm);
    }
}
