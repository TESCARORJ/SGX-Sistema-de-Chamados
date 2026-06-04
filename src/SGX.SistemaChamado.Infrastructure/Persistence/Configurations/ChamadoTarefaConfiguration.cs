using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoTarefaConfiguration : IEntityTypeConfiguration<ChamadoTarefa>
{
    public void Configure(EntityTypeBuilder<ChamadoTarefa> builder)
    {
        builder.ToTable("chamados_tarefas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.ResponsavelUsuarioId).HasColumnName("responsavel_usuario_id");
        builder.Property(x => x.Prazo).HasColumnName("prazo");
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.ConcluidoEm).HasColumnName("concluido_em");
        builder.Property(x => x.ConcluidoPorUsuarioId).HasColumnName("concluido_por_usuario_id");
        builder.Property(x => x.CanceladoEm).HasColumnName("cancelado_em");
        builder.Property(x => x.CanceladoPorUsuarioId).HasColumnName("cancelado_por_usuario_id");
        builder.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(1000);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_chamados_tarefas_chamado_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_chamados_tarefas_status");
        builder.HasIndex(x => x.ResponsavelUsuarioId).HasDatabaseName("ix_chamados_tarefas_responsavel_usuario_id");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_chamados_tarefas_criado_por_usuario_id");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_chamados_tarefas_ativo");
        builder.HasIndex(x => x.Prazo).HasDatabaseName("ix_chamados_tarefas_prazo");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.Tarefas)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResponsavelUsuario)
            .WithMany()
            .HasForeignKey(x => x.ResponsavelUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ConcluidoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.ConcluidoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CanceladoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CanceladoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
