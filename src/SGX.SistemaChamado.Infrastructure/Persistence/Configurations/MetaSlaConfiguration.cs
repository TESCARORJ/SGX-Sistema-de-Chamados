using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class MetaSlaConfiguration : IEntityTypeConfiguration<MetaSla>
{
    public void Configure(EntityTypeBuilder<MetaSla> builder)
    {
        builder.ToTable("sla_metas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PoliticaSlaId).HasColumnName("politica_sla_id").IsRequired();
        builder.Property(x => x.PrioridadeId).HasColumnName("prioridade_id").IsRequired();
        builder.Property(x => x.TempoPrimeiraRespostaMinutos).HasColumnName("tempo_primeira_resposta_minutos").IsRequired();
        builder.Property(x => x.TempoResolucaoMinutos).HasColumnName("tempo_resolucao_minutos").IsRequired();
        builder.Property(x => x.TempoAtualizacaoMinutos).HasColumnName("tempo_atualizacao_minutos");
        builder.Property(x => x.TempoRespostaSubsequenteMinutos).HasColumnName("tempo_resposta_subsequente_minutos");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => new { x.PoliticaSlaId, x.PrioridadeId })
            .IsUnique()
            .HasDatabaseName("ux_sla_metas_politica_prioridade");

        builder.HasOne(x => x.Prioridade)
            .WithMany()
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.SlaMetas);
    }
}
