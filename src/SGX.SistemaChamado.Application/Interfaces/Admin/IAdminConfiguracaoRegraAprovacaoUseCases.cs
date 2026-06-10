using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminConfiguracaoRegraAprovacaoUseCases
{
    Task<PagedResultResponse<ConfiguracaoRegraAprovacaoResumoResponse>> ListarAsync(
        ListarConfiguracoesRegrasAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<ConfiguracaoRegraAprovacaoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ConfiguracaoRegraAprovacaoResponse> CriarAsync(
        CriarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<ConfiguracaoRegraAprovacaoResponse> AtualizarAsync(
        Guid id,
        AtualizarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<AlterarSituacaoCadastroResponse> AlterarStatusAsync(
        Guid id,
        AlterarStatusConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<ValidarConfiguracaoRegraAprovacaoResponse> ValidarAsync(
        ValidarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RegraAprovacaoCandidataResponse>> ListarRegrasCandidatasAsync(
        ContextoAvaliacaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);

    Task<AvaliacaoConfiguracaoRegraAprovacaoResponse> AvaliarRegraAsync(
        ContextoAvaliacaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default);
}
