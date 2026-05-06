using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class StatusChamadoConfiguration : IEntityTypeConfiguration<StatusChamado>
{
    public void Configure(EntityTypeBuilder<StatusChamado> builder)
    {
        builder.ToTable("status_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500);
        builder.Property(x => x.EhStatusFinal).HasColumnName("eh_status_final").IsRequired();
        builder.Property(x => x.PausaSla).HasColumnName("pausa_sla").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("ux_status_chamado_codigo");

        builder.HasData(SeedData.StatusChamado);
    }
}
