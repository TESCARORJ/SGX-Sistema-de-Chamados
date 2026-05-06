using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Infrastructure.Storage;

public sealed class LocalArquivoStorageService(ArquivosOptions arquivosOptions) : IArquivoStorageService
{
    public async Task<ArquivoStorageResult> SalvarAsync(ArquivoStorageRequest request, CancellationToken cancellationToken = default)
    {
        var nomeFisico = Path.GetFileName(request.NomeFisico);
        var diretorioBase = Path.GetFullPath(arquivosOptions.DiretorioAnexos);
        Directory.CreateDirectory(diretorioBase);

        var caminhoAbsoluto = Path.Combine(diretorioBase, nomeFisico);
        var caminhoAbsolutoNormalizado = Path.GetFullPath(caminhoAbsoluto);

        if (!caminhoAbsolutoNormalizado.StartsWith(diretorioBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de anexo invalido.");
        }

        await using var fileStream = new FileStream(caminhoAbsolutoNormalizado, FileMode.Create, FileAccess.Write, FileShare.None);
        await request.Conteudo.CopyToAsync(fileStream, cancellationToken);

        var caminhoRelativo = Path.Combine(arquivosOptions.DiretorioAnexos, nomeFisico)
            .Replace('\\', '/');

        return new ArquivoStorageResult(caminhoRelativo, caminhoAbsolutoNormalizado);
    }

    public Task RemoverAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(caminhoRelativo))
        {
            return Task.CompletedTask;
        }

        var diretorioBase = Path.GetFullPath(arquivosOptions.DiretorioAnexos);
        var caminhoAbsoluto = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), caminhoRelativo));

        if (!caminhoAbsoluto.StartsWith(diretorioBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de anexo invalido.");
        }

        if (File.Exists(caminhoAbsoluto))
        {
            File.Delete(caminhoAbsoluto);
        }

        return Task.CompletedTask;
    }
}
