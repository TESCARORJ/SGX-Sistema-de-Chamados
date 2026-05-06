import { httpClient } from './httpClient'
import type {
  AdminContextoResponse,
  AlterarCategoriaChamadoPayload,
  AlterarPrioridadeChamadoPayload,
  AlterarStatusChamadoPayload,
  AtribuirChamadoPayload,
  ChamadoAdminDetalhe,
  ComentarioAdminChamadoPayload,
  EncerrarChamadoPayload,
  FiltroChamadosAdmin,
  ListaChamadosAdminResponse,
  ReabrirChamadoPayload,
} from '../types/admin'
import type { ComentarioChamado } from '../types/comentario'

function buildQuery(params: FiltroChamadosAdmin): string {
  const search = new URLSearchParams()

  if (params.statusId) search.set('statusId', params.statusId)
  if (params.prioridadeId) search.set('prioridadeId', params.prioridadeId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.responsavelId) search.set('responsavelId', params.responsavelId)
  if (params.solicitanteId) search.set('solicitanteId', params.solicitanteId)
  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (typeof params.slaVencido === 'boolean') search.set('slaVencido', String(params.slaVencido))
  if (params.texto) search.set('texto', params.texto)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const adminService = {
  obterAdminContexto: () => httpClient.get<AdminContextoResponse>('/api/admin/contexto'),

  listarChamadosAdmin: (filtros: FiltroChamadosAdmin = {}) =>
    httpClient.get<ListaChamadosAdminResponse>(`/api/admin/chamados${buildQuery(filtros)}`),

  obterChamadoAdmin: (id: string) => httpClient.get<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}`),

  assumirChamado: (id: string) => httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/assumir`),

  atribuirChamado: (id: string, payload: AtribuirChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/atribuir`, payload),

  alterarStatus: (id: string, payload: AlterarStatusChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/alterar-status`, payload),

  alterarPrioridade: (id: string, payload: AlterarPrioridadeChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/alterar-prioridade`, payload),

  alterarCategoria: (id: string, payload: AlterarCategoriaChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/alterar-categoria`, payload),

  comentarChamadoAdmin: (id: string, payload: ComentarioAdminChamadoPayload) =>
    httpClient.post<ComentarioChamado>(`/api/admin/chamados/${id}/comentarios`, payload),

  encerrarChamado: (id: string, payload: EncerrarChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/encerrar`, payload),

  reabrirChamado: (id: string, payload: ReabrirChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/reabrir`, payload),
}
