using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
        builder.Property(x => x.Login).HasColumnName("login").HasMaxLength(120).IsRequired();
        builder.Property(x => x.SenhaHashLocal).HasColumnName("senha_hash_local").HasMaxLength(1024);
        builder.Property(x => x.DeveAlterarSenha).HasColumnName("deve_alterar_senha").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.UltimaAlteracaoSenhaEm).HasColumnName("ultima_alteracao_senha_em");
        builder.Property(x => x.TentativasInvalidas).HasColumnName("tentativas_invalidas").IsRequired().HasDefaultValue(0);
        builder.Property(x => x.BloqueadoAte).HasColumnName("bloqueado_ate");
        builder.Property(x => x.UltimoLoginEm).HasColumnName("ultimo_login_em");
        builder.Property(x => x.Situacao).HasColumnName("situacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.UltimoAcessoEm).HasColumnName("ultimo_acesso_em");
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("ux_usuarios_email");
        builder.HasIndex(x => x.Login).IsUnique().HasDatabaseName("ux_usuarios_login");

        builder.HasOne(x => x.Departamento)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
