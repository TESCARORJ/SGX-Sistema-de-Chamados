import { httpClient } from './httpClient'
import type {
  AtualizarPoliticaSlaRequest,
  AtualizarConfiguracaoAlertaSlaRequest,
  AtualizarCalendarioCorporativoRequest,
  AtualizarStatusCalendarioCorporativoRequest,
  AtualizarStatusPoliticaSlaRequest,
  CalendarioCorporativoResponse,
  ConfiguracaoAlertaSlaResponse,
  CriarCalendarioCorporativoRequest,
  CriarPoliticaSlaRequest,
  ExcecaoCalendarioCorporativoRequest,
  FiltroDashboardSlaRequest,
  FiltroPoliticaSlaRequest,
  HorarioAtendimentoCalendarioRequest,
  PoliticaSlaResponse,
  SlaDashboardResponse,
} from '../types/slaPolicies'

function buildQuery(params: FiltroPoliticaSlaRequest = {}): string {
  const search = new URLSearchParams()
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.texto) search.set('texto', params.texto)
  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildDashboardQuery(params: FiltroDashboardSlaRequest = {}): string {
  const search = new URLSearchParams()
  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (params.prioridadeId) search.set('prioridadeId', params.prioridadeId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.situacaoSla) search.set('situacaoSla', params.situacaoSla)
  const query = search.toString()
  return query ? `?${query}` : ''
}

export const slaPoliciesService = {
  listar: (filtros: FiltroPoliticaSlaRequest = {}) =>
    httpClient.get<PoliticaSlaResponse[]>(`/api/admin/sla/policies${buildQuery(filtros)}`),
  obterPorId: (id: string) => httpClient.get<PoliticaSlaResponse>(`/api/admin/sla/policies/${id}`),
  criar: (payload: CriarPoliticaSlaRequest) => httpClient.post<PoliticaSlaResponse>('/api/admin/sla/policies', payload),
  atualizar: (id: string, payload: AtualizarPoliticaSlaRequest) =>
    httpClient.put<PoliticaSlaResponse>(`/api/admin/sla/policies/${id}`, payload),
  atualizarStatus: (id: string, payload: AtualizarStatusPoliticaSlaRequest) =>
    httpClient.patch<PoliticaSlaResponse>(`/api/admin/sla/policies/${id}/status`, payload),
  inativar: (id: string) => httpClient.delete<PoliticaSlaResponse>(`/api/admin/sla/policies/${id}`),
  obterConfiguracaoAlertas: () => httpClient.get<ConfiguracaoAlertaSlaResponse>('/api/admin/sla/alert-config'),
  atualizarConfiguracaoAlertas: (payload: AtualizarConfiguracaoAlertaSlaRequest) =>
    httpClient.put<ConfiguracaoAlertaSlaResponse>('/api/admin/sla/alert-config', payload),
  obterDashboard: (filtros: FiltroDashboardSlaRequest = {}) =>
    httpClient.get<SlaDashboardResponse>(`/api/admin/sla/dashboard${buildDashboardQuery(filtros)}`),
  listarCalendarios: () => httpClient.get<CalendarioCorporativoResponse[]>('/api/admin/sla/calendars'),
  obterCalendario: (id: string) => httpClient.get<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}`),
  criarCalendario: (payload: CriarCalendarioCorporativoRequest) =>
    httpClient.post<CalendarioCorporativoResponse>('/api/admin/sla/calendars', payload),
  atualizarCalendario: (id: string, payload: AtualizarCalendarioCorporativoRequest) =>
    httpClient.put<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}`, payload),
  atualizarStatusCalendario: (id: string, payload: AtualizarStatusCalendarioCorporativoRequest) =>
    httpClient.patch<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/status`, payload),
  definirCalendarioPadrao: (id: string) =>
    httpClient.patch<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/default`, {}),
  criarHorarioCalendario: (id: string, payload: HorarioAtendimentoCalendarioRequest) =>
    httpClient.post<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/schedules`, payload),
  atualizarHorarioCalendario: (id: string, scheduleId: string, payload: HorarioAtendimentoCalendarioRequest) =>
    httpClient.put<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/schedules/${scheduleId}`, payload),
  excluirHorarioCalendario: (id: string, scheduleId: string) =>
    httpClient.delete<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/schedules/${scheduleId}`),
  criarExcecaoCalendario: (id: string, payload: ExcecaoCalendarioCorporativoRequest) =>
    httpClient.post<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/exceptions`, payload),
  atualizarExcecaoCalendario: (id: string, exceptionId: string, payload: ExcecaoCalendarioCorporativoRequest) =>
    httpClient.put<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/exceptions/${exceptionId}`, payload),
  excluirExcecaoCalendario: (id: string, exceptionId: string) =>
    httpClient.delete<CalendarioCorporativoResponse>(`/api/admin/sla/calendars/${id}/exceptions/${exceptionId}`),
}
