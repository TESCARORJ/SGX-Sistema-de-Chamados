using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class RoadmapCategoriaConfiguration : IEntityTypeConfiguration<RoadmapCategoria>
{
    public void Configure(EntityTypeBuilder<RoadmapCategoria> builder)
    {
        builder.ToTable("roadmap_categorias");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(1000);
        builder.Property(x => x.Cor).HasColumnName("cor").HasMaxLength(30);
        builder.Property(x => x.Icone).HasColumnName("icone").HasMaxLength(80);
        builder.Property(x => x.Ordem).HasColumnName("ordem");
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);

        builder.HasIndex(x => x.Nome).HasDatabaseName("ux_roadmap_categorias_nome").IsUnique();
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_roadmap_categorias_ativo");
        builder.HasIndex(x => x.Ordem).HasDatabaseName("ix_roadmap_categorias_ordem");

        builder.HasData(SeedData.RoadmapCategorias);
    }
}
