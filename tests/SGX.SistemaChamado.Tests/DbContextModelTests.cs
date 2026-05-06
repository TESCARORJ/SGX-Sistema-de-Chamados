using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DbContextModelTests
{
    [Fact]
    public void DeveCriarModeloEfSemErro()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new SGXSistemaChamadoDbContext(options);

        var chamadoEntity = context.Model.FindEntityType(typeof(Chamado));
        var usuarioEntity = context.Model.FindEntityType(typeof(Usuario));

        Assert.NotNull(chamadoEntity);
        Assert.NotNull(usuarioEntity);
        Assert.Equal("chamados", chamadoEntity!.GetTableName());
        Assert.Equal("usuarios", usuarioEntity!.GetTableName());
    }
}
