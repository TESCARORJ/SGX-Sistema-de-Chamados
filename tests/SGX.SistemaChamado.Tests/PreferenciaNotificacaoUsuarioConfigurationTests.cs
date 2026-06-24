using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class PreferenciaNotificacaoUsuarioConfigurationTests
{
    [Fact]
    public void DeveMapearConfiguracaoEfCoreDePreferenciaNotificacaoUsuario()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new SGXSistemaChamadoDbContext(options);

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entity = designTimeModel.FindEntityType(typeof(PreferenciaNotificacaoUsuario));
        var table = StoreObjectIdentifier.Table("preferencias_notificacao_usuario", null);

        Assert.NotNull(entity);
        Assert.Equal("preferencias_notificacao_usuario", entity!.GetTableName());

        var primaryKey = entity.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal("id", primaryKey!.Properties.Single().GetColumnName(table));

        Assert.Equal("usuario_id", entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.UsuarioId))!.GetColumnName(table));
        Assert.Equal("tipo_evento", entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.TipoEvento))!.GetColumnName(table));
        Assert.Equal("canal", entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.Canal))!.GetColumnName(table));
        Assert.Equal("habilitada", entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.Habilitada))!.GetColumnName(table));

        Assert.False(entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.UsuarioId))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.TipoEvento))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.Canal))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.Habilitada))!.IsNullable);

        Assert.Equal(typeof(int), entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.TipoEvento))!.GetProviderClrType());
        Assert.Equal(typeof(int), entity.FindProperty(nameof(PreferenciaNotificacaoUsuario.Canal))!.GetProviderClrType());

        var fks = entity.GetForeignKeys().ToArray();
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(PreferenciaNotificacaoUsuario.UsuarioId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(PreferenciaNotificacaoUsuario.CriadoPorUsuarioId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(PreferenciaNotificacaoUsuario.AtualizadoPorUsuarioId));
        Assert.All(fks, x => Assert.Equal(DeleteBehavior.Restrict, x.DeleteBehavior));

        var indices = entity.GetIndexes().ToArray();
        Assert.Contains(indices, x => x.GetDatabaseName() == "ux_preferencias_notificacao_usuario_chave" && x.IsUnique);
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_preferencias_notificacao_usuario_usuario_id");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_preferencias_notificacao_usuario_tipo_evento_canal");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_preferencias_notificacao_usuario_criado_por_usuario_id");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_preferencias_notificacao_usuario_atualizado_por_usuario_id");

        Assert.DoesNotContain(fks, x => x.PrincipalEntityType.ClrType == typeof(TemplateNotificacao));
        Assert.DoesNotContain(fks, x => x.PrincipalEntityType.ClrType == typeof(Chamado));
        Assert.DoesNotContain(fks, x => x.PrincipalEntityType.ClrType == typeof(Notificacao));
    }
}
