using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class SlaControleConfiguration : IEntityTypeConfiguration<SlaControle>
{
    public void Configure(EntityTypeBuilder<SlaControle> builder)
    {
        builder.ToTable("sla_controles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.PrazoPrimeiraRespostaEm).HasColumnName("prazo_primeira_resposta_em").IsRequired();
        builder.Property(x => x.PrimeiraRespostaEm).HasColumnName("primeira_resposta_em");
        builder.Property(x => x.PrazoResolucaoEm).HasColumnName("prazo_resolucao_em").IsRequired();
        builder.Property(x => x.ResolvidoEm).HasColumnName("resolvido_em");
        builder.Property(x => x.EstaVencido).HasColumnName("esta_vencido").IsRequired();
        builder.Property(x => x.EstaPausado).HasColumnName("esta_pausado").IsRequired();
        builder.Property(x => x.PausadoEm).HasColumnName("pausado_em");
        builder.Property(x => x.TotalMinutosPausado).HasColumnName("total_minutos_pausado").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).IsUnique().HasDatabaseName("ux_sla_controles_chamado_id");
    }
}
