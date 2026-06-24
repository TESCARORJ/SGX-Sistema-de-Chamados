using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Infrastructure.Persistence.Configurations;

public sealed class TemplateNotificacaoConfiguration : IEntityTypeConfiguration<TemplateNotificacao>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Configure(EntityTypeBuilder<TemplateNotificacao> builder)
    {
        builder.ToTable("templates_notificacao", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_templates_notificacao_versao_positiva",
                "versao > 0");
            tableBuilder.HasCheckConstraint(
                "ck_templates_notificacao_vigencia",
                "vigente_ate IS NULL OR vigente_de IS NULL OR vigente_ate >= vigente_de");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(TemplateNotificacao.MaximoNome).IsRequired();
        builder.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(TemplateNotificacao.MaximoDescricao);
        builder.Property(x => x.TipoEvento).HasColumnName("tipo_evento").HasConversion<int>().IsRequired();
        builder.Property(x => x.Canal).HasColumnName("canal").HasConversion<int>().IsRequired();
        builder.Property(x => x.Versao).HasColumnName("versao").IsRequired();
        builder.Property(x => x.AssuntoTemplate).HasColumnName("assunto_template").HasMaxLength(TemplateNotificacao.MaximoAssuntoTemplate);
        builder.Property(x => x.ConteudoTemplate).HasColumnName("conteudo_template").HasMaxLength(TemplateNotificacao.MaximoConteudoTemplate).IsRequired();
        var variaveisPermitidasBuilder = builder.Property<List<string>>("VariaveisPermitidasPersistidas")
            .HasColumnName("variaveis_permitidas")
            .HasColumnType("text")
            .HasConversion(
                new ValueConverter<List<string>, string>(
                    value => JsonSerializer.Serialize(value ?? new List<string>(), JsonOptions),
                    value => string.IsNullOrWhiteSpace(value)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(value, JsonOptions) ?? new List<string>()));

        variaveisPermitidasBuilder.Metadata.SetValueComparer(
            new ValueComparer<List<string>>(
                (left, right) => (left ?? new List<string>()).SequenceEqual(right ?? new List<string>()),
                value => (value ?? new List<string>()).Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode(StringComparison.Ordinal))),
                value => (value ?? new List<string>()).ToList()));
        builder.Property(x => x.VigenteDe).HasColumnName("vigente_de");
        builder.Property(x => x.VigenteAte).HasColumnName("vigente_ate");
        builder.Property(x => x.CriadoPorUsuarioId).HasColumnName("criado_por_usuario_id").IsRequired();
        builder.Property(x => x.AtualizadoPorUsuarioId).HasColumnName("atualizado_por_usuario_id");
        builder.Property(x => x.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(120).IsRequired();
        builder.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(120);
        builder.Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

        builder.HasIndex(x => new { x.Nome, x.Versao })
            .IsUnique()
            .HasDatabaseName("ux_templates_notificacao_nome_versao");
        builder.HasIndex(x => new { x.TipoEvento, x.Canal, x.Ativo })
            .HasDatabaseName("ix_templates_notificacao_tipo_evento_canal_ativo");
        builder.HasIndex(x => new { x.VigenteDe, x.VigenteAte })
            .HasDatabaseName("ix_templates_notificacao_vigencia");
        builder.HasIndex(x => x.CriadoPorUsuarioId)
            .HasDatabaseName("ix_templates_notificacao_criado_por_usuario_id");
        builder.HasIndex(x => x.AtualizadoPorUsuarioId)
            .HasDatabaseName("ix_templates_notificacao_atualizado_por_usuario_id");

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
