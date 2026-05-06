namespace SGX.SistemaChamado.Api.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Authentication";

    public bool ModoLocalHabilitado { get; init; }
    public string AdminLocalEmail { get; init; } = "admin.local@sgx.local";
    public string AdminLocalNome { get; init; } = "Administrador Local";
}
