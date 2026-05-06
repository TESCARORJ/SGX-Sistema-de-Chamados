export interface ParametroSistemaResumoResponse {
  id: string
  chave: string
  valor: string
  descricao: string | null
  sensivel: boolean
  ativo: boolean
}

export interface ParametroSistemaDetalheResponse extends ParametroSistemaResumoResponse {}

export interface CriarParametroSistemaRequest {
  chave: string
  valor: string
  descricao?: string | null
  sensivel: boolean
}

export interface AtualizarParametroSistemaRequest extends CriarParametroSistemaRequest {}
