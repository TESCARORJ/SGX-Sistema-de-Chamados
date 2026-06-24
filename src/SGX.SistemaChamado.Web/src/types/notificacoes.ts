export interface MinhaNotificacaoResumoResponse {
  id: string
  tipoEvento: number | string
  assunto: string | null
  conteudoResumo: string
  enviadaEm: string
  lida: boolean
  lidaEm: string | null
  chamadoId: string | null
}

export interface MinhaNotificacaoDetalheResponse {
  id: string
  tipoEvento: number | string
  assunto: string | null
  conteudo: string
  enviadaEm: string
  lida: boolean
  lidaEm: string | null
  chamadoId: string | null
  chaveCorrelacao: string | null
}

export interface ListarMinhasNotificacoesRequest {
  pagina: number
  tamanhoPagina: number
  lida?: boolean
}

export interface ListarMinhasNotificacoesResponse {
  itens: MinhaNotificacaoResumoResponse[]
  pagina: number
  tamanhoPagina: number
  total: number
  totalPaginas: number
  totalNaoLidas: number
}

export interface AlterarLeituraNotificacaoResponse {
  notificacaoId: string
  lida: boolean
  lidaEm: string | null
  estadoAlterado: boolean
}

export interface ContagemMinhasNotificacoesNaoLidasResponse {
  totalNaoLidas: number
}

export type FiltroLeituraNotificacao = 'todas' | 'nao-lidas' | 'lidas'
