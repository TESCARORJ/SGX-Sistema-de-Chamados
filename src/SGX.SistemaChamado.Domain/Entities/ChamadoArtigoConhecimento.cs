using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ChamadoArtigoConhecimento : CreationAuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public Guid ArtigoId { get; private set; }
    public DateTime VinculadoEm { get; private set; }
    public Guid VinculadoPorUsuarioId { get; private set; }
    public string? Observacao { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public BaseConhecimentoArtigo Artigo { get; private set; } = default!;
    public Usuario VinculadoPorUsuario { get; private set; } = default!;

    private ChamadoArtigoConhecimento()
    {
    }

    public ChamadoArtigoConhecimento(
        Guid chamadoId,
        Guid artigoId,
        Guid vinculadoPorUsuarioId,
        string? observacao,
        string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado e obrigatorio.", nameof(chamadoId));
        }

        if (artigoId == Guid.Empty)
        {
            throw new ArgumentException("O artigo e obrigatorio.", nameof(artigoId));
        }

        if (vinculadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de vinculacao e obrigatorio.", nameof(vinculadoPorUsuarioId));
        }

        ChamadoId = chamadoId;
        ArtigoId = artigoId;
        VinculadoPorUsuarioId = vinculadoPorUsuarioId;
        VinculadoEm = DateTime.UtcNow;
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
        DefinirCriacao(criadoPor);
    }
}
