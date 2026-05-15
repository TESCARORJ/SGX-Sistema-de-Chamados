using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class EventoAuditoriaConfiguration : IEntityTypeConfiguration<EventoAuditoria>
{
    public void Configure(EntityTypeBuilder<EventoAuditoria> builder)
    {
        builder.ToTable("eventos_auditoria");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DataEvento).HasColumnName("data_evento").IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.UsuarioNome).HasColumnName("usuario_nome").HasMaxLength(180);
        builder.Property(x => x.UsuarioEmail).HasColumnName("usuario_email").HasMaxLength(320);
        builder.Property(x => x.UsuarioLogin).HasColumnName("usuario_login").HasMaxLength(180);
        builder.Property(x => x.IpOrigem).HasColumnName("ip_origem").HasMaxLength(80);
        builder.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(1200);
        builder.Property(x => x.Modulo).HasColumnName("modulo").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Entidade).HasColumnName("entidade").HasMaxLength(120).IsRequired();
        builder.Property(x => x.EntidadeId).HasColumnName("entidade_id").HasMaxLength(120);
        builder.Property(x => x.Acao)
            .HasColumnName("acao")
            .HasConversion<string>()
            .HasMaxLength(60)
            .IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(2000).IsRequired();
        builder.Property(x => x.DadosAntes).HasColumnName("dados_antes").HasColumnType("text");
        builder.Property(x => x.DadosDepois).HasColumnName("dados_depois").HasColumnType("text");
        builder.Property(x => x.Metadados).HasColumnName("metadados").HasColumnType("text");
        builder.Property(x => x.Nivel)
            .HasColumnName("nivel")
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();
        builder.Property(x => x.Sucesso).HasColumnName("sucesso").IsRequired();
        builder.Property(x => x.MensagemErro).HasColumnName("mensagem_erro").HasMaxLength(4000);
        builder.Property(x => x.CorrelacaoId).HasColumnName("correlacao_id").HasMaxLength(120);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();

        builder.HasIndex(x => x.DataEvento).HasDatabaseName("ix_eventos_auditoria_data_evento");
        builder.HasIndex(x => x.UsuarioId).HasDatabaseName("ix_eventos_auditoria_usuario_id");
        builder.HasIndex(x => x.UsuarioEmail).HasDatabaseName("ix_eventos_auditoria_usuario_email");
        builder.HasIndex(x => x.Modulo).HasDatabaseName("ix_eventos_auditoria_modulo");
        builder.HasIndex(x => x.Entidade).HasDatabaseName("ix_eventos_auditoria_entidade");
        builder.HasIndex(x => x.EntidadeId).HasDatabaseName("ix_eventos_auditoria_entidade_id");
        builder.HasIndex(x => x.Acao).HasDatabaseName("ix_eventos_auditoria_acao");
        builder.HasIndex(x => x.CorrelacaoId).HasDatabaseName("ix_eventos_auditoria_correlacao_id");
    }
}
