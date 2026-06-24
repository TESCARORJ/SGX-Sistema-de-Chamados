using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record EntregarNotificacaoSistemaRequest(
    Guid NotificacaoId,
    DateTime EntregueEm);

public sealed record EntregarNotificacaoSistemaResponse(
    Guid NotificacaoId,
    Guid DestinatarioUsuarioId,
    bool Entregue,
    bool JaEstavaEntregue,
    StatusNotificacao Status,
    DateTime? EntregueEm);

public sealed record ListarNotificacoesSistemaUsuarioRequest(
    Guid UsuarioId);

public sealed record NotificacaoSistemaResumoResponse(
    Guid NotificacaoId,
    Guid DestinatarioUsuarioId,
    string? Assunto,
    string Conteudo,
    string ChaveIdempotencia,
    DateTime? EntregueEm,
    DateTime CriadoEm);
