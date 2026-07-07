using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class CatalogoServicoConfiguration : IEntityTypeConfiguration<CatalogoServico>
{
    public void Configure(EntityTypeBuilder<CatalogoServico> builder)
    {
        builder.ToTable("catalogo_servicos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(220).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.InstrucoesSolicitante).HasColumnName("instrucoes_solicitante").HasMaxLength(8000);
        builder.Property(x => x.DepartamentoResponsavelId).HasColumnName("departamento_responsavel_id").IsRequired();
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.SubcategoriaId).HasColumnName("subcategoria_id");
        builder.Property(x => x.PrioridadePadraoId).HasColumnName("prioridade_padrao_id");
        builder.Property(x => x.SlaPadraoId).HasColumnName("sla_padrao_id");
        builder.Property(x => x.ArtigoBaseConhecimentoId).HasColumnName("artigo_base_conhecimento_id");
        builder.Property(x => x.GrupoTecnicoId).HasColumnName("grupo_tecnico_id");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.Visibilidade).HasColumnName("visibilidade").HasConversion<int>().IsRequired();
        builder.Property(x => x.PermiteAberturaChamado).HasColumnName("permite_abertura_chamado").IsRequired();
        builder.Property(x => x.RequerAprovacao).HasColumnName("requer_aprovacao").IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
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

        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("ux_catalogo_servicos_slug");
        builder.HasIndex(x => x.DepartamentoResponsavelId).HasDatabaseName("ix_catalogo_servicos_departamento_responsavel_id");
        builder.HasIndex(x => x.GrupoTecnicoId).HasDatabaseName("ix_catalogo_servicos_grupo_tecnico_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_catalogo_servicos_status");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_catalogo_servicos_ativo");
        builder.HasIndex(x => new { x.Status, x.Ativo }).HasDatabaseName("ix_catalogo_servicos_status_ativo");

        builder.HasOne(x => x.DepartamentoResponsavel)
            .WithMany()
            .HasForeignKey(x => x.DepartamentoResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Categoria)
            .WithMany()
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subcategoria)
            .WithMany()
            .HasForeignKey(x => x.SubcategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PrioridadePadrao)
            .WithMany()
            .HasForeignKey(x => x.PrioridadePadraoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SlaPadrao)
            .WithMany()
            .HasForeignKey(x => x.SlaPadraoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ArtigoBaseConhecimento)
            .WithMany()
            .HasForeignKey(x => x.ArtigoBaseConhecimentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.GrupoTecnico)
            .WithMany()
            .HasForeignKey(x => x.GrupoTecnicoId)
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
    }
}
