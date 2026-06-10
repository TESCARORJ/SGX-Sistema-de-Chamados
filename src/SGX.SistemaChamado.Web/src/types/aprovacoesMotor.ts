export enum StatusInstanciaAprovacaoChamado {
  Pendente = 0,
  Aprovado = 1,
  Reprovado = 2,
  Cancelado = 3,
  Expirado = 4,
  EmReavaliacao = 5,
}

export enum TipoRegraAprovacao {
  Geral = 1,
  NaturezaItsm = 2,
  TipoSolicitacao = 3,
  CatalogoServico = 4,
  CategoriaSubcategoria = 5,
  ImpactoUrgencia = 6,
  CustoOuRiscoFuturo = 7,
  Combinada = 8,
}

export enum EfeitoOperacionalRegraAprovacao {
  Permitir = 1,
  Sinalizar = 2,
  ExigirAprovacao = 3,
  ExigirAprovacaoEBloquearAvanco = 4,
  RequerReavaliacao = 5,
}

export enum EscopoRegraAprovacao {
  EscopoGeralChamado = 1,
  AberturaChamado = 2,
  AtendimentoChamado = 3,
  EncerramentoChamado = 4,
  ReaberturaChamado = 5,
}

export enum TipoFluxoAprovacao {
  Simples = 1,
  Sequencial = 2,
  Paralela = 3,
  Multinivel = 4,
}

export enum TipoResolucaoAprovadorRegraAprovacao {
  NaoDefinido = 1,
  AprovadorEspecifico = 2,
  AprovadorPadrao = 3,
  GrupoAprovadorFuturo = 4,
  ResolucaoDinamicaFutura = 5,
}

export interface InstanciaAprovacaoChamadoResumoResponse {
  id: string
  chamadoId: string
  configuracaoRegraAprovacaoId: string | null
  nomeRegra: string
  tipoRegra: TipoRegraAprovacao
  efeitoOperacional: EfeitoOperacionalRegraAprovacao
  status: StatusInstanciaAprovacaoChamado
  bloqueante: boolean
  aprovadorResolvidoId: string | null
  solicitadaEm: string
  prazoRespostaEm: string | null
  vencimentoEm: string | null
}

export interface DecisaoAprovacaoChamadoResponse {
  id: string
  instanciaAprovacaoChamadoId: string
  etapaAprovacaoChamadoId: string | null
  decididoPorUsuarioId: string
  decididoEm: string
  aprovado: boolean
  justificativa: string | null
}

export interface InstanciaAprovacaoChamadoResponse extends InstanciaAprovacaoChamadoResumoResponse {
  dadosSensiveisSnapshot: string | null
  decisoes: DecisaoAprovacaoChamadoResponse[]
}

export interface ListarConfiguracoesRegrasAprovacaoFiltro {
  termo?: string
  ativo?: boolean
  tipoRegra?: TipoRegraAprovacao
  escopoRegra?: EscopoRegraAprovacao
  naturezaChamado?: number
  tipoSolicitacaoId?: string
  catalogoServicoId?: string
  categoriaId?: string
  subcategoriaId?: string
  efeitoOperacional?: EfeitoOperacionalRegraAprovacao
  tipoFluxoAprovacao?: TipoFluxoAprovacao
  tipoResolucaoAprovador?: TipoResolucaoAprovadorRegraAprovacao
  bloqueante?: boolean
  exigeAprovacao?: boolean
  vigenteEm?: string
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: string
  direcaoOrdenacao?: 'asc' | 'desc'
}

export interface CriarConfiguracaoRegraAprovacaoRequest {
  nome: string
  descricao?: string | null
  tipoRegra: TipoRegraAprovacao
  escopoRegra: EscopoRegraAprovacao
  ordem: number
  prioridade: number
  versao: number
  naturezaChamado?: number | null
  tipoSolicitacaoId?: string | null
  catalogoServicoId?: string | null
  categoriaId?: string | null
  subcategoriaId?: string | null
  impactoMinimo?: number | null
  urgenciaMinima?: number | null
  prioridadeMinima?: number | null
  custoMinimo?: number | null
  nivelRiscoMinimo?: number | null
  exigeAprovacao: boolean
  bloqueante: boolean
  permiteReenvio: boolean
  permiteFallback: boolean
  efeitoOperacional: EfeitoOperacionalRegraAprovacao
  tipoFluxoAprovacao: TipoFluxoAprovacao
  tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao
  aprovadorEspecificoUsuarioId?: string | null
  aprovadorPadraoUsuarioId?: string | null
  prazoDecisaoHoras?: number | null
  vigenteDe?: string | null
  vigenteAte?: string | null
  ativo: boolean
}

export interface AtualizarConfiguracaoRegraAprovacaoRequest extends CriarConfiguracaoRegraAprovacaoRequest {}

export interface AlterarStatusConfiguracaoRegraAprovacaoRequest {
  ativo: boolean
}

export interface ValidarConfiguracaoRegraAprovacaoRequest {
  configuracaoRegraAprovacaoId?: string | null
  configuracao: CriarConfiguracaoRegraAprovacaoRequest
}

