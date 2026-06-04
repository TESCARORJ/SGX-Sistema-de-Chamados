using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class CriarChamadoAprovacaoAdminRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public Guid? AprovadorUsuarioId { get; init; }
    public Guid? SolicitadoPorUsuarioId { get; init; }
    public string? JustificativaSolicitacao { get; init; }
}

public sealed class DecidirChamadoAprovacaoAdminRequest
{
    public string? JustificativaDecisao { get; init; }
}

public sealed class CancelarChamadoAprovacaoAdminRequest
{
    public string? MotivoCancelamento { get; init; }
}

public sealed class ChamadoAprovacaoAdminResponse
{
    public Guid Id { get; init; }
    public Guid ChamadoId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public StatusAprovacaoChamado Status { get; init; }
    public string StatusDescricao { get; init; } = string.Empty;
    public Guid? AprovadorUsuarioId { get; init; }
    public string? AprovadorNome { get; init; }
    public Guid? SolicitadoPorUsuarioId { get; init; }
    public string? SolicitadoPorNome { get; init; }
    public string? JustificativaSolicitacao { get; init; }
    public string? JustificativaDecisao { get; init; }
    public bool BloqueiaAvancoAtendimento { get; init; }
    public DateTime SolicitadaEm { get; init; }
    public DateTime? DecididoEm { get; init; }
    public DateTime? CanceladoEm { get; init; }
    public string? MotivoCancelamento { get; init; }
    public DateTime CriadoEm { get; init; }
    public bool Ativo { get; init; }
}
