using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record GerarNotificacaoRequest(
    EventoCandidatoNotificacao Evento,
    CanalNotificacao Canal,
    Guid? DestinatarioUsuarioId,
    string? DestinatarioEndereco,
    string? Assunto,
    string Conteudo,
    DateTime? AgendadaEm = null);

public sealed record GerarNotificacaoResponse(
    Guid NotificacaoId,
    bool Criada,
    bool JaExistia,
    StatusNotificacao Status,
    string ChaveIdempotencia);
