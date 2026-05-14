using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class ChamadoConfiguration : IEntityTypeConfiguration<Chamado>
{
    public void Configure(EntityTypeBuilder<Chamado> builder)
    {
        builder.ToTable("chamados");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(40).IsRequired();
        builder.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(180).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(4000).IsRequired();
        builder.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
        builder.Property(x => x.ResponsavelId).HasColumnName("responsavel_id");
        builder.Property(x => x.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id").IsRequired();
        builder.Property(x => x.PrioridadeId).HasColumnName("prioridade_id").IsRequired();
        builder.Property(x => x.StatusId).HasColumnName("status_id").IsRequired();
        builder.Property(x => x.Origem).HasColumnName("origem").HasConversion<int>().IsRequired();
        builder.Property(x => x.AbertoEm).HasColumnName("aberto_em").IsRequired();
        builder.Property(x => x.EncerradoEm).HasColumnName("encerrado_em");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("ux_chamados_codigo");

        builder.HasOne(x => x.Solicitante)
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Responsavel)
            .WithMany()
            .HasForeignKey(x => x.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Departamento)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Categoria)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Prioridade)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.PrioridadeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Chamados)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Historicos)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Comentarios)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Anexos)
            .WithOne(x => x.Chamado)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SlaControle)
            .WithOne(x => x.Chamado)
            .HasForeignKey<SlaControle>(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ChamadoSla)
            .WithOne(x => x.Chamado)
            .HasForeignKey<ChamadoSla>(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
