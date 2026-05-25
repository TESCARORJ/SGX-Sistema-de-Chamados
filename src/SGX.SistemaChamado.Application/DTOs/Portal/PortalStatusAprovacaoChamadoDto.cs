using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class PortalStatusAprovacaoChamadoDto
{
    public Guid ChamadoId { get; init; }
    public bool RequerAprovacao { get; init; }
    public bool AprovacaoPendente { get; init; }
    public StatusAprovacaoChamado? StatusAprovacao { get; init; }
    public Guid? AprovacaoChamadoId { get; init; }
    public DateTime? SolicitadaEm { get; init; }
    public DateTime? DecididaEm { get; init; }
    public string? JustificativaDecisao { get; init; }
    public string MensagemOrientativa { get; init; } = string.Empty;
}
