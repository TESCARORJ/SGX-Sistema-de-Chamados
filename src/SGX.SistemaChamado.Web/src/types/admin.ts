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

export interface SubcategoriaAdmin {
  id: string
  categoriaChamadoId: string
  nome: string
}

export interface PrioridadeAdmin {
  id: string
  nome: string
  nivel: number
}

export interface TipoSolicitacaoAdmin {
  id: string
  nome: string
}

export interface LocalUnidadeAdmin {
  id: string
  nome: string
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
  subcategorias: SubcategoriaAdmin[]
  prioridades: PrioridadeAdmin[]
  tiposSolicitacao: TipoSolicitacaoAdmin[]
  locaisUnidade: LocalUnidadeAdmin[]
  status: StatusAdmin[]
  atendentes: AtendenteResumo[]
}

export interface FiltroChamadosAdmin {
  statusId?: string
  prioridadeId?: string
  categoriaId?: string
  subcategoriaId?: string
  tipoSolicitacaoId?: string
  localUnidadeId?: string
  departamentoId?: string
  responsavelId?: string
  solicitanteId?: string
  dataInicio?: string
  dataFim?: string
  slaVencido?: boolean
  slaSituacao?: SituacaoSlaChamado
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
  inventarioAtivoId: string | null
  inventarioAtivoCodigo: string | null
  inventarioAtivoNome: string | null
  abertoEm: string
  atualizadoEm: string | null
  encerradoEm: string | null
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

export interface EventoSlaAdmin {
  id: string
  tipoEvento: number
  tipoEventoDescricao: string
  descricao: string
  dataEvento: string
  usuarioId: string | null
  usuario: string | null
}

export interface SlaAdmin {
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
  inventarioAtivoId: string | null
  inventarioAtivoCodigo: string | null
  inventarioAtivoNome: string | null
  origem: string
  abertoEm: string
  encerradoEm: string | null
  comentarios: ComentarioChamado[]
  anexos: AnexoChamado[]
  historico: HistoricoAdmin[]
  historicoSla: EventoSlaAdmin[]
  sla: SlaAdmin | null
}

export interface ChamadoArtigoConhecimento {
  artigoId: string
  titulo: string
  slug: string
  resumo: string | null
  status: number
  statusDescricao: string
  visibilidade: number
  visibilidadeDescricao: string
  categoriaId: string | null
  categoriaNome: string | null
  vinculadoEm: string
  vinculadoPorUsuarioId: string
  vinculadoPorUsuario: string
  observacao: string | null
}

export interface ArtigoConhecimentoDisponivelParaVinculo {
  artigoId: string
  titulo: string
  slug: string
  resumo: string | null
  status: number
  statusDescricao: string
  visibilidade: number
  visibilidadeDescricao: string
  categoriaId: string | null
  categoriaNome: string | null
  tags: string | null
  publicadoEm: string | null
}

export interface BuscarArtigosConhecimentoParaVinculoFiltro {
  termo?: string
  categoriaId?: string
  page?: number
  pageSize?: number
}

export interface ArtigosConhecimentoDisponiveisResponse {
  items: ArtigoConhecimentoDisponivelParaVinculo[]
  total: number
  pagina: number
  tamanhoPagina: number
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
  subcategoriaId?: string
  tipoSolicitacaoId?: string
  localUnidadeId?: string
  departamentoId?: string
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
