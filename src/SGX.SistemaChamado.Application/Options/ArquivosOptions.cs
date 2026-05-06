namespace SGX.SistemaChamado.Application.Options;

public sealed class ArquivosOptions
{
    public const string SectionName = "Arquivos";

    public string DiretorioAnexos { get; init; } = "storage/anexos";
    public long TamanhoMaximoBytes { get; init; } = 10 * 1024 * 1024;
    public string[] ContentTypesPermitidos { get; init; } = [];
}
