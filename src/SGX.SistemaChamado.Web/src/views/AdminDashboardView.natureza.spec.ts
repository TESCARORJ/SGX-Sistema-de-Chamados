import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminDashboardView - natureza', () => {
  it('deve exibir indicadores por natureza ITSM com labels amigaveis', () => {
    const caminho = new URL('./AdminDashboardView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Distribuicao por natureza ITSM')
    expect(fonte).toContain("nome: 'Incidente'")
    expect(fonte).toContain("nome: 'Requisicao'")
    expect(fonte).toContain("nome: 'Mudanca'")
    expect(fonte).toContain("nome: 'Problema'")
    expect(fonte).toContain("nome: 'Evento/Alerta'")
    expect(fonte).toContain("nome: 'Tarefa operacional'")
  })
})
