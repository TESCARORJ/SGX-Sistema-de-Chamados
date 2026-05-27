import type {
  ChamadosPorCategoria,
  ChamadosPorNatureza,
  ChamadosPorPrioridade,
  ChamadosPorStatus,
  IndicadoresSla,
  ProdutividadeAtendente,
} from './indicadores'

export interface IndicadorCard {
  chave: string
  titulo: string
  valor: number
}

export interface DashboardAdminResponse {
  totalAbertos: number
  totalEmAtendimento: number
  totalAguardandoSolicitante: number
  totalResolvidosPeriodo: number
  totalEncerradosPeriodo: number
  totalVencidos: number
  totalProximosDoVencimento: number
  totalSemResponsavel: number
  cards: IndicadorCard[]
  chamadosPorStatus: ChamadosPorStatus[]
  chamadosPorPrioridade: ChamadosPorPrioridade[]
  chamadosPorCategoria: ChamadosPorCategoria[]
  chamadosPorNatureza: ChamadosPorNatureza[]
  indicadoresSla: IndicadoresSla
  produtividadePorAtendente: ProdutividadeAtendente[]
}
