import { httpClient } from './httpClient'
import type { AlterarSituacaoCadastroResponse } from '../types/adminCadastros'
import type {
  AtualizarInventarioAtivoRequest,
  ChamadoRelacionadoInventarioAtivoPaginado,
  CriarInventarioAtivoRequest,
  FiltroChamadosRelacionadosInventarioAtivoRequest,
  FiltroHistoricoInventarioAtivoRequest,
  FiltroInventarioAtivoRequest,
  HistoricoInventarioAtivoPaginado,
  InventarioAtivoDetalhe,
  InventarioAtivoPaginado,
  MovimentarInventarioAtivoRequest,
  TipoAtivoInventario,
} from '../types/inventarioAtivos'

function buildQuery(params: FiltroInventarioAtivoRequest = {}): string {
  const search = new URLSearchParams()

  if (params.termo) search.set('termo', params.termo)
  if (params.tipoAtivoInventarioId) search.set('tipoAtivoInventarioId', params.tipoAtivoInventarioId)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.localUnidadeId) search.set('localUnidadeId', params.localUnidadeId)
  if (params.usuarioResponsavelId) search.set('usuarioResponsavelId', params.usuarioResponsavelId)
  if (typeof params.statusOperacional === 'number') search.set('statusOperacional', String(params.statusOperacional))
  if (typeof params.statusPatrimonial === 'number') search.set('statusPatrimonial', String(params.statusPatrimonial))
  if (typeof params.criticidade === 'number') search.set('criticidade', String(params.criticidade))
  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.dataAquisicaoInicial) search.set('dataAquisicaoInicial', params.dataAquisicaoInicial)
  if (params.dataAquisicaoFinal) search.set('dataAquisicaoFinal', params.dataAquisicaoFinal)
  if (params.dataFimGarantiaInicial) search.set('dataFimGarantiaInicial', params.dataFimGarantiaInicial)
  if (params.dataFimGarantiaFinal) search.set('dataFimGarantiaFinal', params.dataFimGarantiaFinal)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildPagedQuery(
  params: FiltroHistoricoInventarioAtivoRequest | FiltroChamadosRelacionadosInventarioAtivoRequest = {}
): string {
  const search = new URLSearchParams()
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  const query = search.toString()
  return query ? `?${query}` : ''
}

export const inventarioAtivosAdminService = {
  listar: (filtros: FiltroInventarioAtivoRequest = {}) =>
    httpClient.get<InventarioAtivoPaginado>(`/api/admin/inventario-ativos${buildQuery(filtros)}`),

  obterPorId: (id: string) =>
    httpClient.get<InventarioAtivoDetalhe>(`/api/admin/inventario-ativos/${id}`),

  criar: (payload: CriarInventarioAtivoRequest) =>
    httpClient.post<InventarioAtivoDetalhe>('/api/admin/inventario-ativos', payload),

  atualizar: (id: string, payload: AtualizarInventarioAtivoRequest) =>
    httpClient.put<InventarioAtivoDetalhe>(`/api/admin/inventario-ativos/${id}`, payload),

  inativar: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/inventario-ativos/${id}/inativar`),

  reativar: (id: string) =>
    httpClient.post<AlterarSituacaoCadastroResponse>(`/api/admin/inventario-ativos/${id}/reativar`),

  listarTipos: () =>
    httpClient.get<TipoAtivoInventario[]>('/api/admin/inventario-ativos/tipos'),

  listarHistorico: (id: string, filtros: FiltroHistoricoInventarioAtivoRequest = {}) =>
    httpClient.get<HistoricoInventarioAtivoPaginado>(
      `/api/admin/inventario-ativos/${id}/historico${buildPagedQuery(filtros)}`
    ),

  movimentar: (id: string, payload: MovimentarInventarioAtivoRequest) =>
    httpClient.post<InventarioAtivoDetalhe>(`/api/admin/inventario-ativos/${id}/movimentar`, payload),

  listarChamados: (id: string, filtros: FiltroChamadosRelacionadosInventarioAtivoRequest = {}) =>
    httpClient.get<ChamadoRelacionadoInventarioAtivoPaginado>(
      `/api/admin/inventario-ativos/${id}/chamados${buildPagedQuery(filtros)}`
    ),
}
