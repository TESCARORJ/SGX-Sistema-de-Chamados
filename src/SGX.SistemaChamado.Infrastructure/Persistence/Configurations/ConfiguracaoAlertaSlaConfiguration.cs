using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ConfiguracaoAlertaSlaConfiguration : IEntityTypeConfiguration<ConfiguracaoAlertaSla>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoAlertaSla> builder)
    {
        builder.ToTable("configuracoes_alerta_sla");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.MinutosAntesVencimentoPrimeiraResposta).HasColumnName("minutos_antes_vencimento_primeira_resposta").IsRequired();
        builder.Property(x => x.MinutosAntesVencimentoResolucao).HasColumnName("minutos_antes_vencimento_resolucao").IsRequired();
        builder.Property(x => x.NotificarAtendente).HasColumnName("notificar_atendente").IsRequired();
        builder.Property(x => x.NotificarGestor).HasColumnName("notificar_gestor").IsRequired();
        builder.Property(x => x.NotificarDepartamento).HasColumnName("notificar_departamento").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasData(new
        {
            Id = SeedData.ConfiguracaoAlertaSlaPadraoId,
            MinutosAntesVencimentoPrimeiraResposta = 30,
            MinutosAntesVencimentoResolucao = 120,
            NotificarAtendente = true,
            NotificarGestor = false,
            NotificarDepartamento = false,
            CriadoEm = SeedData.DataBase,
            CriadoPor = SeedData.UsuarioSistema,
            AtualizadoEm = (DateTime?)null,
            AtualizadoPor = (string?)null,
            Ativo = true
        });
    }
}
