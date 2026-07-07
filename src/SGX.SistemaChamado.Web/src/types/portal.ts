import type { AnexoChamado } from './anexo'
import type { StatusAprovacaoChamado } from './aprovacaoChamados'
import type { ComentarioChamado } from './comentario'
import type { TipoCampoFormularioServico } from './formularioServicos'

export type SituacaoSlaChamado =
  | 'NaoAplicavel'
  | 'DentroDoPrazo'
  | 'ProximoDoVencimento'
  | 'Vencido'
  | 'Cumprido'
  | 'Violado'
  | 'Pausado'

export enum NaturezaChamado {
  Incidente = 1,
  Requisicao = 2,
  Mudanca = 3,
  Problema = 4,
  EventoAlerta = 5,
  TarefaOperacional = 6,
}

export enum ImpactoChamado {
  Baixo = 1,
  Medio = 2,
  Alto = 3,
}

export enum UrgenciaChamado {
  Baixa = 1,
  Media = 2,
  Alta = 3,
}

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

export interface SubcategoriaPortal {
  id: string
  categoriaChamadoId: string
  nome: string
}

export interface PrioridadePortal {
  id: string
  nome: string
  nivel: number
}

export interface TipoSolicitacaoPortal {
  id: string
  nome: string
}

export interface LocalUnidadePortal {
  id: string
  nome: string
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
  subcategorias: SubcategoriaPortal[]
  prioridades: PrioridadePortal[]
  tiposSolicitacao: TipoSolicitacaoPortal[]
  locaisUnidade: LocalUnidadePortal[]
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
  naturezaChamado?: NaturezaChamado
  impactoChamado?: ImpactoChamado
  urgenciaChamado?: UrgenciaChamado
  categoria: string
  subcategoria: string | null
  tipoSolicitacao: string | null
  localUnidade: string | null
  departamento: string | null
  categoriaId: string
  subcategoriaId: string | null
  prioridadeId: string
  tipoSolicitacaoId: string | null
  localUnidadeId: string | null
  departamentoId: string | null
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
  requerAprovacao: boolean
  aprovacaoPendente: boolean
  statusAprovacao: StatusAprovacaoChamado | null
  aprovacaoChamadoId: string | null
  aprovacaoSolicitadaEm: string | null
  aprovacaoDecididaEm: string | null
  justificativaAprovacao: string | null
  justificativaReprovacao: string | null
  mensagemOrientativaAprovacao: string
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

export interface RespostaFormularioDetalhePortal {
  campoFormularioServicoId: string
  nome: string
  rotulo: string
  tipo: TipoCampoFormularioServico
  valor: string | null
  valores: string[]
  ordem: number
}

export interface ChamadoDetalhePortal {
  id: string
  codigo: string
  titulo: string
  descricao: string
  status: string
  prioridade: string
  naturezaChamado?: NaturezaChamado
  impactoChamado?: ImpactoChamado
  urgenciaChamado?: UrgenciaChamado
  categoria: string
  subcategoria: string | null
  tipoSolicitacao: string | null
  localUnidade: string | null
  departamento: string | null
  categoriaId: string
  subcategoriaId: string | null
  prioridadeId: string
  tipoSolicitacaoId: string | null
  localUnidadeId: string | null
  departamentoId: string | null
  catalogoServicoId?: string | null
  catalogoServicoNome?: string | null
  solicitante: string
  responsavel: string | null
  abertoEm: string
  encerradoEm: string | null
  comentarios: ComentarioChamado[]
  anexos: AnexoChamado[]
  historico: HistoricoChamado[]
  sla: SlaResumo | null
  requerAprovacao: boolean
  aprovacaoPendente: boolean
  statusAprovacao: StatusAprovacaoChamado | null
  aprovacaoChamadoId: string | null
  aprovacaoSolicitadaEm: string | null
  aprovacaoDecididaEm: string | null
  justificativaAprovacao: string | null
  justificativaReprovacao: string | null
  mensagemOrientativaAprovacao: string
  respostasFormulario: RespostaFormularioDetalhePortal[]
}

export interface PortalStatusAprovacaoChamado {
  chamadoId: string
  requerAprovacao: boolean
  aprovacaoPendente: boolean
  statusAprovacao: StatusAprovacaoChamado | null
  aprovacaoChamadoId: string | null
  solicitadaEm: string | null
  decididaEm: string | null
  justificativaDecisao: string | null
  mensagemOrientativa: string
}

export interface RespostaFormularioAberturaRequest {
  campoFormularioServicoId: string
  valor?: string | null
  valores?: string[] | null
}

export interface CriarChamadoRequest {
  titulo: string
  descricao: string
  catalogoServicoId?: string
  catalogoServicoSlug?: string
  departamentoId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadeId?: string
  naturezaChamado: NaturezaChamado
  impactoChamado: ImpactoChamado
  urgenciaChamado: UrgenciaChamado
  tipoSolicitacaoId?: string
  localUnidadeId?: string
  respostasFormulario?: RespostaFormularioAberturaRequest[]
}

export interface AbrirRequisicaoServicoCatalogoRequest {
  catalogoServicoId: string
  titulo: string
  descricao?: string | null
  respostasFormulario?: RespostaFormularioAberturaRequest[]
}

export type ChamadoCriadoResponse = ChamadoDetalhePortal
export type CriarChamadoPortalPayload = CriarChamadoRequest

export interface ComentarChamadoPayload {
  mensagem: string
  interno?: boolean
}
