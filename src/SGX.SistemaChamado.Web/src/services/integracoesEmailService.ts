import { HttpRequestError, httpClient } from './httpClient'
import type {
  FiltroLogsEmailRequest,
  ListaLogsIntegracaoEmailResponse,
  LogIntegracaoEmailDetalheResponse,
} from '../types/integracaoEmail'

function buildQuery(params: FiltroLogsEmailRequest): string {
  const search = new URLSearchParams()

  if (params.dataInicial) search.set('dataInicial', params.dataInicial)
  if (params.dataFinal) search.set('dataFinal', params.dataFinal)
  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (params.status) search.set('status', params.status)
  if (params.remetente) search.set('remetente', params.remetente)
  if (params.chamadoId) search.set('chamadoId', params.chamadoId)
  if (params.codigoChamado) search.set('codigoChamado', params.codigoChamado)
  if (params.assunto) search.set('assunto', params.assunto)
  if (params.messageId) search.set('messageId', params.messageId)
  if (params.texto) search.set('texto', params.texto)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcao) search.set('direcao', params.direcao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const integracoesEmailService = {
  async listarLogsEmail(filtros: FiltroLogsEmailRequest = {}): Promise<ListaLogsIntegracaoEmailResponse> {
    try {
      return await httpClient.get<ListaLogsIntegracaoEmailResponse>(`/api/admin/integracoes/email/logs${buildQuery(filtros)}`)
    } catch (error) {
      if (error instanceof HttpRequestError && error.status === 403) {
        throw new Error('Voce nao possui permissao para visualizar os logs de integracao de e-mail.')
      }

      throw new Error('Nao foi possivel carregar os logs de integracao de e-mail.')
    }
  },

  async obterLogEmail(id: string): Promise<LogIntegracaoEmailDetalheResponse> {
    try {
      return await httpClient.get<LogIntegracaoEmailDetalheResponse>(`/api/admin/integracoes/email/logs/${id}`)
    } catch (error) {
      if (error instanceof HttpRequestError && error.status === 403) {
        throw new Error('Voce nao possui permissao para visualizar os logs de integracao de e-mail.')
      }

      throw new Error('Nao foi possivel carregar os logs de integracao de e-mail.')
    }
  },

  listarLogs(filtros: FiltroLogsEmailRequest = {}) {
    return this.listarLogsEmail(filtros)
  },

  obterLog(id: string) {
    return this.obterLogEmail(id)
  },
}
