namespace SGX.SistemaChamado.Api.Services;

public sealed record UsuarioAutenticadoContexto(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    string Situacao,
    Guid? DepartamentoId,
    string AutenticadoPor,
    IReadOnlyCollection<string> Perfis,
    IReadOnlyCollection<string> Permissoes,
    bool DeveAlterarSenha = false)
{
    public bool PossuiPerfil(string perfil) => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase);

    public bool PossuiQualquerPerfil(params string[] perfis) =>
        perfis.Any(perfil => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase));
}
