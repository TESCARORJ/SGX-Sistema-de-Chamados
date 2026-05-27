using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoConfiguration : IEntityTypeConfiguration<Chamado>
{
    public void Configure(EntityTypeBuilder<Chamado> builder)
    {
        builder.ToTable("chamados");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(40).IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000).IsRequired();
        builder.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
        builder.Property(x => x.ResponsavelId).HasColumnName("responsavel_id");
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id").IsRequired();
        builder.Property(x => x.SubcategoriaId).HasColumnName("subcategoria_id");
        builder.Property(x => x.PrioridadeId).HasColumnName("prioridade_id").IsRequired();
        builder.Property(x => x.TipoSolicitacaoId).HasColumnName("tipo_solicitacao_id");
        builder.Property(x => x.LocalUnidadeId).HasColumnName("local_unidade_id");
        builder.Property(x => x.CatalogoServicoId).HasColumnName("catalogo_servico_id");
        builder.Property(x => x.InventarioAtivoId).HasColumnName("inventario_ativo_id");
        builder.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(x => x.Origem).HasColumnName("origem").HasConversion<int>().IsRequired();
        builder.Property(x => x.NaturezaChamado).HasColumnName("natureza_chamado").HasConversion<int>().IsRequired();
        builder.Property(x => x.ImpactoChamado).HasColumnName("impacto_chamado").HasConversion<int>().IsRequired();
        builder.Property(x => x.UrgenciaChamado).HasColumnName("urgencia_chamado").HasConversion<int>().IsRequired();
        builder.Property(x => x.AbertoEm).HasColumnName("aberto_em").IsRequired();
        builder.Property(x => x.EncerradoEm).HasColumnName("encerrado_em");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("ux_chamados_codigo");
        builder.HasIndex(x => x.CatalogoServicoId).HasDatabaseName("ix_chamados_catalogo_servico_id");
        builder.HasIndex(x => x.InventarioAtivoId).HasDatabaseName("ix_chamados_inventario_ativo_id");

        builder.HasOne(x => x.Solicitante)
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Responsavel)
            .WithMany()
            .HasForeignKey(x => x.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Departamento)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Categoria)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subcategoria)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.SubcategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Prioridade)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TipoSolicitacao)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.TipoSolicitacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LocalUnidade)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.LocalUnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CatalogoServico)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.CatalogoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InventarioAtivo)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.InventarioAtivoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Historicos)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Anexos)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Aprovacoes)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SlaControle)
            .WithOne(x => x.Chamado)
            .HasForeignKey<SlaControle>(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ChamadoSla)
            .WithOne(x => x.Chamado)
            .HasForeignKey<ChamadoSla>(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
