using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Infrastructure.Storage;

public sealed class LocalArquivoStorageService(ArquivosOptions arquivosOptions) : IArquivoStorageService
{
    public async Task<ArquivoStorageResult> SalvarAsync(ArquivoStorageRequest request, CancellationToken cancellationToken = default)
    {
        var nomeFisico = Path.GetFileName(request.NomeFisico);
        if (string.IsNullOrWhiteSpace(nomeFisico))
        {
            throw new InvalidOperationException("Nome fisico do anexo invalido.");
        }

        var diretorioBase = ObterDiretorioBaseNormalizado();
        Directory.CreateDirectory(diretorioBase);

        var caminhoAbsoluto = ResolverCaminhoSeguro(diretorioBase, Path.Combine(arquivosOptions.DiretorioAnexos, nomeFisico));
        await using var fileStream = new FileStream(caminhoAbsoluto, FileMode.Create, FileAccess.Write, FileShare.None);
        await request.Conteudo.CopyToAsync(fileStream, cancellationToken);

        var caminhoRelativo = Path.Combine(arquivosOptions.DiretorioAnexos, nomeFisico).Replace('\\', '/');
        return new ArquivoStorageResult(caminhoRelativo, caminhoAbsoluto);
    }

    public Task<Stream> AbrirLeituraAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        var diretorioBase = ObterDiretorioBaseNormalizado();
        var caminhoAbsoluto = ResolverCaminhoSeguro(diretorioBase, caminhoRelativo);

        if (!File.Exists(caminhoAbsoluto))
        {
            throw new FileNotFoundException("Arquivo fisico nao encontrado.");
        }

        Stream stream = new FileStream(caminhoAbsoluto, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task RemoverAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(caminhoRelativo))
        {
            return Task.CompletedTask;
        }

        var diretorioBase = ObterDiretorioBaseNormalizado();
        var caminhoAbsoluto = ResolverCaminhoSeguro(diretorioBase, caminhoRelativo);

        if (File.Exists(caminhoAbsoluto))
        {
            File.Delete(caminhoAbsoluto);
        }

        return Task.CompletedTask;
    }

    private string ObterDiretorioBaseNormalizado()
    {
        var diretorioBase = Path.GetFullPath(arquivosOptions.DiretorioAnexos);
        Directory.CreateDirectory(diretorioBase);
        return diretorioBase;
    }

    private static string ResolverCaminhoSeguro(string diretorioBaseNormalizado, string caminhoRelativo)
    {
        if (string.IsNullOrWhiteSpace(caminhoRelativo))
        {
            throw new InvalidOperationException("Caminho de anexo invalido.");
        }

        var caminhoAbsoluto = Path.GetFullPath(caminhoRelativo);
        if (!Path.IsPathRooted(caminhoRelativo))
        {
            caminhoAbsoluto = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), caminhoRelativo));
        }

        var baseComSeparador = diretorioBaseNormalizado.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!caminhoAbsoluto.StartsWith(baseComSeparador, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Caminho de anexo invalido.");
        }

        return caminhoAbsoluto;
    }
}
