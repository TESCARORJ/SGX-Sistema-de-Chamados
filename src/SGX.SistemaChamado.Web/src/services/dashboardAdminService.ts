import { httpClient } from './httpClient'
import type { DashboardAdminResponse } from '../types/dashboard'
import type {
  ChamadosPorCategoria,
  ChamadosPorPrioridade,
  ChamadosPorStatus,
  FiltroIndicadoresRequest,
  IndicadoresSla,
  ProdutividadeAtendente,
} from '../types/indicadores'

function buildQuery(params: FiltroIndicadoresRequest): string {
  const search = new URLSearchParams()

  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.responsavelId) search.set('responsavelId', params.responsavelId)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const dashboardAdminService = {
  obterDashboard: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<DashboardAdminResponse>(`/api/admin/dashboard${buildQuery(filtros)}`),

  obterChamadosPorStatus: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<ChamadosPorStatus[]>(`/api/admin/indicadores/chamados-por-status${buildQuery(filtros)}`),

  obterChamadosPorPrioridade: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<ChamadosPorPrioridade[]>(`/api/admin/indicadores/chamados-por-prioridade${buildQuery(filtros)}`),

  obterChamadosPorCategoria: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<ChamadosPorCategoria[]>(`/api/admin/indicadores/chamados-por-categoria${buildQuery(filtros)}`),

  obterIndicadoresSla: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<IndicadoresSla>(`/api/admin/indicadores/sla${buildQuery(filtros)}`),

  obterProdutividade: (filtros: FiltroIndicadoresRequest = {}) =>
    httpClient.get<ProdutividadeAtendente[]>(`/api/admin/indicadores/produtividade${buildQuery(filtros)}`),
}
