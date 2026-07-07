using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class FormularioServicoConfiguration : IEntityTypeConfiguration<FormularioServico>
{
    public void Configure(EntityTypeBuilder<FormularioServico> builder)
    {
        builder.ToTable("formularios_servico");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CatalogoServicoId).HasColumnName("catalogo_servico_id").IsRequired();
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.CatalogoServicoId)
            .IsUnique()
            .HasDatabaseName("ux_formularios_servico_catalogo_servico_id");

        builder.HasIndex(x => x.Ativo)
            .HasDatabaseName("ix_formularios_servico_ativo");

        builder.HasOne(x => x.CatalogoServico)
            .WithOne(x => x.FormularioServico)
            .HasForeignKey<FormularioServico>(x => x.CatalogoServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
