using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class PrioridadeChamadoConfiguration : IEntityTypeConfiguration<PrioridadeChamado>
{
    public void Configure(EntityTypeBuilder<PrioridadeChamado> builder)
    {
        builder.ToTable("prioridades_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Nivel).HasColumnName("nivel").HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500);
        builder.Property(x => x.PrazoPrimeiraRespostaHoras).HasColumnName("prazo_primeira_resposta_horas").IsRequired();
        builder.Property(x => x.PrazoResolucaoHoras).HasColumnName("prazo_resolucao_horas").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Nivel).IsUnique().HasDatabaseName("ux_prioridades_chamado_nivel");

        builder.HasData(SeedData.PrioridadesChamado);
    }
}
