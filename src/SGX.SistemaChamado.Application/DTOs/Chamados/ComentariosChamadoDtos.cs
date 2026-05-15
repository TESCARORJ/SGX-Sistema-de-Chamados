namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed class CriarComentarioChamadoRequest
{
    public string Mensagem { get; init; } = string.Empty;
    public bool Interno { get; init; }
}

public sealed record ComentarioChamadoResponse(
    Guid Id,
    Guid UsuarioId,
    string Usuario,
    string Mensagem,
    bool Interno,
    DateTime CriadoEm);
