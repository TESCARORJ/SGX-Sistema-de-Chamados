using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class PerfilAcessoPermissaoTests
{
    [Fact]
    public void DeveCriarPerfilAcessoPermissaoValida()
    {
        var perfilId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var permissaoId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var vinculo = new PerfilAcessoPermissao(perfilId, permissaoId, "teste");

        Assert.Equal(perfilId, vinculo.PerfilAcessoId);
        Assert.Equal(permissaoId, vinculo.PermissaoSistemaId);
    }

    [Fact]
    public void DeveExigirPerfilNoVinculo()
    {
        var permissaoId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        Assert.Throws<ArgumentException>(() =>
            new PerfilAcessoPermissao(Guid.Empty, permissaoId, "teste"));
    }

    [Fact]
    public void DeveExigirPermissaoNoVinculo()
    {
        var perfilId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        Assert.Throws<ArgumentException>(() =>
            new PerfilAcessoPermissao(perfilId, Guid.Empty, "teste"));
    }
}
