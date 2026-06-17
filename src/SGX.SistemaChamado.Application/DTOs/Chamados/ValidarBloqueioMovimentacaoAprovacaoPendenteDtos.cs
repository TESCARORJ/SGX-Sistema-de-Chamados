using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public enum TipoAcaoMovimentacaoChamado
{
    Consultar = 1,
    Comentar = 2,
    AnexarEvidencia = 3,
    Triagem = 4,
    Assumir = 5,
    Atribuir = 6,
    Encaminhar = 7,
    AlterarStatus = 8,
    Resolver = 9,
    Encerrar = 10,
    Reabrir = 11,
    ExecutarServicoSensivel = 12,
    AplicarMudanca = 13,
    LiberarAcesso = 14,
    Cancelar = 15,
    AceitarSolucao = 16,
    RejeitarSolucao = 17,
    FecharAutomaticamentePorPrazoAceite = 18
}

public sealed class ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
{
    public Guid ChamadoId { get; init; }
    public TipoAcaoMovimentacaoChamado TipoAcao { get; init; }
    public Guid? StatusDestinoId { get; init; }
    public Guid? UsuarioId { get; init; }
    public bool IgnorarSinalizacao { get; init; }
    public string? Contexto { get; init; }
}

public sealed class ValidarBloqueioMovimentacaoAprovacaoPendenteResponse
{
    public bool Permitido { get; init; }
    public bool Bloqueado { get; init; }
    public bool ApenasSinalizacao { get; init; }
    public string? Motivo { get; init; }
    public string? MensagemUsuario { get; init; }
    public string? OrigemBloqueio { get; init; }
    public Guid? AprovacaoChamadoId { get; init; }
    public Guid? InstanciaAprovacaoChamadoId { get; init; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; init; }
    public string? StatusAprovacao { get; init; }
    public EfeitoOperacionalRegraAprovacao? EfeitoOperacional { get; init; }
    public bool Bloqueante { get; init; }
    public TipoFluxoAprovacao? TipoFluxoAprovacao { get; init; }
    public bool ExigeAprovacao { get; init; }
    public bool PodeContinuarTriagem { get; init; }
}
