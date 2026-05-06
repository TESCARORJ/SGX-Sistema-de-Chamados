namespace SGX.SistemaChamado.Api.Services;

public interface IUsuarioAtualService
{
    Task<UsuarioAutenticadoContexto> ObterAsync(CancellationToken cancellationToken = default);
}
