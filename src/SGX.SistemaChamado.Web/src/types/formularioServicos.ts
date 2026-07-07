export enum TipoCampoFormularioServico {
  TextoCurto = 1,
  TextoLongo = 2,
  Numero = 3,
  Data = 4,
  Booleano = 5,
  SelecaoUnica = 6,
  SelecaoMultipla = 7,
}

export interface OpcaoCampoFormularioServicoAdminDto {
  id: string
  campoFormularioServicoId: string
  valor: string
  rotulo: string
  ordem: number
  ativo: boolean
}

export interface CampoFormularioServicoAdminDto {
  id: string
  formularioServicoVersaoId: string
  nome: string
  rotulo: string
  tipo: TipoCampoFormularioServico
  obrigatorio: boolean
  ordem: number
  textoAjuda: string | null
  visivel: boolean
  ativo: boolean
  opcoes: OpcaoCampoFormularioServicoAdminDto[]
}

export interface FormularioServicoVersaoAdminDto {
  id: string
  formularioServicoId: string
  numero: number
  publicada: boolean
  publicadoEm: string | null
  ativo: boolean
  campos: CampoFormularioServicoAdminDto[]
}

export interface FormularioServicoAdminDto {
  id: string
  catalogoServicoId: string
  nome: string
  descricao: string | null
  ativo: boolean
  criadoEm: string
  atualizadoEm: string | null
}

export interface FormularioServicoDetalheAdminDto extends FormularioServicoAdminDto {
  versoes: FormularioServicoVersaoAdminDto[]
}

export interface ListarFormularioServicoRequest {
  catalogoServicoId?: string
}

export interface CriarFormularioServicoRequest {
  catalogoServicoId: string
  nome: string
  descricao?: string | null
  ativo?: boolean
}

export interface AtualizarFormularioServicoRequest {
  nome: string
  descricao?: string | null
  ativo: boolean
}

export interface CriarFormularioServicoVersaoRequest {
  formularioServicoId: string
  numero: number
  publicada: boolean
  publicadoEm?: string | null
  ativo?: boolean
}

export interface AtualizarFormularioServicoVersaoRequest {
  numero: number
  publicada: boolean
  publicadoEm?: string | null
  ativo: boolean
}

export interface CriarCampoFormularioServicoRequest {
  formularioServicoVersaoId: string
  nome: string
  rotulo: string
  tipo: TipoCampoFormularioServico
  obrigatorio: boolean
  ordem: number
  textoAjuda?: string | null
  visivel: boolean
  ativo?: boolean
}

export interface AtualizarCampoFormularioServicoRequest {
  nome: string
  rotulo: string
  tipo: TipoCampoFormularioServico
  obrigatorio: boolean
  ordem: number
  textoAjuda?: string | null
  visivel: boolean
  ativo: boolean
}

export interface CriarOpcaoCampoFormularioServicoRequest {
  campoFormularioServicoId: string
  valor: string
  rotulo: string
  ordem: number
  ativo?: boolean
}

export interface AtualizarOpcaoCampoFormularioServicoRequest {
  valor: string
  rotulo: string
  ordem: number
  ativo: boolean
}
