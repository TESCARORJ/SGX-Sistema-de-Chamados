import { httpClient } from './httpClient'
import type { AlterarSituacaoCadastroResponse } from '../types/adminCadastros'
import type {
  AtualizarCatalogoServicoRequest,
  CatalogoServicoDetalhe,
  CatalogoServicoPaginado,
  CriarCatalogoServicoRequest,
  FiltroCatalogoServicoRequest,
} from '../types/catalogoServicos'

function buildQuery(params: FiltroCatalogoServicoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (params.departamentoResponsavelId) search.set('departamentoResponsavelId', params.departamentoResponsavelId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.subcategoriaId) search.set('subcategoriaId', params.subcategoriaId)
  if (params.prioridadePadraoId) search.set('prioridadePadraoId', params.prioridadePadraoId)
  if (params.slaPadraoId) search.set('slaPadraoId', params.slaPadraoId)
  if (params.politicaSlaId) search.set('politicaSlaId', params.politicaSlaId)
  if (typeof params.status === 'number') search.set('status', String(params.status))
  if (typeof params.visibilidade === 'number') search.set('visibilidade', String(params.visibilidade))
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (typeof params.permiteAberturaChamado === 'boolean') search.set('permiteAberturaChamado', String(params.permiteAberturaChamado))
  if (typeof params.requerAprovacao === 'boolean') search.set('requerAprovacao', String(params.requerAprovacao))
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const catalogoServicosAdminService = {
  listarServicos: (filtros: FiltroCatalogoServicoRequest = {}) =>
    httpClient.get<CatalogoServicoPaginado>(`/api/admin/catalogo-servicos${buildQuery(filtros)}`),

  obterServico: (id: string) =>
    httpClient.get<CatalogoServicoDetalhe>(`/api/admin/catalogo-servicos/${id}`),

  criarServico: (payload: CriarCatalogoServicoRequest) =>
    httpClient.post<CatalogoServicoDetalhe>('/api/admin/catalogo-servicos', payload),

  atualizarServico: (id: string, payload: AtualizarCatalogoServicoRequest) =>
    httpClient.put<CatalogoServicoDetalhe>(`/api/admin/catalogo-servicos/${id}`, payload),

  publicarServico: (id: string) =>
    httpClient.post<CatalogoServicoDetalhe>(`/api/admin/catalogo-servicos/${id}/publicar`),

  arquivarServico: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/catalogo-servicos/${id}/arquivar`),

  reativarServico: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/catalogo-servicos/${id}/reativar`),
}
