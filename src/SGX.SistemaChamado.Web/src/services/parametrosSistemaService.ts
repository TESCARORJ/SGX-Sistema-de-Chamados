import { httpClient } from './httpClient'
import type { AlterarSituacaoCadastroResponse, FiltroCadastroRequest, PagedResultResponse } from '../types/adminCadastros'
import type {
  AtualizarParametroSistemaRequest,
  CriarParametroSistemaRequest,
  ParametroSistemaDetalheResponse,
  ParametroSistemaResumoResponse,
} from '../types/parametroSistema'

function buildQuery(params: FiltroCadastroRequest = {}): string {
  const search = new URLSearchParams()
  if (params.texto) search.set('texto', params.texto)
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)
  const query = search.toString()
  return query ? `?${query}` : ''
}

export const parametrosSistemaService = {
  listar: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<ParametroSistemaResumoResponse>>(`/api/admin/configuracoes/parametros${buildQuery(filtros)}`),
  obterPorId: (id: string) => httpClient.get<ParametroSistemaDetalheResponse>(`/api/admin/configuracoes/parametros/${id}`),
  criar: (payload: CriarParametroSistemaRequest) =>
    httpClient.post<ParametroSistemaDetalheResponse>('/api/admin/configuracoes/parametros', payload),
  atualizar: (id: string, payload: AtualizarParametroSistemaRequest) =>
    httpClient.put<ParametroSistemaDetalheResponse>(`/api/admin/configuracoes/parametros/${id}`, payload),
  inativar: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/configuracoes/parametros/${id}/inativar`),
  reativar: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/configuracoes/parametros/${id}/reativar`),
}
