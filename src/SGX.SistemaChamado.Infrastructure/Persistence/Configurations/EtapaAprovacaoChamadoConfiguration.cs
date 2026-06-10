using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class EtapaAprovacaoChamadoConfiguration : IEntityTypeConfiguration<EtapaAprovacaoChamado>
{
    public void Configure(EntityTypeBuilder<EtapaAprovacaoChamado> builder)
    {
        builder.ToTable("etapas_aprovacao_chamado", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_ordem",
                "ordem >= 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_nivel",
                "nivel > 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_quorum_minimo",
                "quorum_minimo IS NULL OR quorum_minimo > 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_qtd_aprovacoes_necessarias",
                "quantidade_aprovacoes_necessarias IS NULL OR quantidade_aprovacoes_necessarias > 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_prazo_decisao",
                "prazo_decisao_horas IS NULL OR prazo_decisao_horas > 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_regra_versao_snapshot",
                "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
            tableBuilder.HasCheckConstraint(
                "ck_etapas_aprovacao_chamado_expiracao_planejada",
                "deve_expirar_em IS NULL OR deve_expirar_em >= solicitada_em");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.InstanciaAprovacaoChamadoId).HasColumnName("instancia_aprovacao_chamado_id").IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoEtapa).HasColumnName("tipo_etapa").HasConversion<int>().IsRequired();
        builder.Property(x => x.TipoFluxoAprovacao).HasColumnName("tipo_fluxo_aprovacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.Nivel).HasColumnName("nivel").IsRequired();
        builder.Property(x => x.Ramo).HasColumnName("ramo").HasMaxLength(80);
        builder.Property(x => x.Obrigatoria).HasColumnName("obrigatoria").IsRequired();
        builder.Property(x => x.CriticaParaConsolidacao).HasColumnName("critica_para_consolidacao").IsRequired();
        builder.Property(x => x.PermiteReenvio).HasColumnName("permite_reenvio").IsRequired();
        builder.Property(x => x.PermiteFallback).HasColumnName("permite_fallback").IsRequired();
        builder.Property(x => x.PermiteDelegacao).HasColumnName("permite_delegacao").IsRequired();
        builder.Property(x => x.TipoResolucaoAprovador).HasColumnName("tipo_resolucao_aprovador").HasConversion<int>().IsRequired();
        builder.Property(x => x.AprovadorEspecificoUsuarioId).HasColumnName("aprovador_especifico_usuario_id");
        builder.Property(x => x.AprovadorPadraoUsuarioId).HasColumnName("aprovador_padrao_usuario_id");
        builder.Property(x => x.AprovadorResolvidoUsuarioId).HasColumnName("aprovador_resolvido_usuario_id");
        builder.Property(x => x.GrupoAprovadorSnapshot).HasColumnName("grupo_aprovador_snapshot").HasMaxLength(180);
        builder.Property(x => x.QuorumMinimo).HasColumnName("quorum_minimo");
        builder.Property(x => x.QuantidadeAprovacoesNecessarias).HasColumnName("quantidade_aprovacoes_necessarias");
        builder.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
        builder.Property(x => x.SolicitadaEm).HasColumnName("solicitada_em").IsRequired();
        builder.Property(x => x.PrazoDecisaoHoras).HasColumnName("prazo_decisao_horas");
        builder.Property(x => x.DeveExpirarEm).HasColumnName("deve_expirar_em");
        builder.Property(x => x.ExpiradaEm).HasColumnName("expirada_em");
        builder.Property(x => x.CanceladaEm).HasColumnName("cancelada_em");
        builder.Property(x => x.CanceladaPorUsuarioId).HasColumnName("cancelada_por_usuario_id");
        builder.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento").HasMaxLength(1000);
        builder.Property(x => x.DecididaEm).HasColumnName("decidida_em");
        builder.Property(x => x.EscopoResumoSnapshot).HasColumnName("escopo_resumo_snapshot").HasMaxLength(4000);
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

        builder.HasIndex(x => x.InstanciaAprovacaoChamadoId).HasDatabaseName("ix_etapas_aprovacao_chamado_instancia_aprovacao_chamado_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_etapas_aprovacao_chamado_status");
        builder.HasIndex(x => x.TipoEtapa).HasDatabaseName("ix_etapas_aprovacao_chamado_tipo_etapa");
        builder.HasIndex(x => x.TipoFluxoAprovacao).HasDatabaseName("ix_etapas_aprovacao_chamado_tipo_fluxo_aprovacao");
        builder.HasIndex(x => x.SolicitadaEm).HasDatabaseName("ix_etapas_aprovacao_chamado_solicitada_em");
        builder.HasIndex(x => x.DeveExpirarEm).HasDatabaseName("ix_etapas_aprovacao_chamado_deve_expirar_em");
        builder.HasIndex(x => new { x.InstanciaAprovacaoChamadoId, x.Nivel, x.Ordem, x.Ramo })
            .HasDatabaseName("ix_etapas_aprovacao_chamado_instancia_nivel_ordem_ramo");
        builder.HasIndex(x => new { x.Id, x.InstanciaAprovacaoChamadoId })
            .IsUnique()
            .HasDatabaseName("ux_etapas_aprovacao_chamado_id_instancia");
        builder.HasIndex(x => x.SolicitanteId).HasDatabaseName("ix_etapas_aprovacao_chamado_solicitante_id");
        builder.HasIndex(x => x.AprovadorResolvidoUsuarioId).HasDatabaseName("ix_etapas_aprovacao_chamado_aprovador_resolvido_usuario_id");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_etapas_aprovacao_chamado_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId).HasDatabaseName("ix_etapas_aprovacao_chamado_atualizado_por_usuario_id");

        builder.HasOne(x => x.InstanciaAprovacaoChamado)
            .WithMany(x => x.Etapas)
            .HasForeignKey(x => x.InstanciaAprovacaoChamadoId)
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
