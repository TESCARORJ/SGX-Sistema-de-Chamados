using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class PoliticaSlaConfiguration : IEntityTypeConfiguration<PoliticaSla>
{
    public void Configure(EntityTypeBuilder<PoliticaSla> builder)
    {
        builder.ToTable("sla_politicas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(160).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(1000);
        builder.Property(x => x.Ordem).HasColumnName("ordem").IsRequired();
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.CalendarioCorporativoId).HasColumnName("calendario_corporativo_id");
        builder.Property(x => x.UsarHorarioComercial).HasColumnName("usar_horario_comercial").IsRequired();
        builder.Property(x => x.PausarQuandoAguardandoSolicitante).HasColumnName("pausar_quando_aguardando_solicitante").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Ordem).HasDatabaseName("ix_sla_politicas_ordem");

        builder.HasOne(x => x.Categoria)
            .WithMany()
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Departamento)
            .WithMany()
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CalendarioCorporativo)
            .WithMany()
            .HasForeignKey(x => x.CalendarioCorporativoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Metas)
            .WithOne(x => x.PoliticaSla)
            .HasForeignKey(x => x.PoliticaSlaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.SlaPoliticas);
    }
}
