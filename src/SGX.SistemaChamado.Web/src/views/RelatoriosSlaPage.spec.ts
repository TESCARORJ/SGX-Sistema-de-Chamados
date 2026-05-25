import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('RelatoriosSlaPage', () => {
  it('deve exibir cards e tabelas de sla', () => {
    const caminho = new URL('./RelatoriosSlaPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Relatorios - SLA')
    expect(fonte).toContain('Total com SLA')
    expect(fonte).toContain('SLA por departamento')
    expect(fonte).toContain('Violacoes de SLA')
  })

  it('deve tratar erro e vazio', () => {
    const caminho = new URL('./RelatoriosSlaPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('ErrorState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Sem dados de SLA')
  })
})
