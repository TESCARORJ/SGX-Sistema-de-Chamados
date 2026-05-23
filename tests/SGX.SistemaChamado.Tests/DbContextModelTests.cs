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
        var eventoAuditoriaEntity = context.Model.FindEntityType(typeof(EventoAuditoria));
        var baseConhecimentoArtigoEntity = context.Model.FindEntityType(typeof(BaseConhecimentoArtigo));
        var chamadoArtigoConhecimentoEntity = context.Model.FindEntityType(typeof(ChamadoArtigoConhecimento));

        Assert.NotNull(chamadoEntity);
        Assert.NotNull(usuarioEntity);
        Assert.NotNull(roadmapEntity);
        Assert.NotNull(roadmapImplementacaoEntity);
        Assert.NotNull(roadmapCategoriaEntity);
        Assert.NotNull(roadmapChecklistEntity);
        Assert.NotNull(permissaoEntity);
        Assert.NotNull(perfilPermissaoEntity);
        Assert.NotNull(eventoAuditoriaEntity);
        Assert.NotNull(baseConhecimentoArtigoEntity);
        Assert.NotNull(chamadoArtigoConhecimentoEntity);
        Assert.Equal("chamados", chamadoEntity!.GetTableName());
        Assert.Equal("usuarios", usuarioEntity!.GetTableName());
        Assert.Equal("roadmap_itsm_itens", roadmapEntity!.GetTableName());
        Assert.Equal("roadmap_implementacoes_futuras", roadmapImplementacaoEntity!.GetTableName());
        Assert.Equal("roadmap_categorias", roadmapCategoriaEntity!.GetTableName());
        Assert.Equal("roadmap_checklist_itens", roadmapChecklistEntity!.GetTableName());
        Assert.Equal("permissoes_sistema", permissaoEntity!.GetTableName());
        Assert.Equal("perfis_acesso_permissoes", perfilPermissaoEntity!.GetTableName());
        Assert.Equal("eventos_auditoria", eventoAuditoriaEntity!.GetTableName());
        Assert.Equal("base_conhecimento_artigos", baseConhecimentoArtigoEntity!.GetTableName());
        Assert.Equal("chamados_artigos_conhecimento", chamadoArtigoConhecimentoEntity!.GetTableName());

        var indiceCodigoPermissao = permissaoEntity.GetIndexes()
            .FirstOrDefault(x => x.GetDatabaseName() == "ux_permissoes_sistema_codigo");
        Assert.NotNull(indiceCodigoPermissao);
        Assert.True(indiceCodigoPermissao!.IsUnique);

        var indicesAuditoria = eventoAuditoriaEntity.GetIndexes()
            .Select(x => x.GetDatabaseName())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("ix_eventos_auditoria_data_evento", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_usuario_id", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_usuario_email", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_modulo", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_entidade", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_entidade_id", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_acao", indicesAuditoria);
        Assert.Contains("ix_eventos_auditoria_correlacao_id", indicesAuditoria);
    }
}
