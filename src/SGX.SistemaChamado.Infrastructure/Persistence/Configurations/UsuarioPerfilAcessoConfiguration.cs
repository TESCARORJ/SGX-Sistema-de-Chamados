using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class UsuarioPerfilAcessoConfiguration : IEntityTypeConfiguration<UsuarioPerfilAcesso>
{
    public void Configure(EntityTypeBuilder<UsuarioPerfilAcesso> builder)
    {
        builder.ToTable("usuarios_perfis_acesso");
        builder.HasKey(x => new { x.UsuarioId, x.PerfilAcessoId });

        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.PerfilAcessoId).HasColumnName("perfil_acesso_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasOne(x => x.Usuario)
            .WithMany(x => x.UsuarioPerfis)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PerfilAcesso)
            .WithMany(x => x.UsuarioPerfis)
            .HasForeignKey(x => x.PerfilAcessoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
