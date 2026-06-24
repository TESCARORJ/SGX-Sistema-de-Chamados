namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface INotificacaoProcessamentoRepository
{
    Task<bool> TentarIniciarProcessamentoAsync(
        Guid notificacaoId,
        DateTime iniciadaEm,
        string atualizadoPor,
        Guid? atualizadoPorUsuarioId,
        int limiteTentativas,
        CancellationToken cancellationToken = default);

    Task<bool> TentarRegistrarSucessoAsync(
        Guid notificacaoId,
        DateTime enviadaEm,
        string atualizadoPor,
        Guid? atualizadoPorUsuarioId,
        CancellationToken cancellationToken = default);
}
