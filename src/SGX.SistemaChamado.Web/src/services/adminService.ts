import { httpClient } from './httpClient'
import type {
  AdminContextoResponse,
  AlterarStatusGrupoTecnicoPayload,
  AlterarStatusMembroGrupoTecnicoPayload,
  AlterarCategoriaChamadoPayload,
  AlterarPrioridadeChamadoPayload,
  AlterarStatusChamadoPayload,
  AdicionarMembroGrupoTecnicoPayload,
  AssumirChamadoFilaPayload,
  AtribuirChamadoPayload,
  ChamadoAprovacaoAdminResponse,
  ChamadoAdminDetalhe,
  ChamadoRelacionamentoAdmin,
  ChamadoTarefaAdminResponse,
  ComentarioAdminChamadoPayload,
  EncerrarChamadoPayload,
  FilaAtendimentoGrupoTecnicoResponse,
  FiltroChamadosAdmin,
  GrupoTecnicoDoUsuarioResponse,
  GrupoTecnicoDetalhe,
  ListaChamadosAdminResponse,
  ListaGruposTecnicosResponse,
  ListarFilasAtendimentoGrupoTecnicoFiltro,
  ListarMembrosGrupoTecnicoFiltro,
  ListarGruposTecnicosFiltro,
  MembroGrupoTecnicoResponse,
  ReabrirChamadoPayload,
  SalvarGrupoTecnicoPayload,
  TransferirGrupoTecnicoChamadoPayload,
} from '../types/admin'
import type { ComentarioChamado } from '../types/comentario'

function buildQuery(params: FiltroChamadosAdmin): string {
  const search = new URLSearchParams()

  if (params.naturezaChamado) search.set('naturezaChamado', String(params.naturezaChamado))
  if (params.statusId) search.set('statusId', params.statusId)
  if (params.prioridadeId) search.set('prioridadeId', params.prioridadeId)
  if (params.categoriaId) search.set('categoriaId', params.categoriaId)
  if (params.subcategoriaId) search.set('subcategoriaId', params.subcategoriaId)
  if (params.tipoSolicitacaoId) search.set('tipoSolicitacaoId', params.tipoSolicitacaoId)
  if (params.localUnidadeId) search.set('localUnidadeId', params.localUnidadeId)
  if (params.departamentoId) search.set('departamentoId', params.departamentoId)
  if (params.responsavelId) search.set('responsavelId', params.responsavelId)
  if (params.grupoTecnicoId) search.set('grupoTecnicoId', params.grupoTecnicoId)
  if (params.filaAtendimentoId) search.set('filaAtendimentoId', params.filaAtendimentoId)
  if (params.solicitanteId) search.set('solicitanteId', params.solicitanteId)
  if (params.dataInicio) search.set('dataInicio', params.dataInicio)
  if (params.dataFim) search.set('dataFim', params.dataFim)
  if (typeof params.slaVencido === 'boolean') search.set('slaVencido', String(params.slaVencido))
  if (params.slaSituacao) search.set('slaSituacao', params.slaSituacao)
  if (params.texto) search.set('texto', params.texto)
  if (params.pagina) search.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) search.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) search.set('ordenarPor', params.ordenarPor)
  if (params.direcaoOrdenacao) search.set('direcaoOrdenacao', params.direcaoOrdenacao)

  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildGruposTecnicosQuery(params: ListarGruposTecnicosFiltro = {}): string {
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

function buildMembrosGrupoTecnicoQuery(params: ListarMembrosGrupoTecnicoFiltro = {}): string {
  const search = new URLSearchParams()

  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))

  const query = search.toString()
  return query ? `?${query}` : ''
}

function buildFilasGrupoTecnicoQuery(params: ListarFilasAtendimentoGrupoTecnicoFiltro = {}): string {
  const search = new URLSearchParams()

  if (typeof params.ativo === 'boolean') search.set('ativo', String(params.ativo))
  if (params.busca) search.set('busca', params.busca)

  const query = search.toString()
  return query ? `?${query}` : ''
}

