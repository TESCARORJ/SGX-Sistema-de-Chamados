using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class PerfilAcessoPermissaoConfiguration : IEntityTypeConfiguration<PerfilAcessoPermissao>
{
    public void Configure(EntityTypeBuilder<PerfilAcessoPermissao> builder)
    {
        builder.ToTable("perfis_acesso_permissoes");
        builder.HasKey(x => new { x.PerfilAcessoId, x.PermissaoSistemaId });

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.PerfilAcessoId).HasColumnName("perfil_acesso_id");
        builder.Property(x => x.PermissaoSistemaId).HasColumnName("permissao_sistema_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasOne(x => x.PerfilAcesso)
            .WithMany(x => x.PerfilPermissoes)
            .HasForeignKey(x => x.PerfilAcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PermissaoSistema)
            .WithMany(x => x.PerfilPermissoes)
            .HasForeignKey(x => x.PermissaoSistemaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.PerfisAcessoPermissoes);
    }
}
