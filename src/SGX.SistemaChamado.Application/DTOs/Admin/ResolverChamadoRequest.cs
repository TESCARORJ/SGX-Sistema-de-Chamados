namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed record ResolverChamadoRequest
{
    public string? Solucao { get; init; }
    public bool ComentarioInterno { get; init; }
}
