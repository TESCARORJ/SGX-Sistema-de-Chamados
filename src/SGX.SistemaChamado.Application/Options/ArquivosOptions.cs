namespace SGX.SistemaChamado.Application.Options;

public sealed class ArquivosOptions
{
    public const string SectionName = "Arquivos";

    public string DiretorioAnexos { get; init; } = "storage/anexos";
    public long TamanhoMaximoBytes { get; init; } = 10 * 1024 * 1024;
    public string[] ExtensoesPermitidas { get; init; } =
    [
        ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv", ".zip"
    ];
    public string[] ExtensoesBloqueadas { get; init; } =
    [
        ".exe", ".bat", ".cmd", ".ps1", ".sh", ".js", ".vbs", ".msi", ".dll", ".scr", ".com", ".jar", ".hta", ".reg"
    ];
    public string[] ContentTypesPermitidos { get; init; } = [];
}
