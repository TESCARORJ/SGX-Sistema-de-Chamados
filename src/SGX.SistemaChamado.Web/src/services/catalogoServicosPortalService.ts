import { httpClient } from './httpClient'
import type {
  PortalCatalogoServicoDetalhe,
  PortalCatalogoServicoPaginado,
  PortalFiltroCatalogoServicoRequest,
  PortalPrepararChamadoCatalogoServico,
} from '../types/catalogoServicos'

function buildQuery(params: PortalFiltroCatalogoServicoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (params.departamentoResponsavelId) search.set('departamentoResponsavelId', params.departamentoResponsavelId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.subcategoriaId) search.set('subcategoriaId', params.subcategoriaId)
  if (typeof params.permiteAberturaChamado === 'boolean') search.set('permiteAberturaChamado', String(params.permiteAberturaChamado))
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const catalogoServicosPortalService = {
  listarServicos: (filtros: PortalFiltroCatalogoServicoRequest = {}) =>
    httpClient.get<PortalCatalogoServicoPaginado>(`/api/portal/catalogo-servicos${buildQuery(filtros)}`),

  obterServicoPorSlug: (slug: string) =>
    httpClient.get<PortalCatalogoServicoDetalhe>(`/api/portal/catalogo-servicos/${slug}`),

  prepararAberturaChamado: (slug: string) =>
    httpClient.get<PortalPrepararChamadoCatalogoServico>(`/api/portal/catalogo-servicos/${slug}/preparar-chamado`),
}
