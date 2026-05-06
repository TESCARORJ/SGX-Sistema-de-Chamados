using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ComentarioChamado : AuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Mensagem { get; private set; } = string.Empty;
    public bool Interno { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario Usuario { get; private set; } = default!;

    private ComentarioChamado()
    {
    }

    public ComentarioChamado(Guid chamadoId, Guid usuarioId, string mensagem, bool interno, string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado e obrigatorio.", nameof(chamadoId));
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario do comentario e obrigatorio.", nameof(usuarioId));
        }

        if (string.IsNullOrWhiteSpace(mensagem))
        {
            throw new ArgumentException("A mensagem do comentario e obrigatoria.", nameof(mensagem));
        }

        ChamadoId = chamadoId;
        UsuarioId = usuarioId;
        Mensagem = mensagem.Trim();
        Interno = interno;
        DefinirCriacao(criadoPor);
    }
}
