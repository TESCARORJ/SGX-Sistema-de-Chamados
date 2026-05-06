import type { AnexoChamado } from './anexo'
import type { ComentarioChamado } from './comentario'

export interface AdminUsuarioContexto {
  id: string
  nome: string
  email: string
  login: string
  perfis: string[]
  permissoes: string[]
}

export interface DepartamentoAdmin {
  id: string
  nome: string
  sigla: string
}

export interface CategoriaAdmin {
  id: string
  nome: string
  departamentoId: string | null
}

export interface PrioridadeAdmin {
  id: string
  nome: string
  nivel: number
}

export interface StatusAdmin {
  id: string
  nome: string
  codigo: number
}

export interface AtendenteResumo {
  id: string
  nome: string
  email: string
  perfis: string[]
}

export interface AdminContextoResponse {
  usuario: AdminUsuarioContexto
  departamentos: DepartamentoAdmin[]
  categorias: CategoriaAdmin[]
  prioridades: PrioridadeAdmin[]
  status: StatusAdmin[]
  atendentes: AtendenteResumo[]
}

export interface FiltroChamadosAdmin {
  statusId?: string
  prioridadeId?: string
  categoriaId?: string
  departamentoId?: string
  responsavelId?: string
  solicitanteId?: string
  dataInicio?: string
  dataFim?: string
  slaVencido?: boolean
  texto?: string
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: 'asc' | 'desc'
}

export interface ChamadoAdminResumo {
  id: string
  codigo: string
  titulo: string
  solicitanteNome: string
  solicitanteEmail: string
  responsavelNome: string | null
  status: string
  prioridade: string
  categoria: string
  departamento: string | null
  abertoEm: string
  atualizadoEm: string | null
  encerradoEm: string | null
  slaVencido: boolean
  slaProximoVencimento: boolean
  prazoPrimeiraRespostaEm: string | null
  primeiraRespostaEm: string | null
  prazoResolucaoEm: string | null
  resolvidoEm: string | null
  estaPausado: boolean
  totalMinutosPausado: number
}

export interface ListaChamadosAdminResponse {
  items: ChamadoAdminResumo[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface UsuarioResumoAdmin {
  id: string
  nome: string
  email: string
}

export interface HistoricoAdmin {
  id: string
  tipo: number
  tipoDescricao: string
  descricao: string
  criadoEm: string
  usuarioId: string | null
  usuario: string | null
}

export interface SlaAdmin {
  prazoPrimeiraRespostaEm: string
  primeiraRespostaEm: string | null
  prazoResolucaoEm: string
  resolvidoEm: string | null
  estaVencido: boolean
  estaPausado: boolean
  totalMinutosPausado: number
}

export interface ChamadoAdminDetalhe {
  id: string
  codigo: string
  titulo: string
  descricao: string
  solicitante: UsuarioResumoAdmin
  responsavel: UsuarioResumoAdmin | null
  status: string
  prioridade: string
  categoria: string
  departamento: string | null
  origem: string
  abertoEm: string
  encerradoEm: string | null
  comentarios: ComentarioChamado[]
  anexos: AnexoChamado[]
  historico: HistoricoAdmin[]
  sla: SlaAdmin | null
}

export interface AtribuirChamadoPayload {
  responsavelId: string
}

export interface AlterarStatusChamadoPayload {
  statusId: string
}

export interface AlterarPrioridadeChamadoPayload {
  prioridadeId: string
}

export interface AlterarCategoriaChamadoPayload {
  categoriaId: string
}

export interface ComentarioAdminChamadoPayload {
  mensagem: string
  interno: boolean
}

export interface EncerrarChamadoPayload {
  solucao: string
  comentarioInterno: boolean
}

export interface ReabrirChamadoPayload {
  mensagem: string
}
