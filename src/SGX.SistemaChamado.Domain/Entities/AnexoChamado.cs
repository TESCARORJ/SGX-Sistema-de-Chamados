using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class AnexoChamado : AuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public string NomeArquivo { get; private set; } = string.Empty;
    public string NomeArquivoArmazenado { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long TamanhoBytes { get; private set; }
    public string Caminho { get; private set; } = string.Empty;
    public Guid UsuarioId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario Usuario { get; private set; } = default!;

    private AnexoChamado()
    {
    }

    public AnexoChamado(
        Guid chamadoId,
        string nomeArquivo,
        string nomeArquivoArmazenado,
        string contentType,
        long tamanhoBytes,
        string caminho,
        Guid usuarioId,
        string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado do anexo e obrigatorio.", nameof(chamadoId));
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario do anexo e obrigatorio.", nameof(usuarioId));
        }

        if (string.IsNullOrWhiteSpace(nomeArquivo))
        {
            throw new ArgumentException("O nome do arquivo e obrigatorio.", nameof(nomeArquivo));
        }

        if (string.IsNullOrWhiteSpace(nomeArquivoArmazenado))
        {
            throw new ArgumentException("O nome armazenado do arquivo e obrigatorio.", nameof(nomeArquivoArmazenado));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("O content type do arquivo e obrigatorio.", nameof(contentType));
        }

        if (string.IsNullOrWhiteSpace(caminho))
        {
            throw new ArgumentException("O caminho do anexo e obrigatorio.", nameof(caminho));
        }

        if (tamanhoBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tamanhoBytes), "O tamanho do anexo deve ser maior que zero.");
        }

        ChamadoId = chamadoId;
        NomeArquivo = nomeArquivo.Trim();
        NomeArquivoArmazenado = nomeArquivoArmazenado.Trim();
        ContentType = contentType.Trim();
        TamanhoBytes = tamanhoBytes;
        Caminho = caminho.Trim();
        UsuarioId = usuarioId;
        DefinirCriacao(criadoPor);
    }
}
