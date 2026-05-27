import { httpClient } from './httpClient'
import type {
  FiltroRelatorioAprovacoes,
  FiltroRelatorioAuditoria,
  FiltroRelatorioBaseConhecimento,
  FiltroRelatorioCatalogo,
  FiltroRelatorioChamados,
  FiltroRelatorioInventario,
  FiltroRelatorioSla,
  RelatorioAprovacoesPorOrigem,
  RelatorioAprovacoesResumo,
  RelatorioAprovacoesTempoMedio,
  RelatorioAtendimentoProdutividade,
  RelatorioAuditoriaPorEntidade,
  RelatorioAuditoriaPorUsuario,
  RelatorioAuditoriaResumo,
  RelatorioBaseConhecimentoPorStatus,
  RelatorioBaseConhecimentoResumo,
  RelatorioBaseConhecimentoVinculosChamados,
  RelatorioCatalogoServicosMaisSolicitados,
  RelatorioCatalogoServicosPorDepartamento,
  RelatorioCatalogoServicosResumo,
  RelatorioChamadosDistribuicao,
  RelatorioChamadosResumo,
  RelatorioChamadosSerieTemporal,
  RelatorioInventarioAtivosChamadosRecorrentes,
  RelatorioInventarioAtivosPorDepartamento,
  RelatorioInventarioAtivosPorStatus,
  RelatorioInventarioAtivosResumo,
  RelatoriosAvancadosMetadados,
  RelatorioSlaPorDepartamento,
  RelatorioSlaPorPrioridade,
  RelatorioSlaResumo,
  RelatorioSlaViolacao,
} from '../types/relatoriosAvancados'

type FiltrosRelatorio = Record<string, unknown>

type OpcoesSanitizacaoFiltros = {
  chavesPermitidas?: readonly string[]
  chavesRemovidas?: readonly string[]
  chavesEnum?: readonly string[]
}

const GUID_VAZIO = '00000000-0000-0000-0000-000000000000'
const CHAVES_ENUM_PADRAO = ['agrupamento', 'agruparPor', 'naturezaChamado'] as const

function normalizarNomeParametro(chave: string): string {
  if (!chave) {
    return chave
  }

  return `${chave.charAt(0).toUpperCase()}${chave.slice(1)}`
}

function ehNumeroEnumInvalido(valor: number): boolean {
  return !Number.isFinite(valor) || valor <= 0
}

function limparValorFiltro(
  valor: unknown,
  chave: string,
  chavesEnumNormalizadas: ReadonlySet<string>
): string[] | null {
  const chaveNormalizada = chave.toLowerCase()
  const ehCampoEnum = chavesEnumNormalizadas.has(chaveNormalizada)

  if (valor === undefined || valor === null) {
    return null
  }

  if (Array.isArray(valor)) {
    const itensValidos = valor
      .flatMap((item) => limparValorFiltro(item, chave, chavesEnumNormalizadas) ?? [])
      .filter((item) => item.length > 0)

    return itensValidos.length > 0 ? itensValidos : null
  }

  if (valor instanceof Date) {
    return [valor.toISOString()]
  }

  if (typeof valor === 'string') {
    const valorTrim = valor.trim()
    if (!valorTrim || valorTrim === GUID_VAZIO) {
      return null
    }

    if (ehCampoEnum && valorTrim === '0') {
      return null
    }

    return [valorTrim]
  }

  if (typeof valor === 'number') {
    if (ehCampoEnum && ehNumeroEnumInvalido(valor)) {
      return null
    }

    if (!Number.isFinite(valor)) {
      return null
    }

    return [String(valor)]
  }

  if (typeof valor === 'boolean') {
    return [String(valor)]
  }

  return null
}

