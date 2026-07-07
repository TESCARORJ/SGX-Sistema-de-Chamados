using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoTests
{
    [Fact]
    public void DeveCriarFormularioPorServicoComDadosValidos()
    {
        var catalogoServicoId = Guid.NewGuid();

        var formulario = new FormularioServico(
            catalogoServicoId,
            "Formulario de onboarding",
            "Configuracao estrutural inicial.",
            "teste");

        Assert.Equal(catalogoServicoId, formulario.CatalogoServicoId);
        Assert.Equal("Formulario de onboarding", formulario.Nome);
        Assert.Equal("Configuracao estrutural inicial.", formulario.Descricao);
        Assert.True(formulario.Ativo);
    }

    [Fact]
    public void DeveImpedirFormularioSemServicoDoCatalogo()
    {
        var exception = Assert.Throws<ArgumentException>(() => new FormularioServico(
            Guid.Empty,
            "Formulario invalido",
            null,
            "teste"));

        Assert.Equal("catalogoServicoId", exception.ParamName);
    }

    [Fact]
    public void DevePermitirInativarEReativarFormulario()
    {
        var formulario = new FormularioServico(
            Guid.NewGuid(),
            "Formulario de onboarding",
            null,
            "teste");

        formulario.Inativar("teste");
        Assert.False(formulario.Ativo);
        Assert.NotNull(formulario.AtualizadoEm);

        formulario.Reativar("teste");
        Assert.True(formulario.Ativo);
        Assert.NotNull(formulario.AtualizadoEm);
    }
}
