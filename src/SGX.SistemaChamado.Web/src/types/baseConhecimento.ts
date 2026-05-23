import type { PagedResponse, SortDirection } from './common'

export enum StatusArtigoConhecimento {
  Rascunho = 1,
  EmRevisao = 2,
  Publicado = 3,
  Arquivado = 4,
}

export enum VisibilidadeArtigoConhecimento {
  Solicitante = 1,
  Atendente = 2,
  Administrador = 3,
}

export interface BaseConhecimentoArtigoListagem {
  id: string
  titulo: string
  slug: string
  resumo: string | null
  status: StatusArtigoConhecimento
  statusDescricao: string
  visibilidade: VisibilidadeArtigoConhecimento
  visibilidadeDescricao: string
  categoriaId: string | null
  categoriaNome: string | null
  tags: string | null
  publicadoEm: string | null
  ativo: boolean
  criadoEm: string
  atualizadoEm: string | null
}

export interface BaseConhecimentoArtigoDetalhe {
  id: string
  titulo: string
  slug: string
  resumo: string | null
  conteudo: string
  categoriaId: string | null
  categoriaNome: string | null
  status: StatusArtigoConhecimento
  statusDescricao: string
  visibilidade: VisibilidadeArtigoConhecimento
  visibilidadeDescricao: string
  tags: string | null
  publicadoEm: string | null
  publicadoPorUsuarioId: string | null
  criadoEm: string
  criadoPorUsuarioId: string
  atualizadoEm: string | null
  atualizadoPorUsuarioId: string | null
  arquivadoEm: string | null
  arquivadoPorUsuarioId: string | null
  ativo: boolean
}

export interface CriarBaseConhecimentoArtigoRequest {
  titulo: string
  resumo?: string | null
  conteudo: string
  categoriaId?: string | null
  visibilidade: VisibilidadeArtigoConhecimento
  tags?: string | null
}

export interface AtualizarBaseConhecimentoArtigoRequest extends CriarBaseConhecimentoArtigoRequest {}

export interface FiltroBaseConhecimentoArtigoRequest {
  termo?: string
  status?: StatusArtigoConhecimento
  visibilidade?: VisibilidadeArtigoConhecimento
  categoriaId?: string
  ativo?: boolean
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: SortDirection
}

export type BaseConhecimentoArtigoPaginado = PagedResponse<BaseConhecimentoArtigoListagem>

export interface PortalFiltroBaseConhecimentoRequest {
  termo?: string
  categoriaId?: string
  pagina?: number
  tamanhoPagina?: number
}

export interface PortalBaseConhecimentoArtigoListagem {
  id: string
  titulo: string
  slug: string
  resumo: string | null
  categoriaId: string | null
  categoriaNome: string | null
  tags: string | null
  publicadoEm: string | null
  criadoEm: string
  atualizadoEm: string | null
}

export interface PortalBaseConhecimentoArtigoDetalhe {
  id: string
  titulo: string
  slug: string
  resumo: string | null
  conteudo: string
  categoriaId: string | null
  categoriaNome: string | null
  tags: string | null
  publicadoEm: string | null
  criadoEm: string
  atualizadoEm: string | null
}

export type PortalBaseConhecimentoArtigoPaginado = PagedResponse<PortalBaseConhecimentoArtigoListagem>
