using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class ReprovarAprovacaoChamadoResponse
{
    public bool Reprovada { get; init; }
    public Guid InstanciaAprovacaoChamadoId { get; init; }
    public Guid? EtapaAprovacaoChamadoId { get; init; }
    public Guid DecisaoAprovacaoChamadoId { get; init; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaAnterior { get; init; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaNovo { get; init; }
    public StatusEtapaAprovacaoChamado? StatusEtapaAnterior { get; init; }
    public StatusEtapaAprovacaoChamado? StatusEtapaNovo { get; init; }
    public bool DecisaoFinal { get; init; }
    public bool MantemBloqueio { get; init; }
    public bool ExigeReavaliacao { get; init; }
    public bool PermiteNovaSolicitacao { get; init; }
    public bool CancelaFluxo { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public IReadOnlyCollection<string> Avisos { get; init; } = [];
}
