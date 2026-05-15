using SGX.SistemaChamado.Application.Interfaces.Auditoria;

namespace SGX.SistemaChamado.Infrastructure.Services;

public sealed class AuditoriaContextProviderNulo : IAuditoriaContextProvider
{
    public ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default)
    {
        var contexto = new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        return ValueTask.FromResult(contexto);
    }
}
