using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoSlaConfiguration : IEntityTypeConfiguration<ChamadoSla>
{
    public void Configure(EntityTypeBuilder<ChamadoSla> builder)
    {
        builder.ToTable("chamado_slas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.PoliticaSlaId).HasColumnName("politica_sla_id");
        builder.Property(x => x.PrioridadeId).HasColumnName("prioridade_id").IsRequired();
        builder.Property(x => x.DataInicio).HasColumnName("data_inicio").IsRequired();
        builder.Property(x => x.PrazoPrimeiraResposta).HasColumnName("prazo_primeira_resposta").IsRequired();
        builder.Property(x => x.PrazoResolucao).HasColumnName("prazo_resolucao").IsRequired();
        builder.Property(x => x.DataPrimeiraResposta).HasColumnName("data_primeira_resposta");
        builder.Property(x => x.DataResolucao).HasColumnName("data_resolucao");
        builder.Property(x => x.PrimeiraRespostaCumprida).HasColumnName("primeira_resposta_cumprida");
        builder.Property(x => x.ResolucaoCumprida).HasColumnName("resolucao_cumprida");
        builder.Property(x => x.PrimeiraRespostaViolada).HasColumnName("primeira_resposta_violada").IsRequired();
        builder.Property(x => x.ResolucaoViolada).HasColumnName("resolucao_violada").IsRequired();
        builder.Property(x => x.MinutosPrimeiraResposta).HasColumnName("minutos_primeira_resposta");
        builder.Property(x => x.MinutosResolucao).HasColumnName("minutos_resolucao");
        builder.Property(x => x.Pausado).HasColumnName("pausado").IsRequired();
        builder.Property(x => x.DataPausa).HasColumnName("data_pausa");
        builder.Property(x => x.MinutosPausados).HasColumnName("minutos_pausados").IsRequired();
        builder.Property(x => x.PausarQuandoAguardandoSolicitante).HasColumnName("pausar_quando_aguardando_solicitante").IsRequired();
        builder.Property(x => x.UsarHorarioComercial).HasColumnName("usar_horario_comercial").IsRequired();
        builder.Property(x => x.CalendarioCorporativoId).HasColumnName("calendario_corporativo_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId)
            .IsUnique()
            .HasDatabaseName("ux_chamado_slas_chamado_id");

        builder.HasIndex(x => x.PrazoResolucao)
            .HasDatabaseName("ix_chamado_slas_prazo_resolucao");

        builder.HasOne(x => x.PoliticaSla)
            .WithMany()
            .HasForeignKey(x => x.PoliticaSlaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Prioridade)
            .WithMany()
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CalendarioCorporativo)
            .WithMany()
            .HasForeignKey(x => x.CalendarioCorporativoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