export interface ValidarConfiguracaoRegraAprovacaoResponse {
  valida: boolean
  erros: string[]
  alertas: string[]
}

export interface ConfiguracaoRegraAprovacaoResumoResponse {
  id: string
  nome: string
  tipoRegra: TipoRegraAprovacao
  tipoRegraDescricao: string
  escopoRegra: EscopoRegraAprovacao
  escopoRegraDescricao: string
  efeitoOperacional: EfeitoOperacionalRegraAprovacao
  efeitoOperacionalDescricao: string
  tipoFluxoAprovacao: TipoFluxoAprovacao
  tipoFluxoAprovacaoDescricao: string
  tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao
  tipoResolucaoAprovadorDescricao: string
  naturezaChamado?: number | null
  exigeAprovacao: boolean
  bloqueante: boolean
  prioridade: number
  versao: number
  ativo: boolean
  vigenteDe?: string | null
  vigenteAte?: string | null
  criadoEm: string
  atualizadoEm?: string | null
}

export interface ListaConfiguracoesRegrasAprovacaoResponse {
  items: ConfiguracaoRegraAprovacaoResumoResponse[]
  total: number
  pagina: number
  tamanhoPagina: number
}

export interface ConfiguracaoRegraAprovacaoResponse {
  id: string
  nome: string
  descricao?: string | null
  tipoRegra: TipoRegraAprovacao
  tipoRegraDescricao: string
  escopoRegra: EscopoRegraAprovacao
  escopoRegraDescricao: string
  ordem: number
  prioridade: number
  versao: number
  naturezaChamado?: number | null
  tipoSolicitacaoId?: string | null
  tipoSolicitacaoNome?: string | null
  catalogoServicoId?: string | null
  catalogoServicoNome?: string | null
  categoriaId?: string | null
  categoriaNome?: string | null
  subcategoriaId?: string | null
  subcategoriaNome?: string | null
  impactoMinimo?: number | null
  urgenciaMinima?: number | null
  prioridadeMinima?: number | null
  custoMinimo?: number | null
  nivelRiscoMinimo?: number | null
  exigeAprovacao: boolean
  bloqueante: boolean
  permiteReenvio: boolean
  permiteFallback: boolean
  efeitoOperacional: EfeitoOperacionalRegraAprovacao
  efeitoOperacionalDescricao: string
  tipoFluxoAprovacao: TipoFluxoAprovacao
  tipoFluxoAprovacaoDescricao: string
  tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao
  tipoResolucaoAprovadorDescricao: string
  aprovadorEspecificoUsuarioId?: string | null
  aprovadorEspecificoNome?: string | null
  aprovadorPadraoUsuarioId?: string | null
  aprovadorPadraoNome?: string | null
  prazoDecisaoHoras?: number | null
  vigenteDe?: string | null
  vigenteAte?: string | null
  ativo: boolean
  criadoPorUsuarioId: string
  atualizadoPorUsuarioId?: string | null
  criadoEm: string
  atualizadoEm?: string | null
}

export interface AprovarAprovacaoChamadoRequest {
  instanciaAprovacaoChamadoId: string
  etapaAprovacaoChamadoId?: string | null
  decisorUsuarioId?: string | null
  justificativa?: string | null
  observacao?: string | null
  escopoDecididoSnapshot?: string | null
  decisaoParcial?: boolean
  decisaoFinal?: boolean
  liberaAvanco?: boolean
  permiteNovaSolicitacao?: boolean
}

export interface AprovarAprovacaoChamadoResponse {
  aprovada: boolean
  instanciaAprovacaoChamadoId: string
  etapaAprovacaoChamadoId?: string | null
  decisaoAprovacaoChamadoId?: string | null
  statusInstanciaAnterior?: string | number | null
  statusInstanciaNovo?: string | number | null
  statusEtapaAnterior?: string | number | null
  statusEtapaNovo?: string | number | null
  decisaoFinal?: boolean
  liberaAvanco?: boolean
  motivo?: string | null
  avisos?: string[] | null
}

export interface ReprovarAprovacaoChamadoRequest {
  instanciaAprovacaoChamadoId: string
  etapaAprovacaoChamadoId?: string | null
  decisorUsuarioId?: string | null
  justificativa: string
  observacao?: string | null
  escopoDecididoSnapshot?: string | null
  decisaoParcial?: boolean
  decisaoFinal?: boolean
  mantemBloqueio?: boolean
  exigeReavaliacao?: boolean
  permiteNovaSolicitacao?: boolean
  cancelaFluxo?: boolean
}

export interface ReprovarAprovacaoChamadoResponse {
  reprovada: boolean
  instanciaAprovacaoChamadoId: string
  etapaAprovacaoChamadoId?: string | null
  decisaoAprovacaoChamadoId?: string | null
  statusInstanciaAnterior?: string | number | null
  statusInstanciaNovo?: string | number | null
  statusEtapaAnterior?: string | number | null
  statusEtapaNovo?: string | number | null
  decisaoFinal?: boolean
  mantemBloqueio?: boolean
  exigeReavaliacao?: boolean
  permiteNovaSolicitacao?: boolean
  cancelaFluxo?: boolean
  motivo?: string | null
  avisos?: string[] | null
}
