namespace SGX.SistemaChamado.Api.Contracts.Admin;

public sealed class RedefinirSenhaUsuarioAdminRequest
{
    public string NovaSenha { get; init; } = string.Empty;
    public string ConfirmarNovaSenha { get; init; } = string.Empty;
    public bool DeveAlterarSenha { get; init; } = true;
}

