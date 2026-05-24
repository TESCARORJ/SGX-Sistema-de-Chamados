import type { PagedResponse, SortDirection } from './common'

export enum StatusOperacionalAtivo {
  Operacional = 1,
  EmManutencao = 2,
  ComDefeito = 3,
  Reservado = 4,
  Baixado = 5,
}

export enum StatusPatrimonialAtivo {
  EmUso = 1,
  EmEstoque = 2,
  Emprestado = 3,
  EmTransferencia = 4,
  Descartado = 5,
  Extraviado = 6,
}

export enum CriticidadeAtivo {
  Baixa = 1,
  Media = 2,
  Alta = 3,
  Critica = 4,
}

export enum TipoMovimentacaoAtivo {
  Criacao = 1,
  Edicao = 2,
  TransferenciaDepartamento = 3,
  TransferenciaLocal = 4,
  AlteracaoResponsavel = 5,
  AlteracaoStatusOperacional = 6,
  AlteracaoStatusPatrimonial = 7,
  Manutencao = 8,
  Inativacao = 9,
  Reativacao = 10,
  VinculoChamado = 11,
  RemocaoVinculoChamado = 12,
}

export interface TipoAtivoInventario {
  id: string
  nome: string
  descricao: string | null
  ativo: boolean
}

export interface InventarioAtivoListagem {
  id: string
  codigo: string
  nome: string
  numeroPatrimonio: string | null
  numeroSerie: string | null
  tipoAtivoInventarioId: string
  tipoAtivoInventarioNome: string
  departamentoId: string | null
  departamentoNome: string | null
  localUnidadeId: string | null
  localUnidadeNome: string | null
  usuarioResponsavelId: string | null
  usuarioResponsavelNome: string | null
  statusOperacional: StatusOperacionalAtivo
  statusOperacionalDescricao: string
  statusPatrimonial: StatusPatrimonialAtivo
  statusPatrimonialDescricao: string
  criticidade: CriticidadeAtivo
  criticidadeDescricao: string
  dataAquisicao: string | null
  dataFimGarantia: string | null
  ativo: boolean
  criadoEm: string
  atualizadoEm: string | null
}

export interface InventarioAtivoDetalhe {
  id: string
  codigo: string
  nome: string
  descricao: string | null
  numeroPatrimonio: string | null
  numeroSerie: string | null
  tipoAtivoInventarioId: string
  tipoAtivoInventarioNome: string
  fabricante: string | null
  modelo: string | null
  departamentoId: string | null
  departamentoNome: string | null
  localUnidadeId: string | null
  localUnidadeNome: string | null
  usuarioResponsavelId: string | null
  usuarioResponsavelNome: string | null
  statusOperacional: StatusOperacionalAtivo
  statusOperacionalDescricao: string
  statusPatrimonial: StatusPatrimonialAtivo
  statusPatrimonialDescricao: string
  criticidade: CriticidadeAtivo
  criticidadeDescricao: string
  dataAquisicao: string | null
  dataFimGarantia: string | null
  valorAquisicao: number | null
  fornecedor: string | null
  observacoes: string | null
  ativo: boolean
  criadoEm: string
  criadoPorUsuarioId: string
  atualizadoEm: string | null
  atualizadoPorUsuarioId: string | null
  inativadoEm: string | null
  inativadoPorUsuarioId: string | null
}

export interface CriarInventarioAtivoRequest {
  codigo: string
  nome: string
  descricao?: string | null
  numeroPatrimonio?: string | null
  numeroSerie?: string | null
  tipoAtivoInventarioId: string
  fabricante?: string | null
  modelo?: string | null
  departamentoId?: string | null
  localUnidadeId?: string | null
  usuarioResponsavelId?: string | null
  statusOperacional?: StatusOperacionalAtivo | null
  statusPatrimonial?: StatusPatrimonialAtivo | null
  criticidade?: CriticidadeAtivo | null
  dataAquisicao?: string | null
  dataFimGarantia?: string | null
  valorAquisicao?: number | null
  fornecedor?: string | null
  observacoes?: string | null
}

export interface AtualizarInventarioAtivoRequest {
  codigo: string
  nome: string
  descricao?: string | null
  numeroPatrimonio?: string | null
  numeroSerie?: string | null
  tipoAtivoInventarioId: string
  fabricante?: string | null
  modelo?: string | null
  departamentoId?: string | null
  localUnidadeId?: string | null
  usuarioResponsavelId?: string | null
  statusOperacional: StatusOperacionalAtivo
  statusPatrimonial: StatusPatrimonialAtivo
  criticidade: CriticidadeAtivo
  dataAquisicao?: string | null
  dataFimGarantia?: string | null
  valorAquisicao?: number | null
  fornecedor?: string | null
  observacoes?: string | null
}

export interface FiltroInventarioAtivoRequest {
  termo?: string
  tipoAtivoInventarioId?: string
  departamentoId?: string
  localUnidadeId?: string
  usuarioResponsavelId?: string
  statusOperacional?: StatusOperacionalAtivo
  statusPatrimonial?: StatusPatrimonialAtivo
  criticidade?: CriticidadeAtivo
  ativo?: boolean
  dataAquisicaoInicial?: string
  dataAquisicaoFinal?: string
  dataFimGarantiaInicial?: string
  dataFimGarantiaFinal?: string
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: SortDirection
}

export interface HistoricoInventarioAtivo {
  id: string
  inventarioAtivoId: string
  tipoMovimentacao: TipoMovimentacaoAtivo
  tipoMovimentacaoDescricao: string
  departamentoOrigemNome: string | null
  departamentoDestinoNome: string | null
  localUnidadeOrigemNome: string | null
  localUnidadeDestinoNome: string | null
  usuarioResponsavelOrigemNome: string | null
  usuarioResponsavelDestinoNome: string | null
  statusOperacionalAnterior: StatusOperacionalAtivo | null
  statusOperacionalNovo: StatusOperacionalAtivo | null
  statusPatrimonialAnterior: StatusPatrimonialAtivo | null
  statusPatrimonialNovo: StatusPatrimonialAtivo | null
  observacao: string | null
  criadoEm: string
  criadoPorUsuarioNome: string
}

export interface MovimentarInventarioAtivoRequest {
  departamentoId?: string | null
  localUnidadeId?: string | null
  usuarioResponsavelId?: string | null
  statusOperacional?: StatusOperacionalAtivo | null
  statusPatrimonial?: StatusPatrimonialAtivo | null
  observacao?: string | null
}

export interface ChamadoRelacionadoInventarioAtivo {
  chamadoId: string
  protocolo: string
  titulo: string
  status: string
  prioridade: string
  solicitanteNome: string
  criadoEm: string
  atualizadoEm: string | null
  encerradoEm: string | null
}

export interface FiltroHistoricoInventarioAtivoRequest {
  pagina?: number
  tamanhoPagina?: number
}

export interface FiltroChamadosRelacionadosInventarioAtivoRequest {
  pagina?: number
  tamanhoPagina?: number
}

export type InventarioAtivoPaginado = PagedResponse<InventarioAtivoListagem>
export type HistoricoInventarioAtivoPaginado = PagedResponse<HistoricoInventarioAtivo>
export type ChamadoRelacionadoInventarioAtivoPaginado = PagedResponse<ChamadoRelacionadoInventarioAtivo>
