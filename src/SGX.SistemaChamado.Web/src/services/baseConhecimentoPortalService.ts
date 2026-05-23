import { httpClient } from './httpClient'
import type {
  PortalBaseConhecimentoArtigoDetalhe,
  PortalBaseConhecimentoArtigoPaginado,
  PortalFiltroBaseConhecimentoRequest,
} from '../types/baseConhecimento'

function buildQuery(params: PortalFiltroBaseConhecimentoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const baseConhecimentoPortalService = {
  listarArtigos: (filtros: PortalFiltroBaseConhecimentoRequest = {}) =>
    httpClient.get<PortalBaseConhecimentoArtigoPaginado>(`/api/portal/base-conhecimento/artigos${buildQuery(filtros)}`),

  obterArtigoPorSlug: (slug: string) =>
    httpClient.get<PortalBaseConhecimentoArtigoDetalhe>(`/api/portal/base-conhecimento/artigos/${slug}`),
}
