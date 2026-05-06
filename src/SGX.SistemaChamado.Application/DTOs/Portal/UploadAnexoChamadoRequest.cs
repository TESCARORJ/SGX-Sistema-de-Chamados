namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class UploadAnexoChamadoRequest
{
    public string NomeArquivo { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long TamanhoBytes { get; init; }
    public Stream Conteudo { get; init; } = Stream.Null;
}
