using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record ProcessarEventoCandidatoNotificacaoRequest(
    string EventoId,
    EventoCandidatoNotificacao Evento,
    IReadOnlyDictionary<string, string> Variaveis,
    IReadOnlyCollection<TipoParticipacaoDestinatarioNotificacao> Participacoes,
    IReadOnlyCollection<CanalNotificacao> Canais,
    Guid? AprovacaoChamadoId = null,
    Guid? InstanciaAprovacaoChamadoId = null,
    Guid? GrupoTecnicoId = null,
    Guid? PerfilAcessoId = null,
    bool ExcluirUsuarioOriginador = false);

public sealed record ProcessarEventoCandidatoNotificacaoResponse(
    string EventoId,
    int DestinatariosResolvidos,
    int DestinatariosPermitidos,
    int NotificacoesCriadas,
    int NotificacoesDuplicadas,
    int Ignoradas,
    IReadOnlyCollection<string> Avisos);
