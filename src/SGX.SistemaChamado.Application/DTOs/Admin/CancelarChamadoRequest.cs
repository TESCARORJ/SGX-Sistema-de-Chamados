namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class CancelarChamadoRequest
{
    public required string Motivo { get; init; }
    public bool ComentarioInterno { get; init; }
}
