export type SituacaoSla =
  | 'NaoAplicavel'
  | 'DentroDoPrazo'
  | 'ProximoDoVencimento'
  | 'Vencido'
  | 'Cumprido'
  | 'Violado'
  | 'Pausado'

export type SituacaoSlaEntrada = SituacaoSla | number | string | null | undefined

const situacaoPorCodigo: Record<number, SituacaoSla> = {
  0: 'NaoAplicavel',
  1: 'DentroDoPrazo',
  2: 'ProximoDoVencimento',
  3: 'Vencido',
  4: 'Cumprido',
  5: 'Violado',
  6: 'Pausado',
}

const situacaoPorTexto: Record<string, SituacaoSla> = {
  naoaplicavel: 'NaoAplicavel',
  dentrodoprazo: 'DentroDoPrazo',
  proximodovencimento: 'ProximoDoVencimento',
  vencido: 'Vencido',
  cumprido: 'Cumprido',
  violado: 'Violado',
  pausado: 'Pausado',
}

export function normalizarSituacaoSla(situacao: SituacaoSlaEntrada): SituacaoSla | null {
  if (situacao === null || situacao === undefined) {
    return null
  }

  if (typeof situacao === 'number') {
    return situacaoPorCodigo[situacao] ?? null
  }

  const texto = String(situacao).trim()
  if (!texto) {
    return null
  }

  if (/^\d+$/.test(texto)) {
    const codigo = Number.parseInt(texto, 10)
    return situacaoPorCodigo[codigo] ?? null
  }

  const chave = texto.replace(/[\s_-]+/g, '').toLowerCase()
  return situacaoPorTexto[chave] ?? null
}

export interface SlaBadgeStyle {
  color: string
  textColor: string
  icon: string
  label: string
}

export function obterEstiloSla(
  situacao: SituacaoSla | null,
  fallback: { vencido?: boolean; proximo?: boolean; pausado?: boolean } = {}
): SlaBadgeStyle {
  switch (situacao) {
    case 'NaoAplicavel':
      return {
        color: 'grey-3',
        textColor: 'grey-8',
        icon: 'remove_circle_outline',
        label: 'SLA n\u00e3o aplic\u00e1vel',
      }
    case 'Pausado':
      return { color: 'purple-1', textColor: 'purple-8', icon: 'pause_circle', label: 'SLA pausado' }
    case 'Violado':
    case 'Vencido':
      return { color: 'red-1', textColor: 'red-9', icon: 'warning', label: 'SLA vencido' }
    case 'ProximoDoVencimento':
      return {
        color: 'orange-1',
        textColor: 'orange-9',
        icon: 'schedule',
        label: 'Pr\u00f3ximo do vencimento',
      }
    case 'Cumprido':
      return { color: 'teal-1', textColor: 'teal-9', icon: 'verified', label: 'SLA cumprido' }
    case 'DentroDoPrazo':
      return { color: 'green-1', textColor: 'green-9', icon: 'task_alt', label: 'Dentro do prazo' }
    default:
      if (fallback.vencido) {
        return { color: 'red-1', textColor: 'red-9', icon: 'warning', label: 'SLA vencido' }
      }

      if (fallback.proximo) {
        return {
          color: 'orange-1',
          textColor: 'orange-9',
          icon: 'schedule',
          label: 'Pr\u00f3ximo do vencimento',
        }
      }

      if (fallback.pausado) {
        return { color: 'purple-1', textColor: 'purple-8', icon: 'pause_circle', label: 'SLA pausado' }
      }

      return { color: 'green-1', textColor: 'green-9', icon: 'task_alt', label: 'Dentro do prazo' }
  }
}
