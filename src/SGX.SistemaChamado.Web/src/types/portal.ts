import type { AnexoChamado } from './anexo'
import type { ComentarioChamado } from './comentario'

export type SituacaoSlaChamado =
  | 'NaoAplicavel'
  | 'DentroDoPrazo'
  | 'ProximoDoVencimento'
  | 'Vencido'
  | 'Cumprido'
  | 'Violado'
  | 'Pausado'

export interface PortalUsuarioContexto {
  id: string
  nome: string
  email: string
  login: string
  perfis: string[]
}

export interface DepartamentoPortal {
  id: string
  nome: string
  sigla: string
}

export interface CategoriaPortal {
  id: string
  nome: string
  departamentoId: string | null
}

export interface PrioridadePortal {
  id: string
  nome: string
  nivel: number
}

export interface StatusPortal {
  id: string
  nome: string
  codigo: number
}

export interface PortalContextoResponse {
  usuario: PortalUsuarioContexto
  departamentos: DepartamentoPortal[]
  categorias: CategoriaPortal[]
  prioridades: PrioridadePortal[]
  status: StatusPortal[]
  configuracaoAnexos?: {
    tiposPermitidos: string[]
    tamanhoMaximoBytes: number | null
  } | null
}

export interface FiltroChamadosPortal {
  statusId?: string
  prioridadeId?: string
  categoriaId?: string
  dataInicial?: string
  dataFinal?: string
  texto?: string
  visaoAmpliada?: boolean
  pagina?: number
  tamanhoPagina?: number
}

export interface ChamadoResumoPortal {
  id: string
  codigo: string
  titulo: string
  status: string
  prioridade: string
  categoria: string
  departamento: string | null
  abertoEm: string
  atualizadoEm: string | null
  slaVencido: boolean
  slaProximoVencimento: boolean
  situacaoSla: SituacaoSlaChamado
  politicaSlaNome: string | null
  tempoRestanteMinutos: number | null
  tempoExcedidoMinutos: number | null
  prazoPrimeiraRespostaEm: string | null
  primeiraRespostaEm: string | null
  prazoResolucaoEm: string | null
  resolvidoEm: string | null
  estaPausado: boolean
  totalMinutosPausado: number
}

export interface ListaChamadosPortalResponse {
  items: ChamadoResumoPortal[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface HistoricoChamado {
  id: string
  tipo: number
  tipoDescricao: string
  descricao: string
  criadoEm: string
  usuarioId: string | null
  usuario: string | null
}

export interface SlaResumo {
  politicaSlaNome: string | null
  prioridade: string
  dataInicio: string
  prazoPrimeiraRespostaEm: string
  primeiraRespostaEm: string | null
  prazoResolucaoEm: string
  resolvidoEm: string | null
  primeiraRespostaCumprida: boolean | null
  resolucaoCumprida: boolean | null
  primeiraRespostaViolada: boolean
  resolucaoViolada: boolean
  estaVencido: boolean
  estaPausado: boolean
  situacao: SituacaoSlaChamado
  minutosPrimeiraResposta: number | null
  minutosResolucao: number | null
  tempoRestanteMinutos: number | null
  tempoExcedidoMinutos: number | null
  totalMinutosPausado: number
  usarHorarioComercial: boolean
  calendarioCorporativoNome: string | null
}

export interface ChamadoDetalhePortal {
  id: string
  codigo: string
  titulo: string
  descricao: string
  status: string
  prioridade: string
  categoria: string
  departamento: string | null
  solicitante: string
  responsavel: string | null
  abertoEm: string
  encerradoEm: string | null
  comentarios: ComentarioChamado[]
  anexos: AnexoChamado[]
  historico: HistoricoChamado[]
  sla: SlaResumo | null
}

export interface CriarChamadoRequest {
  titulo: string
  descricao: string
  departamentoId?: string
  categoriaId: string
  prioridadeId: string
}

export type ChamadoCriadoResponse = ChamadoDetalhePortal
export type CriarChamadoPortalPayload = CriarChamadoRequest

export interface ComentarChamadoPayload {
  mensagem: string
}
