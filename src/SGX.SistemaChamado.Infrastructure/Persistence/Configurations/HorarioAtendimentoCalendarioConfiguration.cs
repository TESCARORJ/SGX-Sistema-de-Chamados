using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class HorarioAtendimentoCalendarioConfiguration : IEntityTypeConfiguration<HorarioAtendimentoCalendario>
{
    public void Configure(EntityTypeBuilder<HorarioAtendimentoCalendario> builder)
    {
        builder.ToTable("horarios_atendimento_calendario");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CalendarioCorporativoId).HasColumnName("calendario_corporativo_id").IsRequired();
        builder.Property(x => x.DiaSemana).HasColumnName("dia_semana").HasConversion<int>().IsRequired();
        builder.Property(x => x.HoraInicio).HasColumnName("hora_inicio").HasColumnType("time without time zone").IsRequired();
        builder.Property(x => x.HoraFim).HasColumnName("hora_fim").HasColumnType("time without time zone").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => new { x.CalendarioCorporativoId, x.DiaSemana })
            .HasDatabaseName("ix_horarios_atendimento_calendario_dia");

        builder.HasData(SeedData.HorariosAtendimentoCalendario);
    }
}
