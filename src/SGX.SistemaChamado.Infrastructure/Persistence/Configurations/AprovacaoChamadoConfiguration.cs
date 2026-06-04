using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class AprovacaoChamadoConfiguration : IEntityTypeConfiguration<AprovacaoChamado>
{
    public void Configure(EntityTypeBuilder<AprovacaoChamado> builder)
    {
        builder.ToTable("aprovacoes_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoOrigem).HasColumnName("tipo_origem").HasConversion<int>().IsRequired();
        builder.Property(x => x.BloqueiaAvancoAtendimento).HasColumnName("bloqueia_avanco_atendimento").IsRequired();
        builder.Property(x => x.OrigemDescricao).HasColumnName("origem_descricao").HasMaxLength(300);
        builder.Property(x => x.SolicitanteId).HasColumnName("solicitante_id");
        builder.Property(x => x.AprovadorId).HasColumnName("aprovador_id");
        builder.Property(x => x.JustificativaSolicitacao).HasColumnName("justificativa_solicitacao").HasMaxLength(4000);
        builder.Property(x => x.JustificativaDecisao).HasColumnName("justificativa_decisao").HasMaxLength(4000);
        builder.Property(x => x.SolicitadaEm).HasColumnName("solicitada_em").IsRequired();
        builder.Property(x => x.DecididaEm).HasColumnName("decidida_em");
        builder.Property(x => x.CanceladoEm).HasColumnName("cancelado_em");
        builder.Property(x => x.CanceladoPorUsuarioId).HasColumnName("cancelado_por_usuario_id");
        builder.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(1000);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_aprovacoes_chamado_chamado_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_aprovacoes_chamado_status");
        builder.HasIndex(x => x.SolicitanteId).HasDatabaseName("ix_aprovacoes_chamado_solicitante_id");
        builder.HasIndex(x => x.AprovadorId).HasDatabaseName("ix_aprovacoes_chamado_aprovador_id");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_aprovacoes_chamado_ativo");
        builder.HasIndex(x => x.SolicitadaEm).HasDatabaseName("ix_aprovacoes_chamado_solicitada_em");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_aprovacoes_chamado_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId).HasDatabaseName("ix_aprovacoes_chamado_atualizado_por_usuario_id");
        builder.HasIndex(x => x.CanceladoPorUsuarioId).HasDatabaseName("ix_aprovacoes_chamado_cancelado_por_usuario_id");
        builder.HasIndex(x => new { x.ChamadoId, x.Ativo, x.Status })
            .HasDatabaseName("ix_aprovacoes_chamado_chamado_id_ativo_status");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.Aprovacoes)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Solicitante)
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Aprovador)
            .WithMany()
            .HasForeignKey(x => x.AprovadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CanceladoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CanceladoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AtualizadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
