namespace SGX.SistemaChamado.Application.Interfaces.Auditoria;

public sealed record ContextoAuditoriaAtual(
    DateTime DataEventoUtc,
    Guid? UsuarioId,
    string? UsuarioNome,
    string? UsuarioEmail,
    string? UsuarioLogin,
    string? IpOrigem,
    string? UserAgent,
    string? CorrelacaoId);

public interface IAuditoriaContextProvider
{
    ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default);
}
