using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
{
    private const int MaximoDestinatarioEndereco = 320;
    private const int MaximoAssunto = 300;
    private const int MaximoConteudo = 10000;
    private const int MaximoChaveCorrelacao = 200;
    private const int MaximoChaveIdempotencia = 200;
    private const int MaximoUltimoErro = 2000;
    private const int MaximoMotivoCancelamento = 1000;

    public void Configure(EntityTypeBuilder<Notificacao> builder)
    {
        builder.ToTable("notificacoes", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_notificacoes_destinatario",
                "destinatario_usuario_id IS NOT NULL OR destinatario_endereco IS NOT NULL");
            tableBuilder.HasCheckConstraint(
                "ck_notificacoes_quantidade_tentativas_nao_negativa",
                "quantidade_tentativas >= 0");
            tableBuilder.HasCheckConstraint(
                "ck_notificacoes_lida_em_maior_ou_igual_enviada_em",
                "lida_em IS NULL OR enviada_em IS NULL OR lida_em >= enviada_em");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id");
        builder.Property(x => x.TipoEvento).HasColumnName("tipo_evento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Canal).HasColumnName("canal").HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.DestinatarioUsuarioId).HasColumnName("destinatario_usuario_id");
        builder.Property(x => x.DestinatarioEndereco).HasColumnName("destinatario_endereco").HasMaxLength(MaximoDestinatarioEndereco);
        builder.Property(x => x.Assunto).HasColumnName("assunto").HasMaxLength(MaximoAssunto);
        builder.Property(x => x.Conteudo).HasColumnName("conteudo").HasMaxLength(MaximoConteudo).IsRequired();
        builder.Property(x => x.ChaveCorrelacao).HasColumnName("chave_correlacao").HasMaxLength(MaximoChaveCorrelacao);
        builder.Property(x => x.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(MaximoChaveIdempotencia).IsRequired();
        builder.Property(x => x.AgendadaEm).HasColumnName("agendada_em");
        builder.Property(x => x.ProcessadaEm).HasColumnName("processada_em");
        builder.Property(x => x.EnviadaEm).HasColumnName("enviada_em");
        builder.Property(x => x.LidaEm).HasColumnName("lida_em");
        builder.Property(x => x.FalhouEm).HasColumnName("falhou_em");
        builder.Property(x => x.CanceladaEm).HasColumnName("cancelada_em");
        builder.Property(x => x.QuantidadeTentativas).HasColumnName("quantidade_tentativas").IsRequired();
        builder.Property(x => x.UltimoErro).HasColumnName("ultimo_erro").HasMaxLength(MaximoUltimoErro);
        builder.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(MaximoMotivoCancelamento);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id");
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_notificacoes_chamado_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_notificacoes_status");
        builder.HasIndex(x => x.Canal).HasDatabaseName("ix_notificacoes_canal");
        builder.HasIndex(x => x.AgendadaEm).HasDatabaseName("ix_notificacoes_agendada_em");
        builder.HasIndex(x => x.DestinatarioUsuarioId).HasDatabaseName("ix_notificacoes_destinatario_usuario_id");
        builder.HasIndex(x => x.CriadoEm).HasDatabaseName("ix_notificacoes_criado_em");
        builder.HasIndex(x => x.ChaveIdempotencia).IsUnique().HasDatabaseName("ux_notificacoes_chave_idempotencia");
        builder.HasIndex(x => new { x.Status, x.AgendadaEm }).HasDatabaseName("ix_notificacoes_status_agendada_em");
        builder.HasIndex(x => new { x.DestinatarioUsuarioId, x.Canal, x.Status, x.CriadoEm })
            .HasDatabaseName("ix_notificacoes_destinatario_canal_status_criado_em");

        builder.HasOne(x => x.Chamado)
            .WithMany()
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DestinatarioUsuario)
            .WithMany()
            .HasForeignKey(x => x.DestinatarioUsuarioId)
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
