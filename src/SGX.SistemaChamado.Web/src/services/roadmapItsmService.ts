import { httpClient } from './httpClient'
import type {
  AtualizarRoadmapImplementacaoFuturaRequest,
  AtualizarRoadmapCategoriaRequest,
  AtualizarRoadmapChecklistItemRequest,
  AtualizarRoadmapItsmItemRequest,
  AtualizarStatusRoadmapItsmRequest,
  CriarRoadmapImplementacaoFuturaRequest,
  CriarRoadmapCategoriaRequest,
  CriarRoadmapChecklistItemRequest,
  CriarRoadmapItsmItemRequest,
  FiltroRoadmapCategoriaRequest,
  FiltroRoadmapImplementacaoFuturaRequest,
  FiltroRoadmapItsmRequest,
  PagedResultResponse,
  RoadmapCategoriaResponse,
  RoadmapChecklistItemResponse,
  RoadmapImplementacaoFuturaResponse,
  RoadmapItsmDetalheResponse,
  RoadmapItsmResumoResponse,
} from '../types/roadmapItsm'

function buildQueryRoadmap(params: FiltroRoadmapItsmRequest = {}): string {
  const search = new URLSearchParams()
  if (typeof params.status === 'number') search.set('status', String(params.status))
  if (typeof params.prioridade === 'number') search.set('prioridade', String(params.prioridade))
  if (typeof params.impacto === 'number') search.set('impacto', String(params.impacto))
  if (params.roadmapCategoriaId) search.set('roadmapCategoriaId', params.roadmapCategoriaId)
  if (params.categoria) search.set('categoria', params.categoria)
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildQueryImplementacoes(params: FiltroRoadmapImplementacaoFuturaRequest = {}): string {
  const search = new URLSearchParams()
  if (params.roadmapItemId) search.set('roadmapItemId', params.roadmapItemId)
  if (params.texto) search.set('texto', params.texto)
  if (typeof params.tipo === 'number') search.set('tipo', String(params.tipo))
  if (typeof params.prioridade === 'number') search.set('prioridade', String(params.prioridade))
  if (typeof params.status === 'number') search.set('status', String(params.status))
  if (params.responsavel) search.set('responsavel', params.responsavel)
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (typeof params.pagina === 'number') search.set('pagina', String(params.pagina))
  if (typeof params.tamanhoPagina === 'number') search.set('tamanhoPagina', String(params.tamanhoPagina))
  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildQueryCategorias(params: FiltroRoadmapCategoriaRequest = {}): string {
  const search = new URLSearchParams()
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.texto) search.set('texto', params.texto)
  const query = search.toString()
  return query ? `?${query}` : ''
}

export const roadmapItsmService = {
  listar: (filtros: FiltroRoadmapItsmRequest = {}) =>
    httpClient.get<RoadmapItsmResumoResponse[]>(`/api/admin/roadmap${buildQueryRoadmap(filtros)}`),
  obterPorId: (id: string) => httpClient.get<RoadmapItsmDetalheResponse>(`/api/admin/roadmap/${id}`),
  criar: (payload: CriarRoadmapItsmItemRequest) => httpClient.post<RoadmapItsmDetalheResponse>('/api/admin/roadmap', payload),
  atualizar: (id: string, payload: AtualizarRoadmapItsmItemRequest) =>
    httpClient.put<RoadmapItsmDetalheResponse>(`/api/admin/roadmap/${id}`, payload),
  atualizarStatus: (id: string, payload: AtualizarStatusRoadmapItsmRequest) =>
    httpClient.patch<RoadmapItsmDetalheResponse>(`/api/admin/roadmap/${id}/status`, payload),
  inativar: (id: string) => httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/${id}/inativar`),
  reativar: (id: string) => httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/${id}/reativar`),
  listarCategorias: (filtros: FiltroRoadmapCategoriaRequest = {}) =>
    httpClient.get<RoadmapCategoriaResponse[]>(`/api/admin/roadmap/categorias${buildQueryCategorias(filtros)}`),
  obterCategoriaPorId: (id: string) => httpClient.get<RoadmapCategoriaResponse>(`/api/admin/roadmap/categorias/${id}`),
  criarCategoria: (payload: CriarRoadmapCategoriaRequest) =>
    httpClient.post<RoadmapCategoriaResponse>('/api/admin/roadmap/categorias', payload),
  atualizarCategoria: (id: string, payload: AtualizarRoadmapCategoriaRequest) =>
    httpClient.put<RoadmapCategoriaResponse>(`/api/admin/roadmap/categorias/${id}`, payload),
  inativarCategoria: (id: string) => httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/categorias/${id}/inativar`),
  reativarCategoria: (id: string) => httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/categorias/${id}/reativar`),
  listarChecklistPorItem: (roadmapItemId: string) =>
    httpClient.get<RoadmapChecklistItemResponse[]>(`/api/admin/roadmap/${roadmapItemId}/checklist`),
  criarChecklist: (roadmapItemId: string, payload: CriarRoadmapChecklistItemRequest) =>
    httpClient.post<RoadmapChecklistItemResponse>(`/api/admin/roadmap/${roadmapItemId}/checklist`, payload),
  atualizarChecklist: (id: string, payload: AtualizarRoadmapChecklistItemRequest) =>
    httpClient.put<RoadmapChecklistItemResponse>(`/api/admin/roadmap/checklist/${id}`, payload),
  concluirChecklist: (id: string) =>
    httpClient.post<RoadmapChecklistItemResponse>(`/api/admin/roadmap/checklist/${id}/concluir`),
  reabrirChecklist: (id: string) =>
    httpClient.post<RoadmapChecklistItemResponse>(`/api/admin/roadmap/checklist/${id}/reabrir`),
  inativarChecklist: (id: string) =>
    httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/checklist/${id}/inativar`),
  excluirChecklist: (id: string) =>
    httpClient.delete<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/checklist/${id}`),
  reativarChecklist: (id: string) =>
    httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/checklist/${id}/reativar`),
  listarImplementacoes: (filtros: FiltroRoadmapImplementacaoFuturaRequest = {}) =>
    httpClient.get<PagedResultResponse<RoadmapImplementacaoFuturaResponse>>(
      `/api/admin/roadmap/implementacoes${buildQueryImplementacoes(filtros)}`
    ),
  listarImplementacoesPorItem: (roadmapItemId: string) =>
    httpClient.get<RoadmapImplementacaoFuturaResponse[]>(`/api/admin/roadmap/${roadmapItemId}/implementacoes`),
  obterImplementacaoPorId: (id: string) =>
    httpClient.get<RoadmapImplementacaoFuturaResponse>(`/api/admin/roadmap/implementacoes/${id}`),
  criarImplementacao: (payload: CriarRoadmapImplementacaoFuturaRequest) =>
    httpClient.post<RoadmapImplementacaoFuturaResponse>('/api/admin/roadmap/implementacoes', payload),
  atualizarImplementacao: (id: string, payload: AtualizarRoadmapImplementacaoFuturaRequest) =>
    httpClient.put<RoadmapImplementacaoFuturaResponse>(`/api/admin/roadmap/implementacoes/${id}`, payload),
  concluirImplementacao: (id: string) =>
    httpClient.post<RoadmapImplementacaoFuturaResponse>(`/api/admin/roadmap/implementacoes/${id}/concluir`),
  inativarImplementacao: (id: string) =>
    httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/implementacoes/${id}/inativar`),
  reativarImplementacao: (id: string) =>
    httpClient.post<{ id: string; ativo: boolean; mensagem: string }>(`/api/admin/roadmap/implementacoes/${id}/reativar`),
}
