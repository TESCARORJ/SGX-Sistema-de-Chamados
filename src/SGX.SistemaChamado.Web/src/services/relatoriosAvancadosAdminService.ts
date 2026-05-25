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

function buildQuery(filtros?: FiltrosRelatorio): string {
  if (!filtros) {
    return ''
  }

  const search = new URLSearchParams()

  for (const [chave, valor] of Object.entries(filtros)) {
    if (valor === undefined || valor === null || valor === '') {
      continue
    }

    if (Array.isArray(valor) && valor.length === 0) {
      continue
    }

    if (valor instanceof Date) {
      search.set(chave, valor.toISOString())
      continue
    }

    if (typeof valor === 'string') {
      const valorTrim = valor.trim()
      if (!valorTrim || valorTrim === '00000000-0000-0000-0000-000000000000') {
        continue
      }

      search.set(chave, valorTrim)
      continue
    }

    if (typeof valor === 'boolean' || typeof valor === 'number') {
      search.set(chave, String(valor))
      continue
    }
  }

  const query = search.toString()
  return query ? `?${query}` : ''
}

function getComFiltros<T>(endpoint: string, filtros?: FiltrosRelatorio): Promise<T> {
  return httpClient.get<T>(`${endpoint}${buildQuery(filtros)}`)
}

export const relatoriosAvancadosAdminService = {
  obterMetadados: () => httpClient.get<RelatoriosAvancadosMetadados>('/api/admin/relatorios-avancados/metadados'),

  obterResumoChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosResumo>('/api/admin/relatorios-avancados/chamados/resumo', filtros),

  obterSerieTemporalChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosSerieTemporal>('/api/admin/relatorios-avancados/chamados/serie-temporal', filtros),

  obterDistribuicaoChamados: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioChamadosDistribuicao>('/api/admin/relatorios-avancados/chamados/distribuicao', filtros),

  obterProdutividadeAtendimento: (filtros: FiltroRelatorioChamados = {}) =>
    getComFiltros<RelatorioAtendimentoProdutividade>('/api/admin/relatorios-avancados/atendimento/produtividade', filtros),

  obterResumoSla: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaResumo>('/api/admin/relatorios-avancados/sla/resumo', filtros),

  obterViolacoesSla: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaViolacao[]>('/api/admin/relatorios-avancados/sla/violacoes', filtros),

  obterSlaPorDepartamento: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaPorDepartamento[]>('/api/admin/relatorios-avancados/sla/por-departamento', filtros),

  obterSlaPorPrioridade: (filtros: FiltroRelatorioSla = {}) =>
    getComFiltros<RelatorioSlaPorPrioridade[]>('/api/admin/relatorios-avancados/sla/por-prioridade', filtros),

  obterResumoAprovacoes: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesResumo>('/api/admin/relatorios-avancados/aprovacoes/resumo', filtros),

  obterTempoMedioAprovacoes: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesTempoMedio[]>('/api/admin/relatorios-avancados/aprovacoes/tempo-medio', filtros),

  obterAprovacoesPorOrigem: (filtros: FiltroRelatorioAprovacoes = {}) =>
    getComFiltros<RelatorioAprovacoesPorOrigem[]>('/api/admin/relatorios-avancados/aprovacoes/por-origem', filtros),

  obterResumoCatalogoServicos: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosResumo>('/api/admin/relatorios-avancados/catalogo-servicos/resumo', filtros),

  obterCatalogoServicosMaisSolicitados: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosMaisSolicitados[]>('/api/admin/relatorios-avancados/catalogo-servicos/mais-solicitados', filtros),

  obterCatalogoServicosPorDepartamento: (filtros: FiltroRelatorioCatalogo = {}) =>
    getComFiltros<RelatorioCatalogoServicosPorDepartamento[]>('/api/admin/relatorios-avancados/catalogo-servicos/por-departamento', filtros),

  obterResumoInventarioAtivos: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosResumo>('/api/admin/relatorios-avancados/inventario-ativos/resumo', filtros),

  obterInventarioAtivosPorStatus: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosPorStatus>('/api/admin/relatorios-avancados/inventario-ativos/por-status', filtros),

  obterInventarioAtivosChamadosRecorrentes: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosChamadosRecorrentes[]>('/api/admin/relatorios-avancados/inventario-ativos/chamados-recorrentes', filtros),

  obterInventarioAtivosPorDepartamento: (filtros: FiltroRelatorioInventario = {}) =>
    getComFiltros<RelatorioInventarioAtivosPorDepartamento[]>('/api/admin/relatorios-avancados/inventario-ativos/por-departamento', filtros),

  obterResumoBaseConhecimento: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoResumo>('/api/admin/relatorios-avancados/base-conhecimento/resumo', filtros),

  obterBaseConhecimentoPorStatus: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoPorStatus>('/api/admin/relatorios-avancados/base-conhecimento/por-status', filtros),

  obterBaseConhecimentoVinculosChamados: (filtros: FiltroRelatorioBaseConhecimento = {}) =>
    getComFiltros<RelatorioBaseConhecimentoVinculosChamados[]>('/api/admin/relatorios-avancados/base-conhecimento/vinculos-chamados', filtros),

  obterResumoAuditoria: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaResumo>('/api/admin/relatorios-avancados/auditoria/resumo', filtros),

  obterAuditoriaPorUsuario: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaPorUsuario[]>('/api/admin/relatorios-avancados/auditoria/por-usuario', filtros),

  obterAuditoriaPorEntidade: (filtros: FiltroRelatorioAuditoria = {}) =>
    getComFiltros<RelatorioAuditoriaPorEntidade[]>('/api/admin/relatorios-avancados/auditoria/por-entidade', filtros),
}
