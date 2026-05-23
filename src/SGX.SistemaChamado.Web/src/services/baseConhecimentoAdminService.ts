import { httpClient } from './httpClient'
import type { AlterarSituacaoCadastroResponse } from '../types/adminCadastros'
import type {
  AtualizarBaseConhecimentoArtigoRequest,
  BaseConhecimentoArtigoDetalhe,
  BaseConhecimentoArtigoPaginado,
  CriarBaseConhecimentoArtigoRequest,
  FiltroBaseConhecimentoArtigoRequest,
} from '../types/baseConhecimento'

function buildQuery(params: FiltroBaseConhecimentoArtigoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (typeof params.status === 'number') search.set('status', String(params.status))
  if (typeof params.visibilidade === 'number') search.set('visibilidade', String(params.visibilidade))
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const baseConhecimentoAdminService = {
  listarArtigos: (filtros: FiltroBaseConhecimentoArtigoRequest = {}) =>
    httpClient.get<BaseConhecimentoArtigoPaginado>(`/api/admin/base-conhecimento/artigos${buildQuery(filtros)}`),

  obterArtigo: (id: string) =>
    httpClient.get<BaseConhecimentoArtigoDetalhe>(`/api/admin/base-conhecimento/artigos/${id}`),

  criarArtigo: (payload: CriarBaseConhecimentoArtigoRequest) =>
    httpClient.post<BaseConhecimentoArtigoDetalhe>('/api/admin/base-conhecimento/artigos', payload),

  atualizarArtigo: (id: string, payload: AtualizarBaseConhecimentoArtigoRequest) =>
    httpClient.put<BaseConhecimentoArtigoDetalhe>(`/api/admin/base-conhecimento/artigos/${id}`, payload),

  publicarArtigo: (id: string) =>
    httpClient.post<BaseConhecimentoArtigoDetalhe>(`/api/admin/base-conhecimento/artigos/${id}/publicar`),

  arquivarArtigo: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/base-conhecimento/artigos/${id}/arquivar`),

  reativarArtigo: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/base-conhecimento/artigos/${id}/reativar`),
}
