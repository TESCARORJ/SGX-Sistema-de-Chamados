import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('CatalogoServicosPortalPage', () => {
  it('deve renderizar estrutura principal da listagem do portal', () => {
    const caminho = new URL('./CatalogoServicosPortalPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Catalogo de Servicos"')
    expect(fonte).toContain('label="Buscar servico"')
    expect(fonte).toContain('label="Departamento responsavel"')
    expect(fonte).toContain('label="Categoria"')
    expect(fonte).toContain('label="Ver detalhes"')
  })

  it('deve tratar estados de loading e vazio', () => {
    const caminho = new URL('./CatalogoServicosPortalPage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('LoadingState')
    expect(fonte).toContain('EmptyState')
    expect(fonte).toContain('Nenhum servico encontrado')
  })
})
