using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class RespostaFormularioChamadoConfiguration : IEntityTypeConfiguration<RespostaFormularioChamado>
{
    private const int TamanhoMaximoValoresJson = 16000;

    public void Configure(EntityTypeBuilder<RespostaFormularioChamado> builder)
    {
        builder.ToTable("respostas_formulario_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.FormularioServicoVersaoId).HasColumnName("formulario_servico_versao_id").IsRequired();
        builder.Property(x => x.CampoFormularioServicoId).HasColumnName("campo_formulario_servico_id").IsRequired();
        builder.Property(x => x.Valor).HasColumnName("valor").HasMaxLength(RespostaFormularioChamado.TamanhoMaximoValor);
        builder.Property(x => x.ValoresJson).HasColumnName("valores_json").HasMaxLength(TamanhoMaximoValoresJson);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId)
            .HasDatabaseName("ix_resp_form_chamado");

        builder.HasIndex(x => x.FormularioServicoVersaoId)
            .HasDatabaseName("ix_resp_form_versao");

        builder.HasIndex(x => x.CampoFormularioServicoId)
            .HasDatabaseName("ix_resp_form_campo");

        builder.HasIndex(x => new { x.ChamadoId, x.CampoFormularioServicoId })
            .IsUnique()
            .HasDatabaseName("ux_resp_form_chamado_cmp");

        builder.HasIndex(x => new { x.ChamadoId, x.FormularioServicoVersaoId })
            .HasDatabaseName("ix_resp_form_chamado_ver");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.RespostasFormulario)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FormularioServicoVersao)
            .WithMany()
            .HasForeignKey(x => x.FormularioServicoVersaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CampoFormularioServico)
            .WithMany()
            .HasForeignKey(x => x.CampoFormularioServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
