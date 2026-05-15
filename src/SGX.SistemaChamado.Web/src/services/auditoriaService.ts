import { httpClient } from './httpClient'
import type {
  AuditoriaDashboardResponse,
  EventoAuditoriaDetalhe,
  FiltroAuditoriaRequest,
  FiltroDashboardAuditoriaRequest,
  ResultadoPaginadoEventoAuditoria,
} from '../types/auditoria'

function adicionarParametro(search: URLSearchParams, chave: string, valor: unknown): void {
  if (valor === undefined || valor === null) {
    return
  }

  const texto = String(valor).trim()
  if (!texto) {
    return
  }

  search.set(chave, texto)
}

function buildQueryEventos(filtros: FiltroAuditoriaRequest): string {
  const search = new URLSearchParams()

  adicionarParametro(search, 'dataInicio', filtros.dataInicio)
  adicionarParametro(search, 'dataFim', filtros.dataFim)
  adicionarParametro(search, 'usuarioId', filtros.usuarioId)
  adicionarParametro(search, 'usuarioEmail', filtros.usuarioEmail)
  adicionarParametro(search, 'modulo', filtros.modulo)
  adicionarParametro(search, 'entidade', filtros.entidade)
  adicionarParametro(search, 'entidadeId', filtros.entidadeId)
  adicionarParametro(search, 'acao', filtros.acao)
  adicionarParametro(search, 'nivel', filtros.nivel)
  if (typeof filtros.sucesso === 'boolean') {
    search.set('sucesso', String(filtros.sucesso))
  }
  adicionarParametro(search, 'ipOrigem', filtros.ipOrigem)
  adicionarParametro(search, 'correlacaoId', filtros.correlacaoId)
  adicionarParametro(search, 'texto', filtros.texto)
  adicionarParametro(search, 'pagina', filtros.pagina)
  adicionarParametro(search, 'tamanhoPagina', filtros.tamanhoPagina)

  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildQueryDashboard(filtros: FiltroDashboardAuditoriaRequest): string {
  const search = new URLSearchParams()

  adicionarParametro(search, 'dataInicio', filtros.dataInicio)
  adicionarParametro(search, 'dataFim', filtros.dataFim)
  adicionarParametro(search, 'modulo', filtros.modulo)
  adicionarParametro(search, 'usuarioEmail', filtros.usuarioEmail)
  adicionarParametro(search, 'nivel', filtros.nivel)
  if (typeof filtros.sucesso === 'boolean') {
    search.set('sucesso', String(filtros.sucesso))
  }

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const auditoriaService = {
  listarEventos: (filtros: FiltroAuditoriaRequest) =>
    httpClient.get<ResultadoPaginadoEventoAuditoria>(`/api/admin/auditoria/eventos${buildQueryEventos(filtros)}`),

  obterEvento: (id: string) =>
    httpClient.get<EventoAuditoriaDetalhe>(`/api/admin/auditoria/eventos/${id}`),

  obterDashboard: (filtros: FiltroDashboardAuditoriaRequest) =>
    httpClient.get<AuditoriaDashboardResponse>(`/api/admin/auditoria/dashboard${buildQueryDashboard(filtros)}`),
}
