import type { NaturezaChamado } from './portal'

export interface FiltroIndicadoresRequest {
  dataInicio?: string
  dataFim?: string
  departamentoId?: string
  categoriaId?: string
  responsavelId?: string
  naturezaChamado?: NaturezaChamado
}

export interface ChamadosPorStatus {
  status: string
  total: number
}

export interface ChamadosPorPrioridade {
  prioridade: string
  total: number
}

export interface ChamadosPorCategoria {
  categoria: string
  total: number
}

export interface ChamadosPorNatureza {
  codigo: number
  natureza: string
  total: number
}

export interface IndicadoresSla {
  totalChamados: number
  totalDentroDoPrazo: number
  totalVencidos: number
  percentualCumprimento: number
  totalProximosDoVencimento: number
  mediaHorasResolucao: number | null
  mediaHorasPrimeiraResposta: number | null
}

export interface ProdutividadeAtendente {
  responsavelId: string
  responsavelNome: string
  totalAtendidos: number
  totalEncerrados: number
  totalVencidos: number
  mediaHorasResolucao: number | null
}
