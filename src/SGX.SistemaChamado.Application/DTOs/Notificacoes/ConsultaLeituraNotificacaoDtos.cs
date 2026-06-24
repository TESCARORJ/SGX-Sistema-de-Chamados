using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record ListarMinhasNotificacoesRequest(
    int Pagina = 1,
    int TamanhoPagina = 20,
    bool? Lida = null);

public sealed record MinhaNotificacaoResumoResponse(
    Guid Id,
    TipoEventoNotificacao TipoEvento,
    string? Assunto,
    string ConteudoResumo,
    DateTime EnviadaEm,
    bool Lida,
    DateTime? LidaEm,
    Guid? ChamadoId);

public sealed record ListarMinhasNotificacoesResponse(
    IReadOnlyCollection<MinhaNotificacaoResumoResponse> Itens,
    int Pagina,
    int TamanhoPagina,
    int Total,
    int TotalPaginas,
    int TotalNaoLidas);

public sealed record MinhaNotificacaoDetalheResponse(
    Guid Id,
    TipoEventoNotificacao TipoEvento,
    string? Assunto,
    string Conteudo,
    DateTime EnviadaEm,
    bool Lida,
    DateTime? LidaEm,
    Guid? ChamadoId,
    string? ChaveCorrelacao);

public sealed record AlterarLeituraNotificacaoResponse(
    Guid NotificacaoId,
    bool Lida,
    DateTime? LidaEm,
    bool EstadoAlterado);

public sealed record ContagemMinhasNotificacoesNaoLidasResponse(int TotalNaoLidas);
