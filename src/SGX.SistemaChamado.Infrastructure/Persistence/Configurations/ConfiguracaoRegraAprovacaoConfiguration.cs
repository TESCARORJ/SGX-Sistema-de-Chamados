using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ConfiguracaoRegraAprovacaoConfiguration : IEntityTypeConfiguration<ConfiguracaoRegraAprovacao>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoRegraAprovacao> builder)
    {
        builder.ToTable("configuracoes_regras_aprovacao", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_configuracoes_regras_aprovacao_vigencia",
                "vigente_ate IS NULL OR vigente_de IS NULL OR vigente_ate >= vigente_de");
            tableBuilder.HasCheckConstraint(
                "ck_configuracoes_regras_aprovacao_subcategoria_categoria",
                "subcategoria_id IS NULL OR categoria_id IS NOT NULL");
            tableBuilder.HasCheckConstraint(
                "ck_configuracoes_regras_aprovacao_custo_minimo",
                "custo_minimo IS NULL OR custo_minimo >= 0");
            tableBuilder.HasCheckConstraint(
                "ck_configuracoes_regras_aprovacao_nivel_risco",
                "nivel_risco_minimo IS NULL OR nivel_risco_minimo > 0");
            tableBuilder.HasCheckConstraint(
                "ck_configuracoes_regras_aprovacao_prazo_decisao",
                "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.TipoRegra).HasColumnName("tipo_regra").HasConversion<int>().IsRequired();
        builder.Property(x => x.EscopoRegra).HasColumnName("escopo_regra").HasConversion<int>().IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.Prioridade).HasColumnName("prioridade").IsRequired();
        builder.Property(x => x.Versao).HasColumnName("versao").IsRequired();
        builder.Property(x => x.NaturezaChamado).HasColumnName("natureza_chamado").HasConversion<int?>();
        builder.Property(x => x.TipoSolicitacaoId).HasColumnName("tipo_solicitacao_id");
        builder.Property(x => x.CatalogoServicoId).HasColumnName("catalogo_servico_id");
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.SubcategoriaId).HasColumnName("subcategoria_id");
        builder.Property(x => x.ImpactoMinimo).HasColumnName("impacto_minimo").HasConversion<int?>();
        builder.Property(x => x.UrgenciaMinima).HasColumnName("urgencia_minima").HasConversion<int?>();
        builder.Property(x => x.PrioridadeMinima).HasColumnName("prioridade_minima").HasConversion<int?>();
        builder.Property(x => x.CustoMinimo).HasColumnName("custo_minimo").HasColumnType("numeric(18,2)");
        builder.Property(x => x.NivelRiscoMinimo).HasColumnName("nivel_risco_minimo");
        builder.Property(x => x.ExigeAprovacao).HasColumnName("exige_aprovacao").IsRequired();
        builder.Property(x => x.Bloqueante).HasColumnName("bloqueante").IsRequired();
        builder.Property(x => x.PermiteReenvio).HasColumnName("permite_reenvio").IsRequired();
        builder.Property(x => x.PermiteFallback).HasColumnName("permite_fallback").IsRequired();
        builder.Property(x => x.EfeitoOperacional).HasColumnName("efeito_operacional").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoFluxoAprovacao).HasColumnName("tipo_fluxo_aprovacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoResolucaoAprovador).HasColumnName("tipo_resolucao_aprovador").HasConversion<int>().IsRequired();
        builder.Property(x => x.AprovadorEspecificoUsuarioId).HasColumnName("aprovador_especifico_usuario_id");
        builder.Property(x => x.AprovadorPadraoUsuarioId).HasColumnName("aprovador_padrao_usuario_id");
        builder.Property(x => x.PrazoDecisaoHoras).HasColumnName("prazo_decisao_horas");
        builder.Property(x => x.VigenteDe).HasColumnName("vigente_de");
        builder.Property(x => x.VigenteAte).HasColumnName("vigente_ate");
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => new { x.Nome, x.Versao })
            .IsUnique()
            .HasDatabaseName("ux_configuracoes_regras_aprovacao_nome_versao");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_configuracoes_regras_aprovacao_ativo");
        builder.HasIndex(x => new { x.Ativo, x.EscopoRegra, x.Ordem, x.Prioridade })
            .HasDatabaseName("ix_configuracoes_regras_aprovacao_escopo_ordem_prioridade");
        builder.HasIndex(x => x.TipoRegra).HasDatabaseName("ix_configuracoes_regras_aprovacao_tipo_regra");
        builder.HasIndex(x => x.NaturezaChamado).HasDatabaseName("ix_configuracoes_regras_aprovacao_natureza");
        builder.HasIndex(x => x.TipoSolicitacaoId).HasDatabaseName("ix_configuracoes_regras_aprovacao_tipo_solicitacao_id");
        builder.HasIndex(x => x.CatalogoServicoId).HasDatabaseName("ix_configuracoes_regras_aprovacao_catalogo_servico_id");
        builder.HasIndex(x => x.CategoriaId).HasDatabaseName("ix_configuracoes_regras_aprovacao_categoria_id");
        builder.HasIndex(x => x.SubcategoriaId).HasDatabaseName("ix_configuracoes_regras_aprovacao_subcategoria_id");
        builder.HasIndex(x => x.ImpactoMinimo).HasDatabaseName("ix_configuracoes_regras_aprovacao_impacto_minimo");
        builder.HasIndex(x => x.UrgenciaMinima).HasDatabaseName("ix_configuracoes_regras_aprovacao_urgencia_minima");
        builder.HasIndex(x => x.PrioridadeMinima).HasDatabaseName("ix_configuracoes_regras_aprovacao_prioridade_minima");
        builder.HasIndex(x => new { x.VigenteDe, x.VigenteAte }).HasDatabaseName("ix_configuracoes_regras_aprovacao_vigencia");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_configuracoes_regras_aprovacao_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId).HasDatabaseName("ix_configuracoes_regras_aprovacao_atualizado_por_usuario_id");
        builder.HasIndex(x => x.AprovadorEspecificoUsuarioId).HasDatabaseName("ix_configuracoes_regras_aprovacao_aprovador_especifico_usuario_id");
        builder.HasIndex(x => x.AprovadorPadraoUsuarioId).HasDatabaseName("ix_configuracoes_regras_aprovacao_aprovador_padrao_usuario_id");

        builder.HasOne(x => x.TipoSolicitacao)
            .WithMany()
            .HasForeignKey(x => x.TipoSolicitacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CatalogoServico)
            .WithMany()
            .HasForeignKey(x => x.CatalogoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Categoria)
            .WithMany()
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subcategoria)
            .WithMany()
            .HasForeignKey(x => x.SubcategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AtualizadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovadorEspecificoUsuario)
            .WithMany()
            .HasForeignKey(x => x.AprovadorEspecificoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovadorPadraoUsuario)
            .WithMany()
            .HasForeignKey(x => x.AprovadorPadraoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
