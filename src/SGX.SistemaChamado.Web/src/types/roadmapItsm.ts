export interface FiltroRoadmapItsmRequest {
  status?: number
  prioridade?: number
  impacto?: number
  roadmapCategoriaId?: string
  categoria?: string
  ativo?: boolean
}

export interface RoadmapItsmResumoResponse {
  id: string
  area: string
  categoria: string
  roadmapCategoriaId: string | null
  roadmapCategoriaNome: string | null
  roadmapCategoriaCor: string | null
  roadmapCategoriaIcone: string | null
  situacaoAtual: string
  status: number
  statusDescricao: string
  prioridade: number
  prioridadeDescricao: string
  impacto: number
  impactoDescricao: string
  decisao: number
  decisaoDescricao: string
  responsavel: string | null
  prazoAlvo: string | null
  ordem: number
  ativo: boolean
  statusImplementacao: number
  statusImplementacaoDescricao: string
  statusTecnico: number
  statusTecnicoDescricao: string
  percentualImplementacao: number
  percentualCalculadoPorChecklist: boolean
  quantidadeChecklistAtivo: number
  quantidadeChecklistConcluido: number
  pendenciasTecnicas: string | null
  pendenciasHomologacao: string | null
  evidenciaImplementacao: string | null
  dataConclusaoTecnica: string | null
  dataHomologacao: string | null
  criterioAceite: string | null
  proximaAcao: string | null
}

export interface RoadmapItsmDetalheResponse extends RoadmapItsmResumoResponse {
  atencaoTecnica: string
  observacao: string | null
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export interface CriarRoadmapItsmItemRequest {
  area: string
  categoria: string
  roadmapCategoriaId?: string | null
  situacaoAtual: string
  atencaoTecnica: string
  status: number
  prioridade: number
  impacto: number
  decisao: number
  observacao?: string | null
  responsavel?: string | null
  prazoAlvo?: string | null
  ordem: number
  ativo: boolean
  statusImplementacao: number
  statusTecnico: number
  percentualImplementacao?: number | null
  pendenciasTecnicas?: string | null
  pendenciasHomologacao?: string | null
  evidenciaImplementacao?: string | null
  dataConclusaoTecnica?: string | null
  dataHomologacao?: string | null
  criterioAceite?: string | null
  proximaAcao?: string | null
}

export interface AtualizarRoadmapItsmItemRequest extends CriarRoadmapItsmItemRequest {}

export interface AtualizarStatusRoadmapItsmRequest {
  status: number
  prioridade: number
  decisao: number
  responsavel?: string | null
  prazoAlvo?: string | null
  observacao?: string | null
}

export interface FiltroRoadmapImplementacaoFuturaRequest {
  roadmapItemId?: string
  texto?: string
  tipo?: number
  prioridade?: number
  status?: number
  responsavel?: string
  ativo?: boolean
  pagina?: number
  tamanhoPagina?: number
}

export interface PagedResultResponse<T> {
  items: T[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface RoadmapImplementacaoFuturaResponse {
  id: string
  roadmapItemId: string
  titulo: string
  descricao: string | null
  tipo: number
  tipoDescricao: string
  prioridade: number
  prioridadeDescricao: string
  status: number
  statusDescricao: string
  responsavel: string | null
  prazoAlvo: string | null
  dataConclusao: string | null
  observacao: string | null
  ativo: boolean
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export interface CriarRoadmapImplementacaoFuturaRequest {
  roadmapItemId: string
  titulo: string
  descricao?: string | null
  tipo: number
  prioridade: number
  status: number
  responsavel?: string | null
  prazoAlvo?: string | null
  observacao?: string | null
}

export interface AtualizarRoadmapImplementacaoFuturaRequest {
  titulo: string
  descricao?: string | null
  tipo: number
  prioridade: number
  status: number
  responsavel?: string | null
  prazoAlvo?: string | null
  dataConclusao?: string | null
  observacao?: string | null
  ativo: boolean
}

export interface FiltroRoadmapCategoriaRequest {
  ativo?: boolean
  texto?: string
}

export interface RoadmapCategoriaResponse {
  id: string
  nome: string
  descricao: string | null
  cor: string | null
  icone: string | null
  ordem: number | null
  ativo: boolean
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export interface CriarRoadmapCategoriaRequest {
  nome: string
  descricao?: string | null
  cor?: string | null
  icone?: string | null
  ordem?: number | null
}

export interface AtualizarRoadmapCategoriaRequest extends CriarRoadmapCategoriaRequest {
  ativo: boolean
}

export interface RoadmapChecklistItemResponse {
  id: string
  roadmapItemId: string
  titulo: string
  descricao: string | null
  grupo: number
  grupoDescricao: string
  ordem: number
  concluido: boolean
  obrigatorio: boolean
  ativo: boolean
  criadoEm: string
  criadoPor: string
  atualizadoEm: string | null
  atualizadoPor: string | null
}

export interface CriarRoadmapChecklistItemRequest {
  titulo: string
  descricao?: string | null
  grupo: number
  ordem: number
  concluido: boolean
  obrigatorio: boolean
}

export interface AtualizarRoadmapChecklistItemRequest extends CriarRoadmapChecklistItemRequest {
  ativo: boolean
}
