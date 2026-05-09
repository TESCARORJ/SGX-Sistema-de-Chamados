using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class RoadmapChecklistItemConfiguration : IEntityTypeConfiguration<RoadmapChecklistItem>
{
    public void Configure(EntityTypeBuilder<RoadmapChecklistItem> builder)
    {
        builder.ToTable("roadmap_checklist_itens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.RoadmapItemId).HasColumnName("roadmap_item_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(2000);
        builder.Property(x => x.Grupo).HasColumnName("grupo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.Concluido).HasColumnName("concluido").IsRequired();
        builder.Property(x => x.Obrigatorio).HasColumnName("obrigatorio").IsRequired();
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);

        builder.HasOne(x => x.RoadmapItem)
            .WithMany(x => x.ChecklistItens)
            .HasForeignKey(x => x.RoadmapItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.RoadmapItemId).HasDatabaseName("ix_roadmap_checklist_itens_roadmap_item_id");
        builder.HasIndex(x => x.Grupo).HasDatabaseName("ix_roadmap_checklist_itens_grupo");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_roadmap_checklist_itens_ativo");
        builder.HasIndex(x => new { x.RoadmapItemId, x.Ordem }).HasDatabaseName("ix_roadmap_checklist_itens_item_ordem");

        builder.HasData(SeedData.RoadmapChecklistItens);
    }
}
