using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record ResolverDestinatariosNotificacaoRequest(
    EventoCandidatoNotificacao Evento,
    IReadOnlyCollection<TipoParticipacaoDestinatarioNotificacao> Participacoes,
    Guid? AprovacaoChamadoId = null,
    Guid? InstanciaAprovacaoChamadoId = null,
    Guid? GrupoTecnicoId = null,
    Guid? PerfilAcessoId = null,
    bool ExcluirUsuarioOriginador = false);

public sealed record DestinatarioNotificacaoResolvido(
    Guid UsuarioId,
    string Nome,
    string? Email,
    IReadOnlyCollection<TipoParticipacaoDestinatarioNotificacao> Origens);

public sealed record ResolverDestinatariosNotificacaoResponse(
    IReadOnlyCollection<DestinatarioNotificacaoResolvido> Destinatarios,
    IReadOnlyCollection<string> Avisos);
