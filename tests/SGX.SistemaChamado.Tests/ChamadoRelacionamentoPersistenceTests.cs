using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoPersistenceTests
{
    [Fact]
    public void DeveMapearConfiguracaoEfCoreDeChamadoRelacionamento()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new SGXSistemaChamadoDbContext(options);

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entity = designTimeModel.FindEntityType(typeof(ChamadoRelacionamento));

        Assert.NotNull(entity);
        Assert.Equal("chamados_relacionamentos", entity!.GetTableName());

        var indiceUnicoAtivo = entity.GetIndexes()
            .FirstOrDefault(x => x.GetDatabaseName() == "ux_chamados_relacionamentos_origem_destino_tipo_ativo");

        Assert.NotNull(indiceUnicoAtivo);
        Assert.True(indiceUnicoAtivo!.IsUnique);
        Assert.Equal("ativo = true", indiceUnicoAtivo.GetFilter());

        var checkConstraint = entity.GetCheckConstraints()
            .FirstOrDefault(x => x.Name == "ck_chamados_relacionamentos_origem_destino_diferentes");

        Assert.NotNull(checkConstraint);
    }
}
