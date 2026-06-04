using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class CriarChamadoRelacionamentoRequest
{
    public Guid ChamadoOrigemId { get; init; }
    public Guid ChamadoDestinoId { get; init; }
    public TipoRelacionamentoChamadoEnum TipoRelacionamento { get; init; }
    public string? Justificativa { get; init; }
}

public sealed class CriarChamadoRelacionamentoAdminRequest
{
    public Guid ChamadoDestinoId { get; init; }
    public TipoRelacionamentoChamadoEnum TipoRelacionamento { get; init; }
    public string? Justificativa { get; init; }
}

public sealed class ChamadoRelacionamentoAdminResponse
{
    public Guid Id { get; init; }
    public Guid ChamadoOrigemId { get; init; }
    public string ChamadoOrigemCodigo { get; init; } = string.Empty;
    public Guid ChamadoDestinoId { get; init; }
    public string ChamadoDestinoCodigo { get; init; } = string.Empty;
    public TipoRelacionamentoChamadoEnum TipoRelacionamento { get; init; }
    public string TipoRelacionamentoDescricao { get; init; } = string.Empty;
    public string? Justificativa { get; init; }
    public bool Ativo { get; init; }
    public DateTime CriadoEm { get; init; }
    public string? CriadoPor { get; init; }
    public DateTime? RemovidoEm { get; init; }
    public string? MotivoRemocao { get; init; }
}

public sealed class DependenciaChamadoAdminResponse
{
    public Guid RelacionamentoId { get; init; }
    public Guid ChamadoDependenteId { get; init; }
    public string ChamadoDependenteCodigo { get; init; } = string.Empty;
    public Guid ChamadoBloqueadorId { get; init; }
    public string ChamadoBloqueadorCodigo { get; init; } = string.Empty;
    public TipoRelacionamentoChamadoEnum TipoRelacionamentoOriginal { get; init; }
    public string TipoRelacionamentoDescricao { get; init; } = string.Empty;
    public string? Justificativa { get; init; }
    public DateTime CriadoEm { get; init; }
    public bool ChamadoConsultadoEhDependente { get; init; }
    public bool ChamadoConsultadoEhBloqueador { get; init; }
}

public sealed class BloqueioChamadoAdminResponse
{
    public Guid ChamadoId { get; init; }
    public bool EstaBloqueado { get; init; }
    public bool BloqueiaOutrosChamados { get; init; }
    public IReadOnlyList<DependenciaChamadoAdminResponse> Bloqueadores { get; init; } = [];
    public IReadOnlyList<DependenciaChamadoAdminResponse> ChamadosBloqueados { get; init; } = [];
}

public sealed class RemoverChamadoRelacionamentoRequest
{
    public Guid? ChamadoId { get; init; }
    public Guid RelacionamentoId { get; init; }
    public string? Motivo { get; init; }
}

public sealed class RemoverChamadoRelacionamentoAdminRequest
{
    public string? MotivoRemocao { get; init; }
}
