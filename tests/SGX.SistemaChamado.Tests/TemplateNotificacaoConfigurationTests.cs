using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class TemplateNotificacaoConfigurationTests
{
    [Fact]
    public void DeveMapearConfiguracaoEfCoreDeTemplateNotificacao()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new SGXSistemaChamadoDbContext(options);

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entity = designTimeModel.FindEntityType(typeof(TemplateNotificacao));
        var table = StoreObjectIdentifier.Table("templates_notificacao", null);

        Assert.NotNull(entity);
        Assert.Equal("templates_notificacao", entity!.GetTableName());
        Assert.True(context.Model.FindEntityType(typeof(TemplateNotificacao)) is not null);

        var primaryKey = entity.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal("id", primaryKey!.Properties.Single().GetColumnName(table));

        Assert.Equal("nome", entity.FindProperty(nameof(TemplateNotificacao.Nome))!.GetColumnName(table));
        Assert.Equal("descricao", entity.FindProperty(nameof(TemplateNotificacao.Descricao))!.GetColumnName(table));
        Assert.Equal("tipo_evento", entity.FindProperty(nameof(TemplateNotificacao.TipoEvento))!.GetColumnName(table));
        Assert.Equal("canal", entity.FindProperty(nameof(TemplateNotificacao.Canal))!.GetColumnName(table));
        Assert.Equal("versao", entity.FindProperty(nameof(TemplateNotificacao.Versao))!.GetColumnName(table));
        Assert.Equal("assunto_template", entity.FindProperty(nameof(TemplateNotificacao.AssuntoTemplate))!.GetColumnName(table));
        Assert.Equal("conteudo_template", entity.FindProperty(nameof(TemplateNotificacao.ConteudoTemplate))!.GetColumnName(table));
        Assert.Equal("variaveis_permitidas", entity.FindProperty("VariaveisPermitidasPersistidas")!.GetColumnName(table));
        Assert.Equal("vigente_de", entity.FindProperty(nameof(TemplateNotificacao.VigenteDe))!.GetColumnName(table));
        Assert.Equal("vigente_ate", entity.FindProperty(nameof(TemplateNotificacao.VigenteAte))!.GetColumnName(table));

        Assert.False(entity.FindProperty(nameof(TemplateNotificacao.Nome))!.IsNullable);
        Assert.True(entity.FindProperty(nameof(TemplateNotificacao.Descricao))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(TemplateNotificacao.ConteudoTemplate))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(TemplateNotificacao.Versao))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(TemplateNotificacao.TipoEvento))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(TemplateNotificacao.Canal))!.IsNullable);
        Assert.False(entity.FindProperty("VariaveisPermitidasPersistidas")!.IsNullable);

        Assert.Equal(typeof(int), entity.FindProperty(nameof(TemplateNotificacao.TipoEvento))!.GetProviderClrType());
        Assert.Equal(typeof(int), entity.FindProperty(nameof(TemplateNotificacao.Canal))!.GetProviderClrType());

        Assert.Equal(TemplateNotificacao.MaximoNome, entity.FindProperty(nameof(TemplateNotificacao.Nome))!.GetMaxLength());
        Assert.Equal(TemplateNotificacao.MaximoDescricao, entity.FindProperty(nameof(TemplateNotificacao.Descricao))!.GetMaxLength());
        Assert.Equal(TemplateNotificacao.MaximoAssuntoTemplate, entity.FindProperty(nameof(TemplateNotificacao.AssuntoTemplate))!.GetMaxLength());
        Assert.Equal(TemplateNotificacao.MaximoConteudoTemplate, entity.FindProperty(nameof(TemplateNotificacao.ConteudoTemplate))!.GetMaxLength());

        var fks = entity.GetForeignKeys().ToArray();
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(TemplateNotificacao.CriadoPorUsuarioId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(TemplateNotificacao.AtualizadoPorUsuarioId));
        Assert.All(fks, x => Assert.Equal(DeleteBehavior.Restrict, x.DeleteBehavior));

        var indices = entity.GetIndexes().ToArray();
        Assert.Contains(indices, x => x.GetDatabaseName() == "ux_templates_notificacao_nome_versao" && x.IsUnique);
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_templates_notificacao_tipo_evento_canal_ativo");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_templates_notificacao_vigencia");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_templates_notificacao_criado_por_usuario_id");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_templates_notificacao_atualizado_por_usuario_id");

        var checkVersao = entity.GetCheckConstraints().SingleOrDefault(x => x.Name == "ck_templates_notificacao_versao_positiva");
        var checkVigencia = entity.GetCheckConstraints().SingleOrDefault(x => x.Name == "ck_templates_notificacao_vigencia");

        Assert.NotNull(checkVersao);
        Assert.Equal("versao > 0", checkVersao!.Sql);
        Assert.NotNull(checkVigencia);
        Assert.Equal("vigente_ate IS NULL OR vigente_de IS NULL OR vigente_ate >= vigente_de", checkVigencia!.Sql);
    }
}