export const adminService = {
  obterAdminContexto: () => httpClient.get<AdminContextoResponse>('/api/admin/contexto'),

  listarGruposTecnicos: (filtros: ListarGruposTecnicosFiltro = {}) =>
    httpClient.get<ListaGruposTecnicosResponse>(`/api/admin/grupos-tecnicos${buildGruposTecnicosQuery(filtros)}`),

  obterGrupoTecnico: (id: string) =>
    httpClient.get<GrupoTecnicoDetalhe>(`/api/admin/grupos-tecnicos/${id}`),

  criarGrupoTecnico: (payload: SalvarGrupoTecnicoPayload) =>
    httpClient.post<GrupoTecnicoDetalhe>('/api/admin/grupos-tecnicos', payload),

  atualizarGrupoTecnico: (id: string, payload: SalvarGrupoTecnicoPayload) =>
    httpClient.put<GrupoTecnicoDetalhe>(`/api/admin/grupos-tecnicos/${id}`, payload),

  atualizarStatusGrupoTecnico: (id: string, payload: AlterarStatusGrupoTecnicoPayload) =>
    httpClient.patch<GrupoTecnicoDetalhe>(`/api/admin/grupos-tecnicos/${id}/status`, payload),

  listarMembrosGrupoTecnico: (grupoTecnicoId: string, filtros: ListarMembrosGrupoTecnicoFiltro = {}) =>
    httpClient.get<MembroGrupoTecnicoResponse[]>(
      `/api/admin/grupos-tecnicos/${grupoTecnicoId}/membros${buildMembrosGrupoTecnicoQuery(filtros)}`
    ),

  listarFilasAtendimentoGrupoTecnico: (
    grupoTecnicoId: string,
    filtros: ListarFilasAtendimentoGrupoTecnicoFiltro = {}
  ) =>
    httpClient.get<FilaAtendimentoGrupoTecnicoResponse[]>(
      `/api/admin/grupos-tecnicos/${grupoTecnicoId}/filas${buildFilasGrupoTecnicoQuery(filtros)}`
    ),

  adicionarMembroGrupoTecnico: (grupoTecnicoId: string, payload: AdicionarMembroGrupoTecnicoPayload) =>
    httpClient.post<MembroGrupoTecnicoResponse>(`/api/admin/grupos-tecnicos/${grupoTecnicoId}/membros`, payload),

  alterarStatusMembroGrupoTecnico: (
    grupoTecnicoId: string,
    membroId: string,
    payload: AlterarStatusMembroGrupoTecnicoPayload
  ) =>
    httpClient.patch<MembroGrupoTecnicoResponse>(
      `/api/admin/grupos-tecnicos/${grupoTecnicoId}/membros/${membroId}/status`,
      payload
    ),

  listarGruposTecnicosDoUsuario: (usuarioId: string, ativo?: boolean) => {
    const query = typeof ativo === 'boolean' ? `?ativo=${String(ativo)}` : ''
    return httpClient.get<GrupoTecnicoDoUsuarioResponse[]>(`/api/admin/usuarios/${usuarioId}/grupos-tecnicos${query}`)
  },

  listarChamadosAdmin: (filtros: FiltroChamadosAdmin = {}) =>
    httpClient.get<ListaChamadosAdminResponse>(`/api/admin/chamados${buildQuery(filtros)}`),

  obterChamadoAdmin: (id: string) => httpClient.get<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}`),

  listarRelacionamentosChamado: (id: string, incluirInativos = false) =>
    httpClient.get<ChamadoRelacionamentoAdmin[]>(
      `/api/admin/chamados/${id}/relacionamentos?incluirInativos=${String(incluirInativos)}`
    ),

  listarTarefasChamado: (id: string, incluirInativas = false) =>
    httpClient.get<ChamadoTarefaAdminResponse[]>(
      `/api/admin/chamados/${id}/tarefas?incluirInativas=${String(incluirInativas)}`
    ),

  listarAprovacoesChamado: (id: string, incluirInativas = false) =>
    httpClient.get<ChamadoAprovacaoAdminResponse[]>(
      `/api/admin/chamados/${id}/aprovacoes?incluirInativas=${String(incluirInativas)}`
    ),

  assumirChamado: (id: string) => httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/assumir`),

  assumirChamadoFila: (id: string, payload: AssumirChamadoFilaPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/assumir-fila`, payload),

  transferirGrupoTecnicoChamado: (id: string, payload: TransferirGrupoTecnicoChamadoPayload) =>
    httpClient.post<ChamadoAdminDetalhe>(`/api/admin/chamados/${id}/transferir-grupo-tecnico`, payload),

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
