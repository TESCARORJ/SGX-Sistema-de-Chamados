using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class RoadmapImplementacaoFuturaConfiguration : IEntityTypeConfiguration<RoadmapImplementacaoFutura>
{
    public void Configure(EntityTypeBuilder<RoadmapImplementacaoFutura> builder)
    {
        builder.ToTable("roadmap_implementacoes_futuras");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.RoadmapItemId).HasColumnName("roadmap_item_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(2000);
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Prioridade).HasColumnName("prioridade").HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.Responsavel).HasColumnName("responsavel").HasMaxLength(180);
        builder.Property(x => x.PrazoAlvo).HasColumnName("prazo_alvo");
        builder.Property(x => x.DataConclusao).HasColumnName("data_conclusao");
        builder.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(2000);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);

        builder.HasIndex(x => x.RoadmapItemId).HasDatabaseName("ix_roadmap_impl_futuras_roadmap_item_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_roadmap_impl_futuras_status");
        builder.HasIndex(x => x.Tipo).HasDatabaseName("ix_roadmap_impl_futuras_tipo");
        builder.HasIndex(x => x.Prioridade).HasDatabaseName("ix_roadmap_impl_futuras_prioridade");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_roadmap_impl_futuras_ativo");
    }
}
