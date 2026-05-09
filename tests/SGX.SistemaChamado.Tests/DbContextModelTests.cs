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
        var roadmapEntity = context.Model.FindEntityType(typeof(RoadmapItsmItem));
        var roadmapImplementacaoEntity = context.Model.FindEntityType(typeof(RoadmapImplementacaoFutura));
        var roadmapCategoriaEntity = context.Model.FindEntityType(typeof(RoadmapCategoria));
        var roadmapChecklistEntity = context.Model.FindEntityType(typeof(RoadmapChecklistItem));
        var permissaoEntity = context.Model.FindEntityType(typeof(PermissaoSistema));
        var perfilPermissaoEntity = context.Model.FindEntityType(typeof(PerfilAcessoPermissao));

        Assert.NotNull(chamadoEntity);
        Assert.NotNull(usuarioEntity);
        Assert.NotNull(roadmapEntity);
        Assert.NotNull(roadmapImplementacaoEntity);
        Assert.NotNull(roadmapCategoriaEntity);
        Assert.NotNull(roadmapChecklistEntity);
        Assert.NotNull(permissaoEntity);
        Assert.NotNull(perfilPermissaoEntity);
        Assert.Equal("chamados", chamadoEntity!.GetTableName());
        Assert.Equal("usuarios", usuarioEntity!.GetTableName());
        Assert.Equal("roadmap_itsm_itens", roadmapEntity!.GetTableName());
        Assert.Equal("roadmap_implementacoes_futuras", roadmapImplementacaoEntity!.GetTableName());
        Assert.Equal("roadmap_categorias", roadmapCategoriaEntity!.GetTableName());
        Assert.Equal("roadmap_checklist_itens", roadmapChecklistEntity!.GetTableName());
        Assert.Equal("permissoes_sistema", permissaoEntity!.GetTableName());
        Assert.Equal("perfis_acesso_permissoes", perfilPermissaoEntity!.GetTableName());

        var indiceCodigoPermissao = permissaoEntity.GetIndexes()
            .FirstOrDefault(x => x.GetDatabaseName() == "ux_permissoes_sistema_codigo");
        Assert.NotNull(indiceCodigoPermissao);
        Assert.True(indiceCodigoPermissao!.IsUnique);
    }
}
