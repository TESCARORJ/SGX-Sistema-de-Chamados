using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class HistoricoInventarioAtivoConfiguration : IEntityTypeConfiguration<HistoricoInventarioAtivo>
{
    public void Configure(EntityTypeBuilder<HistoricoInventarioAtivo> builder)
    {
        builder.ToTable("historico_inventario_ativos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.InventarioAtivoId).HasColumnName("inventario_ativo_id").IsRequired();
        builder.Property(x => x.TipoMovimentacao).HasColumnName("tipo_movimentacao").HasConversion<int>().IsRequired();
        builder.Property(x => x.DepartamentoOrigemId).HasColumnName("departamento_origem_id");
        builder.Property(x => x.DepartamentoDestinoId).HasColumnName("departamento_destino_id");
        builder.Property(x => x.LocalUnidadeOrigemId).HasColumnName("local_unidade_origem_id");
        builder.Property(x => x.LocalUnidadeDestinoId).HasColumnName("local_unidade_destino_id");
        builder.Property(x => x.UsuarioResponsavelOrigemId).HasColumnName("usuario_responsavel_origem_id");
        builder.Property(x => x.UsuarioResponsavelDestinoId).HasColumnName("usuario_responsavel_destino_id");
        builder.Property(x => x.StatusOperacionalAnterior).HasColumnName("status_operacional_anterior").HasConversion<int?>();
        builder.Property(x => x.StatusOperacionalNovo).HasColumnName("status_operacional_novo").HasConversion<int?>();
        builder.Property(x => x.StatusPatrimonialAnterior).HasColumnName("status_patrimonial_anterior").HasConversion<int?>();
        builder.Property(x => x.StatusPatrimonialNovo).HasColumnName("status_patrimonial_novo").HasConversion<int?>();
        builder.Property(x => x.Observacao).HasColumnName("observacao").HasMaxLength(2000);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();

        builder.HasIndex(x => x.InventarioAtivoId).HasDatabaseName("ix_historico_inventario_ativos_inventario_ativo_id");
        builder.HasIndex(x => x.CriadoEm).HasDatabaseName("ix_historico_inventario_ativos_criado_em");
        builder.HasIndex(x => x.TipoMovimentacao).HasDatabaseName("ix_historico_inventario_ativos_tipo_movimentacao");

        builder.HasOne(x => x.InventarioAtivo)
            .WithMany(x => x.Historicos)
            .HasForeignKey(x => x.InventarioAtivoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DepartamentoOrigem)
            .WithMany()
            .HasForeignKey(x => x.DepartamentoOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DepartamentoDestino)
            .WithMany()
            .HasForeignKey(x => x.DepartamentoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LocalUnidadeOrigem)
            .WithMany()
            .HasForeignKey(x => x.LocalUnidadeOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LocalUnidadeDestino)
            .WithMany()
            .HasForeignKey(x => x.LocalUnidadeDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UsuarioResponsavelOrigem)
            .WithMany()
            .HasForeignKey(x => x.UsuarioResponsavelOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UsuarioResponsavelDestino)
            .WithMany()
            .HasForeignKey(x => x.UsuarioResponsavelDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
