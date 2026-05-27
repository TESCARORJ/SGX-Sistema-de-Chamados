using SGX.SistemaChamado.Application.Interfaces;

namespace SGX.SistemaChamado.Api.Services;

public sealed class UsuarioContextoAplicacaoService(IUsuarioAtualService usuarioAtualService)
    : IUsuarioContextoAplicacaoService
{
    public async Task<UsuarioContextoAplicacao> ObterAsync(CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioAtualService.ObterAsync(cancellationToken);
        return new UsuarioContextoAplicacao(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            usuario.Perfis,
            usuario.Permissoes);
    }
}
