namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed class RecuperarSenhaRedefinicaoRequest
{
    public string Token { get; init; } = string.Empty;
    public string NovaSenha { get; init; } = string.Empty;
    public string ConfirmacaoNovaSenha { get; init; } = string.Empty;
}
