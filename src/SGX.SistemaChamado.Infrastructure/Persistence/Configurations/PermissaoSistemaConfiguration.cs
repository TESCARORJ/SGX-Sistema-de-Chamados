using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class PermissaoSistemaConfiguration : IEntityTypeConfiguration<PermissaoSistema>
{
    public void Configure(EntityTypeBuilder<PermissaoSistema> builder)
    {
        builder.ToTable("permissoes_sistema");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(160).IsRequired();
        builder.Property(x => x.Modulo).HasColumnName("modulo").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Acao).HasColumnName("acao").HasMaxLength(80).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("ux_permissoes_sistema_codigo");
        builder.HasIndex(x => new { x.Modulo, x.Acao }).IsUnique().HasDatabaseName("ux_permissoes_sistema_modulo_acao");

        builder.HasData(SeedData.PermissoesSistema);
    }
}
