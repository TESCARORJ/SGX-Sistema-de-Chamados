using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class DecisaoAprovacaoChamadoConfiguration : IEntityTypeConfiguration<DecisaoAprovacaoChamado>
{
    public void Configure(EntityTypeBuilder<DecisaoAprovacaoChamado> builder)
    {
        builder.ToTable("decisoes_aprovacao_chamado", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_quorum_esperado",
                "quorum_esperado IS NULL OR quorum_esperado > 0");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_quorum_atingido",
                "quorum_atingido IS NULL OR quorum_atingido > 0");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_quorum_dependencia",
                "quorum_atingido IS NULL OR quorum_esperado IS NOT NULL");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_regra_versao_snapshot",
                "regra_versao_snapshot IS NULL OR regra_versao_snapshot > 0");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_nivel_etapa_snapshot",
                "nivel_etapa_snapshot IS NULL OR nivel_etapa_snapshot > 0");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_ordem_etapa_snapshot",
                "ordem_etapa_snapshot IS NULL OR ordem_etapa_snapshot >= 0");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_bloqueio_liberacao",
                "NOT (libera_avanco AND mantem_bloqueio)");
            tableBuilder.HasCheckConstraint(
                "ck_decisoes_aprovacao_chamado_etapa_status",
                "(etapa_aprovacao_chamado_id IS NULL AND status_etapa_anterior IS NULL AND status_etapa_novo IS NULL AND nivel_etapa_snapshot IS NULL AND ordem_etapa_snapshot IS NULL AND ramo_etapa_snapshot IS NULL) OR (etapa_aprovacao_chamado_id IS NOT NULL AND status_etapa_anterior IS NOT NULL AND status_etapa_novo IS NOT NULL)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.InstanciaAprovacaoChamadoId).HasColumnName("instancia_aprovacao_chamado_id").IsRequired();
        builder.Property(x => x.EtapaAprovacaoChamadoId).HasColumnName("etapa_aprovacao_chamado_id");
        builder.Property(x => x.TipoDecisao).HasColumnName("tipo_decisao").HasConversion<int>().IsRequired();
        builder.Property(x => x.Resultado).HasColumnName("resultado").HasConversion<int>().IsRequired();
        builder.Property(x => x.DataDecisao).HasColumnName("data_decisao").IsRequired();
        builder.Property(x => x.DecisorUsuarioId).HasColumnName("decisor_usuario_id");
        builder.Property(x => x.PapelDecisorSnapshot).HasColumnName("papel_decisor_snapshot").HasMaxLength(120);
        builder.Property(x => x.AutoridadeDecisorSnapshot).HasColumnName("autoridade_decisor_snapshot").HasMaxLength(180);
        builder.Property(x => x.DecisorEhAprovadorEspecifico).HasColumnName("decisor_eh_aprovador_especifico").IsRequired();
        builder.Property(x => x.DecisorEhAprovadorPadrao).HasColumnName("decisor_eh_aprovador_padrao").IsRequired();
        builder.Property(x => x.DecisorEhMembroGrupo).HasColumnName("decisor_eh_membro_grupo").IsRequired();
        builder.Property(x => x.DecisorPorDelegacao).HasColumnName("decisor_por_delegacao").IsRequired();
        builder.Property(x => x.GrupoAprovadorSnapshot).HasColumnName("grupo_aprovador_snapshot").HasMaxLength(180);
        builder.Property(x => x.QuorumEsperado).HasColumnName("quorum_esperado");
        builder.Property(x => x.QuorumAtingido).HasColumnName("quorum_atingido");
        builder.Property(x => x.Justificativa).HasColumnName("justificativa").HasMaxLength(4000);
        builder.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(2000);
        builder.Property(x => x.EscopoDecididoSnapshot).HasColumnName("escopo_decidido_snapshot").HasMaxLength(4000);
        builder.Property(x => x.EfeitoOperacional).HasColumnName("efeito_operacional").HasConversion<int>().IsRequired();
        builder.Property(x => x.DecisaoParcial).HasColumnName("decisao_parcial").IsRequired();
        builder.Property(x => x.DecisaoFinal).HasColumnName("decisao_final").IsRequired();
        builder.Property(x => x.LiberaAvanco).HasColumnName("libera_avanco").IsRequired();
        builder.Property(x => x.MantemBloqueio).HasColumnName("mantem_bloqueio").IsRequired();
        builder.Property(x => x.ExigeReavaliacao).HasColumnName("exige_reavaliacao").IsRequired();
        builder.Property(x => x.PermiteNovaSolicitacao).HasColumnName("permite_nova_solicitacao").IsRequired();
        builder.Property(x => x.CancelaFluxo).HasColumnName("cancela_fluxo").IsRequired();
        builder.Property(x => x.StatusInstanciaAnterior).HasColumnName("status_instancia_anterior").HasConversion<int>().IsRequired();
        builder.Property(x => x.StatusInstanciaNovo).HasColumnName("status_instancia_novo").HasConversion<int>().IsRequired();
        builder.Property(x => x.StatusEtapaAnterior).HasColumnName("status_etapa_anterior").HasConversion<int?>();
        builder.Property(x => x.StatusEtapaNovo).HasColumnName("status_etapa_novo").HasConversion<int?>();
        builder.Property(x => x.StatusChamadoAnteriorId).HasColumnName("status_chamado_anterior_id");
        builder.Property(x => x.StatusChamadoNovoId).HasColumnName("status_chamado_novo_id");
        builder.Property(x => x.NivelEtapaSnapshot).HasColumnName("nivel_etapa_snapshot");
        builder.Property(x => x.OrdemEtapaSnapshot).HasColumnName("ordem_etapa_snapshot");
        builder.Property(x => x.RamoEtapaSnapshot).HasColumnName("ramo_etapa_snapshot").HasMaxLength(80);
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

        builder.HasIndex(x => x.InstanciaAprovacaoChamadoId).HasDatabaseName("ix_decisoes_aprovacao_chamado_instancia_aprovacao_chamado_id");
        builder.HasIndex(x => x.EtapaAprovacaoChamadoId).HasDatabaseName("ix_decisoes_aprovacao_chamado_etapa_aprovacao_chamado_id");
        builder.HasIndex(x => x.TipoDecisao).HasDatabaseName("ix_decisoes_aprovacao_chamado_tipo_decisao");
        builder.HasIndex(x => x.Resultado).HasDatabaseName("ix_decisoes_aprovacao_chamado_resultado");
        builder.HasIndex(x => x.DataDecisao).HasDatabaseName("ix_decisoes_aprovacao_chamado_data_decisao");
        builder.HasIndex(x => x.DecisorUsuarioId).HasDatabaseName("ix_decisoes_aprovacao_chamado_decisor_usuario_id");
        builder.HasIndex(x => x.CriadoPorUsuarioId).HasDatabaseName("ix_decisoes_aprovacao_chamado_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId).HasDatabaseName("ix_decisoes_aprovacao_chamado_atualizado_por_usuario_id");
        builder.HasIndex(x => x.StatusChamadoAnteriorId).HasDatabaseName("ix_decisoes_aprovacao_chamado_status_chamado_anterior_id");
        builder.HasIndex(x => x.StatusChamadoNovoId).HasDatabaseName("ix_decisoes_aprovacao_chamado_status_chamado_novo_id");
        builder.HasIndex(x => new { x.InstanciaAprovacaoChamadoId, x.DataDecisao }).HasDatabaseName("ix_decisoes_aprovacao_chamado_instancia_data_decisao");
        builder.HasIndex(x => new { x.InstanciaAprovacaoChamadoId, x.EtapaAprovacaoChamadoId, x.TipoDecisao }).HasDatabaseName("ix_decisoes_aprovacao_chamado_instancia_etapa_tipo_decisao");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_decisoes_aprovacao_chamado_ativo");

        builder.HasOne(x => x.InstanciaAprovacaoChamado)
            .WithMany(x => x.Decisoes)
            .HasForeignKey(x => x.InstanciaAprovacaoChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EtapaAprovacaoChamado)
            .WithMany(x => x.Decisoes)
            .HasPrincipalKey(x => new { x.Id, x.InstanciaAprovacaoChamadoId })
            .HasForeignKey(x => new { x.EtapaAprovacaoChamadoId, x.InstanciaAprovacaoChamadoId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DecisorUsuario)
            .WithMany()
            .HasForeignKey(x => x.DecisorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StatusChamadoAnterior)
            .WithMany()
            .HasForeignKey(x => x.StatusChamadoAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StatusChamadoNovo)
            .WithMany()
            .HasForeignKey(x => x.StatusChamadoNovoId)
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
