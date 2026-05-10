using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class LogIntegracaoEmailConfiguration : IEntityTypeConfiguration<LogIntegracaoEmail>
{
    public void Configure(EntityTypeBuilder<LogIntegracaoEmail> builder)
    {
        builder.ToTable("logs_integracao_email");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.MessageId).HasColumnName("message_id").HasMaxLength(600);
        builder.Property(x => x.InReplyTo).HasColumnName("in_reply_to").HasMaxLength(600);
        builder.Property(x => x.References).HasColumnName("references").HasMaxLength(4000);
        builder.Property(x => x.Fingerprint).HasColumnName("fingerprint").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Remetente).HasColumnName("remetente").HasMaxLength(320).IsRequired();
        builder.Property(x => x.Destinatario).HasColumnName("destinatario").HasMaxLength(1200);
        builder.Property(x => x.NomeRemetente).HasColumnName("nome_remetente").HasMaxLength(180);
        builder.Property(x => x.Assunto).HasColumnName("assunto").HasMaxLength(600);
        builder.Property(x => x.DataRecebimento).HasColumnName("data_recebimento").IsRequired();
        builder.Property(x => x.DataProcessamento).HasColumnName("data_processamento");
        builder.Property(x => x.StatusProcessamento).HasColumnName("status_processamento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Erro).HasColumnName("erro").HasMaxLength(8000);
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id");
        builder.Property(x => x.Tentativas).HasColumnName("tentativas").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.MessageId)
            .IsUnique()
            .HasFilter("\"message_id\" IS NOT NULL")
            .HasDatabaseName("ux_logs_integracao_email_message_id");

        builder.HasIndex(x => x.Fingerprint)
            .IsUnique()
            .HasDatabaseName("ux_logs_integracao_email_fingerprint");

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_logs_integracao_email_chamado_id");
        builder.HasIndex(x => x.DataRecebimento).HasDatabaseName("ix_logs_integracao_email_data_recebimento");
        builder.HasIndex(x => x.StatusProcessamento).HasDatabaseName("ix_logs_integracao_email_status");

        builder.HasOne(x => x.Chamado)
            .WithMany()
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
