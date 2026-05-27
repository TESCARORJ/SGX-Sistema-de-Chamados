import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('FiltrosChamadoAdmin - natureza', () => {
  it('deve exibir filtro de Natureza ITSM na listagem administrativa', () => {
    const caminho = new URL('./FiltrosChamadoAdmin.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('label="Natureza ITSM"')
    expect(fonte).toContain('NaturezaChamado.Incidente')
    expect(fonte).toContain('NaturezaChamado.Requisicao')
  })
})
