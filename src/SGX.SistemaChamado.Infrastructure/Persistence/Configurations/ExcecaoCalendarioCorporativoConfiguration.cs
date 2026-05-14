using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ExcecaoCalendarioCorporativoConfiguration : IEntityTypeConfiguration<ExcecaoCalendarioCorporativo>
{
    public void Configure(EntityTypeBuilder<ExcecaoCalendarioCorporativo> builder)
    {
        builder.ToTable("excecoes_calendario_corporativo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CalendarioCorporativoId).HasColumnName("calendario_corporativo_id").IsRequired();
        builder.Property(x => x.Data).HasColumnName("data").HasColumnType("date").IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500);
        builder.Property(x => x.HoraInicio).HasColumnName("hora_inicio").HasColumnType("time without time zone");
        builder.Property(x => x.HoraFim).HasColumnName("hora_fim").HasColumnType("time without time zone");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => new { x.CalendarioCorporativoId, x.Data })
            .HasDatabaseName("ix_excecoes_calendario_corporativo_data");
    }
}
