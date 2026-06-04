using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoRelacionamentoConfiguration : IEntityTypeConfiguration<ChamadoRelacionamento>
{
    public void Configure(EntityTypeBuilder<ChamadoRelacionamento> builder)
    {
        builder.ToTable("chamados_relacionamentos", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_chamados_relacionamentos_origem_destino_diferentes",
                "chamado_origem_id <> chamado_destino_id");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoOrigemId).HasColumnName("chamado_origem_id").IsRequired();
        builder.Property(x => x.ChamadoDestinoId).HasColumnName("chamado_destino_id").IsRequired();
        builder.Property(x => x.TipoRelacionamento).HasColumnName("tipo_relacionamento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Justificativa).HasColumnName("justificativa").HasMaxLength(2000);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.RemovidoEm).HasColumnName("removido_em");
        builder.Property(x => x.RemovidoPorUsuarioId).HasColumnName("removido_por_usuario_id");
        builder.Property(x => x.MotivoRemocao).HasColumnName("motivo_remocao").HasMaxLength(1000);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoOrigemId).HasDatabaseName("ix_chamados_relacionamentos_chamado_origem_id");
        builder.HasIndex(x => x.ChamadoDestinoId).HasDatabaseName("ix_chamados_relacionamentos_chamado_destino_id");
        builder.HasIndex(x => x.TipoRelacionamento).HasDatabaseName("ix_chamados_relacionamentos_tipo_relacionamento");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_chamados_relacionamentos_criado_por_usuario_id");
        builder.HasIndex(x => x.RemovidoPorUsuarioId).HasDatabaseName("ix_chamados_relacionamentos_removido_por_usuario_id");
        builder.HasIndex(x => new { x.ChamadoOrigemId, x.ChamadoDestinoId, x.TipoRelacionamento, x.Ativo })
            .HasDatabaseName("ix_chamados_relacionamentos_origem_destino_tipo_ativo");
        builder.HasIndex(x => new { x.ChamadoOrigemId, x.ChamadoDestinoId, x.TipoRelacionamento })
            .IsUnique()
            .HasFilter("ativo = true")
            .HasDatabaseName("ux_chamados_relacionamentos_origem_destino_tipo_ativo");

        builder.HasOne(x => x.ChamadoOrigem)
            .WithMany(x => x.RelacionamentosOrigem)
            .HasForeignKey(x => x.ChamadoOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChamadoDestino)
            .WithMany(x => x.RelacionamentosDestino)
            .HasForeignKey(x => x.ChamadoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RemovidoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.RemovidoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
