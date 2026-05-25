import { httpClient } from './httpClient'
import type {
  AprovacaoChamadoDetalhe,
  AprovacaoChamadoListagemPaginada,
  CancelarAprovacaoChamadoRequest,
  DecidirAprovacaoChamadoRequest,
  FiltroAprovacaoChamadoRequest,
  SolicitarAprovacaoChamadoRequest,
} from '../types/aprovacaoChamados'

function buildQuery(params: FiltroAprovacaoChamadoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.chamadoId) search.set('chamadoId', params.chamadoId)
  if (typeof params.status === 'number') search.set('status', String(params.status))
  if (typeof params.tipoOrigem === 'number') search.set('tipoOrigem', String(params.tipoOrigem))
  if (params.solicitanteId) search.set('solicitanteId', params.solicitanteId)
  if (params.aprovadorId) search.set('aprovadorId', params.aprovadorId)
  if (params.dataSolicitacaoInicial) search.set('dataSolicitacaoInicial', params.dataSolicitacaoInicial)
  if (params.dataSolicitacaoFinal) search.set('dataSolicitacaoFinal', params.dataSolicitacaoFinal)
  if (params.dataDecisaoInicial) search.set('dataDecisaoInicial', params.dataDecisaoInicial)
  if (params.dataDecisaoFinal) search.set('dataDecisaoFinal', params.dataDecisaoFinal)
  if (params.termo) search.set('termo', params.termo)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const aprovacaoChamadosAdminService = {
  listar: (filtros: FiltroAprovacaoChamadoRequest = {}) =>
    httpClient.get<AprovacaoChamadoListagemPaginada>(`/api/admin/aprovacao-chamados${buildQuery(filtros)}`),

  obterPorId: (id: string) =>
    httpClient.get<AprovacaoChamadoDetalhe>(`/api/admin/aprovacao-chamados/${id}`),

  solicitar: (chamadoId: string, payload: SolicitarAprovacaoChamadoRequest) =>
    httpClient.post<AprovacaoChamadoDetalhe>(`/api/admin/chamados/${chamadoId}/aprovacao/solicitar`, payload),

  aprovar: (id: string, payload: DecidirAprovacaoChamadoRequest) =>
    httpClient.post<AprovacaoChamadoDetalhe>(`/api/admin/aprovacao-chamados/${id}/aprovar`, payload),

  reprovar: (id: string, payload: DecidirAprovacaoChamadoRequest) =>
    httpClient.post<AprovacaoChamadoDetalhe>(`/api/admin/aprovacao-chamados/${id}/reprovar`, payload),

  cancelar: (id: string, payload: CancelarAprovacaoChamadoRequest) =>
    httpClient.post<AprovacaoChamadoDetalhe>(`/api/admin/aprovacao-chamados/${id}/cancelar`, payload),
}