function sanitizarFiltros(filtros?: FiltrosRelatorio, opcoes?: OpcoesSanitizacaoFiltros): URLSearchParams {
  const search = new URLSearchParams()
  if (!filtros) {
    return search
  }

  const chavesPermitidasNormalizadas = new Set((opcoes?.chavesPermitidas ?? []).map((item) => item.toLowerCase()))
  const chavesRemovidasNormalizadas = new Set((opcoes?.chavesRemovidas ?? []).map((item) => item.toLowerCase()))
  const chavesEnumNormalizadas = new Set((opcoes?.chavesEnum ?? CHAVES_ENUM_PADRAO).map((item) => item.toLowerCase()))
  const possuiWhitelist = chavesPermitidasNormalizadas.size > 0

  for (const [chaveOriginal, valor] of Object.entries(filtros)) {
    const chaveNormalizada = chaveOriginal.toLowerCase()
    if (chavesRemovidasNormalizadas.has(chaveNormalizada)) {
      continue
    }

    if (possuiWhitelist && !chavesPermitidasNormalizadas.has(chaveNormalizada)) {
      continue
    }

    const valores = limparValorFiltro(valor, chaveOriginal, chavesEnumNormalizadas)
    if (!valores || valores.length === 0) {
      continue
    }

    const nomeParametro = normalizarNomeParametro(chaveOriginal)
    for (const item of valores) {
      search.append(nomeParametro, item)
    }
  }

  return search
}

function buildQuery(filtros?: FiltrosRelatorio, opcoes?: OpcoesSanitizacaoFiltros): string {
  const search = sanitizarFiltros(filtros, opcoes)
  const query = search.toString()
  return query ? `?${query}` : ''
}

function getComFiltros<T>(endpoint: string, filtros?: FiltrosRelatorio, opcoes?: OpcoesSanitizacaoFiltros): Promise<T> {
  return httpClient.get<T>(`${endpoint}${buildQuery(filtros, opcoes)}`)
}

const chavesPeriodo = ['dataInicio', 'dataFim', 'dataInicial', 'dataFinal'] as const
const chavesEnums = ['agrupamento', 'agruparPor'] as const

const opcoesSemAgrupamentos: OpcoesSanitizacaoFiltros = {
  chavesRemovidas: chavesEnums,
}

