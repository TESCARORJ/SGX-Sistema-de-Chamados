using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record MaterializarConteudoNotificacaoRequest(
    TipoEventoNotificacao TipoEvento,
    CanalNotificacao Canal,
    DateTime DataReferencia,
    IReadOnlyDictionary<string, string> Variaveis,
    Guid? TemplateNotificacaoId = null);

public sealed record MaterializarConteudoNotificacaoResponse(
    Guid TemplateNotificacaoId,
    string TemplateNome,
    int TemplateVersao,
    string? Assunto,
    string Conteudo,
    IReadOnlyCollection<string> VariaveisUtilizadas);
