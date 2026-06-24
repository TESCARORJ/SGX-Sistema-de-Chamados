using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PreferenciaNotificacaoUsuario : AuditableEntity
{
    public Guid UsuarioId { get; private set; }
    public TipoEventoNotificacao TipoEvento { get; private set; }
    public CanalNotificacao Canal { get; private set; }
    public bool Habilitada { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Usuario Usuario { get; private set; } = default!;
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }

    private PreferenciaNotificacaoUsuario()
    {
    }

    public PreferenciaNotificacaoUsuario(
        Guid usuarioId,
        TipoEventoNotificacao tipoEvento,
        CanalNotificacao canal,
        bool habilitada,
        Guid criadoPorUsuarioId,
        string criadoPor)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario da preferencia de notificacao e obrigatorio.", nameof(usuarioId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da preferencia de notificacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        if (!Enum.IsDefined(tipoEvento))
        {
            throw new ArgumentException("O tipo de evento da preferencia de notificacao e invalido.", nameof(tipoEvento));
        }

        if (!Enum.IsDefined(canal))
        {
            throw new ArgumentException("O canal da preferencia de notificacao e invalido.", nameof(canal));
        }

        UsuarioId = usuarioId;
        TipoEvento = tipoEvento;
        Canal = canal;
        Habilitada = habilitada;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void Habilitar(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
        Habilitada = true;
    }

    public void Desabilitar(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
        Habilitada = false;
    }

    private void RegistrarAtualizacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da preferencia de notificacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }
}
