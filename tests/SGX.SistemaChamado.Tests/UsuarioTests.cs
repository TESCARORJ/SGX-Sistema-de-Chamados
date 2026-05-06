using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class UsuarioTests
{
    [Fact]
    public void DeveCriarUsuarioValido()
    {
        var usuario = new Usuario(
            "Maria Silva",
            "maria.silva@empresa.com",
            "maria.silva",
            "sistema");

        Assert.Equal("Maria Silva", usuario.Nome);
        Assert.Equal("maria.silva@empresa.com", usuario.Email);
        Assert.Equal("maria.silva", usuario.Login);
    }

    [Fact]
    public void DeveExigirEmail()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("Maria Silva", "", "maria.silva", "sistema"));
    }

    [Fact]
    public void DeveExigirNome()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("", "maria.silva@empresa.com", "maria.silva", "sistema"));
    }
}
