import { httpClient } from './httpClient'
import type {
  AlterarPerfisUsuarioRequest,
  AlterarSituacaoCadastroResponse,
  AtualizarUsuarioAdminRequest,
  CriarUsuarioAdminRequest,
  FiltroCadastroRequest,
  PagedResultResponse,
  UsuarioAdminDetalheResponse,
  UsuarioAdminResumoResponse,
} from '../types/adminCadastros'

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

export const usuariosAdminService = {
  listar: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<UsuarioAdminResumoResponse>>(`/api/admin/cadastros/usuarios${buildQuery(filtros)}`),
  obterPorId: (id: string) => httpClient.get<UsuarioAdminDetalheResponse>(`/api/admin/cadastros/usuarios/${id}`),
  criar: (payload: CriarUsuarioAdminRequest) => httpClient.post<UsuarioAdminDetalheResponse>('/api/admin/cadastros/usuarios', payload),
  atualizar: (id: string, payload: AtualizarUsuarioAdminRequest) =>
    httpClient.put<UsuarioAdminDetalheResponse>(`/api/admin/cadastros/usuarios/${id}`, payload),
  alterarPerfis: (id: string, payload: AlterarPerfisUsuarioRequest) =>
    httpClient.put<UsuarioAdminDetalheResponse>(`/api/admin/cadastros/usuarios/${id}/perfis`, payload),
  inativar: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/usuarios/${id}/inativar`),
  reativar: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/usuarios/${id}/reativar`),
}
