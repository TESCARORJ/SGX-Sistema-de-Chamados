import type { SortDirection } from './common'

export interface FiltroCadastroRequest {
  texto?: string
  ativo?: boolean
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: SortDirection
}

export interface PagedResultResponse<T> {
  items: T[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface AlterarSituacaoCadastroResponse {
  id: string
  ativo: boolean
  mensagem: string
}

export interface PerfilAcessoResumoResponse {
  id: string
  nome: string
  tipoPerfil: number
  tipoPerfilDescricao: string
  ativo: boolean
}

export interface PerfilAcessoDetalheResponse extends PerfilAcessoResumoResponse {
  descricao: string | null
}

export interface PermissaoSistema {
  id: string
  codigo: string
  nome: string
  descricao: string | null
  modulo: string
  acao: string
  ativo: boolean
}

export interface PerfilPermissoes {
  perfilId: string
  nome: string
  tipoPerfil: number
  permissoesDisponiveis: PermissaoSistema[]
  permissoesVinculadas: PermissaoSistema[]
}

export interface AtualizarPermissoesPerfilRequest {
  codigosPermissoes: string[]
}

export interface CriarPerfilAcessoRequest {
  nome: string
  tipoPerfil: number
  descricao?: string | null
}

export interface AtualizarPerfilAcessoRequest extends CriarPerfilAcessoRequest {}

export interface UsuarioAdminResumoResponse {
  id: string
  nome: string
  email: string
  login: string
  situacao: string
  departamentoId: string | null
  departamento: string | null
  ativo: boolean
  perfis: PerfilAcessoResumoResponse[]
}

export interface UsuarioAdminDetalheResponse extends UsuarioAdminResumoResponse {
  ultimoAcessoEm: string | null
}

export interface CriarUsuarioAdminRequest {
  nome: string
  email: string
  login?: string | null
  departamentoId?: string | null
  perfilIds: string[]
}

export interface AtualizarUsuarioAdminRequest {
  nome: string
  email: string
  login?: string | null
  departamentoId?: string | null
  situacao: number
}

export interface AlterarPerfisUsuarioRequest {
  perfilIds: string[]
}

export interface RedefinirSenhaUsuarioAdminRequest {
  novaSenha: string
  confirmarNovaSenha: string
  deveAlterarSenha: boolean
}

export interface DepartamentoResumoResponse {
  id: string
  nome: string
  sigla: string
  ativo: boolean
}

export interface DepartamentoDetalheResponse extends DepartamentoResumoResponse {
  descricao: string | null
}

export interface CriarDepartamentoRequest {
  nome: string
  sigla: string
  descricao?: string | null
}

export interface AtualizarDepartamentoRequest extends CriarDepartamentoRequest {}

export interface CategoriaChamadoResumoResponse {
  id: string
  nome: string
  departamentoId: string | null
  departamento: string | null
  ativo: boolean
}

export interface CategoriaChamadoDetalheResponse extends CategoriaChamadoResumoResponse {
  descricao: string | null
}

export interface CriarCategoriaChamadoRequest {
  nome: string
  descricao?: string | null
  departamentoId?: string | null
}

export interface AtualizarCategoriaChamadoRequest extends CriarCategoriaChamadoRequest {}

export interface SubcategoriaChamadoResumoResponse {
  id: string
  categoriaChamadoId: string
  categoriaNome: string
  nome: string
  ativo: boolean
}

export interface SubcategoriaChamadoDetalheResponse extends SubcategoriaChamadoResumoResponse {
  descricao: string | null
}

export interface CriarSubcategoriaChamadoRequest {
  categoriaChamadoId: string
  nome: string
  descricao?: string | null
}

export interface AtualizarSubcategoriaChamadoRequest extends CriarSubcategoriaChamadoRequest {}

export interface PrioridadeChamadoResumoResponse {
  id: string
  nome: string
  descricao: string | null
  peso: number
  cor: string | null
  nivel?: number
  ativo: boolean
}

export interface PrioridadeChamadoDetalheResponse extends PrioridadeChamadoResumoResponse {}

export interface CriarPrioridadeChamadoRequest {
  nome: string
  descricao?: string | null
  peso: number
  cor?: string | null
}

export interface AtualizarPrioridadeChamadoRequest extends CriarPrioridadeChamadoRequest {}

export interface TipoSolicitacaoResumoResponse {
  id: string
  nome: string
  ativo: boolean
}

export interface TipoSolicitacaoDetalheResponse extends TipoSolicitacaoResumoResponse {
  descricao: string | null
}

export interface CriarTipoSolicitacaoRequest {
  nome: string
  descricao?: string | null
}

export interface AtualizarTipoSolicitacaoRequest extends CriarTipoSolicitacaoRequest {}

export interface LocalUnidadeResumoResponse {
  id: string
  nome: string
  ativo: boolean
}

export interface LocalUnidadeDetalheResponse extends LocalUnidadeResumoResponse {
  descricao: string | null
  endereco: string | null
}

export interface CriarLocalUnidadeRequest {
  nome: string
  descricao?: string | null
  endereco?: string | null
}

export interface AtualizarLocalUnidadeRequest extends CriarLocalUnidadeRequest {}

export interface StatusChamadoResumoResponse {
  id: string
  nome: string
  codigo: number
  descricao: string | null
  ehStatusFinal: boolean
  pausaSla: boolean
  ativo: boolean
}

export interface StatusChamadoDetalheResponse extends StatusChamadoResumoResponse {}

export interface CriarStatusChamadoRequest {
  nome: string
  codigo: number
  descricao?: string | null
  ehStatusFinal: boolean
  pausaSla: boolean
}

export interface AtualizarStatusChamadoRequest extends CriarStatusChamadoRequest {}
