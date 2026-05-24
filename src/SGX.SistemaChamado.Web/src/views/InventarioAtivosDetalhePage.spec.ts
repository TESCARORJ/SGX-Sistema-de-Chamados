import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('InventarioAtivosDetalhePage', () => {
  it('deve exibir secoes principais do detalhe do ativo', () => {
    const caminho = new URL('./InventarioAtivosDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('titulo="Dados gerais"')
    expect(fonte).toContain('titulo="Historico / Movimentacoes"')
    expect(fonte).toContain('titulo="Chamados relacionados"')
    expect(fonte).toContain('label="Movimentar"')
  })

  it('deve suportar modal de movimentacao e listagem de chamados', () => {
    const caminho = new URL('./InventarioAtivosDetalhePage.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Movimentar ativo')
    expect(fonte).toContain('Salvar movimentacao')
    expect(fonte).toContain('Abrir chamado')
    expect(fonte).toContain('inventarioAtivosAdminService.listarHistorico')
    expect(fonte).toContain('inventarioAtivosAdminService.listarChamados')
  })
})
