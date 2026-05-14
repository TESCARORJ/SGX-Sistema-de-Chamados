namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed class AlterarSenhaLocalRequest
{
    public string SenhaAtual { get; init; } = string.Empty;
    public string NovaSenha { get; init; } = string.Empty;
    public string ConfirmacaoNovaSenha { get; init; } = string.Empty;
}