export const relatoriosAvancadosAdminService = {
  obterMetadados: () => httpClient.get<RelatoriosAvancadosMetadados>('/api/admin/relatorios-avancados/metadados'),

  obterResumoChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosResumo>('/api/admin/relatorios-avancados/chamados/resumo', filtros, opcoesSemAgrupamentos),

  obterSerieTemporalChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosSerieTemporal>('/api/admin/relatorios-avancados/chamados/serie-temporal', filtros),

  obterDistribuicaoChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosDistribuicao>('/api/admin/relatorios-avancados/chamados/distribuicao', filtros),

  obterProdutividadeAtendimento: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioAtendimentoProdutividade>('/api/admin/relatorios-avancados/atendimento/produtividade', filtros),

  obterResumoSla: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaResumo>('/api/admin/relatorios-avancados/sla/resumo', filtros, opcoesSemAgrupamentos),

  obterViolacoesSla: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaViolacao[]>('/api/admin/relatorios-avancados/sla/violacoes', filtros),

  obterSlaPorDepartamento: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaPorDepartamento[]>('/api/admin/relatorios-avancados/sla/por-departamento', filtros),

  obterSlaPorPrioridade: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaPorPrioridade[]>('/api/admin/relatorios-avancados/sla/por-prioridade', filtros),

  obterResumoAprovacoes: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesResumo>('/api/admin/relatorios-avancados/aprovacoes/resumo', filtros, opcoesSemAgrupamentos),

  obterTempoMedioAprovacoes: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesTempoMedio[]>('/api/admin/relatorios-avancados/aprovacoes/tempo-medio', filtros),

  obterAprovacoesPorOrigem: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesPorOrigem[]>('/api/admin/relatorios-avancados/aprovacoes/por-origem', filtros, opcoesSemAgrupamentos),

  obterResumoCatalogoServicos: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosResumo>('/api/admin/relatorios-avancados/catalogo-servicos/resumo', filtros, opcoesSemAgrupamentos),

  obterCatalogoServicosMaisSolicitados: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosMaisSolicitados[]>(
      '/api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados',
      filtros,
      opcoesSemAgrupamentos
    ),

  obterCatalogoServicosPorDepartamento: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosPorDepartamento[]>(
      '/api/admin/relatorios-avancados/catalogo-servicos/por-departamento',
      filtros,
      opcoesSemAgrupamentos
    ),

  obterResumoInventarioAtivos: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosResumo>('/api/admin/relatorios-avancados/inventario-ativos/resumo', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'departamentoId', 'localUnidadeId', 'usuarioResponsavelId', 'tipoAtivoInventarioId', 'statusOperacional', 'statusPatrimonial', 'criticidade', 'ativo', 'limiteRanking'],
    }),

  obterInventarioAtivosPorStatus: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosPorStatus>('/api/admin/relatorios-avancados/inventario-ativos/por-status', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'departamentoId', 'localUnidadeId', 'usuarioResponsavelId', 'tipoAtivoInventarioId', 'statusOperacional', 'statusPatrimonial', 'criticidade', 'ativo', 'limiteRanking'],
    }),

  obterInventarioAtivosChamadosRecorrentes: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosChamadosRecorrentes[]>(
      '/api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes',
      filtros,
      {
        chavesPermitidas: [...chavesPeriodo, 'departamentoId', 'localUnidadeId', 'usuarioResponsavelId', 'tipoAtivoInventarioId', 'statusOperacional', 'statusPatrimonial', 'criticidade', 'ativo', 'limiteRanking'],
      }
    ),

  obterInventarioAtivosPorDepartamento: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosPorDepartamento[]>(
      '/api/admin/relatorios-avancados/inventario-ativos/por-departamento',
      filtros,
      {
        chavesPermitidas: [...chavesPeriodo, 'departamentoId', 'localUnidadeId', 'usuarioResponsavelId', 'tipoAtivoInventarioId', 'statusOperacional', 'statusPatrimonial', 'criticidade', 'ativo', 'limiteRanking'],
      }
    ),

  obterResumoBaseConhecimento: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoResumo>('/api/admin/relatorios-avancados/base-conhecimento/resumo', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'categoriaId', 'statusArtigo', 'visibilidadeArtigo', 'ativo', 'limiteRanking'],
    }),

  obterBaseConhecimentoPorStatus: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoPorStatus>('/api/admin/relatorios-avancados/base-conhecimento/por-status', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'categoriaId', 'statusArtigo', 'visibilidadeArtigo', 'ativo', 'limiteRanking'],
    }),

  obterBaseConhecimentoVinculosChamados: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoVinculosChamados[]>(
      '/api/admin/relatorios-avancados/base-conhecimento/vinculos-chamados',
      filtros,
      {
        chavesPermitidas: [...chavesPeriodo, 'categoriaId', 'statusArtigo', 'visibilidadeArtigo', 'ativo', 'limiteRanking'],
      }
    ),

  obterResumoAuditoria: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaResumo>('/api/admin/relatorios-avancados/auditoria/resumo', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'usuarioId', 'entidade', 'tipoAcao', 'termo', 'limiteRanking'],
    }),

  obterAuditoriaPorUsuario: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaPorUsuario[]>('/api/admin/relatorios-avancados/auditoria/por-usuario', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'usuarioId', 'entidade', 'tipoAcao', 'termo', 'limiteRanking'],
    }),

  obterAuditoriaPorEntidade: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaPorEntidade[]>('/api/admin/relatorios-avancados/auditoria/por-entidade', filtros, {
      chavesPermitidas: [...chavesPeriodo, 'usuarioId', 'entidade', 'tipoAcao', 'termo', 'limiteRanking'],
    }),
}
