namespace SGX.SistemaChamado.Application.Interfaces;

public sealed record UsuarioContextoAplicacao(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    IReadOnlyCollection<string> Perfis,
    IReadOnlyCollection<string>? Permissoes = null)
{
    public bool PossuiPerfil(string perfil) => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase);
    public bool PossuiQualquerPerfil(params string[] perfis) =>
        perfis.Any(perfil => Perfis.Contains(perfil, StringComparer.OrdinalIgnoreCase));

    public bool PossuiPermissao(string permissao)
        => Permissoes?.Contains(permissao, StringComparer.OrdinalIgnoreCase) == true;

    public bool PossuiAlgumaPermissao(params string[] permissoes)
        => permissoes.Any(PossuiPermissao);
}

public interface IUsuarioContextoAplicacaoService
{
    Task<UsuarioContextoAplicacao> ObterAsync(CancellationToken cancellationToken = default);
}
