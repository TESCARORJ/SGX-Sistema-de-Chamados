namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed class RecuperarSenhaSolicitacaoRequest
{
    public string Email { get; init; } = string.Empty;
}
