import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('InventarioAtivosListPage', () => {
  it('deve renderizar estrutura principal da listagem administrativa', () => {
    const caminho = new URL('./InventarioAtivosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Inventario/Ativos"')
    expect(fonte).toContain('label="Novo ativo"')
    expect(fonte).toContain('label="Status operacional"')
    expect(fonte).toContain('label="Status patrimonial"')
    expect(fonte).toContain('label="Criticidade"')
  })

  it('deve tratar estados de loading e vazio', () => {
    const caminho = new URL('./InventarioAtivosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Nenhum ativo encontrado')
  })
})
