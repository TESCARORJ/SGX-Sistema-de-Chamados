using System.Diagnostics;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;

namespace SGX.SistemaChamado.Worker.Email.Services;

public sealed class WorkerAuditoriaContextProvider(IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IAuditoriaContextProvider
{
    public async ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var correlacaoId = Activity.Current?.TraceId.ToString();

        return new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            null,
            "SGX.Worker.Email",
            string.IsNullOrWhiteSpace(correlacaoId) ? Guid.NewGuid().ToString("N") : correlacaoId);
    }
}
