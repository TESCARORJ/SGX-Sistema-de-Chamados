import { httpClient } from './httpClient'
import type {
  ChamadoCriadoResponse,
  ChamadoDetalhePortal,
  ComentarChamadoPayload,
  CriarChamadoRequest,
  FiltroChamadosPortal,
  ListaChamadosPortalResponse,
  PortalContextoResponse,
} from '../types/portal'
import type { ComentarioChamado } from '../types/comentario'
import type { AnexoChamado } from '../types/anexo'

function buildQuery(params: FiltroChamadosPortal): string {
  const search = new URLSearchParams()

  if (params.statusId) search.set('statusId', params.statusId)
  if (params.prioridadeId) search.set('prioridadeId', params.prioridadeId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.dataInicial) search.set('dataInicial', params.dataInicial)
  if (params.dataFinal) search.set('dataFinal', params.dataFinal)
  if (params.texto) search.set('texto', params.texto)
  if (params.visaoAmpliada) search.set('visaoAmpliada', 'true')
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const portalService = {
  getPortalContexto: () => httpClient.get<PortalContextoResponse>('/api/portal/contexto'),

  listarMeusChamados: (filtros: FiltroChamadosPortal = {}) =>
    httpClient.get<ListaChamadosPortalResponse>(`/api/portal/chamados${buildQuery(filtros)}`),

  obterChamado: (id: string) => httpClient.get<ChamadoDetalhePortal>(`/api/portal/chamados/${id}`),

  criarChamado: (payload: CriarChamadoRequest) =>
    httpClient.post<ChamadoCriadoResponse>('/api/portal/chamados', payload),

  comentarChamado: (id: string, payload: ComentarChamadoPayload) =>
    httpClient.post<ComentarioChamado>(`/api/portal/chamados/${id}/comentarios`, payload),

  anexarArquivo: (id: string, file: File) => {
    const form = new FormData()
    form.append('arquivo', file)
    return httpClient.post<AnexoChamado>(`/api/portal/chamados/${id}/anexos`, form)
  },

  obterPortalContexto: () => httpClient.get<PortalContextoResponse>('/api/portal/contexto'),
  abrirChamado: (payload: CriarChamadoRequest) =>
    httpClient.post<ChamadoCriadoResponse>('/api/portal/chamados', payload),
}
