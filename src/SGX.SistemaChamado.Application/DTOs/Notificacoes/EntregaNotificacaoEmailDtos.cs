using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record EntregarNotificacaoEmailRequest(
    Guid NotificacaoId,
    DateTime EntregueEm);

public sealed record EntregarNotificacaoEmailResponse(
    Guid NotificacaoId,
    string Destinatario,
    bool Entregue,
    bool JaEstavaEntregue,
    bool Reagendada,
    StatusNotificacao Status,
    int QuantidadeTentativas,
    DateTime? EnviadaEm,
    DateTime? ProximaTentativaEm,
    string? Erro);

public sealed record MensagemEmailNotificacao(
    string Destinatario,
    string Assunto,
    string Conteudo,
    bool ConteudoHtml,
    string? ChaveCorrelacao);

public sealed record ResultadoTransporteEmailNotificacao(
    bool Sucesso,
    bool FalhaTransitoria,
    string? IdentificadorExterno,
    string? Erro);
