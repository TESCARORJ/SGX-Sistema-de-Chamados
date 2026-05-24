import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CatalogoServicosListPage', () => {
  it('deve renderizar estrutura principal da listagem administrativa', () => {
    const caminho = new URL('./CatalogoServicosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Catalogo de Servicos"')
    expect(fonte).toContain('label="Novo servico"')
    expect(fonte).toContain('label="Departamento responsavel"')
    expect(fonte).toContain('label="Status"')
    expect(fonte).toContain('label="Visibilidade"')
  })

  it('deve tratar estados de loading e vazio', () => {
    const caminho = new URL('./CatalogoServicosListPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Nenhum servico encontrado')
  })
})
