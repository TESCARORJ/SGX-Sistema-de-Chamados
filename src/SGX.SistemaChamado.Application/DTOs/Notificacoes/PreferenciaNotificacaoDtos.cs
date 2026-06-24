using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record DefinirPreferenciaNotificacaoUsuarioRequest(
    Guid UsuarioId,
    TipoEventoNotificacao TipoEvento,
    CanalNotificacao Canal,
    bool Habilitada);

public sealed record PreferenciaNotificacaoUsuarioResponse(
    Guid PreferenciaId,
    Guid UsuarioId,
    TipoEventoNotificacao TipoEvento,
    CanalNotificacao Canal,
    bool Habilitada,
    bool Criada,
    bool Atualizada);

public sealed record AvaliarPreferenciaNotificacaoRequest(
    Guid UsuarioId,
    TipoEventoNotificacao TipoEvento,
    CanalNotificacao Canal);

public sealed record AvaliarPreferenciaNotificacaoResponse(
    Guid UsuarioId,
    TipoEventoNotificacao TipoEvento,
    CanalNotificacao Canal,
    bool Permitida,
    bool PreferenciaExplicita,
    bool? HabilitadaConfigurada,
    MotivoDecisaoPreferenciaNotificacao Motivo,
    string DescricaoMotivo);
