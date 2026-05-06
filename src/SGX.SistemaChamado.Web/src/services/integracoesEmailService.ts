import { httpClient } from './httpClient'
import type {
  FiltroLogsEmailRequest,
  ListaLogsIntegracaoEmailResponse,
  LogIntegracaoEmailDetalheResponse,
} from '../types/integracaoEmail'

function buildQuery(params: FiltroLogsEmailRequest): string {
  const search = new URLSearchParams()

  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (params.status) search.set('status', params.status)
  if (params.remetente) search.set('remetente', params.remetente)
  if (params.chamadoId) search.set('chamadoId', params.chamadoId)
  if (params.texto) search.set('texto', params.texto)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const integracoesEmailService = {
  listarLogs: (filtros: FiltroLogsEmailRequest = {}) =>
    httpClient.get<ListaLogsIntegracaoEmailResponse>(`/api/admin/integracoes/email/logs${buildQuery(filtros)}`),

  obterLog: (id: string) =>
    httpClient.get<LogIntegracaoEmailDetalheResponse>(`/api/admin/integracoes/email/logs/${id}`),
}
