import type { PagedResponse, SortDirection } from './common'

export enum StatusCatalogoServico {
  Rascunho = 0,
  Publicado = 1,
  Arquivado = 2,
}

export enum VisibilidadeCatalogoServico {
  Interno = 0,
  Solicitante = 1,
  Atendente = 2,
  Administrador = 3,
}

export interface CatalogoServicoListagem {
  id: string
  nome: string
  slug: string
  descricao: string
  departamentoResponsavelId: string
  departamentoResponsavelNome: string | null
  categoriaId: string | null
  categoriaNome: string | null
  subcategoriaId: string | null
  subcategoriaNome: string | null
  prioridadePadraoId: string | null
  prioridadePadraoNome: string | null
  slaPadraoId: string | null
  slaPadraoNome: string | null
  status: StatusCatalogoServico
  statusDescricao: string
  visibilidade: VisibilidadeCatalogoServico
  visibilidadeDescricao: string
  permiteAberturaChamado: boolean
  requerAprovacao: boolean
  ordem: number
  ativo: boolean
  criadoEm: string
  atualizadoEm: string | null
  publicadoEm: string | null
  arquivadoEm: string | null
}

export interface CatalogoServicoDetalhe {
  id: string
  nome: string
  slug: string
  descricao: string
  instrucoesSolicitante: string | null
  departamentoResponsavelId: string
  departamentoResponsavelNome: string | null
  categoriaId: string | null
  categoriaNome: string | null
  subcategoriaId: string | null
  subcategoriaNome: string | null
  prioridadePadraoId: string | null
  prioridadePadraoNome: string | null
  slaPadraoId: string | null
  slaPadraoNome: string | null
  artigoBaseConhecimentoId: string | null
  artigoBaseConhecimentoTitulo: string | null
  status: StatusCatalogoServico
  statusDescricao: string
  visibilidade: VisibilidadeCatalogoServico
  visibilidadeDescricao: string
  permiteAberturaChamado: boolean
  requerAprovacao: boolean
  ordem: number
  ativo: boolean
  criadoEm: string
  criadoPorUsuarioId: string
  atualizadoEm: string | null
  atualizadoPorUsuarioId: string | null
  publicadoEm: string | null
  publicadoPorUsuarioId: string | null
  arquivadoEm: string | null
  arquivadoPorUsuarioId: string | null
}

export interface CriarCatalogoServicoRequest {
  nome: string
  descricao: string
  instrucoesSolicitante?: string | null
  departamentoResponsavelId: string
  categoriaId?: string | null
  subcategoriaId?: string | null
  prioridadePadraoId?: string | null
  slaPadraoId?: string | null
  politicaSlaId?: string | null
  artigoBaseConhecimentoId?: string | null
  visibilidade: VisibilidadeCatalogoServico
  permiteAberturaChamado?: boolean
  requerAprovacao: boolean
  ordem: number
}

export interface AtualizarCatalogoServicoRequest {
  nome: string
  descricao: string
  instrucoesSolicitante?: string | null
  departamentoResponsavelId: string
  categoriaId?: string | null
  subcategoriaId?: string | null
  prioridadePadraoId?: string | null
  slaPadraoId?: string | null
  politicaSlaId?: string | null
  artigoBaseConhecimentoId?: string | null
  visibilidade: VisibilidadeCatalogoServico
  permiteAberturaChamado: boolean
  requerAprovacao: boolean
  ordem: number
  ativo: boolean
}

export interface FiltroCatalogoServicoRequest {
  termo?: string
  departamentoResponsavelId?: string
  categoriaId?: string
  subcategoriaId?: string
  prioridadePadraoId?: string
  slaPadraoId?: string
  politicaSlaId?: string
  status?: StatusCatalogoServico
  visibilidade?: VisibilidadeCatalogoServico
  ativo?: boolean
  permiteAberturaChamado?: boolean
  requerAprovacao?: boolean
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: SortDirection
}

export type CatalogoServicoPaginado = PagedResponse<CatalogoServicoListagem>

export interface PortalFiltroCatalogoServicoRequest {
  termo?: string
  departamentoResponsavelId?: string
  categoriaId?: string
  subcategoriaId?: string
  permiteAberturaChamado?: boolean
  pagina?: number
  tamanhoPagina?: number
}

export interface PortalCatalogoServicoListagem {
  id: string
  nome: string
  slug: string
  descricao: string | null
  departamentoResponsavelId: string
  departamentoResponsavelNome: string | null
  categoriaId: string | null
  categoriaNome: string | null
  subcategoriaId: string | null
  subcategoriaNome: string | null
  permiteAberturaChamado: boolean
  requerAprovacao: boolean
  visibilidade: VisibilidadeCatalogoServico
  publicadoEm: string | null
  ordem: number
}

export interface PortalCatalogoServicoDetalhe {
  id: string
  nome: string
  slug: string
  descricao: string | null
  instrucoesSolicitante: string | null
  departamentoResponsavelId: string
  departamentoResponsavelNome: string | null
  categoriaId: string | null
  categoriaNome: string | null
  subcategoriaId: string | null
  subcategoriaNome: string | null
  prioridadePadraoId: string | null
  prioridadePadraoNome: string | null
  slaPadraoId: string | null
  slaPadraoNome: string | null
  artigoBaseConhecimentoId: string | null
  artigoBaseConhecimentoTitulo: string | null
  artigoBaseConhecimentoSlug: string | null
  permiteAberturaChamado: boolean
  requerAprovacao: boolean
  visibilidade: VisibilidadeCatalogoServico
  publicadoEm: string | null
}

export type PortalCatalogoServicoPaginado = PagedResponse<PortalCatalogoServicoListagem>

export interface PortalPrepararChamadoCatalogoServico {
  catalogoServicoId: string
  nome: string
  slug: string
  descricao: string | null
  instrucoesSolicitante: string | null
  departamentoResponsavelId: string
  departamentoResponsavelNome: string | null
  categoriaId: string | null
  categoriaNome: string | null
  subcategoriaId: string | null
  subcategoriaNome: string | null
  prioridadePadraoId: string | null
  prioridadePadraoNome: string | null
  slaPadraoId: string | null
  slaPadraoNome: string | null
  requerAprovacao: boolean
  permiteAberturaChamado: boolean
}
