import { httpClient } from './httpClient'
import type {
  AlterarLeituraNotificacaoResponse,
  ContagemMinhasNotificacoesNaoLidasResponse,
  ListarMinhasNotificacoesRequest,
  ListarMinhasNotificacoesResponse,
  MinhaNotificacaoDetalheResponse,
} from '../types/notificacoes'

function buildQuery(params: ListarMinhasNotificacoesRequest): string {
  const search = new URLSearchParams()

  search.set('pagina', String(params.pagina))
  search.set('tamanhoPagina', String(params.tamanhoPagina))

  if (typeof params.lida === 'boolean') {
    search.set('lida', String(params.lida))
  }

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const notificacoesService = {
  listarMinhasNotificacoes: (params: ListarMinhasNotificacoesRequest) =>
    httpClient.get<ListarMinhasNotificacoesResponse>(`/api/notificacoes/minhas${buildQuery(params)}`),

  obterMinhaNotificacao: (id: string) =>
    httpClient.get<MinhaNotificacaoDetalheResponse>(`/api/notificacoes/minhas/${id}`),

  contarMinhasNotificacoesNaoLidas: () =>
    httpClient.get<ContagemMinhasNotificacoesNaoLidasResponse>('/api/notificacoes/minhas/nao-lidas/contagem'),

  marcarMinhaNotificacaoComoLida: (id: string) =>
    httpClient.patch<AlterarLeituraNotificacaoResponse>(`/api/notificacoes/minhas/${id}/lida`),

  marcarMinhaNotificacaoComoNaoLida: (id: string) =>
    httpClient.patch<AlterarLeituraNotificacaoResponse>(`/api/notificacoes/minhas/${id}/nao-lida`),
}
