import { httpClient } from './httpClient'
import type {
  FiltroAuditoriaAutenticacaoRequest,
  ListaEventosAuditoriaAutenticacaoResponse,
} from '../types/auditoriaAutenticacao'

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

function buildQuery(filtros: FiltroAuditoriaAutenticacaoRequest): string {
  const search = new URLSearchParams()

  adicionarParametro(search, 'dataInicio', filtros.dataInicio)
  adicionarParametro(search, 'dataFim', filtros.dataFim)
  adicionarParametro(search, 'usuarioId', filtros.usuarioId)
  adicionarParametro(search, 'usuarioEmail', filtros.usuarioEmail)
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
  adicionarParametro(search, 'provedor', filtros.provedor)
  adicionarParametro(search, 'tipoEventoAutenticacao', filtros.tipoEventoAutenticacao)
  adicionarParametro(search, 'resultadoAutenticacao', filtros.resultadoAutenticacao)
  adicionarParametro(search, 'pagina', filtros.pagina)
  adicionarParametro(search, 'tamanhoPagina', filtros.tamanhoPagina)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const auditoriaAutenticacaoService = {
  listarEventos: (filtros: FiltroAuditoriaAutenticacaoRequest) =>
    httpClient.get<ListaEventosAuditoriaAutenticacaoResponse>(
      `/api/admin/auditoria/autenticacao${buildQuery(filtros)}`
    ),
}
