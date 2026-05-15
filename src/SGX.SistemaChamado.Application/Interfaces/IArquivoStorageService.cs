namespace SGX.SistemaChamado.Application.Interfaces;

public sealed record ArquivoStorageRequest(
    string NomeFisico,
    Stream Conteudo);

public sealed record ArquivoStorageResult(
    string CaminhoRelativo,
    string CaminhoAbsoluto);

public interface IArquivoStorageService
{
    Task<ArquivoStorageResult> SalvarAsync(ArquivoStorageRequest request, CancellationToken cancellationToken = default);
    Task<Stream> AbrirLeituraAsync(string caminhoRelativo, CancellationToken cancellationToken = default);
    Task RemoverAsync(string caminhoRelativo, CancellationToken cancellationToken = default);
}
