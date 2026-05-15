using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class AnexoChamadoConfiguration : IEntityTypeConfiguration<AnexoChamado>
{
    public void Configure(EntityTypeBuilder<AnexoChamado> builder)
    {
        builder.ToTable("anexos_chamado");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ChamadoId).HasColumnName("chamado_id").IsRequired();
        builder.Property(x => x.NomeArquivo).HasColumnName("nome_arquivo").HasMaxLength(260).IsRequired();
        builder.Property(x => x.NomeArquivoArmazenado).HasColumnName("nome_arquivo_armazenado").HasMaxLength(300).IsRequired();
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(120).IsRequired();
        builder.Property(x => x.TamanhoBytes).HasColumnName("tamanho_bytes").IsRequired();
        builder.Property(x => x.Caminho).HasColumnName("caminho").HasMaxLength(600).IsRequired();
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => x.ChamadoId).HasDatabaseName("IX_anexos_chamado_chamado_id");
        builder.HasIndex(x => x.UsuarioId).HasDatabaseName("IX_anexos_chamado_usuario_id");
        builder.HasIndex(x => x.CriadoEm).HasDatabaseName("IX_anexos_chamado_criado_em");

        builder.HasOne(x => x.Chamado)
            .WithMany(x => x.Anexos)
            .HasForeignKey(x => x.ChamadoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Usuario)
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
