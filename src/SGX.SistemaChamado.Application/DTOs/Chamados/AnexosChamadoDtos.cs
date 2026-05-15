namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed record AnexoChamadoResponse(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    DateTime CriadoEm,
    Guid UsuarioId,
    string Usuario);

public sealed record UploadAnexoChamadoResponse(
    Guid Id,
    string NomeArquivo,
    string ContentType,
    long TamanhoBytes,
    DateTime CriadoEm,
    Guid UsuarioId,
    string Usuario);

public sealed class CriarAnexoChamadoRequest
{
    public string NomeArquivo { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long TamanhoBytes { get; init; }
    public Stream Conteudo { get; init; } = Stream.Null;
}

public sealed class DownloadAnexoChamadoResponse
{
    public required Stream Conteudo { get; init; }
    public required string NomeArquivo { get; init; }
    public required string ContentType { get; init; }
}
