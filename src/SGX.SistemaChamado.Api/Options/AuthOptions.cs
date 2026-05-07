namespace SGX.SistemaChamado.Api.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Authentication";

    public bool ModoLocalHabilitado { get; init; }
    public string AdminLocalEmail { get; init; } = "admin@sgxdigital.com";
    public string AdminLocalNome { get; init; } = "Administrador SGX";
}
