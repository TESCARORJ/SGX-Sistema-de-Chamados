using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class CriarChamadoTarefaAdminRequest
{
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public Guid? ResponsavelUsuarioId { get; init; }
    public DateTime? Prazo { get; init; }
}

public sealed class AtualizarStatusChamadoTarefaAdminRequest
{
    public StatusTarefaChamadoEnum Status { get; init; }
}

public sealed class CancelarChamadoTarefaAdminRequest
{
    public string? MotivoCancelamento { get; init; }
}

public sealed class ChamadoTarefaAdminResponse
{
    public Guid Id { get; init; }
    public Guid ChamadoId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public StatusTarefaChamadoEnum Status { get; init; }
    public string StatusDescricao { get; init; } = string.Empty;
    public Guid? ResponsavelUsuarioId { get; init; }
    public string? ResponsavelNome { get; init; }
    public DateTime? Prazo { get; init; }
    public DateTime CriadoEm { get; init; }
    public string CriadoPor { get; init; } = string.Empty;
    public DateTime? AtualizadoEm { get; init; }
    public DateTime? ConcluidoEm { get; init; }
    public DateTime? CanceladoEm { get; init; }
    public string? MotivoCancelamento { get; init; }
    public bool Ativo { get; init; }
}
