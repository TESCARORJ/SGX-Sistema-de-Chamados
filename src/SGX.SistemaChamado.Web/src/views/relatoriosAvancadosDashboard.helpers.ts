import { HttpRequestError } from '../services/httpClient'
import type {
  FiltroRelatorioAprovacoes,
  FiltroRelatorioAuditoria,
  FiltroRelatorioCatalogo,
  FiltroRelatorioChamados,
  FiltroRelatorioInventario,
  FiltroRelatorioSla,
} from '../types/relatoriosAvancados'

export function formatarDataIsoLocal(data: Date): string {
  const ano = data.getFullYear()
  const mes = String(data.getMonth() + 1).padStart(2, '0')
  const dia = String(data.getDate()).padStart(2, '0')
  return `${ano}-${mes}-${dia}`
}

function formatarDataUtcIntervalo(data: Date, fimDoDia: boolean): string {
  const dataIso = formatarDataIsoLocal(data)
  return fimDoDia ? `${dataIso}T23:59:59.999Z` : `${dataIso}T00:00:00.000Z`
}

export function criarFiltroPeriodoPadrao(dataBase: Date = new Date()): Pick<FiltroRelatorioChamados, 'dataInicial' | 'dataFinal'> {
  const dataFinal = new Date(dataBase)
  const dataInicial = new Date(dataBase)
  dataInicial.setDate(dataInicial.getDate() - 30)

  return {
    dataInicial: formatarDataUtcIntervalo(dataInicial, false),
    dataFinal: formatarDataUtcIntervalo(dataFinal, true),
  }
}

export type FiltrosResumoDashboard = {
  chamados: FiltroRelatorioChamados
  sla: FiltroRelatorioSla
  aprovacoes: FiltroRelatorioAprovacoes
  catalogo: FiltroRelatorioCatalogo
  inventario: FiltroRelatorioInventario
  auditoria: FiltroRelatorioAuditoria
}

export function criarFiltrosResumoDashboard(dataBase: Date = new Date()): FiltrosResumoDashboard {
  const periodo = criarFiltroPeriodoPadrao(dataBase)
  return {
    chamados: { ...periodo },
    sla: { ...periodo },
    aprovacoes: { ...periodo },
    catalogo: { ...periodo, limiteRanking: 5 },
    inventario: { ...periodo, limiteRanking: 5 },
    auditoria: { ...periodo, limiteRanking: 5 },
  }
}

export function mapearMensagemErroDashboard(error: unknown): string {
  if (error instanceof HttpRequestError) {
    if (error.status === 400) return 'Filtro invalido'
    if (error.status === 401 || error.status === 403) return 'Sem permissao'
    if (error.status === 404) return 'Endpoint nao encontrado'
    if (error.status >= 500) return 'Erro interno ao carregar'
    return 'Erro ao carregar'
  }

  if (error instanceof Error) {
    const mensagem = error.message.toLowerCase()
    if (mensagem.includes('failed to fetch') || mensagem.includes('network')) {
      return 'API indisponivel'
    }
  }

  return 'Erro ao carregar'
}
