using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class CalendarioCorporativoConfiguration : IEntityTypeConfiguration<CalendarioCorporativo>
{
    public void Configure(EntityTypeBuilder<CalendarioCorporativo> builder)
    {
        builder.ToTable("calendarios_corporativos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(160).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(1000);
        builder.Property(x => x.Padrao).HasColumnName("padrao").IsRequired();
        builder.Property(x => x.TimeZone).HasColumnName("time_zone").HasMaxLength(120).IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Nome).HasDatabaseName("ix_calendarios_corporativos_nome");
        builder.HasIndex(x => x.Padrao)
            .IsUnique()
            .HasFilter("padrao = true AND ativo = true")
            .HasDatabaseName("ux_calendarios_corporativos_padrao_ativo");

        builder.HasMany(x => x.HorariosAtendimento)
            .WithOne(x => x.CalendarioCorporativo)
            .HasForeignKey(x => x.CalendarioCorporativoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Excecoes)
            .WithOne(x => x.CalendarioCorporativo)
            .HasForeignKey(x => x.CalendarioCorporativoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.CalendariosCorporativos);
    }
}
