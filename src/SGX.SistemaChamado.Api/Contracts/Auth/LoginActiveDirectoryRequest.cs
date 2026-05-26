namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed class LoginActiveDirectoryRequest
{
    public string Usuario { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string Dominio { get; init; } = string.Empty;
}
