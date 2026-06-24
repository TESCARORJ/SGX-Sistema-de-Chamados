using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class PreferenciaNotificacaoUsuarioConfiguration : IEntityTypeConfiguration<PreferenciaNotificacaoUsuario>
{
    public void Configure(EntityTypeBuilder<PreferenciaNotificacaoUsuario> builder)
    {
        builder.ToTable("preferencias_notificacao_usuario");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.TipoEvento).HasColumnName("tipo_evento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Canal).HasColumnName("canal").HasConversion<int>().IsRequired();
        builder.Property(x => x.Habilitada).HasColumnName("habilitada").IsRequired();
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(200).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(200);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AtualizadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UsuarioId, x.TipoEvento, x.Canal })
            .IsUnique()
            .HasDatabaseName("ux_preferencias_notificacao_usuario_chave");

        builder.HasIndex(x => x.UsuarioId)
            .HasDatabaseName("ix_preferencias_notificacao_usuario_usuario_id");

        builder.HasIndex(x => new { x.TipoEvento, x.Canal })
            .HasDatabaseName("ix_preferencias_notificacao_usuario_tipo_evento_canal");

        builder.HasIndex(x => x.CriadoPorUsuarioId)
            .HasDatabaseName("ix_preferencias_notificacao_usuario_criado_por_usuario_id");

        builder.HasIndex(x => x.AtualizadoPorUsuarioId)
            .HasDatabaseName("ix_preferencias_notificacao_usuario_atualizado_por_usuario_id");
    }
}
