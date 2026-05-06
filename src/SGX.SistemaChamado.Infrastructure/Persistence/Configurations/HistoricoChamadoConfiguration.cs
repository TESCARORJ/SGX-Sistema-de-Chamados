using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class HistoricoChamadoConfiguration : IEntityTypeConfiguration<HistoricoChamado>
{
    public void Configure(EntityTypeBuilder<HistoricoChamado> builder)
    {
        builder.ToTable("historicos_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<int>().IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(2000).IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
