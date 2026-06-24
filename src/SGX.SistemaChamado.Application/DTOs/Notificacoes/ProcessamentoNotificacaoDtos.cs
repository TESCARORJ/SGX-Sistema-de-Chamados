using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record SelecionarNotificacoesProcessaveisRequest(
    int Limite,
    DateTime DataReferencia);

public sealed record NotificacaoProcessavelResponse(
    Guid NotificacaoId,
    CanalNotificacao Canal,
    int QuantidadeTentativas,
    DateTime? AgendadaEm);

public sealed record IniciarProcessamentoNotificacaoRequest(
    Guid NotificacaoId,
    DateTime IniciadaEm);

public sealed record IniciarProcessamentoNotificacaoResponse(
    Guid NotificacaoId,
    StatusNotificacao Status,
    int QuantidadeTentativas,
    DateTime? ProcessadaEm);

public sealed record RegistrarSucessoEntregaNotificacaoRequest(
    Guid NotificacaoId,
    DateTime EnviadaEm);

public sealed record RegistrarFalhaEntregaNotificacaoRequest(
    Guid NotificacaoId,
    string Erro,
    bool FalhaTransitoria,
    DateTime FalhouEm);

public sealed record RegistrarFalhaEntregaNotificacaoResponse(
    Guid NotificacaoId,
    StatusNotificacao Status,
    int QuantidadeTentativas,
    DateTime? AgendadaEm,
    DateTime? FalhouEm,
    string? UltimoErro);
