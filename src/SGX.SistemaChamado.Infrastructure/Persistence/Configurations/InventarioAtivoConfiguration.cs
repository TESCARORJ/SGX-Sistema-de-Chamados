using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class InventarioAtivoConfiguration : IEntityTypeConfiguration<InventarioAtivo>
{
    public void Configure(EntityTypeBuilder<InventarioAtivo> builder)
    {
        builder.ToTable("inventario_ativos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(60).IsRequired();
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(2000);
        builder.Property(x => x.NumeroPatrimonio).HasColumnName("numero_patrimonio").HasMaxLength(120);
        builder.Property(x => x.NumeroSerie).HasColumnName("numero_serie").HasMaxLength(180);
        builder.Property(x => x.TipoAtivoInventarioId).HasColumnName("tipo_ativo_inventario_id").IsRequired();
        builder.Property(x => x.Fabricante).HasColumnName("fabricante").HasMaxLength(180);
        builder.Property(x => x.Modelo).HasColumnName("modelo").HasMaxLength(180);
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.LocalUnidadeId).HasColumnName("local_unidade_id");
        builder.Property(x => x.UsuarioResponsavelId).HasColumnName("usuario_responsavel_id");
        builder.Property(x => x.StatusOperacional).HasColumnName("status_operacional").HasConversion<int>().IsRequired();
        builder.Property(x => x.StatusPatrimonial).HasColumnName("status_patrimonial").HasConversion<int>().IsRequired();
        builder.Property(x => x.Criticidade).HasColumnName("criticidade").HasConversion<int>().IsRequired();
        builder.Property(x => x.DataAquisicao).HasColumnName("data_aquisicao");
        builder.Property(x => x.DataFimGarantia).HasColumnName("data_fim_garantia");
        builder.Property(x => x.ValorAquisicao).HasColumnName("valor_aquisicao").HasPrecision(18, 2);
        builder.Property(x => x.Fornecedor).HasColumnName("fornecedor").HasMaxLength(180);
        builder.Property(x => x.Observacoes).HasColumnName("observacoes").HasMaxLength(4000);
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.InativadoEm).HasColumnName("inativado_em");
        builder.Property(x => x.InativadoPorUsuarioId).HasColumnName("inativado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("ux_inventario_ativos_codigo");
        builder.HasIndex(x => x.NumeroPatrimonio)
            .IsUnique()
            .HasFilter("numero_patrimonio IS NOT NULL")
            .HasDatabaseName("ux_inventario_ativos_numero_patrimonio");
        builder.HasIndex(x => x.NumeroSerie)
            .IsUnique()
            .HasFilter("numero_serie IS NOT NULL")
            .HasDatabaseName("ux_inventario_ativos_numero_serie");
        builder.HasIndex(x => x.DepartamentoId).HasDatabaseName("ix_inventario_ativos_departamento_id");
        builder.HasIndex(x => x.UsuarioResponsavelId).HasDatabaseName("ix_inventario_ativos_usuario_responsavel_id");
        builder.HasIndex(x => x.StatusOperacional).HasDatabaseName("ix_inventario_ativos_status_operacional");
        builder.HasIndex(x => x.Ativo).HasDatabaseName("ix_inventario_ativos_ativo");

        builder.HasOne(x => x.TipoAtivoInventario)
            .WithMany(x => x.Ativos)
            .HasForeignKey(x => x.TipoAtivoInventarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Departamento)
            .WithMany()
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LocalUnidade)
            .WithMany()
            .HasForeignKey(x => x.LocalUnidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UsuarioResponsavel)
            .WithMany()
            .HasForeignKey(x => x.UsuarioResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(x => x.InativadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
