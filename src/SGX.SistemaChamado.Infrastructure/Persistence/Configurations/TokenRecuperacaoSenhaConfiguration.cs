using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class TokenRecuperacaoSenhaConfiguration : IEntityTypeConfiguration<TokenRecuperacaoSenha>
{
    public void Configure(EntityTypeBuilder<TokenRecuperacaoSenha> builder)
    {
        builder.ToTable("tokens_recuperacao_senha");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(128).IsRequired();
        builder.Property(x => x.ExpiraEm).HasColumnName("expira_em").IsRequired();
        builder.Property(x => x.UtilizadoEm).HasColumnName("utilizado_em");
        builder.Property(x => x.IpSolicitacao).HasColumnName("ip_solicitacao").HasMaxLength(64);
        builder.Property(x => x.UserAgentSolicitacao).HasColumnName("user_agent_solicitacao").HasMaxLength(512);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.TokenHash).HasDatabaseName("ix_tokens_recuperacao_senha_token_hash");
        builder.HasIndex(x => x.UsuarioId).HasDatabaseName("ix_tokens_recuperacao_senha_usuario_id");
        builder.HasIndex(x => x.ExpiraEm).HasDatabaseName("ix_tokens_recuperacao_senha_expira_em");

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
