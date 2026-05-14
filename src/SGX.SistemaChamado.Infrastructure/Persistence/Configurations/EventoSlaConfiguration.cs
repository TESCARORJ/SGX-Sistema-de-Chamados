using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class EventoSlaConfiguration : IEntityTypeConfiguration<EventoSla>
{
    public void Configure(EntityTypeBuilder<EventoSla> builder)
    {
        builder.ToTable("eventos_sla");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.ChamadoSlaId).HasColumnName("chamado_sla_id").IsRequired();
        builder.Property(x => x.TipoEvento).HasColumnName("tipo_evento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.DataEvento).HasColumnName("data_evento").IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(220);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_eventos_sla_chamado_id");
        builder.HasIndex(x => x.ChamadoSlaId).HasDatabaseName("ix_eventos_sla_chamado_sla_id");
        builder.HasIndex(x => x.DataEvento).HasDatabaseName("ix_eventos_sla_data_evento");
        builder.HasIndex(x => x.ChaveIdempotencia)
            .IsUnique()
            .HasFilter("chave_idempotencia IS NOT NULL")
            .HasDatabaseName("ux_eventos_sla_chave_idempotencia");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.EventosSla)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ChamadoSla)
            .WithMany()
            .HasForeignKey(x => x.ChamadoSlaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
