using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class RoadmapItsmItemConfiguration : IEntityTypeConfiguration<RoadmapItsmItem>
{
    public void Configure(EntityTypeBuilder<RoadmapItsmItem> builder)
    {
        builder.ToTable("roadmap_itsm_itens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Area).HasColumnName("area").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Objetivo).HasColumnName("objetivo").HasMaxLength(4000);
        builder.Property(x => x.RoadmapCategoriaId).HasColumnName("roadmap_categoria_id");
        builder.Property(x => x.SituacaoAtual).HasColumnName("situacao_atual").HasMaxLength(800).IsRequired();
        builder.Property(x => x.AtencaoTecnica).HasColumnName("atencao_tecnica").HasMaxLength(1200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.Prioridade).HasColumnName("prioridade").HasConversion<int>().IsRequired();
        builder.Property(x => x.Impacto).HasColumnName("impacto").HasConversion<int>().IsRequired();
        builder.Property(x => x.Decisao).HasColumnName("decisao").HasConversion<int>().IsRequired();
        builder.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(1200);
        builder.Property(x => x.Responsavel).HasColumnName("responsavel").HasMaxLength(180);
        builder.Property(x => x.PrazoAlvo).HasColumnName("prazo_alvo");
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.StatusImplementacao).HasColumnName("status_implementacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.StatusTecnico).HasColumnName("status_tecnico").HasConversion<int>().IsRequired();
        builder.Property(x => x.PercentualImplementacao).HasColumnName("percentual_implementacao").IsRequired();
        builder.Property(x => x.PendenciasTecnicas).HasColumnName("pendencias_tecnicas").HasMaxLength(4000);
        builder.Property(x => x.PendenciasHomologacao).HasColumnName("pendencias_homologacao").HasMaxLength(4000);
        builder.Property(x => x.EvidenciaImplementacao).HasColumnName("evidencia_implementacao").HasMaxLength(1000);
        builder.Property(x => x.DataConclusaoTecnica).HasColumnName("data_conclusao_tecnica");
        builder.Property(x => x.DataHomologacao).HasColumnName("data_homologacao");
        builder.Property(x => x.CriterioAceite).HasColumnName("criterio_aceite").HasMaxLength(4000);
        builder.Property(x => x.ProximaAcao).HasColumnName("proxima_acao").HasMaxLength(4000);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);

        builder.HasMany(x => x.ImplementacoesFuturas)
            .WithOne(x => x.RoadmapItem)
            .HasForeignKey(x => x.RoadmapItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ChecklistItens)
            .WithOne(x => x.RoadmapItem)
            .HasForeignKey(x => x.RoadmapItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RoadmapCategoria)
            .WithMany(x => x.ItensRoadmap)
            .HasForeignKey(x => x.RoadmapCategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.Ordem, x.Categoria }).HasDatabaseName("ix_roadmap_itsm_itens_ordem_categoria");
        builder.HasIndex(x => x.RoadmapCategoriaId).HasDatabaseName("ix_roadmap_itsm_itens_roadmap_categoria_id");

        builder.HasData(SeedData.RoadmapItsmItens);
    }
}
