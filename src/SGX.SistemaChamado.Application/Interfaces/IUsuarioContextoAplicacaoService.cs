namespace SGX.SistemaChamado.Application.Interfaces;

public sealed record UsuarioContextoAplicacao(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    IReadOnlyCollection<string> Perfis)
{
    public bool PossuiPerfil(string perfil) => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase);
    public bool PossuiQualquerPerfil(params string[] perfis) =>
        perfis.Any(perfil => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase));
}

public interface IUsuarioContextoAplicacaoService
{
    Task<UsuarioContextoAplicacao> ObterAsync(CancellationToken cancellationToken = default);
}
