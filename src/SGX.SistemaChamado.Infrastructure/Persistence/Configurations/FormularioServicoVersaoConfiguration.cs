using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class FormularioServicoVersaoConfiguration : IEntityTypeConfiguration<FormularioServicoVersao>
{
    public void Configure(EntityTypeBuilder<FormularioServicoVersao> builder)
    {
        builder.ToTable("formularios_servico_versoes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.FormularioServicoId).HasColumnName("formulario_servico_id").IsRequired();
        builder.Property(x => x.Numero).HasColumnName("numero").IsRequired();
        builder.Property(x => x.Publicada).HasColumnName("publicada").IsRequired();
        builder.Property(x => x.PublicadoEm).HasColumnName("publicado_em");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.FormularioServicoId)
            .HasDatabaseName("ix_form_serv_versao_form");

        builder.HasIndex(x => new { x.FormularioServicoId, x.Numero })
            .IsUnique()
            .HasDatabaseName("ux_form_serv_versao_num");

        builder.HasIndex(x => x.Publicada)
            .HasDatabaseName("ix_form_serv_versao_pub");

        builder.HasIndex(x => x.Ativo)
            .HasDatabaseName("ix_form_serv_versao_ativo");

        builder.HasOne(x => x.FormularioServico)
            .WithMany(x => x.Versoes)
            .HasForeignKey(x => x.FormularioServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
