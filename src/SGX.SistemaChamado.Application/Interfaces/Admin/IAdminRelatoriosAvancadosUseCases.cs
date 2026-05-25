using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IAdminRelatoriosAvancadosUseCases
{
    Task<RelatorioMetadadosDto> ObterMetadadosAsync(CancellationToken cancellationToken = default);
    Task<RelatorioChamadosResumoDto> ObterResumoChamadosAsync(FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioChamadosSerieTemporalDto> ObterSerieTemporalChamadosAsync(FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioChamadosDistribuicaoDto> ObterDistribuicaoChamadosAsync(FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioAtendimentoProdutividadeDto> ObterProdutividadeAtendimentoAsync(FiltroRelatorioAtendimentoRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioSlaResumoDto> ObterResumoSlaAsync(FiltroRelatorioSlaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioSlaViolacaoDto>> ObterViolacoesSlaAsync(FiltroRelatorioSlaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioSlaPorDepartamentoDto>> ObterSlaPorDepartamentoAsync(FiltroRelatorioSlaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioSlaPorPrioridadeDto>> ObterSlaPorPrioridadeAsync(FiltroRelatorioSlaRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioAprovacoesResumoDto> ObterResumoAprovacoesAsync(FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioAprovacoesTempoMedioDto>> ObterTempoMedioAprovacoesAsync(FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioAprovacoesPorOrigemDto>> ObterAprovacoesPorOrigemAsync(FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioCatalogoServicosResumoDto> ObterResumoCatalogoServicosAsync(FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioCatalogoServicosMaisSolicitadosDto>> ObterCatalogoServicosMaisSolicitadosAsync(FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioCatalogoServicosPorDepartamentoDto>> ObterCatalogoServicosPorDepartamentoAsync(FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioInventarioAtivosResumoDto> ObterResumoInventarioAtivosAsync(FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioInventarioAtivosPorStatusDto> ObterInventarioAtivosPorStatusAsync(FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioInventarioAtivosChamadosRecorrentesDto>> ObterInventarioAtivosChamadosRecorrentesAsync(FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioInventarioAtivosPorDepartamentoDto>> ObterInventarioAtivosPorDepartamentoAsync(FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioBaseConhecimentoResumoDto> ObterResumoBaseConhecimentoAsync(FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioBaseConhecimentoPorStatusDto> ObterBaseConhecimentoPorStatusAsync(FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioBaseConhecimentoVinculosChamadosDto>> ObterBaseConhecimentoVinculosChamadosAsync(FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken = default);
    Task<RelatorioAuditoriaResumoDto> ObterResumoAuditoriaAsync(FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioAuditoriaPorUsuarioDto>> ObterAuditoriaPorUsuarioAsync(FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RelatorioAuditoriaPorEntidadeDto>> ObterAuditoriaPorEntidadeAsync(FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken = default);
}
