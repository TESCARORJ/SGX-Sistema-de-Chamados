using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoArtigoConhecimentoConfiguration : IEntityTypeConfiguration<ChamadoArtigoConhecimento>
{
    public void Configure(EntityTypeBuilder<ChamadoArtigoConhecimento> builder)
    {
        builder.ToTable("chamados_artigos_conhecimento");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.ArtigoId).HasColumnName("artigo_id").IsRequired();
        builder.Property(x => x.VinculadoEm).HasColumnName("vinculado_em").IsRequired();
        builder.Property(x => x.VinculadoPorUsuarioId).HasColumnName("vinculado_por_usuario_id").IsRequired();
        builder.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(2000);
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_chamados_artigos_conhecimento_chamado_id");
        builder.HasIndex(x => x.ArtigoId).HasDatabaseName("ix_chamados_artigos_conhecimento_artigo_id");
        builder.HasIndex(x => new { x.ChamadoId, x.ArtigoId }).IsUnique().HasDatabaseName("ux_chamados_artigos_conhecimento_chamado_artigo");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.ArtigosConhecimento)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Artigo)
            .WithMany(x => x.ChamadosVinculados)
            .HasForeignKey(x => x.ArtigoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VinculadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.VinculadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
