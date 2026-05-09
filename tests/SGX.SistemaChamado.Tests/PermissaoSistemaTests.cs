using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class PermissaoSistemaTests
{
    [Fact]
    public void DeveCriarPermissaoSistemaValida()
    {
        var permissao = new PermissaoSistema("Chamados", "Visualizar", null, "teste");

        Assert.Equal("Chamados", permissao.Modulo);
        Assert.Equal("Visualizar", permissao.Acao);
        Assert.Equal("Chamados.Visualizar", permissao.Codigo);
    }

    [Fact]
    public void DeveExigirModuloNaPermissao()
    {
        Assert.Throws<ArgumentException>(() =>
            new PermissaoSistema(" ", "Visualizar", null, "teste"));
    }

    [Fact]
    public void DeveExigirAcaoNaPermissao()
    {
        Assert.Throws<ArgumentException>(() =>
            new PermissaoSistema("Chamados", " ", null, "teste"));
    }
}
