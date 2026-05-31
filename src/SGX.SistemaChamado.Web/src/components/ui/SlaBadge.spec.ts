import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'
import { normalizarSituacaoSla, obterEstiloSla } from './slaBadge'

describe('SlaBadge', () => {
  it('normaliza situacao nula sem warning de tipagem', () => {
    expect(normalizarSituacaoSla(null)).toBeNull()
  })

  it('normaliza situacao string conhecida', () => {
    expect(normalizarSituacaoSla('DentroDoPrazo')).toBe('DentroDoPrazo')
  })

  it('normaliza situacao numerica 0 para NaoAplicavel', () => {
    expect(normalizarSituacaoSla(0)).toBe('NaoAplicavel')
  })

  it('normaliza situacao numerica 3 para Vencido', () => {
    expect(normalizarSituacaoSla(3)).toBe('Vencido')
  })

  it('aceita prop situacao como String ou Number no componente', () => {
    const caminho = new URL('./SlaBadge.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('type: [String, Number]')
  })

  it('mantem label e cor esperados para situacoes conhecidas', () => {
    const vencido = obterEstiloSla(normalizarSituacaoSla(3))
    const pausado = obterEstiloSla(normalizarSituacaoSla('Pausado'))

    expect(vencido).toMatchObject({
      color: 'red-1',
      textColor: 'red-9',
      label: 'SLA vencido',
    })
    expect(pausado).toMatchObject({
      color: 'purple-1',
      textColor: 'purple-8',
      label: 'SLA pausado',
    })
  })
})
