using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class FilaAtendimentoConfiguration : IEntityTypeConfiguration<FilaAtendimento>
{
    public void Configure(EntityTypeBuilder<FilaAtendimento> builder)
    {
        builder.ToTable("filas_atendimento");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.GrupoTecnicoId).HasColumnName("grupo_tecnico_id").IsRequired();
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.GrupoTecnicoId).HasDatabaseName("ix_filas_atendimento_grupo_tecnico_id");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_filas_atendimento_ativo");
        builder.HasIndex(x => new { x.GrupoTecnicoId, x.Nome })
            .IsUnique()
            .HasDatabaseName("ux_filas_atendimento_grupo_nome");

        builder.HasOne(x => x.GrupoTecnico)
            .WithMany(x => x.FilasAtendimento)
            .HasForeignKey(x => x.GrupoTecnicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.FilasAtendimento);
    }
}
