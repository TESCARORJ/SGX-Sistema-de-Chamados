using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class MembroGrupoTecnicoConfiguration : IEntityTypeConfiguration<MembroGrupoTecnico>
{
    public void Configure(EntityTypeBuilder<MembroGrupoTecnico> builder)
    {
        builder.ToTable("membros_grupos_tecnicos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.GrupoTecnicoId).HasColumnName("grupo_tecnico_id").IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.GrupoTecnicoId).HasDatabaseName("ix_membros_grupos_tecnicos_grupo_tecnico_id");
        builder.HasIndex(x => x.UsuarioId).HasDatabaseName("ix_membros_grupos_tecnicos_usuario_id");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_membros_grupos_tecnicos_ativo");
        builder.HasIndex(x => new { x.GrupoTecnicoId, x.UsuarioId })
            .IsUnique()
            .HasDatabaseName("ux_membros_grupos_tecnicos_grupo_usuario");

        builder.HasOne(x => x.GrupoTecnico)
            .WithMany(x => x.Membros)
            .HasForeignKey(x => x.GrupoTecnicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
