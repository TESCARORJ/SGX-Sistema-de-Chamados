using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class SlaConfiguracaoConfiguration : IEntityTypeConfiguration<SlaConfiguracao>
{
    public void Configure(EntityTypeBuilder<SlaConfiguracao> builder)
    {
        builder.ToTable("sla_configuracoes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.PrioridadeId).HasColumnName("prioridade_id").IsRequired();
        builder.Property(x => x.PrazoPrimeiraRespostaHoras).HasColumnName("prazo_primeira_resposta_horas").IsRequired();
        builder.Property(x => x.PrazoResolucaoHoras).HasColumnName("prazo_resolucao_horas").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasOne(x => x.Departamento)
            .WithMany(x => x.SlaConfiguracoes)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Categoria)
            .WithMany(x => x.SlaConfiguracoes)
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Prioridade)
            .WithMany(x => x.SlaConfiguracoes)
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
