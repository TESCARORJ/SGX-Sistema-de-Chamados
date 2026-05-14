namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed record LocalLoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string AutenticadoPor,
    bool DeveAlterarSenha = false);
