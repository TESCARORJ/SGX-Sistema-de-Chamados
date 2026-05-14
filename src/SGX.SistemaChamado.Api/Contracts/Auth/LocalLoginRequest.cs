namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed class LocalLoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}
