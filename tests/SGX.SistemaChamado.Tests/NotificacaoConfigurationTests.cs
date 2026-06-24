using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class NotificacaoConfigurationTests
{
    [Fact]
    public void DeveMapearConfiguracaoEfCoreDeNotificacao()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        using var context = new SGXSistemaChamadoDbContext(options);

        var designTimeModel = context.GetService<IDesignTimeModel>().Model;
        var entity = designTimeModel.FindEntityType(typeof(Notificacao));
        var table = StoreObjectIdentifier.Table("notificacoes", null);

        Assert.NotNull(entity);
        Assert.Equal("notificacoes", entity!.GetTableName());
        Assert.True(context.Model.FindEntityType(typeof(Notificacao)) is not null);

        var primaryKey = entity.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Equal("id", primaryKey!.Properties.Single().GetColumnName(table));

        Assert.Equal("chamado_id", entity.FindProperty(nameof(Notificacao.ChamadoId))!.GetColumnName(table));
        Assert.Equal("tipo_evento", entity.FindProperty(nameof(Notificacao.TipoEvento))!.GetColumnName(table));
        Assert.Equal("canal", entity.FindProperty(nameof(Notificacao.Canal))!.GetColumnName(table));
        Assert.Equal("status", entity.FindProperty(nameof(Notificacao.Status))!.GetColumnName(table));
        Assert.Equal("destinatario_usuario_id", entity.FindProperty(nameof(Notificacao.DestinatarioUsuarioId))!.GetColumnName(table));
        Assert.Equal("destinatario_endereco", entity.FindProperty(nameof(Notificacao.DestinatarioEndereco))!.GetColumnName(table));
        Assert.Equal("assunto", entity.FindProperty(nameof(Notificacao.Assunto))!.GetColumnName(table));
        Assert.Equal("conteudo", entity.FindProperty(nameof(Notificacao.Conteudo))!.GetColumnName(table));
        Assert.Equal("chave_correlacao", entity.FindProperty(nameof(Notificacao.ChaveCorrelacao))!.GetColumnName(table));
        Assert.Equal("chave_idempotencia", entity.FindProperty(nameof(Notificacao.ChaveIdempotencia))!.GetColumnName(table));
        Assert.Equal("agendada_em", entity.FindProperty(nameof(Notificacao.AgendadaEm))!.GetColumnName(table));
        Assert.Equal("lida_em", entity.FindProperty(nameof(Notificacao.LidaEm))!.GetColumnName(table));
        Assert.Equal("criado_em", entity.FindProperty(nameof(Notificacao.CriadoEm))!.GetColumnName(table));

        Assert.True(entity.FindProperty(nameof(Notificacao.ChamadoId))!.IsNullable);
        Assert.True(entity.FindProperty(nameof(Notificacao.DestinatarioUsuarioId))!.IsNullable);
        Assert.True(entity.FindProperty(nameof(Notificacao.DestinatarioEndereco))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Notificacao.Conteudo))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Notificacao.ChaveIdempotencia))!.IsNullable);
        Assert.False(entity.FindProperty(nameof(Notificacao.QuantidadeTentativas))!.IsNullable);

        Assert.Equal(typeof(int), entity.FindProperty(nameof(Notificacao.TipoEvento))!.GetProviderClrType());
        Assert.Equal(typeof(int), entity.FindProperty(nameof(Notificacao.Canal))!.GetProviderClrType());
        Assert.Equal(typeof(int), entity.FindProperty(nameof(Notificacao.Status))!.GetProviderClrType());

        Assert.Equal(320, entity.FindProperty(nameof(Notificacao.DestinatarioEndereco))!.GetMaxLength());
        Assert.Equal(300, entity.FindProperty(nameof(Notificacao.Assunto))!.GetMaxLength());
        Assert.Equal(10000, entity.FindProperty(nameof(Notificacao.Conteudo))!.GetMaxLength());
        Assert.Equal(200, entity.FindProperty(nameof(Notificacao.ChaveCorrelacao))!.GetMaxLength());
        Assert.Equal(200, entity.FindProperty(nameof(Notificacao.ChaveIdempotencia))!.GetMaxLength());
        Assert.Equal(2000, entity.FindProperty(nameof(Notificacao.UltimoErro))!.GetMaxLength());
        Assert.Equal(1000, entity.FindProperty(nameof(Notificacao.MotivoCancelamento))!.GetMaxLength());

        var fks = entity.GetForeignKeys().ToArray();
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Chamado) && x.Properties.Single().Name == nameof(Notificacao.ChamadoId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(Notificacao.DestinatarioUsuarioId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(Notificacao.CriadoPorUsuarioId));
        Assert.Contains(fks, x => x.PrincipalEntityType.ClrType == typeof(Usuario) && x.Properties.Single().Name == nameof(Notificacao.AtualizadoPorUsuarioId));
        Assert.DoesNotContain(fks, x => x.Properties.Any(p => p.Name.Contains("Template", StringComparison.OrdinalIgnoreCase)));
        Assert.DoesNotContain(fks, x => x.Properties.Any(p => p.Name.Contains("Preferencia", StringComparison.OrdinalIgnoreCase)));
        Assert.All(fks, x => Assert.Equal(DeleteBehavior.Restrict, x.DeleteBehavior));

        var indices = entity.GetIndexes().ToArray();
        var indiceIdempotencia = indices.SingleOrDefault(x => x.GetDatabaseName() == "ux_notificacoes_chave_idempotencia");
        Assert.NotNull(indiceIdempotencia);
        Assert.True(indiceIdempotencia!.IsUnique);
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_chamado_id");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_status");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_canal");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_agendada_em");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_destinatario_usuario_id");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_criado_em");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_status_agendada_em");
        Assert.Contains(indices, x => x.GetDatabaseName() == "ix_notificacoes_destinatario_canal_status_criado_em");

        var checkDestinatario = entity.GetCheckConstraints().SingleOrDefault(x => x.Name == "ck_notificacoes_destinatario");
        var checkTentativas = entity.GetCheckConstraints().SingleOrDefault(x => x.Name == "ck_notificacoes_quantidade_tentativas_nao_negativa");
        var checkLeitura = entity.GetCheckConstraints().SingleOrDefault(x => x.Name == "ck_notificacoes_lida_em_maior_ou_igual_enviada_em");

        Assert.NotNull(checkDestinatario);
        Assert.Equal("destinatario_usuario_id IS NOT NULL OR destinatario_endereco IS NOT NULL", checkDestinatario!.Sql);
        Assert.NotNull(checkTentativas);
        Assert.Equal("quantidade_tentativas >= 0", checkTentativas!.Sql);
        Assert.NotNull(checkLeitura);
        Assert.Equal("lida_em IS NULL OR enviada_em IS NULL OR lida_em >= enviada_em", checkLeitura!.Sql);

        Assert.DoesNotContain(designTimeModel.GetEntityTypes(), x => string.Equals(x.GetTableName(), "notificacoes_destinatarios", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(designTimeModel.GetEntityTypes(), x => string.Equals(x.GetTableName(), "preferencias_notificacao", StringComparison.OrdinalIgnoreCase));
    }
}
