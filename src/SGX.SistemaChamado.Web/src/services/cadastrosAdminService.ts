import { httpClient } from './httpClient'
import type {
  AlterarSituacaoCadastroResponse,
  AtualizarCategoriaChamadoRequest,
  AtualizarDepartamentoRequest,
  AtualizarLocalUnidadeRequest,
  AtualizarPerfilAcessoRequest,
  AtualizarPermissoesPerfilRequest,
  AtualizarPrioridadeChamadoRequest,
  AtualizarStatusChamadoRequest,
  AtualizarSubcategoriaChamadoRequest,
  AtualizarTipoSolicitacaoRequest,
  CategoriaChamadoDetalheResponse,
  CategoriaChamadoResumoResponse,
  CriarCategoriaChamadoRequest,
  CriarDepartamentoRequest,
  CriarLocalUnidadeRequest,
  CriarPerfilAcessoRequest,
  CriarPrioridadeChamadoRequest,
  CriarStatusChamadoRequest,
  CriarSubcategoriaChamadoRequest,
  CriarTipoSolicitacaoRequest,
  DepartamentoDetalheResponse,
  DepartamentoResumoResponse,
  FiltroCadastroRequest,
  LocalUnidadeDetalheResponse,
  LocalUnidadeResumoResponse,
  PagedResultResponse,
  PerfilAcessoDetalheResponse,
  PerfilAcessoResumoResponse,
  PerfilPermissoes,
  PermissaoSistema,
  PrioridadeChamadoDetalheResponse,
  PrioridadeChamadoResumoResponse,
  StatusChamadoDetalheResponse,
  StatusChamadoResumoResponse,
  SubcategoriaChamadoDetalheResponse,
  SubcategoriaChamadoResumoResponse,
  TipoSolicitacaoDetalheResponse,
  TipoSolicitacaoResumoResponse,
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

function normalizarPrioridade<T extends PrioridadeChamadoResumoResponse | PrioridadeChamadoDetalheResponse>(item: T): T {
  return {
    ...item,
    nivel: item.nivel ?? item.peso,
  }
}

export const cadastrosAdminService = {
  listarPerfis: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<PerfilAcessoResumoResponse>>(`/api/admin/perfis${buildQuery(filtros)}`),
  obterPerfilPorId: (id: string) => httpClient.get<PerfilAcessoDetalheResponse>(`/api/admin/perfis/${id}`),
  criarPerfil: (payload: CriarPerfilAcessoRequest) => httpClient.post<PerfilAcessoDetalheResponse>('/api/admin/perfis', payload),
  atualizarPerfil: (id: string, payload: AtualizarPerfilAcessoRequest) =>
    httpClient.put<PerfilAcessoDetalheResponse>(`/api/admin/perfis/${id}`, payload),
  listarPermissoes: () => httpClient.get<PermissaoSistema[]>('/api/admin/permissoes'),
  obterPermissoesPerfil: (id: string) => httpClient.get<PerfilPermissoes>(`/api/admin/perfis/${id}/permissoes`),
  atualizarPermissoesPerfil: (id: string, payload: AtualizarPermissoesPerfilRequest) =>
    httpClient.put<PerfilPermissoes>(`/api/admin/perfis/${id}/permissoes`, payload),
  inativarPerfil: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/perfis/${id}/inativar`),
  reativarPerfil: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/perfis/${id}/reativar`),

  listarDepartamentos: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<DepartamentoResumoResponse>>(`/api/admin/departamentos${buildQuery(filtros)}`),
  obterDepartamentoPorId: (id: string) => httpClient.get<DepartamentoDetalheResponse>(`/api/admin/departamentos/${id}`),
  criarDepartamento: (payload: CriarDepartamentoRequest) =>
    httpClient.post<DepartamentoDetalheResponse>('/api/admin/departamentos', payload),
  atualizarDepartamento: (id: string, payload: AtualizarDepartamentoRequest) =>
    httpClient.put<DepartamentoDetalheResponse>(`/api/admin/departamentos/${id}`, payload),
  inativarDepartamento: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/departamentos/${id}/inativar`),
  reativarDepartamento: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/departamentos/${id}/reativar`),

  listarCategorias: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<CategoriaChamadoResumoResponse>>(`/api/admin/categorias${buildQuery(filtros)}`),
  obterCategoriaPorId: (id: string) => httpClient.get<CategoriaChamadoDetalheResponse>(`/api/admin/categorias/${id}`),
  criarCategoria: (payload: CriarCategoriaChamadoRequest) => httpClient.post<CategoriaChamadoDetalheResponse>('/api/admin/categorias', payload),
  atualizarCategoria: (id: string, payload: AtualizarCategoriaChamadoRequest) =>
    httpClient.put<CategoriaChamadoDetalheResponse>(`/api/admin/categorias/${id}`, payload),
  inativarCategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/categorias/${id}/inativar`),
  reativarCategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/categorias/${id}/reativar`),

  listarSubcategorias: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<SubcategoriaChamadoResumoResponse>>(`/api/admin/subcategorias${buildQuery(filtros)}`),
  listarSubcategoriasPorCategoria: (categoriaId: string, ativo?: boolean) => {
    const query = typeof ativo === 'boolean' ? `?ativo=${ativo}` : ''
    return httpClient.get<SubcategoriaChamadoResumoResponse[]>(`/api/admin/categorias/${categoriaId}/subcategorias${query}`)
  },
  obterSubcategoriaPorId: (id: string) => httpClient.get<SubcategoriaChamadoDetalheResponse>(`/api/admin/subcategorias/${id}`),
  criarSubcategoria: (payload: CriarSubcategoriaChamadoRequest) =>
    httpClient.post<SubcategoriaChamadoDetalheResponse>('/api/admin/subcategorias', payload),
  atualizarSubcategoria: (id: string, payload: AtualizarSubcategoriaChamadoRequest) =>
    httpClient.put<SubcategoriaChamadoDetalheResponse>(`/api/admin/subcategorias/${id}`, payload),
  inativarSubcategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/subcategorias/${id}/inativar`),
  reativarSubcategoria: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/subcategorias/${id}/reativar`),

  listarPrioridades: async (filtros: FiltroCadastroRequest = {}) => {
    const response = await httpClient.get<PagedResultResponse<PrioridadeChamadoResumoResponse>>(`/api/admin/prioridades${buildQuery(filtros)}`)
    return {
      ...response,
      items: response.items.map(normalizarPrioridade),
    }
  },
  obterPrioridadePorId: async (id: string) => {
    const response = await httpClient.get<PrioridadeChamadoDetalheResponse>(`/api/admin/prioridades/${id}`)
    return normalizarPrioridade(response)
  },
  criarPrioridade: (payload: CriarPrioridadeChamadoRequest) =>
    httpClient.post<PrioridadeChamadoDetalheResponse>('/api/admin/prioridades', payload),
  atualizarPrioridade: (id: string, payload: AtualizarPrioridadeChamadoRequest) =>
    httpClient.put<PrioridadeChamadoDetalheResponse>(`/api/admin/prioridades/${id}`, payload),
  inativarPrioridade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/prioridades/${id}/inativar`),
  reativarPrioridade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/prioridades/${id}/reativar`),

  listarTiposSolicitacao: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<TipoSolicitacaoResumoResponse>>(`/api/admin/tipos-solicitacao${buildQuery(filtros)}`),
  obterTipoSolicitacaoPorId: (id: string) => httpClient.get<TipoSolicitacaoDetalheResponse>(`/api/admin/tipos-solicitacao/${id}`),
  criarTipoSolicitacao: (payload: CriarTipoSolicitacaoRequest) =>
    httpClient.post<TipoSolicitacaoDetalheResponse>('/api/admin/tipos-solicitacao', payload),
  atualizarTipoSolicitacao: (id: string, payload: AtualizarTipoSolicitacaoRequest) =>
    httpClient.put<TipoSolicitacaoDetalheResponse>(`/api/admin/tipos-solicitacao/${id}`, payload),
  inativarTipoSolicitacao: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/tipos-solicitacao/${id}/inativar`),
  reativarTipoSolicitacao: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/tipos-solicitacao/${id}/reativar`),

  listarLocaisUnidade: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<LocalUnidadeResumoResponse>>(`/api/admin/locais${buildQuery(filtros)}`),
  obterLocalUnidadePorId: (id: string) => httpClient.get<LocalUnidadeDetalheResponse>(`/api/admin/locais/${id}`),
  criarLocalUnidade: (payload: CriarLocalUnidadeRequest) =>
    httpClient.post<LocalUnidadeDetalheResponse>('/api/admin/locais', payload),
  atualizarLocalUnidade: (id: string, payload: AtualizarLocalUnidadeRequest) =>
    httpClient.put<LocalUnidadeDetalheResponse>(`/api/admin/locais/${id}`, payload),
  inativarLocalUnidade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/locais/${id}/inativar`),
  reativarLocalUnidade: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/locais/${id}/reativar`),

  listarStatus: (filtros: FiltroCadastroRequest = {}) =>
    httpClient.get<PagedResultResponse<StatusChamadoResumoResponse>>(`/api/admin/status${buildQuery(filtros)}`),
  obterStatusPorId: (id: string) => httpClient.get<StatusChamadoDetalheResponse>(`/api/admin/status/${id}`),
  criarStatus: (payload: CriarStatusChamadoRequest) => httpClient.post<StatusChamadoDetalheResponse>('/api/admin/status', payload),
  atualizarStatus: (id: string, payload: AtualizarStatusChamadoRequest) =>
    httpClient.put<StatusChamadoDetalheResponse>(`/api/admin/status/${id}`, payload),
  inativarStatus: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/status/${id}/inativar`),
  reativarStatus: (id: string) => httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/status/${id}/reativar`),
}
