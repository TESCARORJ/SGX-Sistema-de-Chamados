using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class CampoFormularioServicoConfiguration : IEntityTypeConfiguration<CampoFormularioServico>
{
    public void Configure(EntityTypeBuilder<CampoFormularioServico> builder)
    {
        builder.ToTable("campos_formulario_servico");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.FormularioServicoVersaoId).HasColumnName("formulario_servico_versao_id").IsRequired();
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Rotulo).HasColumnName("rotulo").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Obrigatorio).HasColumnName("obrigatorio").IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.TextoAjuda).HasColumnName("texto_ajuda").HasMaxLength(500);
        builder.Property(x => x.Visivel).HasColumnName("visivel").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.FormularioServicoVersaoId)
            .HasDatabaseName("ix_campo_form_serv_versao");

        builder.HasIndex(x => new { x.FormularioServicoVersaoId, x.Nome })
            .IsUnique()
            .HasDatabaseName("ux_campo_form_serv_nome");

        builder.HasIndex(x => new { x.FormularioServicoVersaoId, x.Ordem })
            .IsUnique()
            .HasDatabaseName("ux_campo_form_serv_ordem");

        builder.HasIndex(x => x.Ativo)
            .HasDatabaseName("ix_campos_formulario_servico_ativo");

        builder.HasOne(x => x.FormularioServicoVersao)
            .WithMany(x => x.Campos)
            .HasForeignKey(x => x.FormularioServicoVersaoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
