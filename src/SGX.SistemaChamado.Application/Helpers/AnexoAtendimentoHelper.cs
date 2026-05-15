using SGX.SistemaChamado.Application.Options;

namespace SGX.SistemaChamado.Application.Helpers;

public static class AnexoAtendimentoHelper
{
    public static (string NomeArquivoOriginalSeguro, string Extensao) ValidarUpload(
        string nomeArquivo,
        string? contentType,
        long tamanhoBytes,
        ArquivosOptions options)
    {
        if (tamanhoBytes <= 0)
        {
            throw new InvalidOperationException("Arquivo invalido: tamanho deve ser maior que zero.");
        }

        if (tamanhoBytes > options.TamanhoMaximoBytes)
        {
            throw new InvalidOperationException($"Arquivo excede o limite maximo de {options.TamanhoMaximoBytes} bytes.");
        }

        if (string.IsNullOrWhiteSpace(nomeArquivo))
        {
            throw new InvalidOperationException("Nome do arquivo obrigatorio.");
        }

        var nomeOriginalSeguro = Path.GetFileName(nomeArquivo).Trim();
        if (string.IsNullOrWhiteSpace(nomeOriginalSeguro))
        {
            throw new InvalidOperationException("Nome de arquivo invalido.");
        }

        var extensao = Path.GetExtension(nomeOriginalSeguro).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extensao))
        {
            throw new InvalidOperationException("Extensao de arquivo obrigatoria.");
        }

        var extensoesBloqueadas = options.ExtensoesBloqueadas
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (extensoesBloqueadas.Contains(extensao))
        {
            throw new InvalidOperationException("Extensao de arquivo bloqueada por seguranca.");
        }

        var extensoesPermitidas = options.ExtensoesPermitidas
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (extensoesPermitidas.Count > 0 && !extensoesPermitidas.Contains(extensao))
        {
            throw new InvalidOperationException("Extensao de arquivo nao permitida.");
        }

        var contentTypeNormalizado = (contentType ?? string.Empty).Trim().ToLowerInvariant();
        var mediaType = contentTypeNormalizado.Split(';', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? string.Empty;
        var contentTypesPermitidos = options.ContentTypesPermitidos
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var isOctetStream = string.Equals(mediaType, "application/octet-stream", StringComparison.OrdinalIgnoreCase);
        if (contentTypesPermitidos.Count > 0 && !contentTypesPermitidos.Contains(mediaType) && !isOctetStream)
        {
            throw new InvalidOperationException("Content type nao permitido.");
        }

        return (nomeOriginalSeguro, extensao);
    }

    public static string GerarNomeFisicoSeguro(string extensao)
        => $"{Guid.NewGuid():N}{extensao.ToLowerInvariant()}";
}
