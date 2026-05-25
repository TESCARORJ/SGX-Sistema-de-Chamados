import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('RelatoriosChamadosPage', () => {
  it('deve carregar estrutura de resumo de chamados', () => {
    const caminho = new URL('./RelatoriosChamadosPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Relatorios - Chamados')
    expect(fonte).toContain('Total chamados')
    expect(fonte).toContain('Serie temporal')
    expect(fonte).toContain('Distribuicao')
  })

  it('deve possuir estados de erro e vazio', () => {
    const caminho = new URL('./RelatoriosChamadosPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Sem dados de chamados')
  })
})
