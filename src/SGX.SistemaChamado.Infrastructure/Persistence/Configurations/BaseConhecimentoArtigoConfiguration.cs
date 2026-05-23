using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class BaseConhecimentoArtigoConfiguration : IEntityTypeConfiguration<BaseConhecimentoArtigo>
{
    public void Configure(EntityTypeBuilder<BaseConhecimentoArtigo> builder)
    {
        builder.ToTable("base_conhecimento_artigos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(220).IsRequired();
        builder.Property(x => x.Resumo).HasColumnName("resumo").HasMaxLength(1200);
        builder.Property(x => x.Conteudo).HasColumnName("conteudo").HasMaxLength(20000).IsRequired();
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.Visibilidade).HasColumnName("visibilidade").HasConversion<int>().IsRequired();
        builder.Property(x => x.Tags).HasColumnName("tags").HasMaxLength(1200);
        builder.Property(x => x.PublicadoEm).HasColumnName("publicado_em");
        builder.Property(x => x.PublicadoPorUsuarioId).HasColumnName("publicado_por_usuario_id");
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.ArquivadoEm).HasColumnName("arquivado_em");
        builder.Property(x => x.ArquivadoPorUsuarioId).HasColumnName("arquivado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("ux_base_conhecimento_artigos_slug");
        builder.HasIndex(x => x.CategoriaId).HasDatabaseName("ix_base_conhecimento_artigos_categoria_id");
        builder.HasIndex(x => new { x.Status, x.Ativo }).HasDatabaseName("ix_base_conhecimento_artigos_status_ativo");
        builder.HasIndex(x => x.PublicadoEm).HasDatabaseName("ix_base_conhecimento_artigos_publicado_em");

        builder.HasOne(x => x.Categoria)
            .WithMany()
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.PublicadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.ArquivadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ChamadosVinculados)
            .WithOne(x => x.Artigo)
            .HasForeignKey(x => x.ArtigoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
