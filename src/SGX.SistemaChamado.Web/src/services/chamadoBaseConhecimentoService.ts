import { httpClient } from './httpClient'
import type {
  ArtigosConhecimentoDisponiveisResponse,
  BuscarArtigosConhecimentoParaVinculoFiltro,
  ChamadoArtigoConhecimento,
} from '../types/admin'
import type { AlterarSituacaoCadastroResponse } from '../types/adminCadastros'

function buildQuery(params: BuscarArtigosConhecimentoParaVinculoFiltro = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.page) search.set('page', String(params.page))
  if (params.pageSize) search.set('pageSize', String(params.pageSize))

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const chamadoBaseConhecimentoService = {
  listarArtigosDoChamado: (chamadoId: string) =>
    httpClient.get<ChamadoArtigoConhecimento[]>(`/api/admin/chamados/${chamadoId}/artigos-conhecimento`),

  vincularArtigoAoChamado: (chamadoId: string, artigoId: string, observacao?: string) =>
    httpClient.post<ChamadoArtigoConhecimento>(`/api/admin/chamados/${chamadoId}/artigos-conhecimento/${artigoId}`, {
      observacao: observacao || null,
    }),

  removerArtigoDoChamado: (chamadoId: string, artigoId: string) =>
    httpClient.delete<AlterarSituacaoCadastroResponse>(`/api/admin/chamados/${chamadoId}/artigos-conhecimento/${artigoId}`),

  buscarArtigosDisponiveisParaVinculo: (chamadoId: string, filtros: BuscarArtigosConhecimentoParaVinculoFiltro = {}) =>
    httpClient.get<ArtigosConhecimentoDisponiveisResponse>(
      `/api/admin/chamados/${chamadoId}/artigos-conhecimento/disponiveis${buildQuery(filtros)}`
    ),
}
