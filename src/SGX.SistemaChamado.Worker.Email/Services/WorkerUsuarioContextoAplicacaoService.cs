using SGX.SistemaChamado.Application.Interfaces;

namespace SGX.SistemaChamado.Worker.Email.Services;

public sealed class WorkerUsuarioContextoAplicacaoService : IUsuarioContextoAplicacaoService
{
    private static readonly UsuarioContextoAplicacao Contexto = new(
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        "Integracao E-mail",
        "integracao.email@sgx.local",
        "integracao.email.worker",
        ["Administrador"]);

    public Task<UsuarioContextoAplicacao> ObterAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Contexto);
}
