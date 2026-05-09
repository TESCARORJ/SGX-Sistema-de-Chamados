import { httpClient } from './httpClient'
import type {
  AlterarSituacaoCadastroResponse,
  AtualizarPermissoesPerfilRequest,
  AtualizarCategoriaChamadoRequest,
  AtualizarDepartamentoRequest,
  AtualizarPerfilAcessoRequest,
  AtualizarPrioridadeChamadoRequest,
  AtualizarStatusChamadoRequest,
  CategoriaChamadoDetalheResponse,
  CategoriaChamadoResumoResponse,
  CriarCategoriaChamadoRequest,
  CriarDepartamentoRequest,
  CriarPerfilAcessoRequest,
  CriarPrioridadeChamadoRequest,
  CriarStatusChamadoRequest,
  DepartamentoDetalheResponse,
  DepartamentoResumoResponse,
  FiltroCadastroRequest,
  PagedResultResponse,
  PerfilPermissoes,
  PerfilAcessoDetalheResponse,
  PerfilAcessoResumoResponse,
  PermissaoSistema,
  PrioridadeChamadoDetalheResponse,
  PrioridadeChamadoResumoResponse,
  StatusChamadoDetalheResponse,
  StatusChamadoResumoResponse,
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

export const cadastrosAdminService = {
  listarPerfis: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<PerfilAcessoResumoResponse>>(`/api/admin/cadastros/perfis${buildQuery(filtros)}`),
  obterPerfilPorId: (id: string) => httpClient.get<PerfilAcessoDetalheResponse>(`/api/admin/cadastros/perfis/${id}`),
  criarPerfil: (payload: CriarPerfilAcessoRequest) => httpClient.post<PerfilAcessoDetalheResponse>('/api/admin/cadastros/perfis', payload),
  atualizarPerfil: (id: string, payload: AtualizarPerfilAcessoRequest) =>
    httpClient.put<PerfilAcessoDetalheResponse>(`/api/admin/cadastros/perfis/${id}`, payload),
  listarPermissoes: () => httpClient.get<PermissaoSistema[]>('/api/admin/cadastros/permissoes'),
  obterPermissoesPerfil: (id: string) => httpClient.get<PerfilPermissoes>(`/api/admin/cadastros/perfis/${id}/permissoes`),
  atualizarPermissoesPerfil: (id: string, payload: AtualizarPermissoesPerfilRequest) =>
    httpClient.put<PerfilPermissoes>(`/api/admin/cadastros/perfis/${id}/permissoes`, payload),
  inativarPerfil: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/perfis/${id}/inativar`),
  reativarPerfil: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/perfis/${id}/reativar`),

  listarDepartamentos: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<DepartamentoResumoResponse>>(`/api/admin/cadastros/departamentos${buildQuery(filtros)}`),
  obterDepartamentoPorId: (id: string) => httpClient.get<DepartamentoDetalheResponse>(`/api/admin/cadastros/departamentos/${id}`),
  criarDepartamento: (payload: CriarDepartamentoRequest) =>
    httpClient.post<DepartamentoDetalheResponse>('/api/admin/cadastros/departamentos', payload),
  atualizarDepartamento: (id: string, payload: AtualizarDepartamentoRequest) =>
    httpClient.put<DepartamentoDetalheResponse>(`/api/admin/cadastros/departamentos/${id}`, payload),
  inativarDepartamento: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/departamentos/${id}/inativar`),
  reativarDepartamento: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/departamentos/${id}/reativar`),

  listarCategorias: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<CategoriaChamadoResumoResponse>>(`/api/admin/cadastros/categorias${buildQuery(filtros)}`),
  obterCategoriaPorId: (id: string) => httpClient.get<CategoriaChamadoDetalheResponse>(`/api/admin/cadastros/categorias/${id}`),
  criarCategoria: (payload: CriarCategoriaChamadoRequest) => httpClient.post<CategoriaChamadoDetalheResponse>('/api/admin/cadastros/categorias', payload),
  atualizarCategoria: (id: string, payload: AtualizarCategoriaChamadoRequest) =>
    httpClient.put<CategoriaChamadoDetalheResponse>(`/api/admin/cadastros/categorias/${id}`, payload),
  inativarCategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/categorias/${id}/inativar`),
  reativarCategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/categorias/${id}/reativar`),

  listarPrioridades: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<PrioridadeChamadoResumoResponse>>(`/api/admin/cadastros/prioridades${buildQuery(filtros)}`),
  obterPrioridadePorId: (id: string) => httpClient.get<PrioridadeChamadoDetalheResponse>(`/api/admin/cadastros/prioridades/${id}`),
  criarPrioridade: (payload: CriarPrioridadeChamadoRequest) =>
    httpClient.post<PrioridadeChamadoDetalheResponse>('/api/admin/cadastros/prioridades', payload),
  atualizarPrioridade: (id: string, payload: AtualizarPrioridadeChamadoRequest) =>
    httpClient.put<PrioridadeChamadoDetalheResponse>(`/api/admin/cadastros/prioridades/${id}`, payload),
  inativarPrioridade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/prioridades/${id}/inativar`),
  reativarPrioridade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/prioridades/${id}/reativar`),

  listarStatus: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<StatusChamadoResumoResponse>>(`/api/admin/cadastros/status${buildQuery(filtros)}`),
  obterStatusPorId: (id: string) => httpClient.get<StatusChamadoDetalheResponse>(`/api/admin/cadastros/status/${id}`),
  criarStatus: (payload: CriarStatusChamadoRequest) => httpClient.post<StatusChamadoDetalheResponse>('/api/admin/cadastros/status', payload),
  atualizarStatus: (id: string, payload: AtualizarStatusChamadoRequest) =>
    httpClient.put<StatusChamadoDetalheResponse>(`/api/admin/cadastros/status/${id}`, payload),
  inativarStatus: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/status/${id}/inativar`),
  reativarStatus: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/cadastros/status/${id}/reativar`),
}
