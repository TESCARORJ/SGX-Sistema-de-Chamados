import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('RelatoriosChamadosPage', () => {
  it('deve carregar estrutura de resumo de chamados', () => {
    const caminho = new URL('./RelatoriosChamadosPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Relatórios - Chamados')
    expect(fonte).toContain('Total de chamados')
    expect(fonte).toContain('Série temporal')
    expect(fonte).toContain('Distribuição')
  })

  it('deve possuir estados de erro e vazio', () => {
    const caminho = new URL('./RelatoriosChamadosPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Sem dados de chamados')
  })
})
