import type { PagedResponse, SortDirection } from './common'

export enum StatusAprovacaoChamado {
  Pendente = 0,
  Aprovado = 1,
  Reprovado = 2,
  Cancelado = 3,
}

export enum TipoOrigemAprovacaoChamado {
  Manual = 0,
  CatalogoServico = 1,
  Categoria = 2,
  Departamento = 3,
  RegraAdministrativa = 4,
}

export interface AprovacaoChamadoListagem {
  id: string
  chamadoId: string
  numeroProtocoloChamado: string
  tituloChamado: string
  status: StatusAprovacaoChamado
  statusDescricao: string
  tipoOrigem: TipoOrigemAprovacaoChamado
  tipoOrigemDescricao: string
  origemDescricao: string | null
  solicitanteId: string | null
  solicitanteNome: string | null
  aprovadorId: string | null
  aprovadorNome: string | null
  solicitadaEm: string
  decididaEm: string | null
  ativo: boolean
}

export interface AprovacaoChamadoDetalhe {
  id: string
  chamadoId: string
  numeroProtocoloChamado: string
  tituloChamado: string
  descricaoChamado: string | null
  status: StatusAprovacaoChamado
  statusDescricao: string
  tipoOrigem: TipoOrigemAprovacaoChamado
  tipoOrigemDescricao: string
  origemDescricao: string | null
  solicitanteId: string | null
  solicitanteNome: string | null
  aprovadorId: string | null
  aprovadorNome: string | null
  justificativaSolicitacao: string | null
  justificativaDecisao: string | null
  solicitadaEm: string
  decididaEm: string | null
  criadoEm: string
  atualizadoEm: string | null
  ativo: boolean
}

export interface SolicitarAprovacaoChamadoRequest {
  tipoOrigem: TipoOrigemAprovacaoChamado
  origemDescricao?: string
  justificativaSolicitacao?: string
}

export interface DecidirAprovacaoChamadoRequest {
  justificativaDecisao?: string
}

export interface CancelarAprovacaoChamadoRequest {
  justificativaDecisao: string
}

export interface FiltroAprovacaoChamadoRequest {
  chamadoId?: string
  status?: StatusAprovacaoChamado
  tipoOrigem?: TipoOrigemAprovacaoChamado
  solicitanteId?: string
  aprovadorId?: string
  dataSolicitacaoInicial?: string
  dataSolicitacaoFinal?: string
  dataDecisaoInicial?: string
  dataDecisaoFinal?: string
  termo?: string
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: SortDirection
}

export type AprovacaoChamadoListagemPaginada = PagedResponse<AprovacaoChamadoListagem>
