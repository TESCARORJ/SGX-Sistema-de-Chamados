using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ComentarioChamadoConfiguration : IEntityTypeConfiguration<ComentarioChamado>
{
    public void Configure(EntityTypeBuilder<ComentarioChamado> builder)
    {
        builder.ToTable("comentarios_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.Mensagem).HasColumnName("mensagem").HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Interno).HasColumnName("interno").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("IX_comentarios_chamado_chamado_id");
        builder.HasIndex(x => x.UsuarioId).HasDatabaseName("IX_comentarios_chamado_usuario_id");
        builder.HasIndex(x => x.CriadoEm).HasDatabaseName("IX_comentarios_chamado_criado_em");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
