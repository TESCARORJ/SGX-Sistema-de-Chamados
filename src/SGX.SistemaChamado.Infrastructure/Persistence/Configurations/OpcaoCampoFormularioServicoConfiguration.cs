using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class OpcaoCampoFormularioServicoConfiguration : IEntityTypeConfiguration<OpcaoCampoFormularioServico>
{
    public void Configure(EntityTypeBuilder<OpcaoCampoFormularioServico> builder)
    {
        builder.ToTable("opcoes_campos_formulario_servico");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CampoFormularioServicoId).HasColumnName("campo_formulario_servico_id").IsRequired();
        builder.Property(x => x.Valor).HasColumnName("valor").HasMaxLength(120).IsRequired();
        builder.Property(x => x.Rotulo).HasColumnName("rotulo").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.CampoFormularioServicoId)
            .HasDatabaseName("ix_opcao_form_serv_campo");

        builder.HasIndex(x => new { x.CampoFormularioServicoId, x.Valor })
            .IsUnique()
            .HasDatabaseName("ux_opcao_form_serv_valor");

        builder.HasIndex(x => new { x.CampoFormularioServicoId, x.Ordem })
            .IsUnique()
            .HasDatabaseName("ux_opcao_form_serv_ordem");

        builder.HasIndex(x => x.Ativo)
            .HasDatabaseName("ix_opcao_form_serv_ativo");

        builder.HasOne(x => x.CampoFormularioServico)
            .WithMany(x => x.Opcoes)
            .HasForeignKey(x => x.CampoFormularioServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
