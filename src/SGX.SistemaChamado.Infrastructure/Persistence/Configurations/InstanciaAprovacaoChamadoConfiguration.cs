using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class InstanciaAprovacaoChamadoConfiguration : IEntityTypeConfiguration<InstanciaAprovacaoChamado>
{
    public void Configure(EntityTypeBuilder<InstanciaAprovacaoChamado> builder)
    {
        builder.ToTable("instancias_aprovacao_chamado", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_subcategoria_categoria",
                "subcategoria_id IS NULL OR categoria_id IS NOT NULL");
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_custo_avaliado",
                "custo_avaliado IS NULL OR custo_avaliado >= 0");
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_nivel_risco_avaliado",
                "nivel_risco_avaliado IS NULL OR nivel_risco_avaliado > 0");
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_prazo_decisao",
                "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_regra_versao_snapshot",
                "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
            tableBuilder.HasCheckConstraint(
                "ck_instancias_aprovacao_chamado_expiracao_planejada",
                "deve_expirar_em IS NULL OR deve_expirar_em >= solicitada_em");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.ConfiguracaoRegraAprovacaoId).HasColumnName("configuracao_regra_aprovacao_id");
        builder.Property(x => x.AprovacaoChamadoLegadaId).HasColumnName("aprovacao_chamado_legada_id");
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.Origem).HasColumnName("origem").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoFluxoAprovacao).HasColumnName("tipo_fluxo_aprovacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.EfeitoOperacional).HasColumnName("efeito_operacional").HasConversion<int>().IsRequired();
        builder.Property(x => x.EscopoRegra).HasColumnName("escopo_regra").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoRegra).HasColumnName("tipo_regra").HasConversion<int>().IsRequired();
        builder.Property(x => x.NaturezaChamado).HasColumnName("natureza_chamado").HasConversion<int?>();
        builder.Property(x => x.TipoSolicitacaoId).HasColumnName("tipo_solicitacao_id");
        builder.Property(x => x.CatalogoServicoId).HasColumnName("catalogo_servico_id");
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.SubcategoriaId).HasColumnName("subcategoria_id");
        builder.Property(x => x.ImpactoAvaliado).HasColumnName("impacto_avaliado").HasConversion<int?>();
        builder.Property(x => x.UrgenciaAvaliada).HasColumnName("urgencia_avaliada").HasConversion<int?>();
        builder.Property(x => x.PrioridadeAvaliada).HasColumnName("prioridade_avaliada").HasConversion<int?>();
        builder.Property(x => x.CustoAvaliado).HasColumnName("custo_avaliado").HasColumnType("numeric(18,2)");
        builder.Property(x => x.NivelRiscoAvaliado).HasColumnName("nivel_risco_avaliado");
        builder.Property(x => x.ExigeAprovacao).HasColumnName("exige_aprovacao").IsRequired();
        builder.Property(x => x.Bloqueante).HasColumnName("bloqueante").IsRequired();
        builder.Property(x => x.PermiteReenvio).HasColumnName("permite_reenvio").IsRequired();
        builder.Property(x => x.PermiteFallback).HasColumnName("permite_fallback").IsRequired();
        builder.Property(x => x.TipoResolucaoAprovador).HasColumnName("tipo_resolucao_aprovador").HasConversion<int>().IsRequired();
        builder.Property(x => x.AprovadorEspecificoUsuarioId).HasColumnName("aprovador_especifico_usuario_id");
        builder.Property(x => x.AprovadorPadraoUsuarioId).HasColumnName("aprovador_padrao_usuario_id");
        builder.Property(x => x.AprovadorResolvidoUsuarioId).HasColumnName("aprovador_resolvido_usuario_id");
        builder.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
        builder.Property(x => x.SolicitadaEm).HasColumnName("solicitada_em").IsRequired();
        builder.Property(x => x.PrazoDecisaoHoras).HasColumnName("prazo_decisao_horas");
        builder.Property(x => x.DeveExpirarEm).HasColumnName("deve_expirar_em");
        builder.Property(x => x.ExpiradaEm).HasColumnName("expirada_em");
        builder.Property(x => x.CanceladaEm).HasColumnName("cancelada_em");
        builder.Property(x => x.CanceladaPorUsuarioId).HasColumnName("cancelada_por_usuario_id");
        builder.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(1000);
        builder.Property(x => x.DecididaEm).HasColumnName("decidida_em");
        builder.Property(x => x.RegraNomeSnapshot).HasColumnName("regra_nome_snapshot").HasMaxLength(180);
        builder.Property(x => x.RegraVersaoSnapshot).HasColumnName("regra_versao_snapshot");
        builder.Property(x => x.RegraCriterioSnapshot).HasColumnName("regra_criterio_snapshot").HasMaxLength(4000);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("ix_instancias_aprovacao_chamado_chamado_id");
        builder.HasIndex(x => x.ConfiguracaoRegraAprovacaoId).HasDatabaseName("ix_instancias_aprovacao_chamado_configuracao_regra_aprovacao_id");
        builder.HasIndex(x => x.AprovacaoChamadoLegadaId).IsUnique().HasDatabaseName("ux_instancias_aprovacao_chamado_aprovacao_chamado_legada_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_instancias_aprovacao_chamado_status");
        builder.HasIndex(x => x.Origem).HasDatabaseName("ix_instancias_aprovacao_chamado_origem");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_instancias_aprovacao_chamado_ativo");
        builder.HasIndex(x => x.SolicitadaEm).HasDatabaseName("ix_instancias_aprovacao_chamado_solicitada_em");
        builder.HasIndex(x => x.DeveExpirarEm).HasDatabaseName("ix_instancias_aprovacao_chamado_deve_expirar_em");
        builder.HasIndex(x => new { x.ChamadoId, x.Ativo, x.Status }).HasDatabaseName("ix_instancias_aprovacao_chamado_chamado_id_ativo_status");
        builder.HasIndex(x => x.SolicitanteId).HasDatabaseName("ix_instancias_aprovacao_chamado_solicitante_id");
        builder.HasIndex(x => x.AprovadorResolvidoUsuarioId).HasDatabaseName("ix_instancias_aprovacao_chamado_aprovador_resolvido_usuario_id");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_instancias_aprovacao_chamado_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId).HasDatabaseName("ix_instancias_aprovacao_chamado_atualizado_por_usuario_id");
        builder.HasIndex(x => x.CanceladaPorUsuarioId).HasDatabaseName("ix_instancias_aprovacao_chamado_cancelada_por_usuario_id");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.InstanciasAprovacao)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ConfiguracaoRegraAprovacao)
            .WithMany(x => x.InstanciasAprovacao)
            .HasForeignKey(x => x.ConfiguracaoRegraAprovacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovacaoChamadoLegada)
            .WithMany()
            .HasForeignKey(x => x.AprovacaoChamadoLegadaId)
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.HasOne(x => x.Solicitante)
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovadorEspecificoUsuario)
            .WithMany()
            .HasForeignKey(x => x.AprovadorEspecificoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovadorPadraoUsuario)
            .WithMany()
            .HasForeignKey(x => x.AprovadorPadraoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AprovadorResolvidoUsuario)
            .WithMany()
            .HasForeignKey(x => x.AprovadorResolvidoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CanceladaPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CanceladaPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AtualizadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
